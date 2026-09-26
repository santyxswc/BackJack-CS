/**
 * @file MainWindow.axaml.cs
 * @brief Mesa de juego.
 * @author Santiago Caicedo
 */
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using BlackjackAvalonia.Datos;
using BlackjackAvalonia.Juego;

namespace BlackjackAvalonia
{
    /**
     * @brief Mesa de blackjack: apuestas, cartas, resultados y saldo del jugador.
     */
    public partial class MainWindow : Window
    {
        /** Tipos de mensaje del aviso central (definen su color). */
        private enum TipoMensaje { Info, Victoria, Blackjack, Empate, Derrota, Error }

        /** Cuenta del jugador. */
        private readonly CuentaJugador cuenta;
        /** Donde se guarda la cuenta. */
        private readonly RepositorioJugadores repositorio;
        /** Reglas y estado de la ronda. */
        private readonly MesaBlackjack mesa;
        /** Historial de la sesión. */
        private readonly Historial historial;

        /**
         * @brief Crea una mesa de prueba con una cuenta de invitado.
         */
        public MainWindow() : this(new CuentaJugador { Usuario = "invitado", Saldo = RepositorioJugadores.SaldoInicial }, new RepositorioJugadores())
        {
        }

        /**
         * @brief Crea la mesa de un jugador.
         * @param cuenta Cuenta del jugador
         * @param repositorio Donde se guarda la cuenta
         *
         * En el campo de apuesta solo se aceptan dígitos y Enter hace la apuesta.
         */
        public MainWindow(CuentaJugador cuenta, RepositorioJugadores repositorio)
        {
            InitializeComponent();
            this.cuenta = cuenta;
            this.repositorio = repositorio;

            mesa = new MesaBlackjack(new Jugador(cuenta.Saldo));
            historial = new Historial(cuenta.Usuario, cuenta.Saldo);

            Title = $"BlackJack - {cuenta.Usuario}";
            imgFondo.Source = Recursos.Fondo;

            txtApuesta.AddHandler(TextInputEvent, (_, e) =>
            {
                if (e.Text != null && !e.Text.All(char.IsDigit)) e.Handled = true;
            }, RoutingStrategies.Tunnel);
            txtApuesta.KeyDown += (_, e) =>
            {
                if (e.Key == Key.Enter) { Apostar(); e.Handled = true; }
            };

            KeyDown += Ventana_KeyDown;
            Closing += (_, _) => AlCerrar();

            MostrarMensaje(cuenta.Saldo > 0
                ? $"Bienvenido, {cuenta.Usuario}. Haz tu apuesta para comenzar."
                : $"Bienvenido, {cuenta.Usuario}. No tienes saldo: recarga para seguir jugando.", TipoMensaje.Info);
            Refrescar();
        }

        /**
         * @brief Apuesta la cantidad escrita y reparte las cartas.
         */
        private void Apostar()
        {
            if (mesa.Fase != FaseJuego.Apuesta) return;

            if (!int.TryParse(txtApuesta.Text, out int cantidad) || cantidad <= 0)
            {
                MostrarMensaje("Ingresa una cantidad válida para apostar.", TipoMensaje.Error);
                EnfocarApuesta();
                return;
            }

            try
            {
                mesa.Apostar(cantidad);
            }
            catch (InvalidOperationException ex)
            {
                MostrarMensaje(ex.Message, TipoMensaje.Error);
                EnfocarApuesta();
                return;
            }

            historial.Escribir($"Apuesta realizada: ${cantidad}");
            historial.Escribir($"Cartas del jugador: {string.Join(", ", mesa.Jugador.Cartas)}");
            Guardar();

            if (mesa.Fase == FaseJuego.TurnoJugador)
                MostrarMensaje("¿Pides carta, te plantas o doblas?", TipoMensaje.Info);

            DespuesDeJugada();
        }

        /**
         * @brief Ejecuta una jugada del turno y la registra.
         * @param accion Jugada (pedir, plantarse o doblar)
         * @param descripcion Texto para el historial
         */
        private void Jugar(Action accion, string descripcion)
        {
            if (mesa.Fase != FaseJuego.TurnoJugador) return;

            try
            {
                accion();
            }
            catch (InvalidOperationException ex)
            {
                MostrarMensaje(ex.Message, TipoMensaje.Error);
                return;
            }

            historial.Escribir(descripcion);
            historial.Escribir($"Cartas del jugador: {string.Join(", ", mesa.Jugador.Cartas)}");
            DespuesDeJugada();
        }

        /**
         * @brief Si la ronda terminó, registra y muestra el resultado; luego actualiza la mesa.
         */
        private void DespuesDeJugada()
        {
            if (mesa.Fase == FaseJuego.Apuesta && mesa.Resultado.HasValue)
            {
                RegistrarResultado();
                MostrarResultado();
            }
            Refrescar();
        }

        /**
         * @brief Actualiza las estadísticas, escribe el historial y guarda la cuenta.
         */
        private void RegistrarResultado()
        {
            switch (mesa.Resultado)
            {
                case ResultadoRonda.BlackjackJugador:
                case ResultadoRonda.GanaJugador:
                case ResultadoRonda.BancaSePasa:
                    cuenta.Ganadas++;
                    break;
                case ResultadoRonda.Empate:
                    cuenta.Empatadas++;
                    break;
                default:
                    cuenta.Perdidas++;
                    break;
            }

            historial.Escribir($"Cartas de la banca: {string.Join(", ", mesa.Banca.Cartas)}");
            historial.Escribir($"Resultado de la ronda: {mesa.Resultado} ({mesa.GananciaNeta:+#;-#;0})");
            historial.Escribir($"Saldo después de la ronda: ${mesa.Jugador.Saldo}");
            historial.Escribir("--------------------------------------------------");
            Guardar();
        }

        /**
         * @brief Muestra el resultado de la ronda en el aviso central.
         */
        private void MostrarResultado()
        {
            int jugador = MesaBlackjack.Valor(mesa.Jugador.Cartas).Total;
            int banca = MesaBlackjack.Valor(mesa.Banca.Cartas).Total;
            int monto = Math.Abs(mesa.GananciaNeta);

            switch (mesa.Resultado)
            {
                case ResultadoRonda.BlackjackJugador:
                    MostrarMensaje($"¡Blackjack! Ganas ${monto}", TipoMensaje.Blackjack);
                    break;
                case ResultadoRonda.GanaJugador:
                    MostrarMensaje($"¡Ganaste con {jugador} contra {banca}! +${monto}", TipoMensaje.Victoria);
                    break;
                case ResultadoRonda.BancaSePasa:
                    MostrarMensaje($"La banca se pasó con {banca}. ¡Ganas ${monto}!", TipoMensaje.Victoria);
                    break;
                case ResultadoRonda.Empate:
                    MostrarMensaje(jugador == 21 && mesa.Jugador.Cartas.Count == 2 && mesa.Banca.Cartas.Count == 2
                        ? "Ambos tienen Blackjack: empate. Recuperas tu apuesta."
                        : $"Empate a {jugador}. Recuperas tu apuesta.", TipoMensaje.Empate);
                    break;
                case ResultadoRonda.JugadorSePasa:
                    MostrarMensaje($"Te pasaste con {jugador}. Pierdes ${monto}", TipoMensaje.Derrota);
                    break;
                case ResultadoRonda.GanaBanca:
                    MostrarMensaje($"La banca gana con {banca} contra {jugador}. Pierdes ${monto}", TipoMensaje.Derrota);
                    break;
                case ResultadoRonda.BlackjackBanca:
                    MostrarMensaje($"La banca tiene Blackjack. Pierdes ${monto}", TipoMensaje.Derrota);
                    break;
            }

            if (mesa.Jugador.Saldo == 0)
                lblMensaje.Text += "\nTe quedaste sin saldo: usa \"Recargar $1000\".";
        }

        /**
         * @brief Guarda el saldo y las estadísticas de la cuenta.
         */
        private void Guardar()
        {
            cuenta.Saldo = mesa.Jugador.Saldo;
            try
            {
                repositorio.Guardar(cuenta);
            }
            catch (Exception ex)
            {
                MostrarMensaje($"No se pudo guardar el progreso: {ex.Message}", TipoMensaje.Error);
            }
        }

        /**
         * @brief Al cerrar la ventana en medio de una mano, el jugador se planta; luego se guarda todo.
         */
        private void AlCerrar()
        {
            if (mesa.Fase == FaseJuego.TurnoJugador)
            {
                historial.Escribir("Ventana cerrada durante la mano: el jugador se planta.");
                mesa.Plantarse();
                RegistrarResultado();
            }

            Guardar();
            historial.Escribir("Sesión terminada.");
            historial.Dispose();
        }

        /**
         * @brief Actualiza saldo, cartas, valores y botones según la fase de la ronda.
         */
        private void Refrescar()
        {
            var jugador = mesa.Jugador;
            bool jugando = mesa.Fase == FaseJuego.TurnoJugador;

            lblUsuario.Text = cuenta.Usuario;
            lblEstadisticas.Text = $"Ganadas {cuenta.Ganadas}  ·  Perdidas {cuenta.Perdidas}  ·  Empates {cuenta.Empatadas}";
            lblSaldo.Text = $"${jugador.Saldo}";

            MostrarCartas(panelCartasJugador, jugador.Cartas, ocultarSegunda: false);
            MostrarCartas(panelCartasBanca, mesa.Banca.Cartas, ocultarSegunda: mesa.OcultarCartaBanca);

            MostrarValor(bordeValorJugador, lblValorJugador, jugador.Cartas, mostrarSuave: jugando);
            if (mesa.OcultarCartaBanca)
                MostrarValor(bordeValorBanca, lblValorBanca, mesa.Banca.Cartas.Take(1), mostrarSuave: false, sufijo: " + ?");
            else
                MostrarValor(bordeValorBanca, lblValorBanca, mesa.Banca.Cartas, mostrarSuave: false);

            lblApuestaEnJuego.Text = jugando ? $"Apuesta: ${jugador.ApuestaActual}" : "";

            panelApuesta.IsVisible = !jugando;
            panelJuego.IsVisible = jugando;
            btnDoblar.IsEnabled = mesa.PuedeDoblar;
            btnRecargar.IsVisible = !jugando && jugador.Saldo == 0;
            btnCerrarSesion.IsEnabled = !jugando;

            if (!jugando) EnfocarApuesta();
        }

        /**
         * @brief Dibuja las cartas de una mano.
         * @param panel Panel donde se dibujan
         * @param cartas Cartas
         * @param ocultarSegunda true para mostrar la segunda carta boca abajo
         */
        private static void MostrarCartas(Panel panel, List<Carta> cartas, bool ocultarSegunda)
        {
            panel.Children.Clear();
            for (int i = 0; i < cartas.Count; i++)
            {
                var imagen = ocultarSegunda && i == 1 ? Recursos.Dorso : Recursos.ImagenCarta(cartas[i]);
                panel.Children.Add(CrearCarta(imagen, primera: i == 0));
            }
        }

        /**
         * @brief Crea el control de una carta con sombra y esquinas redondeadas.
         * @param imagen Imagen de la carta
         * @param primera true si es la primera de la mano
         * @return Control de la carta
         *
         * Las cartas se superponen como un abanico, dejando visible la esquina con el valor.
         */
        private static Control CrearCarta(Bitmap imagen, bool primera) => new Border
        {
            Width = 100,
            Height = 140,
            CornerRadius = new CornerRadius(7),
            BoxShadow = BoxShadows.Parse("-2 3 10 0 #99000000"),
            Margin = new Thickness(primera ? 0 : -36, 0, 0, 0),
            Child = new Border
            {
                CornerRadius = new CornerRadius(7),
                ClipToBounds = true,
                Background = Brushes.White,
                Child = new Image { Source = imagen, Stretch = Stretch.Fill }
            }
        };

        /**
         * @brief Muestra el valor de una mano.
         * @param borde Etiqueta del valor
         * @param etiqueta Texto del valor
         * @param cartas Cartas visibles
         * @param mostrarSuave true para mostrar las dos opciones de una mano suave ("7/17")
         * @param sufijo Texto al final, como " + ?" para la carta oculta de la banca
         */
        private static void MostrarValor(Border borde, TextBlock etiqueta, IEnumerable<Carta> cartas, bool mostrarSuave, string sufijo = "")
        {
            var lista = cartas.ToList();
            borde.IsVisible = lista.Count > 0;

            var (total, suave) = MesaBlackjack.Valor(lista);
            etiqueta.Text = (mostrarSuave && suave && total < 21 ? $"{total - 10}/{total}" : total.ToString()) + sufijo;
        }

        /**
         * @brief Muestra un mensaje en el aviso central.
         * @param texto Mensaje
         * @param tipo Tipo, que define el color
         */
        private void MostrarMensaje(string texto, TipoMensaje tipo)
        {
            lblMensaje.Text = texto;
            (string fondo, string letra) = tipo switch
            {
                TipoMensaje.Victoria => ("#E600b894", "#FFFFFF"),
                TipoMensaje.Blackjack => ("#F2f1c40f", "#2d3436"),
                TipoMensaje.Empate => ("#E6636e72", "#FFFFFF"),
                TipoMensaje.Derrota => ("#E6c0392b", "#FFFFFF"),
                TipoMensaje.Error => ("#E6c0392b", "#FFFFFF"),
                _ => ("#CC101418", "#FFFFFF")
            };
            bannerMensaje.Background = Brush.Parse(fondo);
            lblMensaje.Foreground = Brush.Parse(letra);
        }

        /**
         * @brief Pone el cursor en el campo de apuesta.
         */
        private void EnfocarApuesta() =>
            Dispatcher.UIThread.Post(() => txtApuesta.Focus(), DispatcherPriority.Input);

        /**
         * @brief Atajos de teclado: P pedir, S plantarse, D doblar y Enter apostar.
         * @param sender Ventana
         * @param e Tecla
         */
        private void Ventana_KeyDown(object sender, KeyEventArgs e)
        {
            if (mesa.Fase == FaseJuego.TurnoJugador)
            {
                switch (e.Key)
                {
                    case Key.P: btnPedir_Click(null, null); e.Handled = true; break;
                    case Key.S: btnPlantarse_Click(null, null); e.Handled = true; break;
                    case Key.D when mesa.PuedeDoblar: btnDoblar_Click(null, null); e.Handled = true; break;
                }
            }
            else if (e.Key == Key.Enter)
            {
                Apostar();
                e.Handled = true;
            }
        }

        /**
         * @brief Botón Apostar.
         * @param sender Botón
         * @param e Evento
         */
        private void btnApostar_Click(object sender, RoutedEventArgs e) => Apostar();

        /**
         * @brief Botón Pedir carta.
         * @param sender Botón
         * @param e Evento
         */
        private void btnPedir_Click(object sender, RoutedEventArgs e) =>
            Jugar(mesa.PedirCarta, "El jugador pidió una carta.");

        /**
         * @brief Botón Plantarse.
         * @param sender Botón
         * @param e Evento
         */
        private void btnPlantarse_Click(object sender, RoutedEventArgs e) =>
            Jugar(mesa.Plantarse, "El jugador se plantó.");

        /**
         * @brief Botón Doblar.
         * @param sender Botón
         * @param e Evento
         */
        private void btnDoblar_Click(object sender, RoutedEventArgs e) =>
            Jugar(mesa.Doblar, "El jugador dobló la apuesta.");

        /**
         * @brief Suma el valor de la ficha a la apuesta, sin pasar del saldo.
         * @param sender Ficha
         * @param e Evento
         */
        private void Ficha_Click(object sender, RoutedEventArgs e)
        {
            int ficha = int.Parse((string)((Button)sender).Tag);
            int.TryParse(txtApuesta.Text, out int actual);
            txtApuesta.Text = Math.Min((long)actual + ficha, mesa.Jugador.Saldo).ToString();
            txtApuesta.CaretIndex = txtApuesta.Text.Length;
            EnfocarApuesta();
        }

        /**
         * @brief Borra la apuesta escrita.
         * @param sender Botón
         * @param e Evento
         */
        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtApuesta.Text = "";
            EnfocarApuesta();
        }

        /**
         * @brief Recarga el saldo inicial cuando el jugador se queda sin dinero.
         * @param sender Botón
         * @param e Evento
         */
        private void btnRecargar_Click(object sender, RoutedEventArgs e)
        {
            mesa.Jugador.Depositar(RepositorioJugadores.SaldoInicial);
            historial.Escribir($"Recarga de saldo: ${RepositorioJugadores.SaldoInicial}");
            Guardar();
            MostrarMensaje($"Recargaste ${RepositorioJugadores.SaldoInicial}. ¡Suerte!", TipoMensaje.Info);
            Refrescar();
        }

        /**
         * @brief Cierra la sesión y vuelve al inicio de sesión.
         * @param sender Botón
         * @param e Evento
         */
        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e) =>
            App.CambiarVentana(this, new LoginWindow(repositorio));
    }
}
