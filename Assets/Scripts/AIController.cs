using System.Collections.Generic;
using System.IO;
using UnityEngine;

class AIController : MonoBehaviour
{
    public string AIDataFilePath;
    public string AIInterfaceFilePath;
    public string AIFeedbackFilePath;

    public float EnvironmentSize = 100;
    public int ThreadCount = 8;

    public bool AITrain = false;
    public Camera PlayerCamera;

    private class Environment
    {
        public List<Vector3> asteroid_positions;
        public Vector3 ai_spawn_position;
        public Vector3 player_spawn_position;
    }
    private List<Environment> environments = new List<Environment>();

    private void generate_environments()
    {
        environments.Add(new Environment());
    }

    private static int feedback(List<Vector3> output)
    {
        return 1;
    }

    private static void run_simulation(Environment env)
    {
        var rand = new System.Random();
        List<Vector3> dt = new List<Vector3>{new Vector3((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble()), new Vector3((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble())};

        PythonInterface.write_input(dt);
        List<Vector3> ot = PythonInterface.read_output();
        PythonInterface.write_feedback(feedback(ot));

        PythonInterface.PyInterface.ListenInterface();
    }

    private void Start()
    {
        if(!AITrain) return;
        PlayerCamera.gameObject.SetActive(false);
        generate_environments();

        PythonInterface.InterfaceFilePath = AIInterfaceFilePath;
        PythonInterface.FeedbackFilePath = AIFeedbackFilePath;
        PythonInterface.InputFilePath = AIDataFilePath;
    }
    private void Update()
    {
        if(!AITrain) return;
        run_simulation(environments[0]);
    }
}