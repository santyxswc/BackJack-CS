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
        private ManoJugador jugador;
        private ManoJugador banca;
        

        public Form1()
        {
            InitializeComponent();

            //imagen de fondo 
            this.BackgroundImage = Image.FromFile(@"D:\Proyecto C#\BlackJack\imagenes\fondo.jpg");
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // Inicializar la baraja y las manos del jugador y la banca
            baraja = new Baraja();
            jugador = new ManoJugador();
            banca = new ManoJugador();

            IniciarJuego();
        }

        private void IniciarJuego()
        {
            baraja = new Baraja();
            jugador = new ManoJugador();
            banca = new ManoJugador();

            // Repartir dos cartas al jugador y a la banca
            jugador.PedirCarta(baraja.RepartirCarta());
            jugador.PedirCarta(baraja.RepartirCarta());
            banca.PedirCarta(baraja.RepartirCarta());
            banca.PedirCarta(baraja.RepartirCarta());

            MostrarCartas();
        }

        private void MostrarCartas()
        {
            lblCartasJugador.Text = "Cartas del Jugador: " ;
            lblCartasBanca.Text = "Cartas de la Banca: " ;
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
                    // Mostrar la carta normalmente
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
            jugador.PedirCarta(baraja.RepartirCarta());
            MostrarCartas();
            jugador.OrdenarCartas();
            int valorJugador = jugador.CalcularValor();
            if (valorJugador > 21)
            {
                MessageBox.Show("¡Te pasaste de 21! La banca gana.");
                IniciarJuego();
            }
        }

        private void btnRetirarse_Click(object sender, EventArgs e)
        {
            // Calcular valor de la banca
            int valorBanca = banca.CalcularValor();
            while (valorBanca < 17)
            {
                banca.PedirCarta(baraja.RepartirCarta());
                valorBanca = banca.CalcularValor();
            }

            // Mostrar todas las cartas de la banca, revelando la carta oculta
            MostrarCartasFinal();

            lblValorBanca.Text = "Valor de la Banca: " + valorBanca;
            MessageBox.Show("Te has retirado.");

            int valorJugador = jugador.CalcularValor();
            if (valorBanca > 21 || valorJugador > valorBanca)
            {
                MessageBox.Show("¡Has ganado!");
            }
            else if (valorJugador == valorBanca)
            {
                MessageBox.Show("¡Empate!");
            }
            else
            {
                MessageBox.Show("La banca gana.");
            }

            IniciarJuego();
        }

        private void MostrarCartasFinal()
        {
            //muestra el valor de las cartas del jugador en la parte derecha 
            lblValorJugador.Text = "Valor del Jugador: " + jugador.CalcularValor();

            // Mostrar todas las cartas de la banca, incluyendo la carta oculta
            lblValorBanca.Text = "Valor de la Banca: " + banca.CalcularValor();

            // Mostrar las cartas del jugador
            MostrarImagenesCartas(jugador.Cartas, panelCartasJugador);

            // Mostrar todas las cartas de la banca
            MostrarImagenesCartas(banca.Cartas, panelCartasBanca);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IniciarJuego();
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
    }
}

