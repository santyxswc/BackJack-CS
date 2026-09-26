using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace BlackjackAvalonia.Datos
{
    public class CuentaJugador
    {
        public string Usuario { get; set; }
        public string HashClave { get; set; }
        public string Sal { get; set; }
        public int Saldo { get; set; }
        public int Ganadas { get; set; }
        public int Perdidas { get; set; }
        public int Empatadas { get; set; }
        public DateTime Creada { get; set; }
        public DateTime UltimoIngreso { get; set; }
    }

    // Guarda las cuentas en un archivo JSON. Las contraseñas se guardan con hash PBKDF2 + sal, nunca en texto plano.
    public class RepositorioJugadores
    {
        public const int SaldoInicial = 1000;

        private static readonly Regex usuarioValido = new(@"^[\p{L}\p{N}_.-]{3,20}$");
        private static readonly JsonSerializerOptions opcionesJson = new() { WriteIndented = true };

        private readonly string rutaArchivo;

        // Linux: ~/.local/share/BlackJack  ·  Windows: %LOCALAPPDATA%\BlackJack  ·  macOS: ~/Library/Application Support/BlackJack
        // SpecialFolderOption.Create: sin esto GetFolderPath devuelve "" si la carpeta aun no existe
        public static string CarpetaDatos =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), "BlackJack");

        public RepositorioJugadores(string carpeta = null)
        {
            carpeta ??= CarpetaDatos;
            Directory.CreateDirectory(carpeta);
            rutaArchivo = Path.Combine(carpeta, "jugadores.json");
        }

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

        // Devuelve la cuenta si usuario y contraseña coinciden; null en caso contrario
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

        // Escribe en un archivo temporal y lo reemplaza, para no dejar el archivo a medias si la app se cierra
        private void Escribir(List<CuentaJugador> cuentas)
        {
            string temporal = rutaArchivo + ".tmp";
            File.WriteAllText(temporal, JsonSerializer.Serialize(cuentas, opcionesJson));
            File.Move(temporal, rutaArchivo, overwrite: true);
        }

        private static bool MismoUsuario(CuentaJugador cuenta, string usuario) =>
            string.Equals(cuenta.Usuario, usuario, StringComparison.OrdinalIgnoreCase);

        private static byte[] CalcularHash(string clave, byte[] sal) =>
            Rfc2898DeriveBytes.Pbkdf2(clave, sal, 100_000, HashAlgorithmName.SHA256, 32);
    }
}
