import time


def seconds(sec, record_is_running):
    if record_is_running:
        sec = sec + 1
        return sec
