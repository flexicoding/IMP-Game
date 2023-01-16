using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
class AIController : MonoBehaviour
{
    public string AIDataFilePath;
    public string AIInterfaceFilePath;
    public Transform PlayerTransform;
    public float WorldXMax = 800;
    public float WorldYMax = 800;
    public float WorldZMax = 800;
    public float Speed = 2;

    private Vector3 world_normalize(Vector3 vec)
    {
        var ot = new Vector3();
        ot.x = vec.x / WorldXMax;
        ot.y = vec.y / WorldYMax;
        ot.z = vec.z / WorldZMax;

        return ot;
    }
    private Vector3 world_denormalize(Vector3 vec)
    {
        var ot = new Vector3();
        ot.x = vec.x * WorldXMax;
        ot.y = vec.y * WorldYMax;
        ot.z = vec.z * WorldZMax;

        return ot;
    }

    private void Start()
    {
        PythonInterface.InterfaceFilePath = AIInterfaceFilePath;
        PythonInterface.InputFilePath = AIDataFilePath;
    }

    //private bool f = false;
    private void FixedUpdate()
    {
        //if(f) return;
        var input_data = new List<Vector3>();
        input_data.Add(world_normalize(PlayerTransform.position));
        input_data.Add(world_normalize(PlayerTransform.GetComponent<Rigidbody>().velocity + PlayerTransform.position));
        input_data.Add(world_normalize(transform.position));

        print("[PYTOCS] Sent request for __data (write)");
        PythonInterface.write_input(input_data);
        print("[PYTOCS] Finished request for __data (write)");
        print("[PYTOCS] Sent request for __interface (read)");
        var output_data = PythonInterface.read_output();
        print("[PYTOCS] Finished request for __interface (read)");

        //GetComponent<Rigidbody>().velocity = world_denormalize(output_data[0]) - transform.position;
        GetComponent<Rigidbody>().velocity = (output_data[0] - transform.position) * Speed;
        //f = true;
    }
}