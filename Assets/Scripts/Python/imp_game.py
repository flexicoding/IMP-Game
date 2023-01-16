from NeuralNetwork.neural_network import *
from pytocs.interface import CSInterface

import matplotlib.pyplot as plt

import os
import time
import asyncio

@dataclass
class CSData:
    cs_data : list
    cs_factor : int
    nt_data : np.ndarray

def check_ray_cast(point_a: np.ndarray, direction_a: np.ndarray, point_b: np.ndarray) -> bool:
    delta_xy_vec = np.round((point_a[0] - direction_a[0]) / (point_a[1] - direction_a[1]), 2)
    delta_xz_vec = np.round((point_a[0] - direction_a[0]) / (point_a[2] - direction_a[2]), 2)

    delta_xy_tst = np.round((point_b[0] - point_a[0]) / (point_b[1] - point_a[1]), 2)
    delta_xz_tst = np.round((point_b[0] - point_a[0]) / (point_b[2] - point_a[2]), 2)

    if (delta_xy_vec == delta_xy_tst) and (delta_xz_vec == delta_xz_tst): return True, 0.0, 0.0
    return False, np.abs(delta_xy_tst - delta_xy_vec), np.abs(delta_xz_tst - delta_xz_vec)

def simulate(prev_network: NeuralNetwork, shape: np.ndarray, activations: list) -> np.ndarray:
    global factor_chart
    global output_chart

    #Generate the position and direction of the AP
    ap_pos = np.random.rand(3)
    ap_dir = (ap_pos + np.random.rand(3)) / 2

    #Generate the position of the AI
    ai_pos = np.random.rand(3)

    #The NN will now generate the direction of the AI
    #-> The inputs are: ap_pos, ap_dir, ai_pos
    #-> The outputs are: ai_dir

    nn = NeuralNetwork(shape[0], shape[1], shape[2], activations)
    rt_1 = nn.compute(np.concatenate([ap_pos, ap_dir, ai_pos]))
    rt_2 = prev_network.compute(np.concatenate([ap_pos, ap_dir, ai_pos]))

    #We want to be heading towards the APs position but never be inside the raycast of their direction
    #We also want the AP to be inside the raycast of our direction
    #We will choose the better network acording to their outputs

    _, p_delta_xy_1, p_delta_xz_1 = check_ray_cast(ai_pos, rt_1, ap_pos)
    _, a_delta_xy_1, a_delta_xz_1 = check_ray_cast(ap_pos, ap_dir, rt_1)

    _, p_delta_xy_2, p_delta_xz_2 = check_ray_cast(ai_pos, rt_2, ap_pos)
    _, a_delta_xy_2, a_delta_xz_2 = check_ray_cast(ap_pos, ap_dir, rt_2)

    adjust_factor_1 = np.clip(0.1 * ( (1.5 * (p_delta_xy_1 + p_delta_xz_1) + (a_delta_xy_1 + a_delta_xz_1)) / 2), 0, 50)
    adjust_factor_2 = np.clip(0.1 * ( (1.5 * (p_delta_xy_2 + p_delta_xz_2) + (a_delta_xy_2 + a_delta_xz_2)) / 2), 0, 50)

    if adjust_factor_1 < adjust_factor_2: 
        factor_chart.append(adjust_factor_1)
        output_chart.append(rt_1)
        print(f"Output: {rt_1}")
        return nn
    factor_chart.append(adjust_factor_2)
    output_chart.append(rt_2)
    print(f"Output: {rt_2}")
    return prev_network

def parse_input(inp: str) -> np.ndarray:
    a = inp.split()

    r = []
    for i in a:
        if a != '': r.append(float(i))
    
    return np.array(r)

def serialize_output(otp: np.ndarray) -> str:
    r = ""
    for i in otp:
        r += str(i) + "\n"
    return r

if __name__ == "__main__":
    #os.chdir("../../../") #Change to the project root

    sigmoid = lambda x: np.clip(1 / (1 + np.exp(-x)), 0, 1)
    identity = lambda x: x

    shape = (
        (9, 10),
        [(10, 15), (15, 10)],
        (10, 3)
    )
    activations = [identity, sigmoid, sigmoid, sigmoid]
    network = NeuralNetwork(shape[0], shape[1], shape[2], activations)
    cs_interface = CSInterface()
    factor_chart = []
    output_chart = []

    for i in range(0, 100):
        network = simulate(network, shape, activations)
        print(f"Generation {i}: {factor_chart[len(factor_chart) - 1]}")

    plt.plot(factor_chart)
    plt.show()

    plt.plot(output_chart)
    plt.show()

    cs_interface.null_request()
    while True:
        print("[PYTOCS] Sent request for __data (read)")
        input_val = parse_input(cs_interface.file_access_request("__data", "r"))
        print("[PYTOCS] Finished request for __data (read)")
        output_val = serialize_output(network.compute(input_val))
        print("[PYTOCS] Sent request for __interface (write)")
        cs_interface.file_access_request("__interface", "w", output_val)
        print("[PYTOCS] Finished request for __interface (wrote)")
