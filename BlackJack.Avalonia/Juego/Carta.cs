/**
 * @file Carta.cs
 * @brief Modelo de carta.
 * @author Santiago Caicedo
 */
namespace BlackjackAvalonia.Juego
{
    /**
     * @brief Carta de la baraja francesa.
     */
    public class Carta
    {
        /** Valor: A, 2 a 10, J, Q o K. */
        public string Valor { get; }
        /** Palo: Picas, Treboles, Corazones o Diamantes. */
        public string Palo { get; }

        /**
         * @brief Crea una carta.
         * @param valor Valor
         * @param palo Palo
         */
        public Carta(string valor, string palo)
        {
            Valor = valor;
            Palo = palo;
        }

        /**
         * @brief Nombre de la carta para el historial.
         * @return Texto como "A de Picas"
         */
        public override string ToString()
        {
            return $"{Valor} de {Palo}";
        }
    }
}
