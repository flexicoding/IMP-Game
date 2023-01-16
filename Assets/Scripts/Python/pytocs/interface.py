import os
import time

class CSInterface:
    def __init__(self, update_delay: int = 10) -> None:
        self.update_delay = update_delay

        self.is_open = True
        self.ping_pong_flag = False

    def listen_interface(self) -> None:
        if not self.is_open: raise Exception("Cannot listen on a closed interface.")

        #Delay operations for a certain ammount of time
        time.sleep(self.update_delay / 100)

        #Find all .pytocs files in the interface dir. Repeat if it isn't our turn
        active_requests = []
        while True:
            for file in os.listdir("./"):
                if file.endswith(".pytocs"):
                    active_requests.append(file)
            if not self.ping_pong_flag: break
            if len(active_requests) != 0: break

        #The pytocs convention only allows at max two files to ever exist in the interface dir
        if len(active_requests) > 2:
            raise Exception(f"Pytocs only ever allows two .pytocs files to exist in the interface directory but {len(active_requests)} files were found.")

        #If it is our turn, we don't want any requests to be present
        if len(active_requests) > 0 and not self.ping_pong_flag:
            raise Exception("Ping-Pong violation")
        
        #If there are no requests, it is our first turn
        if len(active_requests) == 0: return

        #Remove the .pytocs extension
        unextended_file = active_requests[0][:-7]

        #Remove leading slashes
        temp_file = ""
        for c in unextended_file[::-1]:
            if c == '/': break
            temp_file += c
        unextended_file = temp_file[::-1]

        #Get the file which the request is operating on and the request type
        operand_file = ""
        request_type = ""

        found_dot = False
        found_uscore = False
        for c in unextended_file:
            if c == '.' and not found_dot:
                found_dot = True
                continue
            if c == '_' and not found_uscore:
                found_uscore = True
                continue
            if found_dot: operand_file += c
            if not found_uscore: request_type += c

        #If the request was a close request, we must validate the request and close this interface
        if request_type == "close":
            validate_path = "./close_val.pytocs"
            
            open(validate_path, "a").close()

            #We must not delete interface dir. The request origin must do so
            #This interface is now closed
            self.is_open = False

            return
        
        #Validate the request and lock this thread
        validate_path = "./" + request_type + "_val." + operand_file + ".pytocs"

        if request_type == "null": validate_path = "./null_val.pytocs"

        open(validate_path, "a").close()
        while os.path.exists(validate_path):
            time.sleep(self.update_delay / 100)
        
        self.ping_pong_flag = False
        
        #After the request has been concluded, the thread is unlocked
    
    def file_access_request(self, target_file: str, file_mode: str, write_contents: str = "") -> str | None:
        if not self.is_open: raise Exception("Cannot send requests on a closed interface.")

        #First, we need to listen for other requests
        self.listen_interface()

        #It is possible that the interface has been closed by the other end of the interface
        #Also, we must respect the ping-pong principle
        if not self.is_open or self.ping_pong_flag: return

        #We need to create the request file and wait for the request to be validated
        request_path = "./fileacc_req." + target_file + ".pytocs"
        validate_path = "./fileacc_val." + target_file + ".pytocs"
        open(request_path, "a").close()
        while not os.path.exists(validate_path):
            time.sleep(self.update_delay / 100)
        self.ping_pong_flag = True
        
        #We have received the validation so we must delete the request file now
        os.remove(request_path)

        #The request has been validated so we can access the file now
        contents = ""
        write_operation = len(write_contents) != 0

        if write_operation:
            with open(target_file, file_mode) as f:
                f.write(write_contents)
        else:
            with open(target_file, file_mode) as f:
                contents = f.read()

        time.sleep(0.05)

        #We have finished writing so we must delete the validation file for the other process to continue
        os.remove(validate_path)

        #Now return the read contents if applicable
        if not write_operation: return contents

    def close_request(self) -> None:
        if not self.is_open: raise Exception("Cannot send requests on a closed interface.")

        #First, we need to listen for other requests
        self.listen_interface()

        #It is possible that the interface has been closed by the other end of the interface
        if not self.is_open: return

        #We need to create the request file and wait for the request to be validated
        request_path = "./close_req.pytocs"
        validate_path = "./close_val.pytocs"
        open(request_path, "a").close()
        while not os.path.exists(validate_path):
            time.sleep(self.update_delay / 100)

        time.sleep(0.05)

        #The request has been validated and since we are the request origin, we must close off the interface completely
        os.remove(request_path)
        os.remove(validate_path)

        self.is_open = False

    def null_request(self) -> None:
        if not self.is_open: raise Exception("Cannot send requests on a closed interface.")

        #First, we need to listen for other requests
        self.listen_interface()

        #It is possible that the interface has been closed by the other end of the interface
        #Also, we must respect the ping-pong principle
        if not self.is_open or self.ping_pong_flag: return

        #We need to create the request file and wait for the request to be validated
        request_path = "./null_req.pytocs"
        validate_path = "./null_val.pytocs"
        open(request_path, "a").close()
        while not os.path.exists(validate_path):
            time.sleep(self.update_delay / 100)
        self.ping_pong_flag = True

        time.sleep(0.05)

        #Then delete the files
        os.remove(request_path)
        os.remove(validate_path)

        #Now the ping-pong principle still holds true