using BlackJack.Core.Juego;
using static BlackJack.Core.Juego.Rango;

namespace BlackJack.Core.Tests;

public class MesaBlackjackTests
{
    private static MesaBlackjack Mesa(int saldo, params Rango[] reparto) =>
        new(new Jugador(saldo), Mazo.Reparto(reparto));

    [Fact]
    public void Blackjack_natural_paga_tres_a_dos_redondeando_hacia_abajo()
    {
        var mesa = Mesa(1000, As, Nueve, Rey, Siete);
        mesa.Apostar(25);
        Assert.Equal(ResultadoRonda.BlackjackJugador, mesa.Resultado);
        Assert.Equal(37, mesa.GananciaNeta);
        Assert.Equal(1037, mesa.Jugador.Saldo);
    }

    [Fact]
    public void Si_ambos_tienen_blackjack_es_empate_y_se_devuelve_la_apuesta()
    {
        var mesa = Mesa(1000, As, As, Rey, Reina);
        mesa.Apostar(100);
        Assert.Equal(ResultadoRonda.Empate, mesa.Resultado);
        Assert.Equal(1000, mesa.Jugador.Saldo);
    }

    [Fact]
    public void El_blackjack_de_la_banca_termina_la_ronda_antes_del_turno_del_jugador()
    {
        var mesa = Mesa(1000, Diez, As, Nueve, Rey);
        mesa.Apostar(100);
        Assert.Equal(ResultadoRonda.BlackjackBanca, mesa.Resultado);
        Assert.Equal(FaseJuego.Apuesta, mesa.Fase);
        Assert.Equal(900, mesa.Jugador.Saldo);
    }

    [Fact]
    public void Pasarse_de_21_pierde_la_apuesta()
    {
        var mesa = Mesa(1000, Diez, Nueve, Seis, Siete, Rey);
        mesa.Apostar(100);
        mesa.PedirCarta();
        Assert.Equal(ResultadoRonda.JugadorSePasa, mesa.Resultado);
        Assert.Equal(900, mesa.Jugador.Saldo);
    }

    [Fact]
    public void Con_21_el_jugador_se_planta_automaticamente()
    {
        // Jugador 5+6 = 11, pide un 10 → 21. Banca 10+7 = 17, se planta.
        var mesa = Mesa(1000, Cinco, Diez, Seis, Siete, Rey);
        mesa.Apostar(100);
        mesa.PedirCarta();
        Assert.Equal(ResultadoRonda.GanaJugador, mesa.Resultado);
        Assert.Equal(1100, mesa.Jugador.Saldo);
    }

    [Fact]
    public void La_banca_se_planta_con_17_suave()
    {
        // Banca As+6 = 17 suave: no pide más aunque quede un 4 en la baraja.
        var mesa = Mesa(1000, Diez, As, Ocho, Seis, Cuatro);
        mesa.Apostar(100);
        mesa.Plantarse();
        Assert.Equal(2, mesa.Banca.Cartas.Count);
        Assert.Equal(ResultadoRonda.GanaJugador, mesa.Resultado);
    }

    [Fact]
    public void La_banca_pide_hasta_17_y_puede_pasarse()
    {
        var mesa = Mesa(1000, Diez, Diez, Ocho, Seis, Nueve);
        mesa.Apostar(100);
        mesa.Plantarse();
        Assert.Equal(ResultadoRonda.BancaSePasa, mesa.Resultado);
        Assert.Equal(1100, mesa.Jugador.Saldo);
    }

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

    [Fact]
    public void No_se_puede_doblar_sin_saldo_suficiente()
    {
        var mesa = Mesa(150, Cinco, Diez, Seis, Siete);
        mesa.Apostar(100);
        Assert.False(mesa.PuedeDoblar);
        Assert.Throws<InvalidOperationException>(mesa.Doblar);
    }

    [Fact]
    public void No_se_puede_jugar_sin_apostar_ni_apostar_mas_del_saldo()
    {
        var mesa = Mesa(50, Cinco, Diez, Seis, Siete);
        Assert.Throws<InvalidOperationException>(mesa.PedirCarta);
        Assert.Throws<InvalidOperationException>(() => mesa.Apostar(100));
        Assert.Throws<InvalidOperationException>(() => mesa.Apostar(0));
        Assert.Equal(50, mesa.Jugador.Saldo);
    }

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
