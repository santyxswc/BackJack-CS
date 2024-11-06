using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackForm
{
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
}
