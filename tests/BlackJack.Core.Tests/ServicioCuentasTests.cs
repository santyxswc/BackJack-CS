using BlackJack.Core.Cuentas;

namespace BlackJack.Core.Tests;

public class ServicioCuentasTests
{
    private readonly RepositorioEnMemoria repositorio = new();
    private readonly ServicioCuentas servicio;
    private DateTime ahora = new(2026, 1, 1);

    public ServicioCuentasTests() =>
        servicio = new ServicioCuentas(repositorio, new HasherFalso(), () => ahora);

    [Fact]
    public void Registrar_crea_la_cuenta_con_el_saldo_inicial_y_sin_guardar_la_clave()
    {
        var cuenta = servicio.Registrar("  santi ", "clave1");
        Assert.Equal("santi", cuenta.Usuario);
        Assert.Equal(ServicioCuentas.SaldoInicial, cuenta.Saldo);
        Assert.Equal("h:clave1", cuenta.HashClave); // se guarda lo que produce el hasher, no la clave
        Assert.Single(repositorio.Cuentas);
    }

    [Theory]
    [InlineData("ab", "clave1")]
    [InlineData("usuario con espacios", "clave1")]
    [InlineData("valido", "123")]
    [InlineData(null, "clave1")]
    public void Registrar_rechaza_datos_invalidos(string usuario, string clave) =>
        Assert.Throws<InvalidOperationException>(() => servicio.Registrar(usuario, clave));

    [Fact]
    public void No_permite_usuarios_repetidos_sin_distinguir_mayusculas()
    {
        servicio.Registrar("Santi", "clave1");
        Assert.Throws<InvalidOperationException>(() => servicio.Registrar("SANTI", "otra12"));
    }

    [Fact]
    public void Iniciar_sesion_verifica_la_clave_y_registra_el_ingreso()
    {
        servicio.Registrar("santi", "clave1");
        ahora = ahora.AddDays(1);

        Assert.Null(servicio.IniciarSesion("santi", "incorrecta"));
        Assert.Null(servicio.IniciarSesion("nadie", "clave1"));

        var cuenta = servicio.IniciarSesion("SANTI", "clave1");
        Assert.NotNull(cuenta);
        Assert.Equal(ahora, cuenta.UltimoIngreso);
    }
}
