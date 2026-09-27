/**
 * @file RepositorioCuentasJson.cs
 * @brief Cuentas de los jugadores en un archivo JSON.
 * @author Santiago Caicedo
 */
using System.Text.Json;
using BlackJack.Core.Cuentas;

namespace BlackJack.Infrastructure
{
    /**
     * @brief Guarda las cuentas en jugadores.json.
     *
     * Escribe primero en un archivo temporal y luego lo reemplaza, para no dejar el archivo
     * a medias si la aplicación se cierra mientras guarda.
     */
    public class RepositorioCuentasJson : IRepositorioCuentas
    {
        /** Opciones del JSON (con sangría, legible). */
        private static readonly JsonSerializerOptions opcionesJson = new() { WriteIndented = true };

        /** Ruta de jugadores.json. */
        private readonly string rutaArchivo;

        /**
         * @brief Crea el repositorio y su carpeta.
         * @param carpeta Carpeta de los datos
         */
        public RepositorioCuentasJson(string carpeta)
        {
            Directory.CreateDirectory(carpeta);
            rutaArchivo = Path.Combine(carpeta, "jugadores.json");
        }

        /**
         * @brief Busca una cuenta sin distinguir mayúsculas.
         * @param usuario Nombre de usuario
         * @return Cuenta, o null si no existe
         */
        public CuentaJugador Buscar(string usuario) =>
            Cargar().FirstOrDefault(c => MismoUsuario(c, usuario));

        /**
         * @brief Crea o actualiza una cuenta.
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
         * @brief Guarda todas las cuentas de forma atómica.
         * @param cuentas Cuentas a guardar
         */
        private void Escribir(List<CuentaJugador> cuentas)
        {
            string temporal = rutaArchivo + ".tmp";
            File.WriteAllText(temporal, JsonSerializer.Serialize(cuentas, opcionesJson));
            File.Move(temporal, rutaArchivo, overwrite: true);
        }

        /** @brief Compara usuarios sin distinguir mayúsculas. */
        private static bool MismoUsuario(CuentaJugador cuenta, string usuario) =>
            string.Equals(cuenta.Usuario, usuario, StringComparison.OrdinalIgnoreCase);
    }
}
