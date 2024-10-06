using System;
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

            // Inicializar la baraja y las manos del jugador y la banca
            baraja = new Baraja(); // Asegúrate de que la baraja se inicializa aquí
            jugador = new ManoJugador();
            banca = new ManoJugador();
            // Repartir cartas iniciales
            jugador.PedirCarta(baraja.RepartirCarta());
            banca.PedirCarta(baraja.RepartirCarta());

            // Mostrar cartas iniciales
            ActualizarCartas();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Aquí puedes inicializar el juego de Blackjack, o realizar cualquier otra configuración.
            IniciarJuego(); // Este es solo un ejemplo de lo que podrías hacer
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
            lblCartasJugador.Text = "Cartas del Jugador: " + string.Join(", ", jugador.Cartas);
            lblCartasBanca.Text = "Cartas de la Banca: " + banca.Cartas[0] + ", (oculta)"; // La segunda carta de la banca está oculta
            lblValorJugador.Text = "Valor del Jugador: " + jugador.CalcularValor();
        }

        private void btnPedirCarta_Click(object sender, EventArgs e)
        {
            jugador.PedirCarta(baraja.RepartirCarta());
            MostrarCartas();
            jugador.OrdenarCartas();
            ActualizarCartas();
            int valorJugador = jugador.CalcularValor();
            if (valorJugador > 21)
            {
                MessageBox.Show("¡Te pasaste de 21! La banca gana.");
                IniciarJuego();
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

            lblCartasBanca.Text = "Cartas de la Banca: " + string.Join(", ", banca.Cartas);
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

        private void ActualizarCartas()
        {
            // Actualizar cartas del jugador
            lblCartasJugador.Text = "Cartas Jugador: " + string.Join(", ", jugador.Cartas);

            // Actualizar valor del jugador
            lblValorJugador.Text = "Valor Jugador: " + jugador.CalcularValor();

            // Actualizar cartas de la banca
            lblCartasBanca.Text = "Cartas Banca: " + string.Join(", ", banca.Cartas);

            // Actualizar valor de la banca
            lblValorBanca.Text = "Valor Banca: " + banca.CalcularValor();
        }

        private void btnReiniciar_Click(object sender, EventArgs e)
        {
            IniciarJuego();
        }

        private void lblValorBanca_Click(object sender, EventArgs e)
        {

        }
    }
}
