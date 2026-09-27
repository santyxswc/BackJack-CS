/**
 * @file CuentaJugador.cs
 * @brief Cuenta persistente de un jugador.
 * @author Santiago Caicedo
 */
using BlackJack.Core.Juego;

namespace BlackJack.Core.Cuentas
{
    /**
     * @brief Cuenta de un jugador con su saldo y sus estadísticas.
     *
     * Las propiedades son públicas y con setter porque se serializan tal cual en jugadores.json.
     */
    public class CuentaJugador
    {
        /** Nombre de usuario. */
        public string Usuario { get; set; }
        /** Hash de la contraseña (Base64). */
        public string HashClave { get; set; }
        /** Sal del hash (Base64). */
        public string Sal { get; set; }
        /** Dinero disponible. */
        public int Saldo { get; set; }
        /** Rondas ganadas. */
        public int Ganadas { get; set; }
        /** Rondas perdidas. */
        public int Perdidas { get; set; }
        /** Rondas empatadas. */
        public int Empatadas { get; set; }
        /** Fecha de creación de la cuenta. */
        public DateTime Creada { get; set; }
        /** Fecha del último inicio de sesión. */
        public DateTime UltimoIngreso { get; set; }

        /**
         * @brief Suma una ronda terminada a las estadísticas.
         * @param resultado Resultado de la ronda
         */
        public void RegistrarResultado(ResultadoRonda resultado)
        {
            if (resultado.EsVictoria()) Ganadas++;
            else if (resultado.EsDerrota()) Perdidas++;
            else Empatadas++;
        }
    }
}
