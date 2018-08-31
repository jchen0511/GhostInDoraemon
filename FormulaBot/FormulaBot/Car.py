class Car(object):
    MAX_STEERING_ANGLE = 40.0


    def __init__(self, control_function):
        self._driver = None
        self._control_function = control_function


    def register(self, driver):
        self._driver = driver


    def on_dashboard(self, dashboard):
        #normalize the units of all parameters
        last_steering_angle = np.pi / 2 - float(dashboard["steering_angle"]) / 180.0 * np.pi
        throttle = float(dashboard["throttle"])
        brake = float(dashboard["brakes"])
        speed = float(dashboard["speed"])
        img = ImageProcessor.bgr2rgb(np.asarray(Image.open(BytesIO(base64.b64decode(dashboard["image"])))))
        #del dashboard["image"]
        #print datetime.now();
        #print dashboard;
        total_time = dashboard["time"]
        elapsed = total_time

        info = {
            "lap"    : int(dashboard["lap"]) if "lap" in dashboard else 0,
            "elapsed": elapsed,
            "status" : int(dashboard["status"]) if "status" in dashboard else 0,
        }
        self._driver.on_dashboard(img, last_steering_angle, speed, throttle, info)


    def control(self, steering_angle, throttle):
        #convert the values with proper units
        steering_angle = min(max(ImageProcessor.rad2deg(steering_angle), -Car.MAX_STEERING_ANGLE), Car.MAX_STEERING_ANGLE)
        self._control_function(steering_angle, throttle)

