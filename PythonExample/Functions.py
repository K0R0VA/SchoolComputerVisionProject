# coding=Windows-1251
import re
from os import getcwd
from os import getcwd

import cv2
import cv2.aruco as aruco
import numpy as np
import plotly.graph_objects as go
import xlrd


def getMask(frame, low, high, blur):
    hsv_frame = cv2.cvtColor(frame, cv2.COLOR_RGB2HSV_FULL)
    mask = cv2.inRange(hsv_frame, low, high)
    mask = cv2.medianBlur(mask, blur)
    return mask


def getPoint(mask, result, path, color):
    contour, _ = cv2.findContours(mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    maxperimetr = -1
    maxind = -1
    for i in range(0, len(contour)):
        perimeter = cv2.arcLength(contour[i], True)
        if perimeter > maxperimetr:
            maxperimetr = perimeter
            maxind = i
    if maxind >= 0:
        moments = cv2.moments(contour[maxind])
        dM01 = moments['m01']
        dM10 = moments['m10']
        dArea = moments['m00']
        if dArea != 0:
            x: int = int(dM10 / dArea)
            y: int = 480 - int(dM01 / dArea)
            radius = np.int(np.sqrt(dArea / 3.14))
            point1 = (x - radius, y + radius)
            point2 = (x + radius, y - radius)
            cv2.rectangle(result, point1, point2, color, 1)
            if 0 < x < 640 and 0 < y < 480:
                if len(path) > 0:
                    distance = getdistancebeetwenpixel(x, path[-1][0], y, path[-1][1])
                    if radius == path[-1][2]:
                        distortiononPlace(radius, distance, path, result, color, x, y)
                    else:
                        distortioninSphera(radius, path, result, color, x, y)
                else:
                    path.append([x, y, radius])
            else:
                path.append(path[-1])
        else:
            path.append([-999, -999, -999])
    else:
        path.append([-999, -999, -999])


def MakeVideo(result):
    out = cv2.VideoWriter('output.avi', -1, 20.0, (640, 480))
    out.write(result)


Dictionary = aruco.Dictionary_get(aruco.DICT_ARUCO_ORIGINAL)


def getdistancebeetwenpixel(x1, x2, y1, y2):
    return np.sqrt(np.power(x1 - x2, 2) + np.power(y1 - y2, 2))


def detectMark(frame, weighmark, id=671):
    corners, ids, _ = aruco.detectMarkers(frame, Dictionary)
    aruco.drawDetectedMarkers(frame, corners)
    if corners is not None and len(corners) > 0 and id == ids[0]:
        green = corners[0][0][0]
        blue = corners[0][0][1]
        red = corners[0][0][2]
        yellow = corners[0][0][3]
        cv2.line(frame, (0, 0), (green[0], green[1]), (0, 255, 0), 3)
        cv2.line(frame, (0, 0), (blue[0], blue[1]), (0, 0, 255), 3)
        cv2.line(frame, (0, 0), (red[0], red[1]), (255, 0, 0), 3)
        cv2.line(frame, (0, 0), (yellow[0], yellow[1]), (0, 255, 255), 3)
        deltaY = getdistancebeetwenpixel(green[0], red[0], green[1], red[1])
        deltaX = getdistancebeetwenpixel(green[0], yellow[0], green[1], yellow[1])
        delta = np.sqrt(np.power(deltaX, 2) + np.power(deltaY, 2)) / (weighmark * 1.41421356237)
        return delta
    else:
        return 1


def Show3D(FileName):
    wb = xlrd.open_workbook(getcwd() + FileName + '.xlsx')
    sheet = wb.sheet_by_index(0)
    x = []
    y = []
    z = []
    if sheet.cell_value(2, 0) != '':
        for i in range(2, sheet.nrows):
            x.append(int(sheet.cell_value(int(i), 0)))
            y.append(int(sheet.cell_value(int(i), 1)))
            z.append(int(sheet.cell_value(int(i), 2)))

        fig = go.Figure(data=[go.Scatter3d(
            x=x,
            y=y,
            z=z,
            mode='lines+markers',
            marker=dict(
                size=12,
                color=z,  # set color to an array/list of desired values
                colorscale='Viridis',  # choose a colorscale
                opacity=0.8
            )
        )])

        # tight layout
        fig.update_layout(margin=dict(l=0, r=0, b=0, t=0))
        fig.show()


def Show3D2(FileName):
    wb = xlrd.open_workbook(getcwd() + FileName + '.xlsx')
    sheet = wb.sheet_by_index(0)
    x = []
    y = []
    z = []
    if sheet.cell_value(2, 5) != '':
        for i in range(2, sheet.nrows):
            x.append(int(sheet.cell_value(int(i), 5)))
            y.append(int(sheet.cell_value(int(i), 6)))
            z.append(int(sheet.cell_value(int(i), 7)))

        fig = go.Figure(data=[go.Scatter3d(
            x=x,
            y=y,
            z=z,
            mode='lines+markers',
            marker=dict(
                size=12,
                color=z,  # set color to an array/list of desired values
                colorscale='Viridis',  # choose a colorscale
                opacity=0.8
            )
        )])

    fig.update_layout(margin=dict(l=0, r=0, b=0, t=0))
    fig.show()


def ShowSpeedChart(FileName):
    wb = xlrd.open_workbook(getcwd() + FileName + '.xlsx')
    sheet = wb.sheet_by_index(0)
    x_obj_1 = []
    y_obj_1 = []
    x_obj_2 = []
    y_obj_2 = []
    if sheet.cell_value(2, 1) != '':
        for i in range(3, sheet.nrows):
            x1 = sheet.cell_value(int(i - 1), 0)
            x2 = sheet.cell_value(int(i), 0)
            y1 = sheet.cell_value(int(i - 1), 1)
            y2 = sheet.cell_value(int(i), 1)
            z1 = sheet.cell_value(int(i - 1), 2)
            z2 = sheet.cell_value(int(i), 2)
            t1 = sheet.cell_value(int(i - 1), 3)
            t2 = sheet.cell_value(int(i), 3)
            if x1 != x2 or y1 != y2 or z1 != z2:
                speed1 = np.sqrt(np.power(x2 - x1, 2) + np.power(y2 - y1, 2) + np.power(z2 - z1, 2)) / np.abs(
                    float(t1) - float(t2))
                x_obj_1.append(speed1 / 1000)
                y_obj_1.append(sheet.cell_value(int(i), 3))
    if sheet.cell_value(2, 6) != '':
        for i in range(3, sheet.nrows):
            x1 = sheet.cell_value(int(i - 1), 5)
            x2 = sheet.cell_value(int(i), 5)
            y1 = sheet.cell_value(int(i - 1), 6)
            y2 = sheet.cell_value(int(i), 6)
            z1 = sheet.cell_value(int(i - 1), 7)
            z2 = sheet.cell_value(int(i), 7)
            t1 = sheet.cell_value(int(i - 1), 8)
            t2 = sheet.cell_value(int(i), 8)
            if x1 != x2 or y1 != y2 or z1 != z2:
                speed2 = np.sqrt(np.power(x2 - x1, 2) + np.power(y2 - y1, 2) + np.power(z2 - z1, 2)) / np.abs(
                    float(t1) - float(t2))
                x_obj_2.append(speed2 / 1000)
                y_obj_2.append(sheet.cell_value(int(i), 8))
    fig = go.Figure()
    if sheet.cell_value(2, 0) != '':
        fig.add_trace(go.Scatter(x=y_obj_1, y=x_obj_1,
                                 mode='lines+markers',
                                 name='object1'))
    if sheet.cell_value(2, 6) != '':
        fig.add_trace(go.Scatter(x=y_obj_2, y=x_obj_2,
                                 mode='lines+markers',
                                 name='object2'))
    fig.update_layout(title='График изменения скоростей отслеживаемых объектов',
                      xaxis_title='Скорость (м/с)',
                      yaxis_title='Время (с)')
    fig.show()


def Xt_Yt(FileName):
    wb = xlrd.open_workbook(getcwd() + FileName + '.xlsx')
    sheet = wb.sheet_by_index(0)
    x = []
    y = []
    t = []
    for i in range(2, sheet.nrows):
        x.append(sheet.cell_value(int(i), 0))
        y.append(sheet.cell_value(int(i), 1))
        t.append(sheet.cell_value(int(i), 3))
    fig = go.Figure()
    if sheet.cell_value(2, 6) != '':
        fig.add_trace(go.Scatter(x=t, y=y,
                                 mode='lines+markers',
                                 name='object2'))
    fig.update_layout(title='График колебаний',
                      xaxis_title='Время (с)',
                      yaxis_title='Перемещение (мм)')
    fig.show()


def Xt(FileName):
    wb = xlrd.open_workbook(getcwd() + FileName + '.xlsx')
    sheet = wb.sheet_by_index(0)
    x = []
    y = []
    t = []
    for i in range(2, sheet.nrows):
        x.append(sheet.cell_value(int(i), 0))
        y.append(sheet.cell_value(int(i), 1))
        t.append(sheet.cell_value(int(i), 3))
    fig = go.Figure()
    if sheet.cell_value(2, 6) != '':
        fig.add_trace(go.Scatter(x=t, y=x,
                                 mode='lines+markers',
                                 name='object2'))
    fig.update_layout(title='График колебаний',
                      xaxis_title='Время (с)',
                      yaxis_title='Перемещение (мм)')
    fig.show()


def hypotenuse(width, height):
    return np.sqrt(np.power(width, 2) + np.power(height, 2))


def drowtrack(result, path, length, color):
    if length > 10:
        for i in range(length - 10, length - 1):
            cv2.line(result, (path[i][0], path[i][1]), (path[i + 1][0], path[i + 1][1]),
                     color, 2)


def distortiononPlace(radius, distance, path, result, color, x, y):
    if radius * 0.35 > distance:
        path.append(path[-1])
        print('жижа')
        length = len(path)
        drowtrack(result, path, length, color)
    else:
        path.append([x, y, radius])
        length = len(path)
        drowtrack(result, path, length, color)


def distortioninSphera(radius, path, result, color, x, y):
    if 2 > radius - path[-1][2] > -2:
        path.append([x, y, path[-1][2]])
        length = len(path)
        drowtrack(result, path, length, color)
    else:
        path.append([x, y, radius])
        length = len(path)
        drowtrack(result, path, length, color)
