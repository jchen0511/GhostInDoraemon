from Pid import PID
from ImageProcessor import ImageProcessor
import CustomLog

class AutoDrive(object):
    STEERING_PID_Kp             = 0.3
    STEERING_PID_Ki             = 0.01
    STEERING_PID_Kd             = 0.1
    STEERING_PID_max_integral   = 10
    THROTTLE_PID_Kp             = 0.02
    THROTTLE_PID_Ki             = 0.005
    THROTTLE_PID_Kd             = 0.02
    THROTTLE_PID_max_integral   = 0.5
    MAX_STEERING_HISTORY        = 3
    MAX_THROTTLE_HISTORY        = 3
    DEFAULT_SPEED               = 0.5

    debug = True

    def __init__(self, car, record_folder = None):
        self._record_folder    = record_folder
        self._steering_pid     = PID(Kp=self.STEERING_PID_Kp  , Ki=self.STEERING_PID_Ki  , Kd=self.STEERING_PID_Kd  , max_integral=self.STEERING_PID_max_integral  )
        self._throttle_pid     = PID(Kp=self.THROTTLE_PID_Kp  , Ki=self.THROTTLE_PID_Ki  , Kd=self.THROTTLE_PID_Kd  , max_integral=self.THROTTLE_PID_max_integral  )
        self._throttle_pid.assign_set_point(self.DEFAULT_SPEED)
        self._steering_history = []
        self._throttle_history = []
        self._car = car
        self._car.register(self)


    def on_dashboard(self, src_img, last_steering_angle, speed, throttle, info):
        track_img     = ImageProcessor.preprocess(src_img)
        current_angle = ImageProcessor.find_steering_angle_by_color(track_img, last_steering_angle, debug = self.debug)
        #current_angle = ImageProcessor.find_steering_angle_by_line(track_img, last_steering_angle, debug = self.debug)
        steering_angle = self._steering_pid.update(-current_angle)
        throttle       = self._throttle_pid.update(speed)

        if self.debug:
            ImageProcessor.show_image(src_img, "source")
            ImageProcessor.show_image(track_img, "track")
            CustomLog.logit("steering PID: %0.2f (%0.2f) => %0.2f (%0.2f)" % (current_angle, ImageProcessor.rad2deg(current_angle), steering_angle, ImageProcessor.rad2deg(steering_angle)))
            CustomLog.logit("throttle PID: %0.4f => %0.4f" % (speed, throttle))
            CustomLog.logit("info: %s" % repr(info))

        if self._record_folder:
            suffix = "-deg%0.3f" % (ImageProcessor.rad2deg(steering_angle))
            ImageProcessor.save_image(self._record_folder, src_img  , prefix = "cam", suffix = suffix)
            ImageProcessor.save_image(self._record_folder, track_img, prefix = "trk", suffix = suffix)

        #smooth the control signals
        self._steering_history.append(steering_angle)
        self._steering_history = self._steering_history[-self.MAX_STEERING_HISTORY:]
        self._throttle_history.append(throttle)
        self._throttle_history = self._throttle_history[-self.MAX_THROTTLE_HISTORY:]

        self._car.control(sum(self._steering_history)/self.MAX_STEERING_HISTORY, sum(self._throttle_history)/self.MAX_THROTTLE_HISTORY)