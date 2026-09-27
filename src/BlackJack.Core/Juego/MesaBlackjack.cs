/**
 * @file MesaBlackjack.cs
 * @brief Reglas del blackjack.
 * @author Santiago Caicedo
 */
namespace BlackJack.Core.Juego
{
    /**
     * @brief Momento de la ronda.
     */
    public enum FaseJuego
    {
        /** Esperando la apuesta (antes de la primera ronda o entre rondas). */
        Apuesta,
        /** Cartas repartidas; el jugador decide. */
        TurnoJugador
    }

    /**
     * @brief Resultado de una ronda terminada.
     */
    public enum ResultadoRonda
    {
        /** El jugador tiene 21 con sus dos primeras cartas (paga 3:2). */
        BlackjackJugador,
        /** El jugador queda más cerca de 21 que la banca. */
        GanaJugador,
        /** La banca pasa de 21. */
        BancaSePasa,
        /** Mismo valor, o ambos con blackjack. */
        Empate,
        /** El jugador pasa de 21. */
        JugadorSePasa,
        /** La banca queda más cerca de 21. */
        GanaBanca,
        /** La banca tiene blackjack. */
        BlackjackBanca
    }

    /**
     * @brief Clasificación de un resultado para estadísticas y mensajes.
     */
    public static class ResultadoRondaExtensiones
    {
        /** true si el jugador gana la ronda. */
        public static bool EsVictoria(this ResultadoRonda r) =>
            r is ResultadoRonda.BlackjackJugador or ResultadoRonda.GanaJugador or ResultadoRonda.BancaSePasa;

        /** true si el jugador pierde la ronda. */
        public static bool EsDerrota(this ResultadoRonda r) => !r.EsVictoria() && r != ResultadoRonda.Empate;
    }

    /**
     * @brief Controla una ronda de blackjack y aplica las reglas.
     *
     * Reglas implementadas:
     * - Primero se apuesta y después se reparten las cartas (jugador, banca, jugador, banca).
     * - El blackjack natural (21 con dos cartas) paga 3:2; si ambos lo tienen es empate.
     * - La banca revisa si tiene blackjack antes del turno del jugador.
     * - Se puede doblar solo con las dos primeras cartas: se duplica la apuesta y se recibe una sola carta.
     * - Al llegar a 21 el jugador se planta automáticamente.
     * - La banca pide carta hasta llegar a 17 y se planta con 17 suave.
     */
    public class MesaBlackjack
    {
        /** La banca pide carta mientras tenga menos que esto. */
        public const int PlantaBanca = 17;

        /** Crea la baraja de cada ronda. */
        private readonly Func<Baraja> crearBaraja;
        /** Baraja de la ronda actual. */
        private Baraja baraja;

        /** Jugador de la mesa. */
        public Jugador Jugador { get; }
        /** Mano de la banca. */
        public Mano Banca { get; } = new();
        /** Fase actual de la ronda. */
        public FaseJuego Fase { get; private set; } = FaseJuego.Apuesta;
        /** Resultado de la última ronda; null mientras se juega. */
        public ResultadoRonda? Resultado { get; private set; }
        /** Cambio en el saldo del jugador en la última ronda terminada. */
        public int GananciaNeta { get; private set; }

        /**
         * @brief Crea la mesa.
         * @param jugador Jugador con su saldo
         * @param crearBaraja Función que crea la baraja de cada ronda; por defecto, una baraja mezclada
         */
        public MesaBlackjack(Jugador jugador, Func<Baraja> crearBaraja = null)
        {
            Jugador = jugador ?? throw new ArgumentNullException(nameof(jugador));
            this.crearBaraja = crearBaraja ?? (() => new Baraja());
        }

        /** Indica si la segunda carta de la banca va boca abajo. */
        public bool OcultarCartaBanca => Fase == FaseJuego.TurnoJugador;

        /** Indica si el jugador puede doblar: primeras dos cartas y saldo suficiente. */
        public bool PuedeDoblar =>
            Fase == FaseJuego.TurnoJugador &&
            Jugador.Mano.Cartas.Count == 2 &&
            Jugador.Saldo >= Jugador.ApuestaActual;

        /**
         * @brief Hace la apuesta, reparte las cartas y revisa los blackjacks naturales.
         * @param cantidad Cantidad a apostar
         * @exception InvalidOperationException Si hay una ronda en curso, la cantidad no es válida o el saldo no alcanza
         */
        public void Apostar(int cantidad)
        {
            if (Fase != FaseJuego.Apuesta)
                throw new InvalidOperationException("Ya hay una ronda en curso.");

            Jugador.HacerApuesta(cantidad);

            baraja = crearBaraja();
            Jugador.Mano.Vaciar();
            Banca.Vaciar();
            Resultado = null;
            GananciaNeta = 0;

            Jugador.Mano.Agregar(baraja.RepartirCarta());
            Banca.Agregar(baraja.RepartirCarta());
            Jugador.Mano.Agregar(baraja.RepartirCarta());
            Banca.Agregar(baraja.RepartirCarta());

            bool blackjackJugador = Jugador.Mano.EsBlackjack;
            bool blackjackBanca = Banca.EsBlackjack;

            if (blackjackJugador && blackjackBanca)
                Terminar(ResultadoRonda.Empate);
            else if (blackjackJugador)
                Terminar(ResultadoRonda.BlackjackJugador);
            else if (blackjackBanca)
                Terminar(ResultadoRonda.BlackjackBanca);
            else
                Fase = FaseJuego.TurnoJugador;
        }

        /**
         * @brief Da una carta al jugador; con más de 21 pierde y con 21 se planta solo.
         * @exception InvalidOperationException Si no es el turno del jugador
         */
        public void PedirCarta()
        {
            ValidarTurno();
            Jugador.Mano.Agregar(baraja.RepartirCarta());

            var valor = Jugador.Mano.Valor;
            if (valor.SePasa)
                Terminar(ResultadoRonda.JugadorSePasa);
            else if (valor.Total == 21)
                Plantarse();
        }

        /**
         * @brief Duplica la apuesta, da una sola carta y termina el turno.
         * @exception InvalidOperationException Si no se puede doblar
         */
        public void Doblar()
        {
            if (!PuedeDoblar)
                throw new InvalidOperationException("Solo puedes doblar con tus dos primeras cartas y saldo suficiente.");

            Jugador.HacerApuesta(Jugador.ApuestaActual);
            Jugador.Mano.Agregar(baraja.RepartirCarta());

            if (Jugador.Mano.Valor.SePasa)
                Terminar(ResultadoRonda.JugadorSePasa);
            else
                Plantarse();
        }

        /**
         * @brief Termina el turno: la banca juega y se decide la ronda.
         * @exception InvalidOperationException Si no es el turno del jugador
         */
        public void Plantarse()
        {
            ValidarTurno();

            while (Banca.Valor.Total < PlantaBanca)
                Banca.Agregar(baraja.RepartirCarta());

            int jugador = Jugador.Mano.Valor.Total;
            int banca = Banca.Valor.Total;

            if (banca > 21)
                Terminar(ResultadoRonda.BancaSePasa);
            else if (jugador > banca)
                Terminar(ResultadoRonda.GanaJugador);
            else if (jugador == banca)
                Terminar(ResultadoRonda.Empate);
            else
                Terminar(ResultadoRonda.GanaBanca);
        }

        /**
         * @brief Ganancia neta de un resultado para una apuesta.
         * @param resultado Resultado de la ronda
         * @param apuesta Cantidad apostada
         * @return 3:2 en blackjack (redondeado hacia abajo), 1:1 al ganar, 0 en empate y -apuesta al perder
         */
        public static int CalcularGanancia(ResultadoRonda resultado, int apuesta) => resultado switch
        {
            ResultadoRonda.BlackjackJugador => apuesta * 3 / 2,
            ResultadoRonda.Empate => 0,
            _ when resultado.EsVictoria() => apuesta,
            _ => -apuesta
        };

        /**
         * @brief Verifica que sea el turno del jugador.
         * @exception InvalidOperationException Si no lo es
         */
        private void ValidarTurno()
        {
            if (Fase != FaseJuego.TurnoJugador)
                throw new InvalidOperationException("Primero debes hacer una apuesta.");
        }

        /**
         * @brief Paga o cobra la apuesta según el resultado y cierra la ronda.
         * @param resultado Resultado de la ronda
         */
        private void Terminar(ResultadoRonda resultado)
        {
            GananciaNeta = CalcularGanancia(resultado, Jugador.ApuestaActual);
            Jugador.Liquidar(GananciaNeta);
            Resultado = resultado;
            Fase = FaseJuego.Apuesta;
        }
    }
}
