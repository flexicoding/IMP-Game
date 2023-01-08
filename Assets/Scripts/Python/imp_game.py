from NeuralNetwork.neural_network import *
from pytocs.interface import CSInterface

import os
import time
import asyncio

@dataclass
class CSData:
    cs_data : list
    cs_factor : int
    nt_data : np.ndarray

prev_dt_contents = []
prev_fact_contents = int
def interface() -> CSData:
    global prev_dt_contents
    global network
    global cs_interface

    rt = CSData([], 0, np.array([]))

    temp = []
    contents = cs_interface.file_access_request("__data", "r").split("\n")
    for i in range(0, len(contents) - 1): 
        if contents[i] == '': continue
        temp.append(float(contents[i]))

    rt.cs_data = temp
    rt.nt_data = network.compute(np.array(rt.cs_data))
    print(rt.nt_data)

    write_contents = ""
    for i in rt.nt_data: write_contents += str(i[0]) + "\n"
    cs_interface.file_access_request("__interface_in", "w", write_contents)

    rt.cs_factor = int(cs_interface.file_access_request("__interface_out", "r"))
    
    return rt

if __name__ == "__main__":
    os.chdir("../../../") #Change to the project root

    sigmoid = lambda x: 1 / (1 + np.exp(-x))
    relu = lambda x: x * (x > 0)
    tanh = lambda x: np.tanh(x)

    shape = (
        (6, 10),
        [(10, 20), (20, 15), (15, 10)],
        (10, 3)
    )
    activations = [sigmoid, relu, relu, relu, sigmoid]
    network = NeuralNetwork(shape[0], shape[1], shape[2], activations)
    cs_interface = CSInterface()

    progress_function_weights = lambda x: 0.1 / (0.029 * x ** 2 + 1)
    progress_function_bias = lambda x: 0.3 / (0.029 * x ** 2 + 1)

    cs_interface.null_request()

    while(True):
        dt = interface()

        prev_dt_contents = dt.cs_data
        prev_fact_contents = dt.cs_factor

        network.adjust(progress_function_weights(dt.cs_factor), progress_function_bias(dt.cs_factor))
    