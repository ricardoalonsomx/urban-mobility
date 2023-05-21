using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class AgentData
{
    public List<Vector3> positions;
}

public class CarData : AgentData
{
    public CarData() {}
    public CarData(string param) {
        positions = new List<Vector3>();
        directions = new List<Vector3>();
    }
    public List<Vector3> directions;
}

public class TrafficLightData
{
    public List<int> status;
    
}

public class AgentController : MonoBehaviour
{
    string serverUrl = "http://localhost:8585";
    string sendConfigEndpoint = "/init";
    string updateEndpoint = "/update";
    string getAgentsEndpoint = "/getAgents";
    string getObstaclesEndpoint = "/getObstacles";
    string getTrafficLightEndpoint = "/getTrafficLights";
    string getDestinationEndpoint = "/getDestinations";

    AgentData destinationData;
    CarData carData;
    TrafficLightData trafficLightData;
    GameObject[] agents;
    GameObject[] trafficLights;

    List<Vector3> destinationPositions;
    List<Vector3> oldPositions;
    List<Vector3> newPositions;
    List<Vector3> directions;
    List<int> status;

    int nAgents;
    public GameObject[] carPrefabs;
    public float timeToUpdate = 5.0f, timer, dt;
    public float carRotationSpeed = 5.0f;
    bool hold = true;

    void Start()
    {
        nAgents = MainMenu.nAgents;
        destinationPositions = new List<Vector3>();
        oldPositions = new List<Vector3>();
        newPositions = new List<Vector3>();
        directions = new List<Vector3>();
        status = new List<int>();

        agents = new GameObject[nAgents];

        // Hold the simulation till the server starts
        timer = 0;

        // Start flask server
        // StartCoroutine(StartFlaskServer());

        // Instantiate all vehicles
        if (nAgents > 100) 
            nAgents = 100;

        if (nAgents <= 0)
            nAgents = 1;
            
        for(int i = 0; i < nAgents; i++)
            agents[i] = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Length-1)], Vector3.zero, Quaternion.identity);

        StartCoroutine(StartConfigurations());
    }

    // Start initial configurations for the connection with the server
    IEnumerator StartConfigurations()
    {
        // Wait till our connection to the server is succesful
        UnityWebRequest www;

        do
        {
            www = UnityWebRequest.Get(serverUrl);
            yield return www.SendWebRequest();
        } while (www.result == UnityWebRequest.Result.ConnectionError);
        
        // Start coroutines to configurate server and cameras
        StartCoroutine(SendConfiguration()); 
        StartCoroutine(GetCarCamerasRequest());
    }

    // Send our parameters to configure the server
    IEnumerator SendConfiguration()
    {
        WWWForm form = new WWWForm();

        form.AddField("numAgents", nAgents.ToString());

        UnityWebRequest www = UnityWebRequest.Post(serverUrl + sendConfigEndpoint, form);
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            UnityEngine.Debug.Log(www.error);
        }
        else
        {
            // If our init request is succesful we get our data and start the timer (Update loop)
            StartCoroutine(GetCarData());
            StartCoroutine(GetDestinationData());
            timer = timeToUpdate;
        }
    }

    IEnumerator GetCarCamerasRequest()
    {
        yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("CarFollowingCamera").Length >= agents.Length);
        this.gameObject.GetComponent<CameraManager>().GetCarCameras();
    }

    private void Update() 
    {
        float t = timer/timeToUpdate;
        UnityWebRequest www;
        // Smooth out the transition at start and end
        dt = t * t * ( 3f - 2f*t);

        if (timer >= timeToUpdate)
        {
            timer = 0;
            hold = true;
            StartCoroutine(UpdateSimulation());
        }

        if (!hold)
        {
            UnityWebRequest mywww;
            trafficLights = GameObject.FindGameObjectsWithTag("TrafficLight");

            for (int s = 0; s < agents.Length; s++)
            {
                if (destinationPositions.Contains(oldPositions[s]) && agents[s].active) {
                    agents[s].SetActive(false);
                } else {
                    Vector3 interpolated = Vector3.Lerp(oldPositions[s], newPositions[s], dt);

                    agents[s].transform.localPosition = interpolated;
                    
                    Vector3 dir = oldPositions[s] - newPositions[s];
                    agents[s].transform.rotation = Quaternion.Slerp(agents[s].transform.rotation, Quaternion.LookRotation(directions[s]), carRotationSpeed*Time.deltaTime);
                }
            }

            for (int s = 0; s < trafficLights.Length; s++) {
                for (int o = 0; o < 3; o++) {
                    if (o == status[s])
                        trafficLights[s].transform.GetChild(o).gameObject.SetActive(true);
                    else
                        trafficLights[s].transform.GetChild(o).gameObject.SetActive(false);
                }
            }
            // Move time from the last frame
            timer += Time.deltaTime;
        }
    }

    IEnumerator UpdateSimulation()
    {
        UnityWebRequest www = UnityWebRequest.Get(serverUrl + updateEndpoint);
             
        yield return www.SendWebRequest();
 
        if (www.result != UnityWebRequest.Result.Success)
            UnityEngine.Debug.Log(www.error);
        else 
        {
            StartCoroutine(GetCarData());
            StartCoroutine(GetTrafficLightData());
            StartCoroutine(GetTrafficLightData());
        }
    }

    IEnumerator GetCarData() 
    {
        UnityWebRequest www = UnityWebRequest.Get(serverUrl + getAgentsEndpoint);
        yield return www.SendWebRequest();
 
        if (www.result != UnityWebRequest.Result.Success)
            UnityEngine.Debug.Log(www.error);
        else 
        {
            carData = JsonUtility.FromJson<CarData>(www.downloadHandler.text);

            oldPositions = new List<Vector3>(newPositions);
            newPositions.Clear();
            directions.Clear();

            foreach(Vector3 v in carData.positions)
                newPositions.Add(v);
            
            foreach(Vector3 v in carData.directions)
                directions.Add(v);

            hold = false;
        }
    }

    IEnumerator GetTrafficLightData() 
    {
        UnityWebRequest www = UnityWebRequest.Get(serverUrl + getTrafficLightEndpoint);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            UnityEngine.Debug.Log(www.error);

        else 
        {
            trafficLightData = JsonUtility.FromJson<TrafficLightData>(www.downloadHandler.text);

            status.Clear();
            
            foreach(int s in trafficLightData.status)
                status.Add(s);
        }
    }

    IEnumerator GetDestinationData() 
    {
        UnityWebRequest www = UnityWebRequest.Get(serverUrl + getDestinationEndpoint);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            UnityEngine.Debug.Log(www.error);

        else 
        {
            destinationData = JsonUtility.FromJson<AgentData>(www.downloadHandler.text);
            
            destinationPositions.Clear();

            foreach (Vector3 pos in destinationData.positions) {
                destinationPositions.Add(pos);
            }
        }
    }

    IEnumerator  StartFlaskServer()
    {
        UnityWebRequest www = UnityWebRequest.Get(serverUrl);
        yield return www.SendWebRequest();

        // If we don't have connection to the server (it's not initialized yet), we create a new instance of it
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            string pythonScriptPath = Path.Combine(Application.streamingAssetsPath, "Server/");

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "python3";
            startInfo.Arguments = Path.Combine(pythonScriptPath, "tests.py"); // "flask_server.py");
            startInfo.WorkingDirectory = pythonScriptPath;

            // startInfo.RedirectStandardOutput = true;
            // startInfo.UseShellExecute = false;
            // startInfo.CreateNoWindow = true;

            Process process = new Process();
            process.StartInfo = startInfo;
            process.EnableRaisingEvents = true;

            process.Start();
            process.BeginOutputReadLine();
            // process.WaitForExit();
        }
    }
}
    