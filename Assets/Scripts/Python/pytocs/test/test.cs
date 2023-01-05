/*class Program
{
    static void Main(string[] args)
    {
        var py_interface = new PYInterface();
        var contents = py_interface.FileAccessRequest("test.txt", FileMode.Open);
        contents = contents.Substring(contents.Length - 2);
        Console.WriteLine((int.Parse(contents) + 10).ToString());

        py_interface.FileAccessRequest("test.txt", FileMode.Append, (int.Parse(contents) + 10).ToString()); //ISSUE!

        py_interface.CloseRequest();
    }
}*/