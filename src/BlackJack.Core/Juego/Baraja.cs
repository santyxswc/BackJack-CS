/**
 * @file Baraja.cs
 * @brief Baraja de 52 cartas.
 * @author Santiago Caicedo
 */
namespace BlackJack.Core.Juego
{
    /**
     * @brief Baraja que reparte cartas en orden.
     */
    public class Baraja
    {
        /** Cartas que quedan por repartir. */
        private readonly Queue<Carta> cartas;

        /**
         * @brief Crea las 52 cartas y las mezcla.
         * @param random Generador para mezclar; por defecto Random.Shared
         */
        public Baraja(Random random = null)
        {
            var nuevas = Enum.GetValues<Palo>()
                .SelectMany(palo => Enum.GetValues<Rango>().Select(rango => new Carta(rango, palo)))
                .ToArray();
            Mezclar(nuevas, random ?? Random.Shared);
            cartas = new Queue<Carta>(nuevas);
        }

        /**
         * @brief Crea una baraja con un orden fijo, sin mezclar.
         * @param cartasEnOrden Cartas en el orden en que se repartirán
         *
         * Permite reproducir manos concretas en las pruebas.
         */
        public Baraja(IEnumerable<Carta> cartasEnOrden)
        {
            cartas = new Queue<Carta>(cartasEnOrden);
        }

        /** Cartas que quedan en la baraja. */
        public int Restantes => cartas.Count;

        /**
         * @brief Saca la siguiente carta.
         * @return Carta repartida
         * @exception InvalidOperationException Si la baraja está vacía
         */
        public Carta RepartirCarta() =>
            cartas.TryDequeue(out var carta)
                ? carta
                : throw new InvalidOperationException("La baraja se quedó sin cartas.");

        /**
         * @brief Mezcla con el algoritmo de Fisher-Yates.
         * @param cartas Cartas a mezclar
         * @param random Generador de números aleatorios
         */
        private static void Mezclar(Carta[] cartas, Random random)
        {
            for (int i = cartas.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (cartas[i], cartas[j]) = (cartas[j], cartas[i]);
            }
        }
    }
}
