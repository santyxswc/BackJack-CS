using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BlackjackAvalonia.Datos;

namespace BlackjackAvalonia
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new LoginWindow(new RepositorioJugadores());
            }

            base.OnFrameworkInitializationCompleted();
        }

        // Reemplaza la ventana principal (login <-> mesa) sin que la aplicacion se cierre
        public static void CambiarVentana(Avalonia.Controls.Window actual, Avalonia.Controls.Window nueva)
        {
            if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = nueva;

            nueva.Show();
            actual.Close();
        }
    }
}
