using Avalonia.Media.Imaging;
using BlackjackForm;

namespace BlackjackAvalonia
{
    // Carga (una sola vez) las imagenes copiadas junto al ejecutable
    public static class Recursos
    {
        private static readonly string carpeta = Path.Combine(AppContext.BaseDirectory, "imagenes");
        private static readonly Dictionary<string, Bitmap> cache = new();

        public static Bitmap Fondo => Cargar("Fondo.jpg");
        public static Bitmap Dorso => Cargar("Dorso.jpg");
        public static Bitmap ImagenCarta(Carta carta) => Cargar($"{carta.Valor}_de_{carta.Palo}.jpg");

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
