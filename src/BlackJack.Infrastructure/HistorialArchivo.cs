/**
 * @file HistorialArchivo.cs
 * @brief Historial de una sesión en un archivo de texto.
 * @author Santiago Caicedo
 */
using BlackJack.Core.Sesion;

namespace BlackJack.Infrastructure
{
    /**
     * @brief Escribe el historial de una sesión en historial/partida_<usuario>_<fecha>.txt.
     */
    public sealed class HistorialArchivo : IHistorialPartida
    {
        /** Archivo de la sesión; null cuando está cerrado. */
        private StreamWriter writer;

        /**
         * @brief Crea el archivo de la sesión.
         * @param carpeta Carpeta de los historiales
         * @param usuario Jugador de la sesión
         */
        public HistorialArchivo(string carpeta, string usuario)
        {
            Directory.CreateDirectory(carpeta);
            string ruta = Path.Combine(carpeta, $"partida_{usuario}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");
            writer = new StreamWriter(ruta, append: true) { AutoFlush = true };
        }

        /**
         * @brief Agrega una línea al historial.
         * @param linea Texto a escribir
         */
        public void Escribir(string linea) => writer?.WriteLine(linea);

        /** @brief Cierra el archivo. */
        public void Dispose()
        {
            writer?.Dispose();
            writer = null;
        }
    }
}
