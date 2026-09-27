/**
 * @file SesionJuegoTests.cs
 * @brief Pruebas de una sesión de juego.
 * @author Santiago Caicedo
 */
using BlackJack.Core.Cuentas;
using BlackJack.Core.Juego;
using BlackJack.Core.Sesion;
using static BlackJack.Core.Juego.Rango;

namespace BlackJack.Core.Tests;

/**
 * @brief Pruebas de SesionJuego con repositorio e historial en memoria.
 */
public class SesionJuegoTests
{
    /** Cuentas en memoria. */
    private readonly RepositorioEnMemoria repositorio = new();
    /** Historial en memoria. */
    private readonly HistorialEnMemoria historial = new();
    /** Cuenta del jugador de la sesión. */
    private readonly CuentaJugador cuenta = new() { Usuario = "santi", Saldo = 1000 };

    /**
     * @brief Abre una sesión con una baraja preparada.
     * @param reparto Rangos en orden de reparto
     * @return Sesión de juego
     */
    private SesionJuego Sesion(params Rango[] reparto) =>
        new(cuenta, new ServicioCuentas(repositorio, new HasherFalso()), historial, Mazo.Reparto(reparto));

    /**
     * @brief Una ronda ganada actualiza saldo, estadísticas e historial, y se guarda al apostar y al terminar.
     */
    [Fact]
    public void Una_ronda_ganada_actualiza_saldo_estadisticas_e_historial_y_se_guarda()
    {
        var sesion = Sesion(Diez, Diez, Nueve, Siete);
        sesion.Apostar(100);
        sesion.Plantarse();

        Assert.True(sesion.RondaTerminada);
        Assert.Equal(1100, cuenta.Saldo);
        Assert.Equal(1, cuenta.Ganadas);
        Assert.Contains(historial.Lineas, l => l.StartsWith("Resultado de la ronda: GanaJugador"));
        Assert.True(repositorio.Guardados >= 2);
    }

    /**
     * @brief Una jugada no válida no cambia ni guarda la cuenta.
     */
    [Fact]
    public void Una_jugada_invalida_no_cambia_la_cuenta()
    {
        var sesion = Sesion(Diez, Diez, Nueve, Siete);
        Assert.Throws<InvalidOperationException>(sesion.PedirCarta);
        Assert.Equal(1000, cuenta.Saldo);
        Assert.Equal(0, repositorio.Guardados);
    }

    /**
     * @brief Cerrar en medio de una mano planta al jugador, guarda y cierra el historial.
     */
    [Fact]
    public void Cerrar_en_medio_de_una_mano_planta_al_jugador_y_cierra_el_historial()
    {
        var sesion = Sesion(Diez, Diez, Cinco, Siete);
        sesion.Apostar(100);
        sesion.Cerrar();

        Assert.Equal(1, cuenta.Perdidas);
        Assert.Equal(900, cuenta.Saldo);
        Assert.True(historial.Cerrado);
        Assert.Equal("Sesión terminada.", historial.Lineas[^1]);
    }

    /**
     * @brief Recargar suma el saldo inicial, pero solo entre rondas.
     */
    [Fact]
    public void Recargar_suma_el_saldo_inicial_solo_entre_rondas()
    {
        var sesion = Sesion(Diez, Diez, Cinco, Siete);
        sesion.Apostar(100);
        Assert.Throws<InvalidOperationException>(sesion.Recargar);

        sesion.Plantarse();
        sesion.Recargar();
        Assert.Equal(900 + ServicioCuentas.SaldoInicial, cuenta.Saldo);
    }
}
