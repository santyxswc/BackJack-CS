namespace BlackjackAvalonia.Datos
{
    // Archivo de texto con lo que pasa en cada sesion de juego
    public sealed class Historial : IDisposable
    {
        private StreamWriter writer;

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

        public void Escribir(string linea) => writer?.WriteLine(linea);

        public void Dispose()
        {
            writer?.Dispose();
            writer = null;
        }
    }
}
