/**
 * @file SesionJuego.cs
 * @brief Caso de uso: una sesión de juego de un jugador en la mesa.
 * @author Santiago Caicedo
 */
using BlackJack.Core.Cuentas;
using BlackJack.Core.Juego;

namespace BlackJack.Core.Sesion
{
    /**
     * @brief Coordina la mesa, la cuenta del jugador y el historial durante una sesión.
     *
     * Cada jugada se aplica en la mesa y se registra en el historial; al terminar la ronda
     * se actualizan las estadísticas y se guarda la cuenta.
     */
    public sealed class SesionJuego : IDisposable
    {
        /** Separador entre rondas en el historial. */
        private const string Separador = "--------------------------------------------------";

        /** Servicio que guarda la cuenta. */
        private readonly ServicioCuentas cuentas;
        /** Historial de la sesión. */
        private readonly IHistorialPartida historial;
        /** true cuando la sesión ya se cerró. */
        private bool cerrada;

        /** Cuenta del jugador. */
        public CuentaJugador Cuenta { get; }
        /** Reglas y estado de la ronda. */
        public MesaBlackjack Mesa { get; }

        /**
         * @brief Abre la sesión y escribe el encabezado del historial.
         * @param cuenta Cuenta del jugador
         * @param cuentas Servicio para guardar la cuenta
         * @param historial Destino del historial
         * @param crearBaraja Baraja de cada ronda; por defecto, mezclada
         */
        public SesionJuego(CuentaJugador cuenta, ServicioCuentas cuentas, IHistorialPartida historial, Func<Baraja> crearBaraja = null)
        {
            Cuenta = cuenta ?? throw new ArgumentNullException(nameof(cuenta));
            this.cuentas = cuentas ?? throw new ArgumentNullException(nameof(cuentas));
            this.historial = historial ?? throw new ArgumentNullException(nameof(historial));
            Mesa = new MesaBlackjack(new Jugador(cuenta.Saldo), crearBaraja);

            historial.Escribir($"Jugador: {cuenta.Usuario}");
            historial.Escribir($"Fecha de inicio: {DateTime.Now}");
            historial.Escribir($"Saldo inicial: ${cuenta.Saldo}");
            historial.Escribir(Separador);
        }

        /** Indica si la última jugada terminó la ronda (hay resultado que mostrar). */
        public bool RondaTerminada => Mesa.Fase == FaseJuego.Apuesta && Mesa.Resultado.HasValue;

        /**
         * @brief Apuesta y reparte; guarda el saldo descontado.
         * @param cantidad Cantidad a apostar
         * @exception InvalidOperationException Si la apuesta no es válida
         */
        public void Apostar(int cantidad)
        {
            Mesa.Apostar(cantidad);
            historial.Escribir($"Apuesta realizada: ${cantidad}");
            EscribirCartasJugador();
            DespuesDeJugada(guardarSiempre: true);
        }

        /** @brief El jugador pide carta. */
        public void PedirCarta() => Jugar(Mesa.PedirCarta, "El jugador pidió una carta.");

        /** @brief El jugador se planta. */
        public void Plantarse() => Jugar(Mesa.Plantarse, "El jugador se plantó.");

        /** @brief El jugador dobla la apuesta. */
        public void Doblar() => Jugar(Mesa.Doblar, "El jugador dobló la apuesta.");

        /**
         * @brief Recarga el saldo inicial.
         * @exception InvalidOperationException Si hay una ronda en curso
         */
        public void Recargar()
        {
            if (Mesa.Fase != FaseJuego.Apuesta)
                throw new InvalidOperationException("Termina la ronda antes de recargar.");

            Mesa.Jugador.Depositar(ServicioCuentas.SaldoInicial);
            historial.Escribir($"Recarga de saldo: ${ServicioCuentas.SaldoInicial}");
            Guardar();
        }

        /**
         * @brief Cierra la sesión: si hay una mano en curso el jugador se planta; luego guarda todo.
         */
        public void Cerrar()
        {
            if (cerrada) return;
            cerrada = true;

            if (Mesa.Fase == FaseJuego.TurnoJugador)
            {
                historial.Escribir("Sesión cerrada durante la mano: el jugador se planta.");
                Mesa.Plantarse();
                RegistrarResultado();
            }

            Guardar();
            historial.Escribir("Sesión terminada.");
            historial.Dispose();
        }

        /** @brief Equivale a Cerrar(). */
        public void Dispose() => Cerrar();

        /**
         * @brief Ejecuta una jugada del turno y la registra.
         * @param accion Jugada de la mesa
         * @param descripcion Texto para el historial
         */
        private void Jugar(Action accion, string descripcion)
        {
            accion();
            historial.Escribir(descripcion);
            EscribirCartasJugador();
            DespuesDeJugada(guardarSiempre: false);
        }

        /**
         * @brief Si la ronda terminó, actualiza estadísticas e historial; guarda cuando corresponde.
         * @param guardarSiempre true para guardar aunque la ronda siga
         */
        private void DespuesDeJugada(bool guardarSiempre)
        {
            if (RondaTerminada)
                RegistrarResultado();
            else if (guardarSiempre)
                Guardar();
        }

        /**
         * @brief Suma el resultado a la cuenta, lo escribe en el historial y guarda.
         */
        private void RegistrarResultado()
        {
            Cuenta.RegistrarResultado(Mesa.Resultado.Value);
            historial.Escribir($"Cartas de la banca: {string.Join(", ", Mesa.Banca.Cartas)}");
            historial.Escribir($"Resultado de la ronda: {Mesa.Resultado} ({Mesa.GananciaNeta:+#;-#;0})");
            historial.Escribir($"Saldo después de la ronda: ${Mesa.Jugador.Saldo}");
            historial.Escribir(Separador);
            Guardar();
        }

        /** @brief Escribe la mano del jugador en el historial. */
        private void EscribirCartasJugador() =>
            historial.Escribir($"Cartas del jugador: {string.Join(", ", Mesa.Jugador.Mano.Cartas)}");

        /** @brief Copia el saldo de la mesa a la cuenta y la guarda. */
        private void Guardar()
        {
            Cuenta.Saldo = Mesa.Jugador.Saldo;
            cuentas.Guardar(Cuenta);
        }
    }
}
