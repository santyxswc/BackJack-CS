/**
 * @file IHistorialPartida.cs
 * @brief Contrato del registro de una sesión de juego.
 * @author Santiago Caicedo
 */
namespace BlackJack.Core.Sesion
{
    /**
     * @brief Destino de las líneas del historial de una sesión.
     */
    public interface IHistorialPartida : IDisposable
    {
        /**
         * @brief Agrega una línea al historial.
         * @param linea Texto a escribir
         */
        void Escribir(string linea);
    }
}
