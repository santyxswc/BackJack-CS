/**
 * @file ServicioCuentas.cs
 * @brief Registro e inicio de sesión de jugadores.
 * @author Santiago Caicedo
 */
using System.Text.RegularExpressions;

namespace BlackJack.Core.Cuentas
{
    /**
     * @brief Reglas de las cuentas: validación, registro, inicio de sesión y recargas.
     */
    public class ServicioCuentas
    {
        /** Saldo de una cuenta nueva y de una recarga. */
        public const int SaldoInicial = 1000;
        /** Longitud mínima de la contraseña. */
        public const int LongitudMinimaClave = 4;

        /** Formato válido de un usuario: 3 a 20 letras, números, guion bajo, punto o guion. */
        private static readonly Regex usuarioValido = new(@"^[\p{L}\p{N}_.-]{3,20}$");

        /** Almacén de cuentas. */
        private readonly IRepositorioCuentas repositorio;
        /** Derivación de contraseñas. */
        private readonly IHasherClaves hasher;
        /** Fecha y hora actual (inyectable en pruebas). */
        private readonly Func<DateTime> ahora;

        /**
         * @brief Crea el servicio.
         * @param repositorio Almacén de cuentas
         * @param hasher Derivación de contraseñas
         * @param ahora Reloj; por defecto DateTime.Now
         */
        public ServicioCuentas(IRepositorioCuentas repositorio, IHasherClaves hasher, Func<DateTime> ahora = null)
        {
            this.repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
            this.hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
            this.ahora = ahora ?? (() => DateTime.Now);
        }

        /**
         * @brief Crea una cuenta con el saldo inicial.
         * @param usuario Nombre de usuario
         * @param clave Contraseña
         * @return Cuenta creada
         * @exception InvalidOperationException Si los datos no son válidos o el usuario ya existe
         */
        public CuentaJugador Registrar(string usuario, string clave)
        {
            usuario = usuario?.Trim() ?? "";

            if (!usuarioValido.IsMatch(usuario))
                throw new InvalidOperationException("El usuario debe tener entre 3 y 20 caracteres (letras, números, _ . -).");
            if (string.IsNullOrEmpty(clave) || clave.Length < LongitudMinimaClave)
                throw new InvalidOperationException($"La contraseña debe tener al menos {LongitudMinimaClave} caracteres.");
            if (repositorio.Buscar(usuario) != null)
                throw new InvalidOperationException("Ese usuario ya existe. Inicia sesión.");

            var (hash, sal) = hasher.Crear(clave);
            var cuenta = new CuentaJugador
            {
                Usuario = usuario,
                HashClave = hash,
                Sal = sal,
                Saldo = SaldoInicial,
                Creada = ahora(),
                UltimoIngreso = ahora()
            };

            repositorio.Guardar(cuenta);
            return cuenta;
        }

        /**
         * @brief Verifica las credenciales y registra el ingreso.
         * @param usuario Nombre de usuario (no distingue mayúsculas)
         * @param clave Contraseña
         * @return Cuenta, o null si las credenciales no son correctas
         */
        public CuentaJugador IniciarSesion(string usuario, string clave)
        {
            var cuenta = repositorio.Buscar(usuario?.Trim() ?? "");
            if (cuenta == null || clave == null || !hasher.Verificar(clave, cuenta.HashClave, cuenta.Sal))
                return null;

            cuenta.UltimoIngreso = ahora();
            repositorio.Guardar(cuenta);
            return cuenta;
        }

        /**
         * @brief Guarda el progreso de una cuenta.
         * @param cuenta Cuenta a guardar
         */
        public void Guardar(CuentaJugador cuenta) => repositorio.Guardar(cuenta);
    }
}
