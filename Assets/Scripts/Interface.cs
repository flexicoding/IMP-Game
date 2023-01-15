using System.IO;
using System.Threading;
using System.Collections.Generic;

public class PYInterface
{
    public bool IsOpen = false;

    private int update_delay = 10;
    private bool ping_pong_flag = true;

    public PYInterface(int update_delay = 10)
    {
        this.update_delay = update_delay;
        IsOpen = true;
        ping_pong_flag = true;
    }

    private static string reverse(string str)
    {
        string temp = "";
        for(int i = str.Length - 1; i > -1; i--) temp += str[i];
        return temp;
    }

    public void ListenInterface()
    {
        if(!IsOpen) throw new System.Exception("Cannot listen on a closed interface.");

        //Delay operations for a certain ammount of time
        Thread.Sleep(update_delay);

        //Find all .pytocs files in the interface dir. Repeat if it isn't our turn
        var active_requests = new List<string>();

        while(true)
        {
            foreach(var f in Directory.GetFiles("./"))
            {
                if(f.EndsWith(".pytocs")) active_requests.Add(f);
            }
            if(!ping_pong_flag) break;
            if(active_requests.Count != 0) break;
        }
        //The pytocs convention only allows at max two files to ever exist in the interface dir
        if(active_requests.Count > 2) throw new System.Exception($"Pytocs only ever allows two .pytocs files to exist in the interface directory but {active_requests.Count} files were found.");

        //If it is our turn, we don't want any requests to be present
        if(active_requests.Count > 0 && !ping_pong_flag) throw new System.Exception("Ping-Pong violation");

        //If there are no requests, it is our first turn
        if(active_requests.Count == 0) return;

        //Remove the .pytocs extension
        var unextended_file = active_requests[0].Substring(0, active_requests[0].Length - 7);

        //Remove leading slashes
        var temp_file = "";
        foreach(var c in reverse(unextended_file))
        {
            if(c == '/') break;
            temp_file += c;
        }
        unextended_file = reverse(temp_file);
        if(unextended_file == null) throw new System.Exception("A critical error occured!");

        //Get the file which the request is operating on and the request type
        var operand_file = "";
        var request_type = "";

        var found_dot = false;
        var found_uscore = false;
        foreach(var c in unextended_file)
        {
            if(c == '.' && !found_dot)
            {
                found_dot = true;
                continue;
            }
            if(c == '_' && !found_uscore)
            {
                found_uscore = true;
                continue;
            }
            if(found_dot) operand_file += c;
            if(!found_uscore) request_type += c;
        }

        //If the request was a close request, we must validate the request and close this interface
        if(request_type == "close")
        {
            var close_validate_path = "./close_val.pytocs";

            File.Create(close_validate_path).Dispose();

            //We must not delete interface dir. The request origin must do so
            //This interface is now closed
            IsOpen = false;

            return;
        }

        //Validate the request and lock this thread
        var validate_path = "./" + request_type + "_val." + operand_file + ".pytocs";

        if(request_type == "null") validate_path = "./null_val.pytocs";

        File.Create(validate_path).Dispose();
        while(File.Exists(validate_path))
        {
            Thread.Sleep(update_delay);
        }

        //After the request has been concluded, the thread is unlocked

        ping_pong_flag = false;
    }

    public string FileAccessRequest(string target_file, FileMode file_mode, string write_contents = "")
    {
        if(!IsOpen) throw new System.Exception("Cannot send requests on a closed interface.");

        //First, we need to listen for other requests
        ListenInterface();

        //It is possible that the interface has been closed by the other end of the interface
        //Also, we must respect the ping-pong principle
        if(!IsOpen || ping_pong_flag) return "";

        //We need to create the request file and wait for the request to be validated
        var request_path = "./fileacc_req." + target_file + ".pytocs";
        var validate_path = "./fileacc_val." + target_file + ".pytocs";
        File.Create(request_path).Dispose();
        while(!File.Exists(validate_path))
        {
            Thread.Sleep(update_delay);
        }
        ping_pong_flag = true;
        
        //We have received the validation so we must delete the request file now
        File.Delete(request_path);

        //The request has been validated so we can access the file now
        var contents = "";
        var write_operation = write_contents.Length != 0;
        var file_stream = new FileStream(target_file, file_mode);
        
        if(write_operation)
        {
            using(var f = new StreamWriter(file_stream))
            {
                f.Write(write_contents);
            }
        }
        else
        {
            using(var f = new StreamReader(file_stream))
            {
                contents = f.ReadToEnd();
            }
        }

        Thread.Sleep(50);
        
        //We have finished writing so we must delete the validation file for the other process to continue
        File.Delete(validate_path);

        //Now return the read contents
        return contents;
    }

    public void CloseRequest()
    {
        if(!IsOpen) return;

        //First, we need to listen for other requests
        ListenInterface();

        //It is possible that the interface has been closed by the other end of the interface
        if(!IsOpen) return;

        //We need to create the request file and wait for the request to be validated
        var request_path = "./close_req.pytocs";
        var validate_path = "./close_val.pytocs";
        File.Create(request_path).Dispose();
        while(File.Exists(validate_path))
        {
            Thread.Sleep(update_delay);
        }

        Thread.Sleep(50);

        //The request has been validated and since we are the request origin, we must close off the interface completely
        File.Delete(request_path);
        File.Delete(validate_path);

        IsOpen = false;
    }

    public void NullRequest()
    {
        if(!IsOpen) return;

        //First, we need to listen for other requests
        ListenInterface();

        //It is possible that the interface has been closed by the other end of the interface
        if(!IsOpen) return;

        //We need to create the request file and wait for the request to be validated
        var request_path = "./null_req.pytocs";
        var validate_path = "./null_val.pytocs";
        File.Create(request_path).Dispose();
        while(File.Exists(validate_path))
        {
            Thread.Sleep(update_delay);
        }
        ping_pong_flag = true;

        Thread.Sleep(50);
        
        //Now delete the filese
        File.Delete(request_path);
        File.Delete(validate_path);

        //Now the ping-pong principle holds true
    }
}