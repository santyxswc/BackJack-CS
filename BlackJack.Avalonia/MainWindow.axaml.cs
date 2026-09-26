using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using BlackjackAvalonia.Datos;
using BlackjackAvalonia.Juego;
using BlackjackForm;

namespace BlackjackAvalonia
{
    public partial class MainWindow : Window
    {
        private enum TipoMensaje { Info, Victoria, Blackjack, Empate, Derrota, Error }

        private readonly CuentaJugador cuenta;
        private readonly RepositorioJugadores repositorio;
        private readonly MesaBlackjack mesa;
        private readonly Historial historial;

        public MainWindow() : this(new CuentaJugador { Usuario = "invitado", Saldo = RepositorioJugadores.SaldoInicial }, new RepositorioJugadores())
        {
        }

        public MainWindow(CuentaJugador cuenta, RepositorioJugadores repositorio)
        {
            InitializeComponent();
            this.cuenta = cuenta;
            this.repositorio = repositorio;

            mesa = new MesaBlackjack(new Jugador(cuenta.Saldo));
            historial = new Historial(cuenta.Usuario, cuenta.Saldo);

            Title = $"BlackJack - {cuenta.Usuario}";
            imgFondo.Source = Recursos.Fondo;

            // Solo digitos en la apuesta; Enter apuesta
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

        // ---------- Acciones ----------

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
            Guardar(); // La apuesta ya salio del saldo

            if (mesa.Fase == FaseJuego.TurnoJugador)
                MostrarMensaje("¿Pides carta, te plantas o doblas?", TipoMensaje.Info);

            DespuesDeJugada();
        }

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

        private void DespuesDeJugada()
        {
            if (mesa.Fase == FaseJuego.Apuesta && mesa.Resultado.HasValue)
            {
                RegistrarResultado();
                MostrarResultado();
            }
            Refrescar();
        }

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

        // Si se cierra en medio de una mano, el jugador se planta con lo que tiene
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

        // ---------- Interfaz ----------

        private void Refrescar()
        {
            var jugador = mesa.Jugador;
            bool jugando = mesa.Fase == FaseJuego.TurnoJugador;

            lblUsuario.Text = cuenta.Usuario;
            lblEstadisticas.Text = $"Ganadas {cuenta.Ganadas}  ·  Perdidas {cuenta.Perdidas}  ·  Empates {cuenta.Empatadas}";
            lblSaldo.Text = $"${jugador.Saldo}";

            MostrarCartas(panelCartasJugador, jugador.Cartas, ocultarSegunda: false);
            MostrarCartas(panelCartasBanca, mesa.Banca.Cartas, ocultarSegunda: mesa.OcultarCartaBanca);

            // Los dos valores de una mano suave ("7/17") solo se muestran mientras el jugador decide
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

        private static void MostrarCartas(Panel panel, List<Carta> cartas, bool ocultarSegunda)
        {
            panel.Children.Clear();
            for (int i = 0; i < cartas.Count; i++)
            {
                var imagen = ocultarSegunda && i == 1 ? Recursos.Dorso : Recursos.ImagenCarta(cartas[i]);
                panel.Children.Add(CrearCarta(imagen, primera: i == 0));
            }
        }

        // Las cartas se superponen como un abanico, dejando visible la esquina con el valor
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

        private static void MostrarValor(Border borde, TextBlock etiqueta, IEnumerable<Carta> cartas, bool mostrarSuave, string sufijo = "")
        {
            var lista = cartas.ToList();
            borde.IsVisible = lista.Count > 0;

            var (total, suave) = MesaBlackjack.Valor(lista);
            etiqueta.Text = (mostrarSuave && suave && total < 21 ? $"{total - 10}/{total}" : total.ToString()) + sufijo;
        }

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

        private void EnfocarApuesta() =>
            Dispatcher.UIThread.Post(() => txtApuesta.Focus(), DispatcherPriority.Input);

        // ---------- Eventos ----------

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

        private void btnApostar_Click(object sender, RoutedEventArgs e) => Apostar();

        private void btnPedir_Click(object sender, RoutedEventArgs e) =>
            Jugar(mesa.PedirCarta, "El jugador pidió una carta.");

        private void btnPlantarse_Click(object sender, RoutedEventArgs e) =>
            Jugar(mesa.Plantarse, "El jugador se plantó.");

        private void btnDoblar_Click(object sender, RoutedEventArgs e) =>
            Jugar(mesa.Doblar, "El jugador dobló la apuesta.");

        private void Ficha_Click(object sender, RoutedEventArgs e)
        {
            int ficha = int.Parse((string)((Button)sender).Tag);
            int.TryParse(txtApuesta.Text, out int actual);
            txtApuesta.Text = Math.Min((long)actual + ficha, mesa.Jugador.Saldo).ToString();
            txtApuesta.CaretIndex = txtApuesta.Text.Length;
            EnfocarApuesta();
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtApuesta.Text = "";
            EnfocarApuesta();
        }

        private void btnRecargar_Click(object sender, RoutedEventArgs e)
        {
            mesa.Jugador.Depositar(RepositorioJugadores.SaldoInicial);
            historial.Escribir($"Recarga de saldo: ${RepositorioJugadores.SaldoInicial}");
            Guardar();
            MostrarMensaje($"Recargaste ${RepositorioJugadores.SaldoInicial}. ¡Suerte!", TipoMensaje.Info);
            Refrescar();
        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e) =>
            App.CambiarVentana(this, new LoginWindow(repositorio));
    }
}
