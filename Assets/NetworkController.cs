using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using NativeWebSocket;
using Vector3 = UnityEngine.Vector3;

public class NetworkController : MonoBehaviour
{
    private WebSocket websocket;
    private Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();

    async void Start()
    {
        websocket = new WebSocket("ws://localhost:8080");

        websocket.OnOpen += () => { Debug.Log("Connection open!"); };

        websocket.OnError += (e) => { Debug.Log("Error: " + e); };

        websocket.OnClose += (e) => { Debug.Log("Connection closed!"); };

        websocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Received: " + message);

            var data = JsonUtility.FromJson<ReceivedData>(message);

            if (!objects.ContainsKey(data.id))
            {
                objects[data.id] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }

            objects[data.id].transform.position = new Vector3(data.x, data.y, 0);
        };
        SpawnOrUpdateCube(new ReceivedData { id = "example", x = 0, y = 0 });
        await websocket.Connect();
    }

    
    
    private void SpawnOrUpdateCube(ReceivedData data)
    {
        if (!objects.ContainsKey(data.id))
        {
            GameObject newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newCube.transform.position = new Vector3(data.x, data.y, 0);
            
            objects.Add(data.id, newCube);
        }
        else
        {
            objects[data.id].transform.position = new Vector3(data.x, data.y, 0);
        }
    }
    
    async void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    async void OnDestroy()
    {
        await websocket.Close();
    }

    [System.Serializable]
    public class ReceivedData
    {
        public string id;
        public float x;
        public float y;
    }
}