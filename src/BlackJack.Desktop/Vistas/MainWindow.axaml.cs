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
using BlackJack.Core.Cuentas;
using BlackJack.Core.Juego;
using BlackJack.Core.Sesion;

namespace BlackJack.Desktop.Vistas
{
    /**
     * @brief Mesa de blackjack: dibuja el estado de la sesión y traduce la entrada del usuario en jugadas.
     */
    public partial class MainWindow : Window
    {
        /** Tipos de mensaje del aviso central (definen su color). */
        private enum TipoMensaje { Info, Victoria, Blackjack, Empate, Derrota, Error }

        /** Sesión de juego del jugador. */
        private readonly SesionJuego sesion;
        /** Navegación de vuelta al inicio de sesión. */
        private readonly INavegacion navegacion;

        /** Atajo a la mesa de la sesión. */
        private MesaBlackjack Mesa => sesion.Mesa;

        /**
         * @brief Constructor para el diseñador de Avalonia; la aplicación usa el que recibe dependencias.
         */
        public MainWindow() => InitializeComponent();

        /**
         * @brief Crea la mesa de un jugador.
         * @param sesion Sesión de juego abierta
         * @param navegacion Navegación de vuelta al inicio de sesión
         *
         * En el campo de apuesta solo se aceptan dígitos y Enter hace la apuesta.
         */
        public MainWindow(SesionJuego sesion, INavegacion navegacion)
        {
            InitializeComponent();
            this.sesion = sesion;
            this.navegacion = navegacion;

            Title = $"BlackJack - {sesion.Cuenta.Usuario}";
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
            Closing += (_, _) => sesion.Cerrar();

            MostrarMensaje(Mesa.Jugador.Saldo > 0
                ? $"Bienvenido, {sesion.Cuenta.Usuario}. Haz tu apuesta para comenzar."
                : $"Bienvenido, {sesion.Cuenta.Usuario}. No tienes saldo: recarga para seguir jugando.", TipoMensaje.Info);
            Refrescar();
        }

        /**
         * @brief Apuesta la cantidad escrita y reparte las cartas.
         */
        private void Apostar()
        {
            if (Mesa.Fase != FaseJuego.Apuesta) return;

            if (!int.TryParse(txtApuesta.Text, out int cantidad) || cantidad <= 0)
            {
                MostrarMensaje("Ingresa una cantidad válida para apostar.", TipoMensaje.Error);
                EnfocarApuesta();
                return;
            }

            if (!Ejecutar(() => sesion.Apostar(cantidad)))
            {
                EnfocarApuesta();
                return;
            }

            if (Mesa.Fase == FaseJuego.TurnoJugador)
                MostrarMensaje("¿Pides carta, te plantas o doblas?", TipoMensaje.Info);
            DespuesDeJugada();
        }

        /**
         * @brief Ejecuta una jugada del turno.
         * @param jugada Jugada de la sesión (pedir, plantarse o doblar)
         */
        private void Jugar(Action jugada)
        {
            if (Mesa.Fase != FaseJuego.TurnoJugador) return;
            if (Ejecutar(jugada))
                DespuesDeJugada();
        }

        /**
         * @brief Ejecuta una acción de la sesión y muestra el error si falla.
         * @param accion Acción a ejecutar
         * @return true si terminó sin errores
         *
         * Los errores de reglas (InvalidOperationException) y de guardado (E/S) se muestran en el aviso central.
         */
        private bool Ejecutar(Action accion)
        {
            try
            {
                accion();
                return true;
            }
            catch (Exception ex) when (ex is InvalidOperationException or IOException or UnauthorizedAccessException)
            {
                MostrarMensaje(ex.Message, TipoMensaje.Error);
                return false;
            }
        }

        /**
         * @brief Si la ronda terminó muestra el resultado; luego actualiza la mesa.
         */
        private void DespuesDeJugada()
        {
            if (sesion.RondaTerminada)
                MostrarResultado();
            Refrescar();
        }

        /**
         * @brief Muestra el resultado de la ronda en el aviso central.
         */
        private void MostrarResultado()
        {
            int jugador = Mesa.Jugador.Mano.Valor.Total;
            int banca = Mesa.Banca.Valor.Total;
            int monto = Math.Abs(Mesa.GananciaNeta);

            switch (Mesa.Resultado)
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
                    MostrarMensaje(Mesa.Jugador.Mano.EsBlackjack && Mesa.Banca.EsBlackjack
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

            if (Mesa.Jugador.Saldo == 0)
                lblMensaje.Text += $"\nTe quedaste sin saldo: usa \"Recargar ${ServicioCuentas.SaldoInicial}\".";
        }

        /**
         * @brief Actualiza saldo, cartas, valores y botones según la fase de la ronda.
         */
        private void Refrescar()
        {
            var jugador = Mesa.Jugador;
            var cuenta = sesion.Cuenta;
            bool jugando = Mesa.Fase == FaseJuego.TurnoJugador;

            lblUsuario.Text = cuenta.Usuario;
            lblEstadisticas.Text = $"Ganadas {cuenta.Ganadas}  ·  Perdidas {cuenta.Perdidas}  ·  Empates {cuenta.Empatadas}";
            lblSaldo.Text = $"${jugador.Saldo}";

            MostrarCartas(panelCartasJugador, jugador.Mano.Cartas, ocultarSegunda: false);
            MostrarCartas(panelCartasBanca, Mesa.Banca.Cartas, ocultarSegunda: Mesa.OcultarCartaBanca);

            MostrarValor(bordeValorJugador, lblValorJugador, jugador.Mano.Cartas, mostrarSuave: jugando);
            if (Mesa.OcultarCartaBanca)
                MostrarValor(bordeValorBanca, lblValorBanca, Mesa.Banca.Cartas.Take(1), mostrarSuave: false, sufijo: " + ?");
            else
                MostrarValor(bordeValorBanca, lblValorBanca, Mesa.Banca.Cartas, mostrarSuave: false);

            lblApuestaEnJuego.Text = jugando ? $"Apuesta: ${jugador.ApuestaActual}" : "";

            panelApuesta.IsVisible = !jugando;
            panelJuego.IsVisible = jugando;
            btnDoblar.IsEnabled = Mesa.PuedeDoblar;
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
        private static void MostrarCartas(Panel panel, IReadOnlyList<Carta> cartas, bool ocultarSegunda)
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

            var (total, suave) = ValorMano.De(lista);
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
            if (Mesa.Fase == FaseJuego.TurnoJugador)
            {
                switch (e.Key)
                {
                    case Key.P: Jugar(sesion.PedirCarta); e.Handled = true; break;
                    case Key.S: Jugar(sesion.Plantarse); e.Handled = true; break;
                    case Key.D when Mesa.PuedeDoblar: Jugar(sesion.Doblar); e.Handled = true; break;
                }
            }
            else if (e.Key == Key.Enter)
            {
                Apostar();
                e.Handled = true;
            }
        }

        /** @brief Botón Apostar. */
        private void btnApostar_Click(object sender, RoutedEventArgs e) => Apostar();

        /** @brief Botón Pedir carta. */
        private void btnPedir_Click(object sender, RoutedEventArgs e) => Jugar(sesion.PedirCarta);

        /** @brief Botón Plantarse. */
        private void btnPlantarse_Click(object sender, RoutedEventArgs e) => Jugar(sesion.Plantarse);

        /** @brief Botón Doblar. */
        private void btnDoblar_Click(object sender, RoutedEventArgs e) => Jugar(sesion.Doblar);

        /**
         * @brief Suma el valor de la ficha a la apuesta, sin pasar del saldo.
         * @param sender Ficha
         * @param e Evento
         */
        private void Ficha_Click(object sender, RoutedEventArgs e)
        {
            int ficha = int.Parse((string)((Button)sender).Tag);
            int.TryParse(txtApuesta.Text, out int actual);
            txtApuesta.Text = Math.Min((long)actual + ficha, Mesa.Jugador.Saldo).ToString();
            txtApuesta.CaretIndex = txtApuesta.Text.Length;
            EnfocarApuesta();
        }

        /** @brief Borra la apuesta escrita. */
        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtApuesta.Text = "";
            EnfocarApuesta();
        }

        /** @brief Recarga el saldo inicial cuando el jugador se queda sin dinero. */
        private void btnRecargar_Click(object sender, RoutedEventArgs e)
        {
            if (!Ejecutar(sesion.Recargar)) return;
            MostrarMensaje($"Recargaste ${ServicioCuentas.SaldoInicial}. ¡Suerte!", TipoMensaje.Info);
            Refrescar();
        }

        /** @brief Cierra la sesión y vuelve al inicio de sesión. */
        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e) =>
            navegacion.AbrirInicioSesion(this);
    }
}
