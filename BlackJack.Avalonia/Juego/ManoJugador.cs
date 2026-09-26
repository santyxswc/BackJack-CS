/**
 * @file ManoJugador.cs
 * @brief Mano de la banca.
 * @author Santiago Caicedo
 */
namespace BlackjackAvalonia.Juego
{
    /**
     * @brief Cartas de la banca.
     */
    public class ManoJugador
    {
        /** Cartas de la mano. */
        public List<Carta> Cartas { get; } = new List<Carta>();

        /**
         * @brief Agrega una carta a la mano.
         * @param carta Carta recibida
         */
        public void PedirCarta(Carta carta)
        {
            Cartas.Add(carta);
        }
    }
}
