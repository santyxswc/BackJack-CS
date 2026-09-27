using BlackJack.Core.Cuentas;
using BlackJack.Core.Juego;
using BlackJack.Core.Sesion;

namespace BlackJack.Core.Tests;

/// Utilidades compartidas por las pruebas: barajas preparadas y dobles de los contratos.
internal static class Mazo
{
    /// Crea una baraja que reparte exactamente estos rangos (el palo no afecta al juego).
    public static Baraja Con(params Rango[] rangos) =>
        new(rangos.Select(r => new Carta(r, Palo.Picas)));

    /// Orden de reparto: jugador, banca, jugador, banca y luego las cartas pedidas.
    public static Func<Baraja> Reparto(params Rango[] rangos) => () => Con(rangos);
}

internal sealed class RepositorioEnMemoria : IRepositorioCuentas
{
    public List<CuentaJugador> Cuentas { get; } = new();
    public int Guardados { get; private set; }

    public CuentaJugador Buscar(string usuario) =>
        Cuentas.FirstOrDefault(c => string.Equals(c.Usuario, usuario, StringComparison.OrdinalIgnoreCase));

    public void Guardar(CuentaJugador cuenta)
    {
        Guardados++;
        if (!Cuentas.Contains(cuenta)) Cuentas.Add(cuenta);
    }
}

/// Hasher trivial para no pagar el coste de PBKDF2 en las pruebas de reglas.
internal sealed class HasherFalso : IHasherClaves
{
    public (string Hash, string Sal) Crear(string clave) => ("h:" + clave, "sal");
    public bool Verificar(string clave, string hash, string sal) => hash == "h:" + clave;
}

internal sealed class HistorialEnMemoria : IHistorialPartida
{
    public List<string> Lineas { get; } = new();
    public bool Cerrado { get; private set; }
    public void Escribir(string linea) => Lineas.Add(linea);
    public void Dispose() => Cerrado = true;
}
