# Implementacion de aiortc con Unity para trasmisión de video

## Descripción del server.py

El server.py es quien inicia la trasmisión de video, este está programado para que la IP de quien ejecute el script, sirva de enlace con el cliente **WebRTC** y **WebSockets**. Las librerías implementadas son [websockets 13.1](https://pypi.org/project/websockets/), y [aiortc](https://github.com/aiortc/aiortc?tab=readme-ov-file).

### Instalación de dependencias

Para instalar las dependencias necesarias, puede usar los siguientes comandos:

```sh
pip install websockets==13.1
pip install aiortc
```

## Descripción de los archivos C#

Al archivo WebRTCManager.cs se le debe agragar la IP del dispositivo donde se ejecute server.py, para iniciar las negociaciaciones de los ICE Candidates y SDPs necesarias para que la trasmisión sea con exito.

### Preparación del entorno en Unity:

El primer paso es agregar la librería WebSockets-Sharp. Se descarga **websocket-sharp.dll** a través de [NuGet Gallery](https://www.nuget.org/), descomprima el archivo **.nupkg**, ingrese a la carpeta lib, en ella encontrará el **.dll**, se adjunta junto a los archivos de Unity en **Assets/Plugins**. Puede encontrar la documentacion aca: [websocket-sharp](https://github.com/sta/websocket-sharp/tree/master).

WebRTC: la versión implementada para el protocolo de conexión es la 2.4.0-exp.11 · October 04, 2022. Con esta versión se logró establecer una correcta conexión para el envio de los ICE Candidates y la transmisión de las SDP. A través de Window/Package Manager, y agregando el paquete al entorno por medio de Add package from git URL... Puede encontrar la documentacion aca: [Unity](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/index.html)

### Dependencia adicional

Para la integración de WebRTC en Unity, se debe agregar la siguiente dependencia:

```sh
com.unity.webrtc@2.4.0-exp.11
```

## Entorno Unity

La preparación del entorno consta de la creación de un canvas UI, panel y RawImage en el entorno, por ejemplo:

<p align="center">
  <img src="/Imagenes/Scene.png" alt="Create UI Canvas" width="400">
</p>
Adjunte el contenido de la carpera  **C#** a su entrono y asegurece de tener WebSockets ya instalado:
<p align="center">
  <img src="/Imagenes/Scripts.png" alt="Scrtips" width="400">
</p>
