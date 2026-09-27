/**
 * @file HasherPbkdf2.cs
 * @brief Derivación de contraseñas con PBKDF2-SHA256.
 * @author Santiago Caicedo
 */
using System.Security.Cryptography;
using BlackJack.Core.Cuentas;

namespace BlackJack.Infrastructure
{
    /**
     * @brief PBKDF2-SHA256 con sal aleatoria de 16 bytes y 100 000 iteraciones.
     */
    public class HasherPbkdf2 : IHasherClaves
    {
        /** Iteraciones de PBKDF2. */
        private const int Iteraciones = 100_000;
        /** Tamaño de la sal en bytes. */
        private const int BytesSal = 16;
        /** Tamaño del hash en bytes. */
        private const int BytesHash = 32;

        /**
         * @brief Genera un hash con una sal nueva.
         * @param clave Contraseña
         * @return Hash y sal codificados en Base64
         */
        public (string Hash, string Sal) Crear(string clave)
        {
            byte[] sal = RandomNumberGenerator.GetBytes(BytesSal);
            return (Convert.ToBase64String(Derivar(clave, sal)), Convert.ToBase64String(sal));
        }

        /**
         * @brief Comprueba una contraseña contra su hash en tiempo constante.
         * @param clave Contraseña a verificar
         * @param hash Hash guardado (Base64)
         * @param sal Sal guardada (Base64)
         * @return true si coincide
         */
        public bool Verificar(string clave, string hash, string sal)
        {
            byte[] esperado = Convert.FromBase64String(hash);
            byte[] calculado = Derivar(clave, Convert.FromBase64String(sal));
            return CryptographicOperations.FixedTimeEquals(esperado, calculado);
        }

        /** @brief Aplica PBKDF2-SHA256. */
        private static byte[] Derivar(string clave, byte[] sal) =>
            Rfc2898DeriveBytes.Pbkdf2(clave, sal, Iteraciones, HashAlgorithmName.SHA256, BytesHash);
    }
}
