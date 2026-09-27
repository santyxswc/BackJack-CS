/**
 * @file App.axaml.cs
 * @brief Aplicación Avalonia.
 * @author Santiago Caicedo
 */
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace BlackJack.Desktop
{
    /**
     * @brief Carga los estilos globales y abre el inicio de sesión.
     */
    public partial class App : Application
    {
        /**
         * @brief Carga App.axaml (estilos de botones, fichas y campos).
         */
        public override void Initialize() => AvaloniaXamlLoader.Load(this);

        /**
         * @brief Compone los servicios y abre la ventana de inicio de sesión.
         */
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime escritorio)
                escritorio.MainWindow = new Navegacion(escritorio).CrearVentanaInicial();

            base.OnFrameworkInitializationCompleted();
        }
    }
}
