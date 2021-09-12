from PIL import Image
from random import randint
import numpy as np
from numpy import asarray
from copy import deepcopy

def clamp(n, smallest, largest): 
    return max(smallest, min(n, largest))
def grayscale(px) -> float:
    return px[0] * 0.299 + px[1] * 0.587 + px[2] * 0.114

def convertToNormalTexture(tex : np.array):

    normalTex = deepcopy(tex)

    def sumPixels(p0, p1):
        return [p0[0] + p1[0], p0[1] + p1[1], p0[2] + p1[2], p0[3] + p1[3]]

    def getNeighborPixels(x, y):
        if (x < 2 or x > len(normalTex) - 2 or \
            y < 2 or y > len(normalTex) -2):
            x = 5
            y = 5
        left = tex[x - 1][y]  
        right = tex[x + 1][y]
        up = tex[x][y - 1]
        down = tex[x][y + 1]
        return [left, right, up, down]
    
    def avgNeighborPixels(neighbors, channel):
        sumVal = (sum(sum(neighbors[0], neighbors[1]), sum(neighbors[2], neighbors[3]))) / 4.0
        retVal = [_channel / 4 for _channel in sumVal]
        return retVal[channel]
    
    def calculateColor(neighbors):
        [left, right, up, down] = neighbors
        xDelta = (grayscale(left) - grayscale(right) + 255) * 0.5
        yDelta = (grayscale(up) - grayscale(down) + 255) * 0.5

        if (left[2] != 0 or right[2] != 0 or up[2] != 0 or down[2] != 0):
            xval = int(clamp(xDelta + randint(2, 10), 0, 255))
            yval = int(clamp(yDelta + randint(2, 10), 0, 255))
            
            return [xval, yval, 255, 255]
        else:
            return [int(xDelta), int(yDelta), 255, 255]

    for y in range(0, len(tex)):
        for x in range(0, len(tex[0])):
            neighbors = getNeighborPixels(x, y)
            normalTex[x][y] = calculateColor(neighbors)
    
    return normalTex

def combineTexturesHorizontal(tex1, tex2): # assumes textures are same size
    w = len(tex1)
    h = len(tex1[0])
    combinedTexture = [[[0, 255, 0, 255] for i in range(w * 2)] for i in range(h)]

    for y in range(w):
        for x in range(h):
            combinedTexture[x][y] = tex1[x][y]

    for y in range(w, w * 2):
        for x in range(h):
            combinedTexture[x][y] = tex2[x][y - w]

    combinedTexture = np.array(combinedTexture)
    print (len(combinedTexture[0]))
    return combinedTexture

def createNumberTexture(firstDigit, secondDigit):
    
    def createDigitTexture(digit):
        tex = asarray(Image.open('normal_' + str(digit) + '_tex.png'))
        normalTex = convertToNormalTexture(tex)
        return normalTex

    def saveTextureAsImage(tex, number):
        img = Image.fromarray(tex)
        img.save('Generated Normal Textures/normal_' + str(number) + '_tex.png')
    
    if (firstDigit == 0):
        normalTex = createDigitTexture(secondDigit)
        saveTextureAsImage(normalTex, secondDigit)
        print ('created -> ' + str(secondDigit))
        return

    normalTex1 = createDigitTexture(firstDigit)
    normalTex2 = createDigitTexture(secondDigit)

    combinedTexture = combineTexturesHorizontal(normalTex1, normalTex2)
    
    number = 10 * firstDigit + secondDigit
    saveTextureAsImage(combinedTexture, number)
    print ('created -> ' + str(number))


createNumberTexture(0, 1)
# for i in range(1):
#     for j in range(10):
#         createNumberTexture(i, j)
