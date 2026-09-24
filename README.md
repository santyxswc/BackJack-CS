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

## Modo de Juego

1. Al iniciar la aplicación, presiona el botón para repartir las cartas iniciales.
2. Decide si pedir otra carta (**Pedir**) o quedarte con tu mano actual (**Plantarse**).
3. El crupier jugará su turno siguiendo las reglas del juego.
4. Se determinará el ganador y se actualizará el historial de la partida.
