#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
Created on Wen Oct 26 05:22:00 2022

The class of kalmen filter based on OpenCV

@author: Xinyang Chen
"""

import cv2
import numpy as np

class KalmanFilter():
    def __init__(self, is3D=True, processNoise=0.003, measurementNoise=0.5):
        self.is3D = is3D# 3D filter or 2D filter
        
        # Initialize the filter
        if is3D:
            self.kalman = cv2.KalmanFilter(6, 3, cv2.CV_32F)
            self.kalman.measurementMatrix = np.array([[1,0,0,0,0,0],[0,1,0,0,0,0],[0,0,1,0,0,0]], np.float32)
            self.kalman.transitionMatrix = np.array([[1,0,0,1,0,0],[0,1,0,0,1,0],[0,0,1,0,0,1],[0,0,0,1,0,0],[0,0,0,0,1,0],[0,0,0,0,0,1]], np.float32)
            self.kalman.processNoiseCov = np.array([[1,0,0,0,0,0],[0,1,0,0,0,0],[0,0,1,0,0,0],[0,0,0,1,0,0],[0,0,0,0,1,0],[0,0,0,0,0,1]], np.float32) * processNoise
            self.kalman.measurementNoiseCov = np.array([[1,0,0],[0,1,0],[0,0,1]], np.float32) * measurementNoise
        else:
            self.kalman = cv2.KalmanFilter(4, 2, cv2.CV_32F)
            self.kalman.measurementMatrix = np.array([[1,0,0,0],[0,1,0,0]], np.float32)
            self.kalman.transitionMatrix = np.array([[1,0,1,0],[0,1,0,1],[0,0,1,0],[0,0,0,1]], np.float32)
            self.kalman.processNoiseCov = np.array([[1,0,0,0],[0,1,0,0],[0,0,1,0],[0,0,0,1]], np.float32) * processNoise
            self.kalman.measurementNoiseCov = np.array([[1,0],[0,1]], np.float32) * measurementNoise

    def process(self, measurement):
        """
        @brief: Process the mearsured data with Kalman filter
        param measurement: mearsured data
        """
        self.kalman.correct(measurement)
        predPos = self.kalman.predict()

        if self.is3D:
            return predPos[:3, 0]
        else:
            return predPos[:2, 0]