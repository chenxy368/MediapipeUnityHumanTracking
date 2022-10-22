import cv2
import mediapipe as mp
import zmq
import signal
import sys

class handDetector():
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
        imgRGB = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
        self.results = self.hands.process(imgRGB)
        
        if self.results.multi_hand_landmarks:
            for handLms in self.results.multi_hand_landmarks:
                if draw:
                    self.mpDraw.draw_landmarks(img, handLms,self.mpHands.HAND_CONNECTIONS)
                   
        return img
    
    def findPosition(self):
        leftlmList = []
        rightlmList = []
        if self.results.multi_hand_landmarks:
            for idx, handlms in enumerate(self.results.multi_hand_landmarks):
                for idy, lm in enumerate(handlms.landmark):
                    if (self.results.multi_handedness[idx].classification[0].label == "Left"):
                        rightlmList.extend([1 - lm.y, lm.x, lm.z])
                    else:
                        leftlmList.extend([1 - lm.y, lm.x, lm.z])
                    
        return leftlmList, rightlmList




    
def main():
    width, height = 1280, 720

    cap = cv2.VideoCapture(0)
    cap.set(3, width)
    cap.set(4, height)

    detector = handDetector()

    #Communication (messageQueue local communication)
    context = zmq.Context()
    socket = context.socket(zmq.PUSH)  # <- the PUSH socket
    socket.setsockopt(zmq.SNDTIMEO, 5000)
    socket.bind('tcp://*:5555')

    while True:
        success, img = cap.read()
        
        img = detector.findHands(img)
    
        # Landmark values - (x, y, z) * 21
        leftData, rightData = detector.findPosition()
        print(str.encode(str(leftData) + "+" + str(rightData)))
        data = {
            'str': str(leftData) + "+" + str(rightData)
        }
        socket.send_json(data)
        
        img = cv2.resize(img, (0, 0), None, 0.5, 0.5)
        cv2.imshow("Image", img)

        if cv2.waitKey(1) & 0xFF == 27:
            break

    cap.release()
    cv2.destroyAllWindows()

if __name__ == '__main__':
    main()