/**
 * @file InfraestructuraTests.cs
 * @brief Pruebas de la persistencia en disco y del hash de contraseñas.
 * @author Santiago Caicedo
 */
using BlackJack.Core.Cuentas;
using BlackJack.Infrastructure;

namespace BlackJack.Core.Tests;

/**
 * @brief Pruebas de RepositorioCuentasJson y HasherPbkdf2 en una carpeta temporal.
 */
public sealed class InfraestructuraTests : IDisposable
{
    /** Carpeta temporal de la prueba. */
    private readonly string carpeta = Path.Combine(Path.GetTempPath(), "blackjack-tests-" + Guid.NewGuid());

    /**
     * @brief Borra la carpeta temporal.
     */
    public void Dispose()
    {
        if (Directory.Exists(carpeta)) Directory.Delete(carpeta, recursive: true);
    }

    /**
     * @brief Las cuentas se guardan en disco y se encuentran sin distinguir mayúsculas.
     */
    [Fact]
    public void El_repositorio_json_persiste_y_busca_sin_distinguir_mayusculas()
    {
        var repositorio = new RepositorioCuentasJson(carpeta);
        repositorio.Guardar(new CuentaJugador { Usuario = "Santi", Saldo = 1500, Ganadas = 3 });

        var releida = new RepositorioCuentasJson(carpeta).Buscar("santi");
        Assert.Equal(1500, releida.Saldo);
        Assert.Equal(3, releida.Ganadas);
        Assert.False(File.Exists(Path.Combine(carpeta, "jugadores.json.tmp")));
    }

    /**
     * @brief Un jugadores.json dañado produce un error claro.
     */
    [Fact]
    public void Un_archivo_danado_se_reporta_con_un_error_claro()
    {
        Directory.CreateDirectory(carpeta);
        File.WriteAllText(Path.Combine(carpeta, "jugadores.json"), "{ no es json");
        var error = Assert.Throws<InvalidOperationException>(() => new RepositorioCuentasJson(carpeta).Buscar("x"));
        Assert.Contains("dañado", error.Message);
    }

    /**
     * @brief PBKDF2 acepta la contraseña correcta, rechaza otra y usa una sal distinta cada vez.
     */
    [Fact]
    public void Pbkdf2_verifica_la_clave_correcta_y_usa_sal_distinta_cada_vez()
    {
        var hasher = new HasherPbkdf2();
        var (hash, sal) = hasher.Crear("secreta");

        Assert.True(hasher.Verificar("secreta", hash, sal));
        Assert.False(hasher.Verificar("otra", hash, sal));
        Assert.NotEqual(sal, hasher.Crear("secreta").Sal);
    }
}
