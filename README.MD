## Victor Base C# Binding & SDK - INDEV

- VictorNative es el binding de interoperabilidad en C# para la librería nativa Victor, proporcionando acceso eficiente a sus funcionalidades de indexación y búsqueda.
- Dentro de él, se implementa el desarrollo y posterior testing del SDK para su posterior paquetizacion en NuGet.


## Estructura del proyecto / File Structure

![Estructura actual](Assets/estructura%20de%20archivos.png)

- InterOp contiene VictorSDK: Dentro está el archivo fuente del SDK, que es una de las capas de abstracción de las funciones nativas.
- A su vez, InterOp contiene el fuente VictorNative.cs, que es la primer capa de contacto con las funciones del core de la bbdd.
- Se incluyen una carpeta de Test, donde se escriben pruebas de usuario y unitarias.
- Common contiene archivos fuente con implementaciones que mas de un fuente requieren.
---

- Pendiente de generar docs.
- Pendiente de abrir los primeros issues.

## Contribuciones / Contributions


