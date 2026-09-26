/**
 * @file App.axaml.cs
 * @brief Aplicación Avalonia.
 * @author Santiago Caicedo
 */
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BlackjackAvalonia.Datos;

namespace BlackjackAvalonia
{
    /**
     * @brief Carga los estilos globales y abre el inicio de sesión.
     */
    public partial class App : Application
    {
        /**
         * @brief Carga App.axaml (estilos de botones, fichas y campos).
         */
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /**
         * @brief Abre la ventana de inicio de sesión.
         */
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new LoginWindow(new RepositorioJugadores());
            }

            base.OnFrameworkInitializationCompleted();
        }

        /**
         * @brief Reemplaza la ventana principal sin cerrar la aplicación.
         * @param actual Ventana que se cierra
         * @param nueva Ventana que se abre
         *
         * Se usa para pasar del inicio de sesión a la mesa y viceversa.
         */
        public static void CambiarVentana(Avalonia.Controls.Window actual, Avalonia.Controls.Window nueva)
        {
            if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = nueva;

            nueva.Show();
            actual.Close();
        }
    }
}
