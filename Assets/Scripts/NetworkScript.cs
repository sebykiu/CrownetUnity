using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

namespace Network
{
    internal class NetworkScript : MonoBehaviour
    {
        // Instantiate in Unity
        //public  MobilityManager mobilityManager;

        private ConcurrentQueue<Action> _mainThreadWorkQueue = new ConcurrentQueue<Action>();
        private const int Port = 54321;
        private static readonly IPAddress IPAddress = IPAddress.Any;
        private static readonly byte[] Buffer = new byte[1024];
        private static Socket _serverSocket;
        private static Socket _clientSocket;
        private Thread _receiveThread;
        private bool _isRunning = true;
        public event Action<Message> OnMessageReceived;
        private float mainThreadTime;


        private void Start()
        {
            try
            {
                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                var endPoint = new IPEndPoint(IPAddress, Port);
                _serverSocket.Bind(endPoint);
                _serverSocket.Listen(1);
                Debug.Log("Server is listening for connections!");

                AcceptClientAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception occurred while starting the server: " + ex.Message);
            }
        }

        private void FixedUpdate()
        {
            mainThreadTime = Time.time;
            while (!_mainThreadWorkQueue.IsEmpty && _isRunning && _clientSocket != null && _clientSocket.Connected)
            {
                if (_mainThreadWorkQueue.TryDequeue(out Action action))
                {
                    action.Invoke();
                }
            }
        }

        private void OnDestroy()
        {
            try
            {
                _isRunning = false;
                if (_clientSocket != null)
                {
                    _receiveThread.Join();
                    _clientSocket.Close();
                }

                _serverSocket?.Close();
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception occurred while destroying the server: " + ex.Message);
            }
        }

        private async void AcceptClientAsync()
        {
            try
            {
                _clientSocket = await Task<Socket>.Factory.FromAsync(
                    _serverSocket.BeginAccept,
                    _serverSocket.EndAccept,
                    null
                );

                Debug.Log("Client connected!");

                _receiveThread = new Thread(ReceiveData);
                _receiveThread.Start();
            }
            catch (SocketException ex)
            {
                Debug.LogError("Socket exception occurred while accepting client: " + ex.Message);
            }
            catch (Exception ex)
            {
                Debug.LogError("Unknown exception occurred while accepting client: " + ex.Message);
            }
        }

        private void ReceiveData()
        {
            try
            {
                Debug.Log("A new Thread was created!");

                while (_isRunning && _clientSocket != null && _clientSocket.Connected)
                {
                    Debug.Log("[NetworkController - Receive Data in Loop]");

                    var lengthBuffer = new byte[4];
                    _clientSocket.Receive(lengthBuffer, SocketFlags.None);

                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(lengthBuffer);
                    }

                    int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

                    if (messageLength == 0)
                    {
                        Debug.Log("Received empty message. Client is disconnected!");
                        break;
                    }

                    var messageBuffer = new byte[messageLength];
                    var received = _clientSocket.Receive(messageBuffer, SocketFlags.None);

                    var response = Encoding.UTF8.GetString(messageBuffer, 0, received);

                    var message = JsonConvert.DeserializeObject<Message>(response);

                    _mainThreadWorkQueue.Enqueue(() => { OnMessageReceived?.Invoke(message); });
                }
            }
            catch (SocketException ex)
            {
                Debug.LogError("Socket exception occurred while receiving data: " + ex.Message);
            }
            catch (Exception ex)
            {
                Debug.LogError("Unknown exception occurred while receiving data: " + ex.Message);
            }
            finally
            {
                Debug.Log("Thread closed");
            }
        }
    }
}


public class Message
{
    public string SourceId { get; set; }
    public string TargetId { get; set; }

    public Coordinates Coordinates { get; set; }
    public string ObjectType { get; set; }
    
    public float SimTime { get; set; }
}

public class Coordinates
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
}