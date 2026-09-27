/**
 * @file LoginWindow.axaml.cs
 * @brief Ventana de inicio de sesión.
 * @author Santiago Caicedo
 */
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using BlackJack.Core.Cuentas;

namespace BlackJack.Desktop.Vistas
{
    /**
     * @brief Inicio de sesión y creación de cuentas de jugador.
     */
    public partial class LoginWindow : Window
    {
        /** Registro e inicio de sesión. */
        private readonly ServicioCuentas cuentas;
        /** Cambio de ventana al entrar. */
        private readonly INavegacion navegacion;

        /**
         * @brief Constructor para el diseñador de Avalonia; la aplicación usa el que recibe dependencias.
         */
        public LoginWindow() => InitializeComponent();

        /**
         * @brief Crea la ventana.
         * @param cuentas Registro e inicio de sesión
         * @param navegacion Navegación hacia la mesa
         *
         * Enter en el usuario pasa a la contraseña; Enter en la contraseña inicia sesión.
         */
        public LoginWindow(ServicioCuentas cuentas, INavegacion navegacion)
        {
            InitializeComponent();
            this.cuentas = cuentas;
            this.navegacion = navegacion;
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
                AbrirMesa(cuentas.Registrar(txtUsuario.Text, txtClave.Text));
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
                var cuenta = cuentas.IniciarSesion(txtUsuario.Text, txtClave.Text);
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
        private void AbrirMesa(CuentaJugador cuenta) => navegacion.AbrirMesa(this, cuenta);

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
