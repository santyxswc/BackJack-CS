/**
 * @file MesaBlackjack.cs
 * @brief Reglas del blackjack.
 * @author Santiago Caicedo
 */
namespace BlackjackAvalonia.Juego
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
        /** Crea la baraja de cada ronda. */
        private readonly Func<Baraja> crearBaraja;
        /** Baraja de la ronda actual. */
        private Baraja baraja;

        /** Jugador de la mesa. */
        public Jugador Jugador { get; }
        /** Mano de la banca. */
        public ManoJugador Banca { get; private set; } = new ManoJugador();
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
            Jugador = jugador;
            this.crearBaraja = crearBaraja ?? (() => new Baraja());
        }

        /** Indica si la segunda carta de la banca va boca abajo. */
        public bool OcultarCartaBanca => Fase == FaseJuego.TurnoJugador;

        /** Indica si el jugador puede doblar: primeras dos cartas y saldo suficiente. */
        public bool PuedeDoblar =>
            Fase == FaseJuego.TurnoJugador &&
            Jugador.Cartas.Count == 2 &&
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
            if (cantidad <= 0)
                throw new InvalidOperationException("Ingresa una cantidad válida para apostar.");

            Jugador.HacerApuesta(cantidad);

            baraja = crearBaraja();
            Jugador.Cartas.Clear();
            Banca = new ManoJugador();
            Resultado = null;
            GananciaNeta = 0;

            Jugador.PedirCarta(baraja.RepartirCarta());
            Banca.PedirCarta(baraja.RepartirCarta());
            Jugador.PedirCarta(baraja.RepartirCarta());
            Banca.PedirCarta(baraja.RepartirCarta());

            bool blackjackJugador = Valor(Jugador.Cartas).Total == 21;
            bool blackjackBanca = Valor(Banca.Cartas).Total == 21;

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
            Jugador.PedirCarta(baraja.RepartirCarta());

            int total = Valor(Jugador.Cartas).Total;
            if (total > 21)
                Terminar(ResultadoRonda.JugadorSePasa);
            else if (total == 21)
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
            Jugador.PedirCarta(baraja.RepartirCarta());

            if (Valor(Jugador.Cartas).Total > 21)
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

            while (Valor(Banca.Cartas).Total < 17)
            {
                Banca.PedirCarta(baraja.RepartirCarta());
            }

            int jugador = Valor(Jugador.Cartas).Total;
            int banca = Valor(Banca.Cartas).Total;

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
         * @brief Calcula el valor de una mano.
         * @param cartas Cartas de la mano
         * @return Total y si es suave (un As cuenta como 11)
         *
         * Las figuras valen 10 y el As vale 11 si no hace pasar de 21, o 1 si lo hace.
         */
        public static (int Total, bool Suave) Valor(IEnumerable<Carta> cartas)
        {
            int total = 0;
            bool hayAs = false;

            foreach (var carta in cartas)
            {
                if (carta.Valor == "A")
                {
                    total += 1;
                    hayAs = true;
                }
                else if (carta.Valor == "J" || carta.Valor == "Q" || carta.Valor == "K")
                    total += 10;
                else
                    total += int.Parse(carta.Valor);
            }

            if (hayAs && total + 10 <= 21)
                return (total + 10, true);

            return (total, false);
        }

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
            int apuesta = Jugador.ApuestaActual;

            switch (resultado)
            {
                case ResultadoRonda.BlackjackJugador:
                    GananciaNeta = apuesta * 3 / 2;
                    Jugador.GanarBlackjack();
                    break;
                case ResultadoRonda.GanaJugador:
                case ResultadoRonda.BancaSePasa:
                    GananciaNeta = apuesta;
                    Jugador.GanarApuesta();
                    break;
                case ResultadoRonda.Empate:
                    GananciaNeta = 0;
                    Jugador.EmpatarApuesta();
                    break;
                default:
                    GananciaNeta = -apuesta;
                    Jugador.PerderApuesta();
                    break;
            }

            Resultado = resultado;
            Fase = FaseJuego.Apuesta;
        }
    }
}
