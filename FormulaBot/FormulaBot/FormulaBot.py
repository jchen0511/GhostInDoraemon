#!env python
#
# Auto-driving Bot
#
# Revision: v1.2
# Released Date: Aug 20, 2018
#
from time import time
from PIL  import Image
from io   import BytesIO

#import datetime
import os
import cv2
import math
import numpy as np
import base64
import logging

import AutoDrive
import Car



def logit(msg):
    print("%s" % msg)


if __name__ == "__main__":
    import shutil
    import argparse
    from datetime import datetime

    import socketio
    import eventlet
    import eventlet.wsgi
    from flask import Flask

    parser = argparse.ArgumentParser(description='AutoDriveBot')
    parser.add_argument('record',
        type=str,
        nargs='?',
        default='',
        help='Path to image folder to record the images.')
    args = parser.parse_args()

    if args.record:
        if not os.path.exists(args.record):
            os.makedirs(args.record)
        logit("Start recording images to %s..." % args.record)

    sio = socketio.Server()
    def send_control(steering_angle, throttle):
        sio.emit("steer",
            data={
                'steering_angle': str(steering_angle),
                'throttle': str(throttle)
            },
            skip_sid=True)

    car = Car.Car(control_function = send_control)
    drive = AutoDrive.AutoDrive(car, args.record)

    @sio.on('telemetry')
    def telemetry(sid, dashboard):
        if dashboard:
            car.on_dashboard(dashboard)
        else:
            sio.emit('manual', data={}, skip_sid=True)

    @sio.on('connect')
    def connect(sid, environ):
        car.control(0, 0)

    app = socketio.Middleware(sio, Flask(__name__))
    eventlet.wsgi.server(eventlet.listen(('', 4567)), app)
