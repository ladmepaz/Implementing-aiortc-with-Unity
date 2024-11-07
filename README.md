# Implementacion de aiortc con Unity para trasmisión de video

## Descripción del server.py

El servidor es quien inicia la trasmisión de video, este está programado en python para que la IP de quien ejecute el script, sirva de enlace con el cliente **WebRTC** y **WebSockets**. Las librerías implementadas son [websockets 13.1](https://pypi.org/project/websockets/), y [aiortc](https://github.com/aiortc/aiortc?tab=readme-ov-file).

### Configuración de video

Las pruebas principales se realizaron en una Raspberry Pi 4 Model B, sin embargo, el script también se ejecutó en los computadores del laboratorio de datos. Python facilita su ejecución en muchas plataformas, como también lo puede ser una Jetson Nano de Nvidia.

### Instalación de dependencias

Para instalar las dependencias necesarias, puedes usar los siguientes comandos:

```sh
pip install websockets==13.1
pip install aiortc
```
