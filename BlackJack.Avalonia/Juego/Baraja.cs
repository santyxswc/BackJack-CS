/**
 * @file Baraja.cs
 * @brief Baraja de 52 cartas.
 * @author Santiago Caicedo
 */
namespace BlackjackAvalonia.Juego
{
    /**
     * @brief Baraja que reparte cartas desde la primera posición.
     */
    public class Baraja
    {
        /** Cartas que quedan por repartir. */
        private readonly List<Carta> cartas;
        /** Generador para mezclar. */
        private readonly Random random = new Random();

        /**
         * @brief Crea una baraja con un orden fijo, sin mezclar.
         * @param cartasEnOrden Cartas en el orden en que se repartirán
         *
         * Se usa en las pruebas para repetir una mano.
         */
        public Baraja(IEnumerable<Carta> cartasEnOrden)
        {
            cartas = new List<Carta>(cartasEnOrden);
        }

        /**
         * @brief Crea las 52 cartas y las mezcla.
         */
        public Baraja()
        {
            cartas = new List<Carta>();
            string[] valores = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
            string[] palos = { "Picas", "Treboles", "Corazones", "Diamantes" };

            foreach (var palo in palos)
            {
                foreach (var valor in valores)
                {
                    cartas.Add(new Carta(valor, palo));
                }
            }

            Mezclar();
        }

        /**
         * @brief Mezcla las cartas con el algoritmo de Fisher-Yates.
         */
        public void Mezclar()
        {
            for (int i = cartas.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (cartas[i], cartas[j]) = (cartas[j], cartas[i]);
            }
        }

        /**
         * @brief Saca la siguiente carta.
         * @return Carta, o null si la baraja está vacía
         */
        public Carta RepartirCarta()
        {
            if (cartas.Count == 0)
                return null;

            Carta carta = cartas[0];
            cartas.RemoveAt(0);
            return carta;
        }
    }
}
