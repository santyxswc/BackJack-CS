/**
 * @file ValorManoTests.cs
 * @brief Pruebas del valor de una mano, las cartas y la baraja.
 * @author Santiago Caicedo
 */
using BlackJack.Core.Juego;

namespace BlackJack.Core.Tests;

/**
 * @brief Pruebas de ValorMano, Carta y Baraja.
 */
public class ValorManoTests
{
    /**
     * @brief Calcula el valor de una mano con estos rangos.
     * @param rangos Rangos de las cartas
     * @return Valor de la mano
     */
    private static ValorMano Valor(params Rango[] rangos) =>
        ValorMano.De(rangos.Select(r => new Carta(r, Palo.Corazones)));

    /**
     * @brief J, Q y K valen 10 cada una.
     */
    [Fact]
    public void Las_figuras_valen_diez() =>
        Assert.Equal(new ValorMano(30, false), Valor(Rango.Jota, Rango.Reina, Rango.Rey));

    /**
     * @brief Un As con un 6 suma 17 suave.
     */
    [Fact]
    public void El_as_cuenta_como_once_si_no_se_pasa() =>
        Assert.Equal(new ValorMano(17, true), Valor(Rango.As, Rango.Seis));

    /**
     * @brief Si contar el As como 11 pasa de 21, vale 1.
     */
    [Fact]
    public void El_as_cuenta_como_uno_si_once_haria_pasarse() =>
        Assert.Equal(new ValorMano(17, false), Valor(Rango.As, Rango.Seis, Rango.Diez));

    /**
     * @brief Dos ases suman 12: solo uno puede valer 11.
     */
    [Fact]
    public void Solo_un_as_puede_contar_once() =>
        Assert.Equal(new ValorMano(12, true), Valor(Rango.As, Rango.As));

    /**
     * @brief El nombre de la carta usa el símbolo impreso (A, 10, Q...).
     * @param rango Rango de la carta
     * @param simbolo Símbolo esperado
     */
    [Theory]
    [InlineData(Rango.As, "A")]
    [InlineData(Rango.Diez, "10")]
    [InlineData(Rango.Reina, "Q")]
    public void El_simbolo_coincide_con_el_de_la_carta(Rango rango, string simbolo) =>
        Assert.Equal($"{simbolo} de Treboles", new Carta(rango, Palo.Treboles).ToString());

    /**
     * @brief Una baraja mezclada reparte 52 cartas distintas y luego se agota.
     */
    [Fact]
    public void La_baraja_tiene_52_cartas_distintas()
    {
        var baraja = new Baraja(new Random(42));
        var cartas = Enumerable.Range(0, 52).Select(_ => baraja.RepartirCarta()).ToList();
        Assert.Equal(52, cartas.Distinct().Count());
        Assert.Throws<InvalidOperationException>(() => baraja.RepartirCarta());
    }
}
