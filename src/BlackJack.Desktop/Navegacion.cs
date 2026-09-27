/**
 * @file Navegacion.cs
 * @brief Cambio entre la ventana de inicio de sesión y la mesa.
 * @author Santiago Caicedo
 */
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using BlackJack.Core.Cuentas;
using BlackJack.Core.Sesion;
using BlackJack.Desktop.Vistas;
using BlackJack.Infrastructure;

namespace BlackJack.Desktop
{
    /**
     * @brief Navegación entre ventanas; las vistas dependen de esta interfaz, no de cómo se crean las otras.
     */
    public interface INavegacion
    {
        /**
         * @brief Abre la mesa de un jugador en lugar de la ventana actual.
         * @param actual Ventana que se cierra
         * @param cuenta Cuenta del jugador
         */
        void AbrirMesa(Window actual, CuentaJugador cuenta);

        /**
         * @brief Vuelve al inicio de sesión.
         * @param actual Ventana que se cierra
         */
        void AbrirInicioSesion(Window actual);
    }

    /**
     * @brief Raíz de composición: crea los servicios una vez y construye cada ventana con sus dependencias.
     */
    public sealed class Navegacion : INavegacion
    {
        /** Ciclo de vida de la aplicación de escritorio. */
        private readonly IClassicDesktopStyleApplicationLifetime escritorio;
        /** Registro e inicio de sesión. */
        private readonly ServicioCuentas cuentas;

        /**
         * @brief Crea la navegación con los servicios de producción.
         * @param escritorio Ciclo de vida de Avalonia
         */
        public Navegacion(IClassicDesktopStyleApplicationLifetime escritorio)
        {
            this.escritorio = escritorio;
            cuentas = new ServicioCuentas(new RepositorioCuentasJson(RutasDatos.Carpeta), new HasherPbkdf2());
        }

        /** @brief Ventana inicial de la aplicación. */
        public Window CrearVentanaInicial() => new LoginWindow(cuentas, this);

        /** @copydoc INavegacion::AbrirMesa */
        public void AbrirMesa(Window actual, CuentaJugador cuenta)
        {
            var sesion = new SesionJuego(cuenta, cuentas, new HistorialArchivo(RutasDatos.CarpetaHistorial, cuenta.Usuario));
            Cambiar(actual, new MainWindow(sesion, this));
        }

        /** @copydoc INavegacion::AbrirInicioSesion */
        public void AbrirInicioSesion(Window actual) => Cambiar(actual, new LoginWindow(cuentas, this));

        /**
         * @brief Reemplaza la ventana principal sin cerrar la aplicación.
         * @param actual Ventana que se cierra
         * @param nueva Ventana que se abre
         */
        private void Cambiar(Window actual, Window nueva)
        {
            escritorio.MainWindow = nueva;
            nueva.Show();
            actual.Close();
        }
    }
}
