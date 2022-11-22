from NeuralNetwork.neural_network import *

import os
import time

def cs_interface(dt: np.ndarray) -> int:
    with open("__interface_in", "w+") as f:
        for i in dt:
            f.write(str(i[0]))
            f.write("\n")
    return 0

def cs_data() -> np.ndarray:
    temp = []
    with open("__data", "rb+") as f:
        lines = f.readlines()
        for i in range(0, len(lines)):
            ln = lines[i].split()
            temp.append([])
            for j in range(0, len(ln)):
                temp[i].append(float(ln[j]))
    return np.array(temp)

def read_success() -> int:
    with open("__interface_out", "rb") as f:
        return int.from_bytes(f.readline(), 'little', signed=False)
    return 1

if __name__ == "__main__":
    sigmoid = lambda x: 1 / (1 + np.exp(-x))
    relu = lambda x: x * (x > 0)
    tanh = lambda x: np.tanh(x)

    shape = (
        (5, 10),
        [(10, 20), (20, 15), (15, 10)],
        (10, 5)
    )
    activations = [sigmoid, relu, relu, relu, sigmoid]
    network = NeuralNetwork(shape[0], shape[1], shape[2], activations)

    progress_function_weights = lambda x, y: 0.1 / (0.029 * x ** 2 + 1) - (0.01 / (0.01 * y ** 2 + 0.1) + 0.1)
    progress_function_bias = lambda x, y: 0.3 / (0.029 * x ** 2 + 1) - (0.1 / (0.01 * y ** 2 + 0.2) + 0.2)

    generations = 100
    for gen in range(0, generations):
        input_dt = cs_data()
        cs_interface(network.compute(input_dt))
        factor = read_success()

        network.adjust(progress_function_weights(factor, gen), progress_function_bias(factor, gen))
    print(network.network_output)