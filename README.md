# MediapipeUnityHumanTracking
Windows mediapipe+unity human tracking.

Currently only support hand.

Run app.py and then run the scene in unity. (I set the timeout for socket to 10 seconds.)

see reference https://github.com/TesseraktZero/UnityHandTrackingWithMediapipe.git

Major modification:

1. Change from android+PC to PC. Using ZeroMq to send data from python program to Unity.

2. Remove the Kalman filter in Unity. Implement a new Kalman filter with OpenCV in python.

3. Correct the landmark positions in Unity to let them fit the figure model.

More update in the future.
