# BlackJack-CS

## Descripción

BlackJack-CS es un juego de cartas clásico de Blackjack desarrollado en C# y Windows Forms (.NET). 
El proyecto implementa la lógica completa del juego (reparto de cartas, cálculo de puntuaciones, as flexible con valor 1 u 11, plantarse, pedir carta y mano del crupier) aplicando principios de Programación Orientada a Objetos y estructuras de datos.

## Características

- Partida interactiva de Blackjack contra la casa / crupier.
- Baraja de 52 cartas con imágenes gráficas de naipes.
- Lógica de valor de cartas con detección automática de Blackjack y superación de 21 puntos.
- Registro y guardado de historial de partidas.
- Interfaz gráfica intuitiva con Windows Forms.

## Tecnologías Utilizadas

- **Lenguaje:** C#
- **Framework:** .NET Framework / .NET Core (Windows Forms)
- **IDE:** Visual Studio

## Instalación y Ejecución

1. Clona el repositorio:
   ```bash
   git clone https://github.com/santyxswc/BlackJack-CS.git
   ```

2. Navega al directorio del proyecto:
   ```bash
   cd BlackJack-CS
   ```

3. Abre la solución `BlackJack.sln` en **Visual Studio**.

4. Compila y ejecuta el proyecto (`F5` o botón de Inicio).

### Ejecutar en Linux / macOS (Avalonia)

El proyecto original usa Windows Forms (.NET Framework 4.7.2) y solo corre en Windows.
`BlackJack.Avalonia/` es la versión multiplataforma hecha con [Avalonia](https://avaloniaui.net/):
comparte `Carta.cs`, `Baraja.cs`, `JuegoBlackjack.cs` y la carpeta `imagenes/`, y funciona en Linux, macOS y Windows.

Requisito: [.NET 10 SDK](https://dotnet.microsoft.com/).

```bash
dotnet run --project BlackJack.Avalonia
```

#### Cuentas de jugador

- Al abrir el juego se inicia sesión o se crea una cuenta (las cuentas nuevas empiezan con $1000).
- El saldo y las estadísticas (ganadas, perdidas, empates) se guardan después de cada apuesta y cada ronda.
- Las contraseñas se guardan con hash PBKDF2-SHA256 y sal, nunca en texto plano.
- Si cierras la ventana en medio de una mano, el jugador se planta automáticamente.
- Con saldo $0 aparece el botón "Recargar $1000".

Los datos se guardan en `~/.local/share/BlackJack/` (Linux), `%LOCALAPPDATA%\BlackJack\` (Windows)
o `~/Library/Application Support/BlackJack/` (macOS):

- `jugadores.json`: cuentas, saldo y estadísticas.
- `historial/`: un archivo de texto por sesión con cada apuesta, carta y resultado.

#### Reglas implementadas (versión Avalonia)

- Primero se apuesta y después se reparten las cartas.
- Blackjack natural (21 con dos cartas) paga 3:2; si ambos lo tienen es empate.
- La banca revisa si tiene blackjack antes del turno del jugador.
- Doblar: solo con las dos primeras cartas; se duplica la apuesta y se recibe una sola carta.
- La banca pide carta hasta 17 y se planta con 17 suave.
- Ganar paga 1:1 y el empate devuelve la apuesta.

#### Controles

| Acción | Ratón | Teclado |
|---|---|---|
| Apostar | Fichas + "Apostar" | Escribir la cantidad + `Enter` (Enter repite la última apuesta) |
| Pedir carta | "Pedir carta" | `P` |
| Plantarse | "Plantarse" | `S` |
| Doblar | "Doblar" | `D` |

## Modo de Juego

1. Al iniciar la aplicación, presiona el botón para repartir las cartas iniciales.
2. Decide si pedir otra carta (**Pedir**) o quedarte con tu mano actual (**Plantarse**).
3. El crupier jugará su turno siguiendo las reglas del juego.
4. Se determinará el ganador y se actualizará el historial de la partida.
