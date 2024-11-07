# Implementacion de aiortc con Unity para trasmisión de video

## Descripción del server.py

El server.py es quien inicia la trasmisión de video, este está programado para que la IP de quien ejecute el script, sirva de enlace con el cliente **WebRTC** y **WebSockets**. Las librerías implementadas son [websockets 13.1](https://pypi.org/project/websockets/), y [aiortc](https://github.com/aiortc/aiortc?tab=readme-ov-file).

### Instalación de dependencias

Para instalar las dependencias necesarias, puedes usar los siguientes comandos:

```sh
pip install websockets==13.1
pip install aiortc
```

## Descripción de los archivos C#

Al archivo WebRTCManager.cs se le debe agragar la IP del dispositivo donde se ejecute server.py, para iniciar las negociaciaciones de los ICE Candidates y SDPs necesarias para que la trasmisión sea con exito.

### Preparación del entorno en Unity:

El primer paso es agregar la librería WebSockets-Sharp. Se descarga **websocket-sharp.dll** a través de [NuGet Gallery](https://www.nuget.org/), descomprima el archivo **.nupkg**, ingrese a la carpeta lib, en ella encontrará el **.dll**, se adjunta junto a los archivos de Unity en **Assets/Plugins**. Puede encontrar la documentacion aca: [websocket-sharp](https://github.com/sta/websocket-sharp/tree/master).
