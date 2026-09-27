/**
 * @file Soporte.cs
 * @brief Utilidades compartidas por las pruebas: barajas preparadas y dobles de los contratos.
 * @author Santiago Caicedo
 */
using BlackJack.Core.Cuentas;
using BlackJack.Core.Juego;
using BlackJack.Core.Sesion;

namespace BlackJack.Core.Tests;

/**
 * @brief Barajas con un orden fijo para reproducir manos concretas.
 */
internal static class Mazo
{
    /**
     * @brief Crea una baraja que reparte exactamente estos rangos (el palo no afecta al juego).
     * @param rangos Rangos en orden de reparto
     * @return Baraja preparada
     */
    public static Baraja Con(params Rango[] rangos) =>
        new(rangos.Select(r => new Carta(r, Palo.Picas)));

    /**
     * @brief Fábrica de barajas para la mesa. Orden: jugador, banca, jugador, banca y luego las cartas pedidas.
     * @param rangos Rangos en orden de reparto
     * @return Función que crea la baraja de cada ronda
     */
    public static Func<Baraja> Reparto(params Rango[] rangos) => () => Con(rangos);
}

/**
 * @brief Repositorio de cuentas en memoria.
 */
internal sealed class RepositorioEnMemoria : IRepositorioCuentas
{
    /** Cuentas guardadas. */
    public List<CuentaJugador> Cuentas { get; } = new();
    /** Veces que se llamó a Guardar. */
    public int Guardados { get; private set; }

    /**
     * @brief Busca una cuenta sin distinguir mayúsculas.
     * @param usuario Nombre de usuario
     * @return Cuenta, o null si no existe
     */
    public CuentaJugador Buscar(string usuario) =>
        Cuentas.FirstOrDefault(c => string.Equals(c.Usuario, usuario, StringComparison.OrdinalIgnoreCase));

    /**
     * @brief Registra la cuenta si es nueva y cuenta el guardado.
     * @param cuenta Cuenta a guardar
     */
    public void Guardar(CuentaJugador cuenta)
    {
        Guardados++;
        if (!Cuentas.Contains(cuenta)) Cuentas.Add(cuenta);
    }
}

/**
 * @brief Hasher sin el coste de PBKDF2 para las pruebas de reglas.
 */
internal sealed class HasherFalso : IHasherClaves
{
    /**
     * @brief Genera un hash predecible.
     * @param clave Contraseña
     * @return Hash "h:clave" y una sal fija
     */
    public (string Hash, string Sal) Crear(string clave) => ("h:" + clave, "sal");
    /**
     * @brief Comprueba la contraseña contra el hash predecible.
     * @param clave Contraseña
     * @param hash Hash guardado
     * @param sal Sal (no se usa)
     * @return true si coincide
     */
    public bool Verificar(string clave, string hash, string sal) => hash == "h:" + clave;
}

/**
 * @brief Historial que guarda las líneas en una lista.
 */
internal sealed class HistorialEnMemoria : IHistorialPartida
{
    /** Líneas escritas. */
    public List<string> Lineas { get; } = new();
    /** Indica si se cerró el historial. */
    public bool Cerrado { get; private set; }
    /**
     * @brief Agrega una línea.
     * @param linea Texto
     */
    public void Escribir(string linea) => Lineas.Add(linea);
    /**
     * @brief Marca el historial como cerrado.
     */
    public void Dispose() => Cerrado = true;
}
