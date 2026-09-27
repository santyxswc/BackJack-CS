/**
 * @file IHasherClaves.cs
 * @brief Contrato para derivar y verificar contraseñas.
 * @author Santiago Caicedo
 */
namespace BlackJack.Core.Cuentas
{
    /**
     * @brief Deriva hashes de contraseñas con sal; nunca se guarda la contraseña.
     */
    public interface IHasherClaves
    {
        /**
         * @brief Genera un hash con una sal nueva.
         * @param clave Contraseña
         * @return Hash y sal codificados en Base64
         */
        (string Hash, string Sal) Crear(string clave);

        /**
         * @brief Comprueba una contraseña contra su hash en tiempo constante.
         * @param clave Contraseña a verificar
         * @param hash Hash guardado (Base64)
         * @param sal Sal guardada (Base64)
         * @return true si coincide
         */
        bool Verificar(string clave, string hash, string sal);
    }
}
