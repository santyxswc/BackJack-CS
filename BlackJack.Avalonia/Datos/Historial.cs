/**
 * @file Historial.cs
 * @brief Historial de partidas en archivos de texto.
 * @author Santiago Caicedo
 */
namespace BlackjackAvalonia.Datos
{
    /**
     * @brief Escribe en un archivo de texto lo que pasa en cada sesión de juego.
     */
    public sealed class Historial : IDisposable
    {
        /** Archivo de la sesión; null cuando está cerrado. */
        private StreamWriter writer;

        /**
         * @brief Crea el archivo de la sesión y escribe su encabezado.
         * @param usuario Jugador de la sesión
         * @param saldo Saldo inicial
         *
         * El archivo queda en la carpeta historial de los datos del juego.
         */
        public Historial(string usuario, int saldo)
        {
            string carpeta = Path.Combine(RepositorioJugadores.CarpetaDatos, "historial");
            Directory.CreateDirectory(carpeta);

            string rutaArchivo = Path.Combine(carpeta, $"partida_{usuario}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");
            writer = new StreamWriter(rutaArchivo, true) { AutoFlush = true };

            Escribir($"Jugador: {usuario}");
            Escribir($"Fecha de inicio: {DateTime.Now}");
            Escribir($"Saldo inicial: ${saldo}");
            Escribir("--------------------------------------------------");
        }

        /**
         * @brief Agrega una línea al historial.
         * @param linea Texto a escribir
         */
        public void Escribir(string linea) => writer?.WriteLine(linea);

        /**
         * @brief Cierra el archivo.
         */
        public void Dispose()
        {
            writer?.Dispose();
            writer = null;
        }
    }
}
