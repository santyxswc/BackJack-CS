/**
 * @file Jugador.cs
 * @brief Jugador con su mano, saldo y apuesta.
 * @author Santiago Caicedo
 */
namespace BlackJack.Core.Juego
{
    /**
     * @brief Participante que apuesta contra la banca.
     *
     * Las cartas están en Mano; esta clase maneja el saldo y la apuesta.
     */
    public class Jugador
    {
        /** Cartas de la ronda actual. */
        public Mano Mano { get; } = new();
        /** Dinero disponible (sin contar la apuesta en juego). */
        public int Saldo { get; private set; }
        /** Dinero apostado en la ronda actual. */
        public int ApuestaActual { get; private set; }

        /**
         * @brief Crea el jugador.
         * @param saldoInicial Saldo con el que empieza
         * @exception ArgumentOutOfRangeException Si el saldo es negativo
         */
        public Jugador(int saldoInicial)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(saldoInicial);
            Saldo = saldoInicial;
        }

        /**
         * @brief Pasa dinero del saldo a la apuesta. Se suma a la apuesta actual; así también se dobla.
         * @param cantidad Cantidad a apostar
         * @exception InvalidOperationException Si la cantidad no es positiva o el saldo no alcanza
         */
        public void HacerApuesta(int cantidad)
        {
            if (cantidad <= 0)
                throw new InvalidOperationException("Ingresa una cantidad válida para apostar.");
            if (cantidad > Saldo)
                throw new InvalidOperationException("Saldo insuficiente para hacer esa apuesta.");

            ApuestaActual += cantidad;
            Saldo -= cantidad;
        }

        /**
         * @brief Liquida la apuesta: devuelve la apuesta más la ganancia (0 en empate, negativa si pierde).
         * @param gananciaNeta Cambio neto en el saldo respecto a antes de apostar
         */
        public void Liquidar(int gananciaNeta)
        {
            Saldo += Math.Max(ApuestaActual + gananciaNeta, 0);
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
                throw new InvalidOperationException("La cantidad a depositar debe ser mayor a cero.");
            Saldo += cantidad;
        }
    }
}
