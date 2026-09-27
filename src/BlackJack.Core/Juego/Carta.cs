/**
 * @file Carta.cs
 * @brief Carta de la baraja francesa.
 * @author Santiago Caicedo
 */
namespace BlackJack.Core.Juego
{
    /**
     * @brief Palo de una carta. El nombre coincide con el de sus imágenes.
     */
    public enum Palo
    {
        Picas,
        Treboles,
        Corazones,
        Diamantes
    }

    /**
     * @brief Rango de una carta; el valor numérico es su puntuación base (el As vale 1).
     */
    public enum Rango
    {
        As = 1, Dos, Tres, Cuatro, Cinco, Seis, Siete, Ocho, Nueve, Diez, Jota, Reina, Rey
    }

    /**
     * @brief Carta inmutable: rango y palo.
     * @param Rango Rango de la carta
     * @param Palo Palo de la carta
     */
    public sealed record Carta(Rango Rango, Palo Palo)
    {
        /** Símbolo impreso en la carta: A, 2 a 10, J, Q o K. */
        public string Simbolo => Rango switch
        {
            Rango.As => "A",
            Rango.Jota => "J",
            Rango.Reina => "Q",
            Rango.Rey => "K",
            _ => ((int)Rango).ToString()
        };

        /** Puntos base: las figuras valen 10 y el As 1 (Mano decide si cuenta como 11). */
        public int Puntos => Math.Min((int)Rango, 10);

        /** Indica si la carta es un As. */
        public bool EsAs => Rango == Rango.As;

        /**
         * @brief Nombre de la carta para el historial.
         * @return Texto como "A de Picas"
         */
        public override string ToString() => $"{Simbolo} de {Palo}";
    }
}
