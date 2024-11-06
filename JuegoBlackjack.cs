using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace BlackjackForm {
    public class Jugador
    {
        public List<Carta> Cartas { get; }
        public int Saldo { get; private set; }
        public int ApuestaActual { get; set; }

        public Jugador(int saldoInicial)
        {
            Cartas = new List<Carta>();
            Saldo = saldoInicial;
            ApuestaActual = 0;
        }

        public void PedirCarta(Carta carta)
        {
            Cartas.Add(carta);
        }

        public void HacerApuesta(int cantidad)
        {
            if (cantidad > Saldo)
            {
                throw new InvalidOperationException("Saldo insuficiente para hacer esa apuesta.");
            }
            ApuestaActual = cantidad; // Establecer la cantidad de apuesta actual
            Saldo -= cantidad; // Resta la apuesta del saldo
        }



        public void GanarApuesta()
        {
            Saldo += ApuestaActual * 2; // Gana el doble de lo apostado
            ApuestaActual = 0;
        }

        public void EmpatarApuesta()
        {
            Saldo += ApuestaActual; // Recupera la apuesta
            ApuestaActual = 0;

        }

        public void PerderApuesta()
        {
            ApuestaActual = 0; // Pierde la apuesta, no modifica el saldo

        }


        public int CalcularValor()
        {
            int valorTotal = 0;
            int ases = 0;

            foreach (var carta in Cartas)
            {
                if (carta.Valor == "J" || carta.Valor == "Q" || carta.Valor == "K")
                    valorTotal += 10;
                else if (carta.Valor == "A")
                {
                    ases++;
                    valorTotal += 11;
                }
                else
                    valorTotal += int.Parse(carta.Valor);
            }

            while (valorTotal > 21 && ases > 0)
            {
                valorTotal -= 10;
                ases--;
            }

            return valorTotal;
        }

        public void OrdenarCartas()
        {
            Cartas.Sort((c1, c2) => c2.ObtenerValorNumerico().CompareTo(c1.ObtenerValorNumerico()));
        }
    }

    public class ManoJugador
    {
        public List<Carta> Cartas { get; }

        public ManoJugador()
        {
            Cartas = new List<Carta>();
        }

        public void PedirCarta(Carta carta)
        {
            Cartas.Add(carta);
        }

        public int CalcularValor()
        {
            int valorTotal = 0;
            int ases = 0;

            foreach (var carta in Cartas)
            {
                if (carta.Valor == "J" || carta.Valor == "Q" || carta.Valor == "K")
                    valorTotal += 10;
                else if (carta.Valor == "A")
                {
                    ases++;
                    valorTotal += 11;
                }
                else
                    valorTotal += int.Parse(carta.Valor);
            }

            while (valorTotal > 21 && ases > 0)
            {
                valorTotal -= 10;
                ases--;
            }

            return valorTotal;
        }

        public void OrdenarCartas()
        {
            Cartas.Sort((c1, c2) => c2.ObtenerValorNumerico().CompareTo(c1.ObtenerValorNumerico()));
        }

        public string ObtenerRutaImagen(Carta carta)
        {
            string rutaBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\imagenes");
            return System.IO.Path.Combine(rutaBase, $"{carta.Valor}_de_{carta.Palo}.jpg");
        }
    }

}

