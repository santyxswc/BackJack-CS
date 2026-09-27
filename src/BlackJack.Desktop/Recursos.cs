/**
 * @file Recursos.cs
 * @brief Imágenes del juego.
 * @author Santiago Caicedo
 */
using Avalonia.Media.Imaging;
using BlackJack.Core.Juego;

namespace BlackJack.Desktop
{
    /**
     * @brief Carga las imágenes de la carpeta imagenes (junto al ejecutable).
     *
     * Cada imagen se lee del disco una sola vez.
     */
    public static class Recursos
    {
        /** Carpeta de las imágenes. */
        private static readonly string carpeta = Path.Combine(AppContext.BaseDirectory, "imagenes");
        /** Imágenes ya cargadas, por nombre de archivo. */
        private static readonly Dictionary<string, Bitmap> cache = new();

        /** Fondo de la mesa. */
        public static Bitmap Fondo => Cargar("Fondo.jpg");
        /** Reverso de las cartas. */
        public static Bitmap Dorso => Cargar("Dorso.jpg");
        /**
         * @brief Imagen de una carta.
         * @param carta Carta
         * @return Imagen, o null si el archivo no existe
         */
        public static Bitmap ImagenCarta(Carta carta) => Cargar($"{carta.Simbolo}_de_{carta.Palo}.jpg");

        /**
         * @brief Carga una imagen o la toma de la caché.
         * @param archivo Nombre del archivo
         * @return Imagen, o null si no existe
         */
        private static Bitmap Cargar(string archivo)
        {
            if (cache.TryGetValue(archivo, out var imagen))
                return imagen;

            string ruta = Path.Combine(carpeta, archivo);
            imagen = File.Exists(ruta) ? new Bitmap(ruta) : null;
            cache[archivo] = imagen;
            return imagen;
        }
    }
}
