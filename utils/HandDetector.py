#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
Created on Wen Oct 26 05:22:00 2022

The class of hand detector based on mediapipe

@author: Xinyang Chen
"""

import cv2
import mediapipe as mp
import numpy as np

class HandDetector():
    def __init__(self, mode=True, maxHands=2,  model_complexity=1, detectionCon=0.5, trackCon=0.5):
        self.mode = mode# hand tracking mode
        self.maxHands = maxHands# number of hands
        self.model_complexity = model_complexity
        self.detectionCon = detectionCon# detection confidence(palm detector)
        self.trackCon = trackCon# tracking confidence(landmark model)

        self.mpHands = mp.solutions.hands# hand tracking solution
        self.hands = self.mpHands.Hands(self.mode, self.maxHands, self.model_complexity, self.detectionCon, self.trackCon)
        self.mpDraw = mp.solutions.drawing_utils# draw

    def findHands(self, img, draw=True):
        """
        @brief: Get the hand tracing result with mediapipe
        param img: the input image
        param draw: enable drawing, if true draw the landmarks on img
        
        return img: image after drawing
        """
        imgRGB = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
        self.results = self.hands.process(imgRGB)
        
        if self.results.multi_hand_landmarks:
            for handLms in self.results.multi_hand_landmarks:
                if draw:
                    self.mpDraw.draw_landmarks(img, handLms, self.mpHands.HAND_CONNECTIONS)
                   
        return img
    
    def findPosition(self):
        """
        @brief: Get the landmark position array. Notice that to fit unity coordinate
        system switch the x and y coordinate, and flip the x coordinate

        return leftlmList: the array of left hand landmarks
        return rightlmList: the array of right hand landmarks
        """
        leftlmList = np.array([], np.float32)
        rightlmList = np.array([], np.float32)
        if self.results.multi_hand_landmarks:
            for idx, handlms in enumerate(self.results.multi_hand_landmarks):
                if (self.results.multi_handedness[idx].classification[0].label == "Left"):
                    rightlmList = np.zeros([21, 3], np.float32)
                    for idy, lm in enumerate(handlms.landmark):   
                        rightlmList[idy, :] = np.array([1.0 - lm.y, lm.x, lm.z], np.float32)
                else:
                    leftlmList = np.zeros([21, 3], np.float32)
                    for idy, lm in enumerate(handlms.landmark):
                        leftlmList[idy, :] = np.array([1.0 - lm.y, lm.x, lm.z], np.float32)
                    
        return leftlmList, rightlmList
