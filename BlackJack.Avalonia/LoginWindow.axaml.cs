/**
 * @file LoginWindow.axaml.cs
 * @brief Ventana de inicio de sesión.
 * @author Santiago Caicedo
 */
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using BlackjackAvalonia.Datos;

namespace BlackjackAvalonia
{
    /**
     * @brief Inicio de sesión y creación de cuentas de jugador.
     */
    public partial class LoginWindow : Window
    {
        /** Cuentas de los jugadores. */
        private readonly RepositorioJugadores repositorio;

        /**
         * @brief Crea la ventana con el repositorio por defecto.
         */
        public LoginWindow() : this(new RepositorioJugadores())
        {
        }

        /**
         * @brief Crea la ventana.
         * @param repositorio Cuentas de los jugadores
         *
         * Enter en el usuario pasa a la contraseña; Enter en la contraseña inicia sesión.
         */
        public LoginWindow(RepositorioJugadores repositorio)
        {
            InitializeComponent();
            this.repositorio = repositorio;
            imgFondo.Source = Recursos.Fondo;

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

        /**
         * @brief Botón Entrar.
         * @param sender Botón
         * @param e Evento
         */
        private void btnEntrar_Click(object sender, RoutedEventArgs e) => Entrar();

        /**
         * @brief Botón Crear cuenta nueva: registra la cuenta y abre la mesa.
         * @param sender Botón
         * @param e Evento
         */
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

        /**
         * @brief Verifica las credenciales y abre la mesa.
         */
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

        /**
         * @brief Abre la mesa del jugador y cierra esta ventana.
         * @param cuenta Cuenta del jugador
         */
        private void AbrirMesa(CuentaJugador cuenta) =>
            App.CambiarVentana(this, new MainWindow(cuenta, repositorio));

        /**
         * @brief Muestra un error debajo de los campos.
         * @param mensaje Mensaje
         */
        private void MostrarError(string mensaje)
        {
            lblError.Text = mensaje;
            bordeError.IsVisible = true;
        }
    }
}
