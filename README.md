# Implementación de aiortc con Unity para transmisión de video

## Descripción del server.py

El `server.py` es quien inicia la transmisión de video, este está programado para que la IP de quien ejecute el script, sirva de enlace con el cliente **WebRTC** y **WebSockets**. Las librerías implementadas son [websockets 13.1](https://pypi.org/project/websockets/), y [aiortc](https://github.com/aiortc/aiortc?tab=readme-ov-file).

### Instalación de dependencias

Para instalar las dependencias necesarias, puede usar los siguientes comandos:

```sh
pip install websockets==13.1
pip install aiortc
```

## Descripción de los archivos C#

Al archivo `WebRTCManager.cs` se le debe agragar la IP del dispositivo donde se ejecute `server.py`, para iniciar las negociaciaciones de los ICE Candidates y SDPs necesarias para que la trasmisión sea con exito.

### Preparación del entorno en Unity:

El primer paso es agregar la librería WebSockets-Sharp. Se descarga **websocket-sharp.dll** a través de [NuGet Gallery](https://www.nuget.org/), descomprima el archivo **.nupkg**, ingrese a la carpeta lib, en ella encontrará el **.dll**, se adjunta junto a los archivos de Unity en **Assets/Plugins**. Puede encontrar la documentacion en: [websocket-sharp](https://github.com/sta/websocket-sharp/tree/master).

WebRTC: la versión implementada para el protocolo de conexión es la 2.4.0-exp.11 · October 04, 2022. Con esta versión se logró establecer una correcta conexión para el envio de los ICE Candidates y la transmisión de las SDP. A través de Window/Package Manager, y agregando el paquete al entorno por medio de Add package from git URL... Puede encontrar la documentacion en: [Unity](https://docs.unity3d.com/Packages/com.unity.webrtc@2.4/manual/index.html)

### Dependencia adicional

Para la integración de WebRTC en Unity, se debe agregar la siguiente dependencia:

```sh
com.unity.webrtc@2.4.0-exp.11
```

## Entorno Unity

La preparación del entorno consta de la creación de un canvas UI, panel, RawImage y un objeto vacio WebSocket, dónde se asignará el script, por ejemplo:

<p align="center">
  <img src="/Imagenes/Scene.png" alt="Create UI Canvas" width="300">
</p>

Adjunte el contenido de la carpera **`C#`** a su entrono y asegurece de tener WebSockets ya instalado:

<p align="center">
  <img src="/Imagenes/Scripts.png" alt="Scrtips" width="400">
</p>

Asigne el script `WebRTCManager.cs` al objeto WebSocket, asignele al script el RawImage que creó, e indique el puerto "Port" y la direccion ip en "Server Ip" (Sin espacios ni caracteres especiales) del despositivo donde desee ejecutar el script `server.py`, por ejemplo:

<p align="center">
  <img src="/Imagenes/WebSocket.png" alt="Configurar script WebRTCManager.cs">
</p>

Configure el objeto RawImage, en este caso se ajusta a 640x480:

<p align="center">
  <img src="/Imagenes/RawImage.png" alt="Configuara RawImage">
</p>

## Iniciar transmision

Para este ejemplo, se ejecutó `server.py` en una Raspberry Pi 4, que está conectada a la misma red wifi que el computador donde se encunetra el entorno de Unity:

<p align="center">
  <img src="/Imagenes/Rasp.png" alt="Iniciando el servidor">
</p>

Asegúrese de primero ejecutar `server.py`. El siguiente paso es inicializar el entorno Unity. Si asignó de manera correcta la dirección IP en el entorno, en la consola podrá observar la transmisión de los **ICE Candidates** y **SDPs**. Por último, se carga la textura de manera automática al RawImage:

<p align="center">
  <img src="/Imagenes/Video.png" alt="Recibiendo video">
</p>

Como puede observar, la recepción de video ha comenzado. En este caso se visualiza el teclado y las Raspberry Pi usadas en esta práctica.

#Autores
[Over Alexander Mejia Rosado](mailto:omejiar@unal.edu.co)
[Ronsald Mateo Ceballos Lozano](mailto:rceballosl@unal.edu.co)

