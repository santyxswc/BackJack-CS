/**
 * @file RepositorioJugadores.cs
 * @brief Cuentas de los jugadores.
 * @author Santiago Caicedo
 */
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace BlackjackAvalonia.Datos
{
    /**
     * @brief Cuenta de un jugador con su saldo y sus estadísticas.
     */
    public class CuentaJugador
    {
        /** Nombre de usuario. */
        public string Usuario { get; set; }
        /** Hash PBKDF2 de la contraseña (Base64). */
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
    }

    /**
     * @brief Guarda las cuentas en un archivo JSON (jugadores.json).
     *
     * Las contraseñas se guardan con PBKDF2-SHA256 y sal, nunca en texto plano.
     */
    public class RepositorioJugadores
    {
        /** Saldo de una cuenta nueva y de una recarga. */
        public const int SaldoInicial = 1000;

        /** Formato válido de un usuario: 3 a 20 letras, números, guion bajo, punto o guion. */
        private static readonly Regex usuarioValido = new(@"^[\p{L}\p{N}_.-]{3,20}$");
        /** Opciones del JSON (con sangría, legible). */
        private static readonly JsonSerializerOptions opcionesJson = new() { WriteIndented = true };

        /** Ruta de jugadores.json. */
        private readonly string rutaArchivo;

        /**
         * @brief Carpeta de datos del juego.
         *
         * Linux: ~/.local/share/BlackJack, Windows: %LOCALAPPDATA%\\BlackJack, macOS: ~/Library/Application Support/BlackJack.
         * Se crea si no existe.
         */
        public static string CarpetaDatos =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), "BlackJack");

        /**
         * @brief Crea el repositorio y su carpeta.
         * @param carpeta Carpeta de los datos; por defecto CarpetaDatos
         */
        public RepositorioJugadores(string carpeta = null)
        {
            carpeta ??= CarpetaDatos;
            Directory.CreateDirectory(carpeta);
            rutaArchivo = Path.Combine(carpeta, "jugadores.json");
        }

        /**
         * @brief Crea una cuenta con el saldo inicial.
         * @param usuario Nombre de usuario
         * @param clave Contraseña (mínimo 4 caracteres)
         * @return Cuenta creada
         * @exception InvalidOperationException Si los datos no son válidos o el usuario ya existe
         */
        public CuentaJugador Registrar(string usuario, string clave)
        {
            usuario = usuario?.Trim() ?? "";

            if (!usuarioValido.IsMatch(usuario))
                throw new InvalidOperationException("El usuario debe tener entre 3 y 20 caracteres (letras, números, _ . -).");
            if (string.IsNullOrEmpty(clave) || clave.Length < 4)
                throw new InvalidOperationException("La contraseña debe tener al menos 4 caracteres.");

            var cuentas = Cargar();
            if (cuentas.Any(c => MismoUsuario(c, usuario)))
                throw new InvalidOperationException("Ese usuario ya existe. Inicia sesión.");

            byte[] sal = RandomNumberGenerator.GetBytes(16);
            var cuenta = new CuentaJugador
            {
                Usuario = usuario,
                Sal = Convert.ToBase64String(sal),
                HashClave = Convert.ToBase64String(CalcularHash(clave, sal)),
                Saldo = SaldoInicial,
                Creada = DateTime.Now,
                UltimoIngreso = DateTime.Now
            };

            cuentas.Add(cuenta);
            Escribir(cuentas);
            return cuenta;
        }

        /**
         * @brief Verifica las credenciales.
         * @param usuario Nombre de usuario (no distingue mayúsculas)
         * @param clave Contraseña
         * @return Cuenta, o null si las credenciales no son correctas
         */
        public CuentaJugador IniciarSesion(string usuario, string clave)
        {
            usuario = usuario?.Trim() ?? "";
            var cuenta = Cargar().FirstOrDefault(c => MismoUsuario(c, usuario));
            if (cuenta == null || clave == null)
                return null;

            byte[] esperado = Convert.FromBase64String(cuenta.HashClave);
            byte[] calculado = CalcularHash(clave, Convert.FromBase64String(cuenta.Sal));
            if (!CryptographicOperations.FixedTimeEquals(esperado, calculado))
                return null;

            cuenta.UltimoIngreso = DateTime.Now;
            Guardar(cuenta);
            return cuenta;
        }

        /**
         * @brief Guarda los cambios de una cuenta.
         * @param cuenta Cuenta a guardar
         */
        public void Guardar(CuentaJugador cuenta)
        {
            var cuentas = Cargar();
            int indice = cuentas.FindIndex(c => MismoUsuario(c, cuenta.Usuario));
            if (indice >= 0)
                cuentas[indice] = cuenta;
            else
                cuentas.Add(cuenta);
            Escribir(cuentas);
        }

        /**
         * @brief Lee todas las cuentas.
         * @return Cuentas guardadas
         * @exception InvalidOperationException Si el archivo está dañado
         */
        private List<CuentaJugador> Cargar()
        {
            if (!File.Exists(rutaArchivo))
                return new List<CuentaJugador>();

            try
            {
                return JsonSerializer.Deserialize<List<CuentaJugador>>(File.ReadAllText(rutaArchivo)) ?? new List<CuentaJugador>();
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"El archivo de jugadores está dañado: {rutaArchivo}", ex);
            }
        }

        /**
         * @brief Guarda todas las cuentas.
         * @param cuentas Cuentas a guardar
         *
         * Escribe en un archivo temporal y lo reemplaza, para no dejar el archivo a medias si la aplicación se cierra.
         */
        private void Escribir(List<CuentaJugador> cuentas)
        {
            string temporal = rutaArchivo + ".tmp";
            File.WriteAllText(temporal, JsonSerializer.Serialize(cuentas, opcionesJson));
            File.Move(temporal, rutaArchivo, overwrite: true);
        }

        /**
         * @brief Compara usuarios sin distinguir mayúsculas.
         * @param cuenta Cuenta
         * @param usuario Usuario buscado
         * @return true si coinciden
         */
        private static bool MismoUsuario(CuentaJugador cuenta, string usuario) =>
            string.Equals(cuenta.Usuario, usuario, StringComparison.OrdinalIgnoreCase);

        /**
         * @brief Calcula el hash PBKDF2-SHA256 de una contraseña.
         * @param clave Contraseña
         * @param sal Sal
         * @return Hash de 32 bytes
         */
        private static byte[] CalcularHash(string clave, byte[] sal) =>
            Rfc2898DeriveBytes.Pbkdf2(clave, sal, 100_000, HashAlgorithmName.SHA256, 32);
    }
}
