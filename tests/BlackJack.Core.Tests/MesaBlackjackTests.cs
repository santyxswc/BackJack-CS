/**
 * @file MesaBlackjackTests.cs
 * @brief Pruebas de las reglas de la mesa.
 * @author Santiago Caicedo
 */
using BlackJack.Core.Juego;
using static BlackJack.Core.Juego.Rango;

namespace BlackJack.Core.Tests;

/**
 * @brief Pruebas de MesaBlackjack con barajas en un orden fijo.
 */
public class MesaBlackjackTests
{
    /**
     * @brief Crea una mesa con un jugador y una baraja preparada.
     * @param saldo Saldo inicial del jugador
     * @param reparto Rangos en orden de reparto
     * @return Mesa lista para apostar
     */
    private static MesaBlackjack Mesa(int saldo, params Rango[] reparto) =>
        new(new Jugador(saldo), Mazo.Reparto(reparto));

    /**
     * @brief El blackjack natural paga 3:2; con $25 se ganan $37.
     */
    [Fact]
    public void Blackjack_natural_paga_tres_a_dos_redondeando_hacia_abajo()
    {
        var mesa = Mesa(1000, As, Nueve, Rey, Siete);
        mesa.Apostar(25);
        Assert.Equal(ResultadoRonda.BlackjackJugador, mesa.Resultado);
        Assert.Equal(37, mesa.GananciaNeta);
        Assert.Equal(1037, mesa.Jugador.Saldo);
    }

    /**
     * @brief Si jugador y banca tienen blackjack, es empate y se devuelve la apuesta.
     */
    [Fact]
    public void Si_ambos_tienen_blackjack_es_empate_y_se_devuelve_la_apuesta()
    {
        var mesa = Mesa(1000, As, As, Rey, Reina);
        mesa.Apostar(100);
        Assert.Equal(ResultadoRonda.Empate, mesa.Resultado);
        Assert.Equal(1000, mesa.Jugador.Saldo);
    }

    /**
     * @brief El blackjack de la banca termina la ronda antes del turno del jugador.
     */
    [Fact]
    public void El_blackjack_de_la_banca_termina_la_ronda_antes_del_turno_del_jugador()
    {
        var mesa = Mesa(1000, Diez, As, Nueve, Rey);
        mesa.Apostar(100);
        Assert.Equal(ResultadoRonda.BlackjackBanca, mesa.Resultado);
        Assert.Equal(FaseJuego.Apuesta, mesa.Fase);
        Assert.Equal(900, mesa.Jugador.Saldo);
    }

    /**
     * @brief Pasarse de 21 pierde la apuesta.
     */
    [Fact]
    public void Pasarse_de_21_pierde_la_apuesta()
    {
        var mesa = Mesa(1000, Diez, Nueve, Seis, Siete, Rey);
        mesa.Apostar(100);
        mesa.PedirCarta();
        Assert.Equal(ResultadoRonda.JugadorSePasa, mesa.Resultado);
        Assert.Equal(900, mesa.Jugador.Saldo);
    }

    /**
     * @brief Con 21 el jugador se planta solo: 5+6 pide un 10; la banca tiene 10+7 y se planta.
     */
    [Fact]
    public void Con_21_el_jugador_se_planta_automaticamente()
    {

        var mesa = Mesa(1000, Cinco, Diez, Seis, Siete, Rey);
        mesa.Apostar(100);
        mesa.PedirCarta();
        Assert.Equal(ResultadoRonda.GanaJugador, mesa.Resultado);
        Assert.Equal(1100, mesa.Jugador.Saldo);
    }

    /**
     * @brief La banca se planta con 17 suave (As+6) aunque quede un 4 en la baraja.
     */
    [Fact]
    public void La_banca_se_planta_con_17_suave()
    {

        var mesa = Mesa(1000, Diez, As, Ocho, Seis, Cuatro);
        mesa.Apostar(100);
        mesa.Plantarse();
        Assert.Equal(2, mesa.Banca.Cartas.Count);
        Assert.Equal(ResultadoRonda.GanaJugador, mesa.Resultado);
    }

    /**
     * @brief La banca pide carta por debajo de 17 y puede pasarse.
     */
    [Fact]
    public void La_banca_pide_hasta_17_y_puede_pasarse()
    {
        var mesa = Mesa(1000, Diez, Diez, Ocho, Seis, Nueve);
        mesa.Apostar(100);
        mesa.Plantarse();
        Assert.Equal(ResultadoRonda.BancaSePasa, mesa.Resultado);
        Assert.Equal(1100, mesa.Jugador.Saldo);
    }

    /**
     * @brief Doblar duplica la apuesta, da una sola carta y termina el turno.
     */
    [Fact]
    public void Doblar_duplica_la_apuesta_da_una_carta_y_termina_el_turno()
    {
        var mesa = Mesa(1000, Cinco, Diez, Seis, Siete, Diez);
        mesa.Apostar(100);
        Assert.True(mesa.PuedeDoblar);
        mesa.Doblar();
        Assert.Equal(3, mesa.Jugador.Mano.Cartas.Count);
        Assert.Equal(ResultadoRonda.GanaJugador, mesa.Resultado);
        Assert.Equal(1200, mesa.Jugador.Saldo);
    }

    /**
     * @brief No se puede doblar sin saldo para igualar la apuesta.
     */
    [Fact]
    public void No_se_puede_doblar_sin_saldo_suficiente()
    {
        var mesa = Mesa(150, Cinco, Diez, Seis, Siete);
        mesa.Apostar(100);
        Assert.False(mesa.PuedeDoblar);
        Assert.Throws<InvalidOperationException>(mesa.Doblar);
    }

    /**
     * @brief No se juega sin apostar, ni se apuesta más del saldo o una cantidad no positiva.
     */
    [Fact]
    public void No_se_puede_jugar_sin_apostar_ni_apostar_mas_del_saldo()
    {
        var mesa = Mesa(50, Cinco, Diez, Seis, Siete);
        Assert.Throws<InvalidOperationException>(mesa.PedirCarta);
        Assert.Throws<InvalidOperationException>(() => mesa.Apostar(100));
        Assert.Throws<InvalidOperationException>(() => mesa.Apostar(0));
        Assert.Equal(50, mesa.Jugador.Saldo);
    }

    /**
     * @brief Ganancia neta de cada resultado.
     * @param resultado Resultado de la ronda
     * @param apuesta Cantidad apostada
     * @param ganancia Ganancia esperada
     */
    [Theory]
    [InlineData(ResultadoRonda.BlackjackJugador, 100, 150)]
    [InlineData(ResultadoRonda.GanaJugador, 100, 100)]
    [InlineData(ResultadoRonda.BancaSePasa, 100, 100)]
    [InlineData(ResultadoRonda.Empate, 100, 0)]
    [InlineData(ResultadoRonda.GanaBanca, 100, -100)]
    [InlineData(ResultadoRonda.BlackjackBanca, 100, -100)]
    public void Tabla_de_pagos(ResultadoRonda resultado, int apuesta, int ganancia) =>
        Assert.Equal(ganancia, MesaBlackjack.CalcularGanancia(resultado, apuesta));
}
