using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class PythonInterface
{
    public static string InterfaceFilePath;
    public static string FeedbackFilePath;
    public static string InputFilePath;

    public static PYInterface PyInterface = new PYInterface();

    public static List<Vector3> read_output()
    {
        var temp = new List<Vector3>();
        var contents = PyInterface.FileAccessRequest(InterfaceFilePath, FileMode.Open).Split('\n');

        for(int i = 0; i < contents.Length - 1; i += 3)
        {   
            var pos = new Vector3();
            pos.x = (float)decimal.Parse(contents[i], System.Globalization.NumberStyles.Float);
            pos.y = (float)decimal.Parse(contents[i + 1], System.Globalization.NumberStyles.Float);
            pos.z = (float)decimal.Parse(contents[i + 2], System.Globalization.NumberStyles.Float);
            temp.Add(pos);
        }

        return temp;
    }
    public static void write_input(List<Vector3> input)
    {
        var write_contents = "";
        foreach(var p in input)
        {
            write_contents += p.x.ToString() + "\n";
            write_contents += p.y.ToString() + "\n";
            write_contents += p.z.ToString() + "\n";
        }

        PyInterface.FileAccessRequest(InputFilePath, FileMode.Open, write_contents);
    }
    public static void write_feedback(int feedback)
    {
        PyInterface.FileAccessRequest(FeedbackFilePath, FileMode.Open, feedback.ToString());
    }
}