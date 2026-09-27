using BlackJack.Core.Juego;

namespace BlackJack.Core.Tests;

public class ValorManoTests
{
    private static ValorMano Valor(params Rango[] rangos) =>
        ValorMano.De(rangos.Select(r => new Carta(r, Palo.Corazones)));

    [Fact]
    public void Las_figuras_valen_diez() =>
        Assert.Equal(new ValorMano(30, false), Valor(Rango.Jota, Rango.Reina, Rango.Rey));

    [Fact]
    public void El_as_cuenta_como_once_si_no_se_pasa() =>
        Assert.Equal(new ValorMano(17, true), Valor(Rango.As, Rango.Seis));

    [Fact]
    public void El_as_cuenta_como_uno_si_once_haria_pasarse() =>
        Assert.Equal(new ValorMano(17, false), Valor(Rango.As, Rango.Seis, Rango.Diez));

    [Fact]
    public void Solo_un_as_puede_contar_once() =>
        Assert.Equal(new ValorMano(12, true), Valor(Rango.As, Rango.As));

    [Theory]
    [InlineData(Rango.As, "A")]
    [InlineData(Rango.Diez, "10")]
    [InlineData(Rango.Reina, "Q")]
    public void El_simbolo_coincide_con_el_de_la_carta(Rango rango, string simbolo) =>
        Assert.Equal($"{simbolo} de Treboles", new Carta(rango, Palo.Treboles).ToString());

    [Fact]
    public void La_baraja_tiene_52_cartas_distintas()
    {
        var baraja = new Baraja(new Random(42));
        var cartas = Enumerable.Range(0, 52).Select(_ => baraja.RepartirCarta()).ToList();
        Assert.Equal(52, cartas.Distinct().Count());
        Assert.Throws<InvalidOperationException>(() => baraja.RepartirCarta());
    }
}
