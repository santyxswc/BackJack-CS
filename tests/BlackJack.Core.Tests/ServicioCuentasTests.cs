/**
 * @file ServicioCuentasTests.cs
 * @brief Pruebas del registro e inicio de sesión.
 * @author Santiago Caicedo
 */
using BlackJack.Core.Cuentas;

namespace BlackJack.Core.Tests;

/**
 * @brief Pruebas de ServicioCuentas con un repositorio en memoria y un reloj fijo.
 */
public class ServicioCuentasTests
{
    /** Cuentas en memoria. */
    private readonly RepositorioEnMemoria repositorio = new();
    /** Servicio bajo prueba. */
    private readonly ServicioCuentas servicio;
    /** Hora que devuelve el reloj del servicio. */
    private DateTime ahora = new(2026, 1, 1);

    /**
     * @brief Crea el servicio con el reloj de la prueba.
     */
    public ServicioCuentasTests() =>
        servicio = new ServicioCuentas(repositorio, new HasherFalso(), () => ahora);

    /**
     * @brief Registrar crea la cuenta con el saldo inicial y guarda el hash, no la contraseña.
     */
    [Fact]
    public void Registrar_crea_la_cuenta_con_el_saldo_inicial_y_sin_guardar_la_clave()
    {
        var cuenta = servicio.Registrar("  santi ", "clave1");
        Assert.Equal("santi", cuenta.Usuario);
        Assert.Equal(ServicioCuentas.SaldoInicial, cuenta.Saldo);
        Assert.Equal("h:clave1", cuenta.HashClave);
        Assert.Single(repositorio.Cuentas);
    }

    /**
     * @brief Registrar rechaza usuarios o contraseñas no válidos.
     * @param usuario Usuario
     * @param clave Contraseña
     */
    [Theory]
    [InlineData("ab", "clave1")]
    [InlineData("usuario con espacios", "clave1")]
    [InlineData("valido", "123")]
    [InlineData(null, "clave1")]
    public void Registrar_rechaza_datos_invalidos(string usuario, string clave) =>
        Assert.Throws<InvalidOperationException>(() => servicio.Registrar(usuario, clave));

    /**
     * @brief No permite registrar dos veces el mismo usuario, aunque cambien las mayúsculas.
     */
    [Fact]
    public void No_permite_usuarios_repetidos_sin_distinguir_mayusculas()
    {
        servicio.Registrar("Santi", "clave1");
        Assert.Throws<InvalidOperationException>(() => servicio.Registrar("SANTI", "otra12"));
    }

    /**
     * @brief Iniciar sesión verifica la contraseña y actualiza la fecha del último ingreso.
     */
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
