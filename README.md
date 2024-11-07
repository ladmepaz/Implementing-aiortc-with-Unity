# Implementacion de aiortc con Unity para trasmisión de video

## Descripción del server.py

El server.py es quien inicia la trasmisión de video, este está programado para que la IP de quien ejecute el script, sirva de enlace con el cliente **WebRTC** y **WebSockets**. Las librerías implementadas son [websockets 13.1](https://pypi.org/project/websockets/), y [aiortc](https://github.com/aiortc/aiortc?tab=readme-ov-file).

### Instalación de dependencias

Para instalar las dependencias necesarias, puedes usar los siguientes comandos:

```sh
pip install websockets==13.1
pip install aiortc
```
