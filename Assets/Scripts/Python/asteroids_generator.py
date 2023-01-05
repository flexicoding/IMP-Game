import numpy as np

minValue = int(input("minimum range of positions: "))
maxValue = int(input("maximum range of positions: "))
ammount = int(input("ammount of asteroids: "))
defaultDistance = float(input("default distance: "))

defaultCoordinatesA = []
disturbCoordinates = []
coordinates = []

for k in range(ammount):
    for j in range(ammount):
        for i in range(ammount):
            defaultCoordinatesA.append([j * defaultDistance, i * defaultDistance, k * defaultDistance])
defaultCoordinates = np.array(defaultCoordinatesA)

for l in range(ammount):
    disturbCoordinates = np.round(np.random.uniform(minValue, maxValue, (ammount, 3)), 4)

coordinates = disturbCoordinates + defaultCoordinates
print(coordinates)

with open("positions.txt","a") as f:
    f.truncate(0)
    for i in coordinates:
        f.write("(")
        for j in i:
            f.write(f"{float(j)} ")
        f.write(")")
        f.write("\n")
