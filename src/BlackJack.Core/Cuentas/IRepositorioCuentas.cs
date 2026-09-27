/**
 * @file IRepositorioCuentas.cs
 * @brief Contrato de persistencia de cuentas.
 * @author Santiago Caicedo
 */
namespace BlackJack.Core.Cuentas
{
    /**
     * @brief Almacén de las cuentas de los jugadores.
     */
    public interface IRepositorioCuentas
    {
        /**
         * @brief Busca una cuenta sin distinguir mayúsculas.
         * @param usuario Nombre de usuario
         * @return Cuenta, o null si no existe
         */
        CuentaJugador Buscar(string usuario);

        /**
         * @brief Crea o actualiza una cuenta.
         * @param cuenta Cuenta a guardar
         */
        void Guardar(CuentaJugador cuenta);
    }
}
