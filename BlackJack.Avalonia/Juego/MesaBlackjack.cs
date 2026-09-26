using BlackjackForm;

namespace BlackjackAvalonia.Juego
{
    public enum FaseJuego
    {
        Apuesta,       // Esperando apuesta (antes de la primera ronda o entre rondas)
        TurnoJugador   // Cartas repartidas, el jugador decide
    }

    public enum ResultadoRonda
    {
        BlackjackJugador,
        GanaJugador,
        BancaSePasa,
        Empate,
        JugadorSePasa,
        GanaBanca,
        BlackjackBanca
    }

    // Reglas implementadas:
    // - Primero se apuesta y despues se reparten las cartas (jugador, banca, jugador, banca).
    // - Blackjack natural (21 con dos cartas) paga 3:2; si ambos lo tienen es empate.
    // - La banca revisa si tiene blackjack antes del turno del jugador.
    // - Se puede doblar solo con las dos primeras cartas: se duplica la apuesta y se recibe una sola carta.
    // - Al llegar a 21 el jugador se planta automaticamente.
    // - La banca pide carta hasta llegar a 17 y se planta con 17 suave.
    public class MesaBlackjack
    {
        private readonly Func<Baraja> crearBaraja;
        private Baraja baraja;

        public Jugador Jugador { get; }
        public ManoJugador Banca { get; private set; } = new ManoJugador();
        public FaseJuego Fase { get; private set; } = FaseJuego.Apuesta;
        public ResultadoRonda? Resultado { get; private set; }

        // Cambio en el saldo del jugador en la ultima ronda terminada
        public int GananciaNeta { get; private set; }

        public MesaBlackjack(Jugador jugador, Func<Baraja> crearBaraja = null)
        {
            Jugador = jugador;
            this.crearBaraja = crearBaraja ?? (() => new Baraja());
        }

        public bool OcultarCartaBanca => Fase == FaseJuego.TurnoJugador;

        public bool PuedeDoblar =>
            Fase == FaseJuego.TurnoJugador &&
            Jugador.Cartas.Count == 2 &&
            Jugador.Saldo >= Jugador.ApuestaActual;

        public void Apostar(int cantidad)
        {
            if (Fase != FaseJuego.Apuesta)
                throw new InvalidOperationException("Ya hay una ronda en curso.");
            if (cantidad <= 0)
                throw new InvalidOperationException("Ingresa una cantidad válida para apostar.");

            Jugador.HacerApuesta(cantidad); // Lanza excepcion si no hay saldo suficiente

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

        // Total de la mano; Suave indica que un As esta contando como 11
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

        private void ValidarTurno()
        {
            if (Fase != FaseJuego.TurnoJugador)
                throw new InvalidOperationException("Primero debes hacer una apuesta.");
        }

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
