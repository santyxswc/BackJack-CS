/**
 * @file Program.cs
 * @brief Punto de entrada del juego.
 * @author Santiago Caicedo
 *
 * BlackJack de escritorio multiplataforma (Windows, Linux y macOS) hecho con Avalonia.
 */
using Avalonia;

namespace BlackJack.Desktop
{
    /**
     * @brief Arranque de la aplicación.
     */
    internal static class Program
    {
        /**
         * @brief Inicia Avalonia con la ventana de inicio de sesión.
         * @param args Argumentos de la línea de comandos
         */
        [STAThread]
        public static void Main(string[] args) =>
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

        /**
         * @brief Configura Avalonia: plataforma, fuente Inter y trazas.
         * @return Constructor de la aplicación
         */
        public static AppBuilder BuildAvaloniaApp() =>
            AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}
