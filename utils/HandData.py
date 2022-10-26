#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
Created on Wen Oct 26 05:22:00 2022

The class of hand landmark data structure

@author: Xinyang Chen
"""

import numpy as np
from utils.KalmanFilter import KalmanFilter
import copy

class HandData():
    def __init__(self):
        self.prevFramePos = np.array([], np.float32)# Landmarks position on previous frame(leave for latter improvement)
        self.currFramePos = np.array([], np.float32)# Landmarks position on current frame
        self.filters = []# Kalman filters

        # Initialize Kalman filters
        self.filters.append(KalmanFilter(False))
        for i in range(20):
            self.filters.append(KalmanFilter())
 
    def process(self, landmarkList):
        """
        @brief: Process the raw landmark data from mediapipe with kalman filter
        param landmarkList: raw landmark data from mediapipe
        """
        if landmarkList.size != 0:
            if self.currFramePos.size == 0:
                self.currFramePos = np.zeros([21, 3], np.float32)
            self.currFramePos[...] = landmarkList[...]
        
        if self.currFramePos.size != 0:
            self.currFramePos[0, :2] = self.filters[0].process(self.currFramePos[0, :2])
            for i in range(20):
                self.currFramePos[i+1, :] = self.filters[i+1].process(self.currFramePos[i+1, :] - self.currFramePos[0, :]) + self.currFramePos[0, :]
                
            if self.prevFramePos.size == 0:
                self.prevFramePos = np.zeros([21, 3], np.float32)
            self.prevFramePos = copy.deepcopy(self.currFramePos)