from typing import *
from dataclasses import dataclass
from enum import IntEnum

import numpy as np
import random as rand

@dataclass
class NeuralLayer:
    __layer_weights = np.ndarray((0,))
    __layer_bias = np.ndarray((0,))
    __layer_activation = np.ndarray((0,))
    
    layer_inputs = np.ndarray((0,))
    layer_outputs = np.ndarray((0,))

    layer_function = lambda x: 1 / (1 + np.exp(-x))

    def __init__(self, width: int, height: int, activation, input = None) -> None:
        self.layer_function = activation
        self.layer_inputs = input

        self.__layer_weights = np.random.uniform(-0.9999, 1, (height, width))
        self.__layer_bias = np.random.uniform(-4.9999, 5, (height,))
        
    def compute(self, input = None) -> np.ndarray:
        if input.all() != None:
            self.layer_inputs = input
        
        p = np.dot(self.__layer_weights, self.layer_inputs)
        b = p + self.__layer_bias
        self.__layer_activation = self.layer_function(b)
        self.layer_outputs = self.__layer_activation.copy()
        return self.layer_outputs

    def adjust(self, weight_factor: float, bias_factor: float) -> None:
        for i in self.__layer_weights:
            i += rand.uniform(-weight_factor, weight_factor)
        np.clip(self.__layer_weights, -1.999, 1.999)
        for i in self.__layer_bias:
            i += rand.uniform(-bias_factor, bias_factor)
        np.clip(self.__layer_bias, -9.999, 9.999)

    def activation_value(self) -> np.ndarray:
        return self.__layer_activation

    def weight_value(self) -> np.ndarray:
        return self.__layer_weights
    
    def bias_value(self) -> np.ndarray:
        return self.__layer_bias

class NeuralNetwork:
    __input_layer = None
    __hidden_layer = list()
    __output_layer = None

    network_input = None
    network_output = None
    
    def __init__(self, input_shape: Tuple[int, int], hidden_shape: List[Tuple[int, int]], output_shape: Tuple[int, int], activations: List) -> None:
        self.__input_layer = NeuralLayer(input_shape[0], input_shape[1], activations[0])
        i = 1
        for shape in hidden_shape:
            self.__hidden_layer.append(NeuralLayer(shape[0], shape[1], activations[i]))
            i += 1
        self.__output_layer = NeuralLayer(output_shape[0], output_shape[1], activations[-1])

    def compute(self, input: np.ndarray) -> np.ndarray:
        if input.all() != None:
            self.network_input = input

        temp_activation = self.__input_layer.compute(self.network_input)
        for layer in self.__hidden_layer:
            temp_activation = layer.compute(temp_activation)
        self.network_output = self.__output_layer.compute(temp_activation)
        return self.network_output

    def adjust(self, weight_factor: float, bias_factor: float) -> None:
        self.__input_layer.adjust(weight_factor, bias_factor)
        for l in self.__hidden_layer:
            l.adjust(weight_factor, bias_factor)
        self.__output_layer.adjust(weight_factor, bias_factor)

    def get_input_layer(self) -> NeuralLayer: return self.__input_layer
    def get_hidden_layers(self): return self.__hidden_layer
    def get_output_layer(self) -> NeuralLayer: return self.__output_layer
