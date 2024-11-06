using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackForm
{
    public class Baraja
    {
        private List<Carta> cartas;
        private Random random = new Random();

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

        public void Mezclar()
        {
            for (int i = cartas.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                Carta temp = cartas[i];
                cartas[i] = cartas[j];
                cartas[j] = temp;
            }
        }

        public Carta RepartirCarta()
        {
            if (cartas.Count > 0)
            {
                Carta carta = cartas[0];
                cartas.RemoveAt(0);
                return carta;
            }
            return null;
        }
    }


}
