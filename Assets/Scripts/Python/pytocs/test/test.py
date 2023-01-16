from interface import CSInterface

import os

if __name__ == "__main__":
    interface = CSInterface()
    interface.file_access_request("test.txt", "a", "15")

    print(interface.file_access_request("test.txt", "r")[-2:])  #ISSUE!
    
    interface.close_request()