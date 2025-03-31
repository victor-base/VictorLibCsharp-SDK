## Victor Base C# Binding - INDEV

- VictorNative es el binding de interoperabilidad en C# para la librería nativa Victor, proporcionando acceso eficiente a sus funcionalidades de indexación y búsqueda

## Estructura del proyecto

VictorBaseDotNET/
│── src/
│   ├── interop/              # Contiene las definiciones de funciones nativas
│   │   └── NativeMethods.cs   # Enlace directo a las funciones C nativas con DllImport
│   ├── victorbase/           # Código de nivel más alto que abstrae la interacción con la librería nativa
│── test/                     # Pruebas de integración y unitarias
│── VictorBaseDotNET.csproj       # Archivo de proyecto de .NET
│── VictorBaseDotNET.sln          # Solución principal
│── Directory.Build.props     # Configuración para compatibilidad multiplataforma
│── README.md                 # Este archivo


- Pendiente de generar docs