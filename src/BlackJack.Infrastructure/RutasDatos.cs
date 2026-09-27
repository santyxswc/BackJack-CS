/**
 * @file RutasDatos.cs
 * @brief Ubicación de los datos del juego en cada sistema operativo.
 * @author Santiago Caicedo
 */
namespace BlackJack.Infrastructure
{
    /**
     * @brief Carpeta de datos del usuario.
     *
     * Linux: ~/.local/share/BlackJack, Windows: %LOCALAPPDATA%\\BlackJack, macOS: ~/Library/Application Support/BlackJack.
     */
    public static class RutasDatos
    {
        /** Carpeta raíz de los datos (se crea si no existe). */
        public static string Carpeta =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), "BlackJack");

        /** Carpeta de los historiales de sesión. */
        public static string CarpetaHistorial => Path.Combine(Carpeta, "historial");
    }
}
