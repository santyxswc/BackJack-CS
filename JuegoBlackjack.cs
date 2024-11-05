using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

public class Carta
{
    public string Valor { get; }
    public string Palo { get; }

    public Carta(string valor, string palo)
    {
        Valor = valor;
        Palo = palo;
    }

    public int ObtenerValorNumerico()
    {
        if (Valor == "A") return 14;
        if (Valor == "K") return 13;
        if (Valor == "Q") return 12;
        if (Valor == "J") return 11;
        return int.Parse(Valor);
    }

    public override string ToString()
    {
        return $"{Valor} de {Palo}";
    }
}

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

public class Jugador
{
    public List<Carta> Cartas { get; }
    public int Saldo { get; private set; }
    public int ApuestaActual { get;  set; }

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


