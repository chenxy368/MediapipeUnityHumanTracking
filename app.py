#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
Created on Wen Oct 26 05:22:00 2022

Run this file to start hand tracking and send data to unity

@author: Xinyang Chen
"""

import cv2
import zmq
from utils import HandDetector
from utils import HandData

def main():
    # Camera Settings
    width, height = 1280, 720
    cap = cv2.VideoCapture(0)
    cap.set(3, width)
    cap.set(4, height)

    detector = HandDetector()

    # Communication (message queue local communication)
    context = zmq.Context()
    socket = context.socket(zmq.PUSH)  # <- the PUSH socket
    socket.setsockopt(zmq.SNDTIMEO, 10000)
    socket.bind('tcp://*:5557')
    
    # Landmarks datastructure
    leftHandData = HandData()
    rightHandData = HandData()
    
    while True:
        # Get camera input
        success, img = cap.read()
        img = detector.findHands(img)
    
        # Process and get landmarks
        tmpLeftData, tmpRightData = detector.findPosition()
        leftHandData.process(tmpLeftData)
        rightHandData.process(tmpRightData)
    
        # Send to unity
        leftData = leftHandData.currFramePos.flatten().tolist()
        rightData = rightHandData.currFramePos.flatten().tolist()
        #print(str.encode(str(leftData) + "+" + str(rightData)))
        data = {
            'str': str(leftData) + "+" + str(rightData)
        }
        socket.send_json(data)
        
        # Show raw tracking result
        #img = cv2.resize(img, (0, 0), None, 0.5, 0.5)
        cv2.imshow("Image", img)

        if cv2.waitKey(1) & 0xFF == 27:
            break

    cap.release()
    cv2.destroyAllWindows()

if __name__ == '__main__':
    main()