using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using BlackjackAvalonia.Datos;

namespace BlackjackAvalonia
{
    public partial class LoginWindow : Window
    {
        private readonly RepositorioJugadores repositorio;

        public LoginWindow() : this(new RepositorioJugadores())
        {
        }

        public LoginWindow(RepositorioJugadores repositorio)
        {
            InitializeComponent();
            this.repositorio = repositorio;
            imgFondo.Source = Recursos.Fondo;

            // Enter en el usuario pasa a la contraseña; Enter en la contraseña inicia sesion
            txtUsuario.KeyDown += (_, e) =>
            {
                if (e.Key == Key.Enter) { txtClave.Focus(); e.Handled = true; }
            };
            txtClave.KeyDown += (_, e) =>
            {
                if (e.Key == Key.Enter) { Entrar(); e.Handled = true; }
            };
            Opened += (_, _) => txtUsuario.Focus();
        }

        private void btnEntrar_Click(object sender, RoutedEventArgs e) => Entrar();

        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AbrirMesa(repositorio.Registrar(txtUsuario.Text, txtClave.Text));
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
            }
        }

        private void Entrar()
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrEmpty(txtClave.Text))
            {
                MostrarError("Ingresa usuario y contraseña.");
                return;
            }

            try
            {
                var cuenta = repositorio.IniciarSesion(txtUsuario.Text, txtClave.Text);
                if (cuenta == null)
                {
                    MostrarError("Usuario o contraseña incorrectos.");
                    txtClave.Text = "";
                    txtClave.Focus();
                    return;
                }
                AbrirMesa(cuenta);
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
            }
        }

        private void AbrirMesa(CuentaJugador cuenta) =>
            App.CambiarVentana(this, new MainWindow(cuenta, repositorio));

        private void MostrarError(string mensaje)
        {
            lblError.Text = mensaje;
            bordeError.IsVisible = true;
        }
    }
}
