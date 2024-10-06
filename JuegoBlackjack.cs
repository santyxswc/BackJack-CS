using System;
using System.Collections.Generic;

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
        string[] palos = { "Picas", "Tréboles", "Corazones", "Diamantes" };

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
}