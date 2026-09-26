/**
 * @file Jugador.cs
 * @brief Jugador con su saldo y su apuesta.
 * @author Santiago Caicedo
 */
namespace BlackjackAvalonia.Juego
{
    /**
     * @brief Mano, saldo y apuesta del jugador en la mesa.
     */
    public class Jugador
    {
        /** Cartas de la mano actual. */
        public List<Carta> Cartas { get; }
        /** Dinero disponible (sin contar la apuesta en juego). */
        public int Saldo { get; private set; }
        /** Dinero apostado en la ronda actual. */
        public int ApuestaActual { get; set; }

        /**
         * @brief Crea el jugador.
         * @param saldoInicial Saldo con el que empieza
         */
        public Jugador(int saldoInicial)
        {
            Cartas = new List<Carta>();
            Saldo = saldoInicial;
            ApuestaActual = 0;
        }

        /**
         * @brief Agrega una carta a la mano.
         * @param carta Carta recibida
         */
        public void PedirCarta(Carta carta)
        {
            Cartas.Add(carta);
        }

        /**
         * @brief Pasa dinero del saldo a la apuesta.
         * @param cantidad Cantidad a apostar
         * @exception InvalidOperationException Si el saldo no alcanza
         *
         * Se suma a la apuesta actual; así también se dobla.
         */
        public void HacerApuesta(int cantidad)
        {
            if (cantidad > Saldo)
            {
                throw new InvalidOperationException("Saldo insuficiente para hacer esa apuesta.");
            }
            ApuestaActual += cantidad;
            Saldo -= cantidad;
        }

        /**
         * @brief Paga un blackjack natural: devuelve la apuesta más 3:2.
         */
        public void GanarBlackjack()
        {
            Saldo += ApuestaActual + ApuestaActual * 3 / 2;
            ApuestaActual = 0;
        }

        /**
         * @brief Paga una ronda ganada: devuelve la apuesta más 1:1.
         */
        public void GanarApuesta()
        {
            Saldo += ApuestaActual * 2;
            ApuestaActual = 0;
        }

        /**
         * @brief Devuelve la apuesta en un empate.
         */
        public void EmpatarApuesta()
        {
            Saldo += ApuestaActual;
            ApuestaActual = 0;
        }

        /**
         * @brief Pierde la apuesta (ya salió del saldo).
         */
        public void PerderApuesta()
        {
            ApuestaActual = 0;
        }

        /**
         * @brief Agrega dinero al saldo.
         * @param cantidad Cantidad a depositar
         * @exception InvalidOperationException Si la cantidad no es positiva
         */
        public void Depositar(int cantidad)
        {
            if (cantidad <= 0)
            {
                throw new InvalidOperationException("La cantidad a depositar debe ser mayor a cero.");
            }
            Saldo += cantidad;
        }
    }
}
