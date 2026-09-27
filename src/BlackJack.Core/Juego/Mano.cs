/**
 * @file Mano.cs
 * @brief Mano de cartas y su valor.
 * @author Santiago Caicedo
 */
namespace BlackJack.Core.Juego
{
    /**
     * @brief Valor de una mano.
     * @param Total Puntos de la mano
     * @param Suave true si un As cuenta como 11
     */
    public readonly record struct ValorMano(int Total, bool Suave)
    {
        /** Indica si la mano pasa de 21. */
        public bool SePasa => Total > 21;

        /**
         * @brief Calcula el valor de un conjunto de cartas.
         * @param cartas Cartas
         * @return Valor de la mano
         *
         * Las figuras valen 10 y un As vale 11 si no hace pasar de 21, o 1 si lo hace.
         */
        public static ValorMano De(IEnumerable<Carta> cartas)
        {
            int total = 0;
            bool hayAs = false;
            foreach (var carta in cartas)
            {
                total += carta.Puntos;
                hayAs |= carta.EsAs;
            }

            return hayAs && total + 10 <= 21
                ? new ValorMano(total + 10, true)
                : new ValorMano(total, false);
        }
    }

    /**
     * @brief Cartas de un participante (jugador o banca).
     */
    public class Mano
    {
        /** Cartas recibidas. */
        private readonly List<Carta> cartas = new();

        /** Cartas de la mano, en el orden en que se recibieron. */
        public IReadOnlyList<Carta> Cartas => cartas;

        /** Valor actual de la mano. */
        public ValorMano Valor => ValorMano.De(cartas);

        /** Blackjack natural: 21 con las dos primeras cartas. */
        public bool EsBlackjack => cartas.Count == 2 && Valor.Total == 21;

        /**
         * @brief Agrega una carta.
         * @param carta Carta recibida
         */
        public void Agregar(Carta carta) => cartas.Add(carta);

        /**
         * @brief Descarta todas las cartas.
         */
        public void Vaciar() => cartas.Clear();
    }
}
