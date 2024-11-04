using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BlackjackForm
{
    public partial class Form1 : Form
    {
        private Baraja baraja;
        private Jugador jugador;  
        private ManoJugador banca;

        public Form1()
        {
            InitializeComponent();

            // Imagen de fondo
            this.btnApostar.Click += new System.EventHandler(this.btnApostar_Click);
            this.BackgroundImage = Image.FromFile(@"D:\Proyecto C#\BlackJack\imagenes\fondo.jpg");
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // Inicializar la baraja y las manos del jugador y la banca
            baraja = new Baraja();
            jugador = new Jugador(1000);  // Saldo inicial de 1000
            banca = new ManoJugador();
            ActualizarSaldo();
            IniciarJuego();
        }

        private void IniciarJuego()
        {
            baraja = new Baraja();
            jugador.Cartas.Clear();
            banca = new ManoJugador();

            // Reiniciar la apuesta actual al iniciar un nuevo juego
            jugador.ApuestaActual = 0;

            jugador.PedirCarta(baraja.RepartirCarta());
            jugador.PedirCarta(baraja.RepartirCarta());
            banca.PedirCarta(baraja.RepartirCarta());
            banca.PedirCarta(baraja.RepartirCarta());

            MostrarCartas();
        }


        private void MostrarCartas()
        {
            lblCartasJugador.Text = "Cartas del Jugador: ";
            lblCartasBanca.Text = "Cartas de la Banca: ";
            lblValorJugador.Text = "Valor del Jugador: " + jugador.CalcularValor();

            // Mostrar las cartas del jugador
            MostrarImagenesCartas(jugador.Cartas, panelCartasJugador);

            // Mostrar ambas cartas de la banca, pero la segunda como dorso
            MostrarImagenesCartas(banca.Cartas, panelCartasBanca, mostrarDorso: true);
        }

        private void MostrarImagenesCartas(List<Carta> cartas, FlowLayoutPanel panel, bool mostrarDorso = false)
        {
            panel.Controls.Clear(); // Limpiar las imágenes anteriores

            for (int i = 0; i < cartas.Count; i++)
            {
                PictureBox pictureBox = new PictureBox();
                pictureBox.Size = new Size(100, 150);

                if (mostrarDorso && i == 1)
                {
                    pictureBox.Image = CargarImagenDorso();
                }
                else
                {
                    pictureBox.Image = CargarImagenCarta(cartas[i]);
                }

                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                panel.Controls.Add(pictureBox);
            }
        }

        private Image CargarImagenDorso()
        {
            string rutaDorso = Path.Combine(@"D:\Proyecto C#\BlackJack\imagenes", "Dorso.jpg");

            if (File.Exists(rutaDorso))
            {
                return Image.FromFile(rutaDorso);
            }
            else
            {
                MessageBox.Show($"Imagen del dorso no encontrada: Dorso.jpg");
                return null;
            }
        }

        private Image CargarImagenCarta(Carta carta)
        {
            string nombreArchivo = $"{carta.Valor}_de_{carta.Palo}.jpg";
            string rutaImagen = Path.Combine(@"D:\Proyecto C#\BlackJack\imagenes", nombreArchivo);

            if (File.Exists(rutaImagen))
            {
                return Image.FromFile(rutaImagen);
            }
            else
            {
                MessageBox.Show($"Imagen no encontrada: {nombreArchivo}");
                return null;
            }
        }

        private void btnPedirCarta_Click(object sender, EventArgs e)
        {
            if (jugador.ApuestaActual <= 0)
            {
                MessageBox.Show("necesitas apostar antes de pedir una carta");
                return;
            }
            jugador.PedirCarta(baraja.RepartirCarta());
            MostrarCartas();
            jugador.OrdenarCartas();
            int valorJugador = jugador.CalcularValor();

            if (valorJugador > 21)
            {
                MessageBox.Show("¡Te pasaste de 21! La banca gana.");
                FinalizarRonda(false);  // El jugador pierde la apuesta
            }
        }


        private void btnRetirarse_Click(object sender, EventArgs e)
        {
            int valorBanca = banca.CalcularValor();
            while (valorBanca < 17)
            {
                banca.PedirCarta(baraja.RepartirCarta());
                valorBanca = banca.CalcularValor();
            }

            MostrarCartasFinal();
            int valorJugador = jugador.CalcularValor();

            bool jugadorGana = valorBanca > 21 || valorJugador > valorBanca;
            bool empate = valorJugador == valorBanca;

            if (jugadorGana)
            {
                MessageBox.Show("¡Has ganado!");
                jugador.GanarApuesta();
            }
            else if (empate)
            {
                MessageBox.Show("¡Empate!");
                jugador.EmpatarApuesta();
            }
            else
            {
                MessageBox.Show("La banca gana.");
                jugador.PerderApuesta();
            }

            FinalizarRonda(jugadorGana || empate);
        }





        private void FinalizarRonda(bool ganador)
        {
            if (ganador)
            {
                jugador.GanarApuesta(); // Aplica ganancias solo si es ganador
            }
            else if (jugador.ApuestaActual > 0)
            {
                jugador.PerderApuesta(); // Solo perderá si la apuesta es mayor a cero
            }

            ActualizarSaldo(); // Actualizar saldo después de cada ronda

            if (jugador.Saldo <= 0)
            {
                MessageBox.Show("No tienes saldo suficiente para continuar.");
                btnPedirCarta.Enabled = false;
                btnRetirarse.Enabled = false;
            }
            else
            {
                IniciarJuego();
            }
        }





        private void btnApostar_Click(object sender, EventArgs e)
        {
            // Aquí va la lógica para realizar la apuesta
            int cantidadApuesta;

            if (!int.TryParse(txtApuesta.Text, out cantidadApuesta) || cantidadApuesta <= 0)
            {
                MessageBox.Show("Por favor, ingresa una cantidad válida para apostar.");
                return;
            }

            try
            {
                jugador.HacerApuesta(cantidadApuesta);
                ActualizarSaldo();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void MostrarCartasFinal()
        {
            // Muestra el valor de las cartas del jugador
            lblValorJugador.Text = "Valor del Jugador: " + jugador.CalcularValor();

            // Mostrar todas las cartas de la banca, incluyendo la carta oculta
            lblValorBanca.Text = "Valor de la Banca: " + banca.CalcularValor();

            // Mostrar las cartas del jugador
            MostrarImagenesCartas(jugador.Cartas, panelCartasJugador);

            // Mostrar todas las cartas de la banca
            MostrarImagenesCartas(banca.Cartas, panelCartasBanca);
        }

        private void ActualizarSaldo()
        {
            lblSaldo.Text = "Saldo: $" + jugador.Saldo;
            apuesta.Text = "La apuesta actual es: $" + jugador.ApuestaActual * 2;
        }


        private void txtApuesta_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir solo dígitos y control de retroceso
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            IniciarJuego();
            ActualizarSaldo();
        }

        private void bindingNavigator1_RefreshItems(object sender, EventArgs e)
        {

        }

        private void panelCartasJugador_Paint(object sender, PaintEventArgs e)
        {

        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void panelCartasBanca_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblCartasBanca_Click(object sender, EventArgs e)
        {

        }

        private void lblCartasJugador_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void nudApuesta_ValueChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblSaldo_Click(object sender, EventArgs e)
        {

        }

        private void btnApostar_Click_1(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
