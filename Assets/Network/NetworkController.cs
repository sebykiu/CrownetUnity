using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Network
{
    internal class NetworkController : MonoBehaviour
    {
        private const int Port = 12345;
        private static readonly IPAddress IPAddress = IPAddress.Any;
        private static readonly byte[] Buffer = new byte[1024];
        private static Socket _serverSocket;
        private static Socket _clientSocket;
        private Thread _receiveThread;

        private void Start()
        {
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var endPoint = new IPEndPoint(IPAddress, Port);
            _serverSocket.Bind(endPoint);
            _serverSocket.Listen(1);
            Debug.Log("Server is listening for connections!");

            AcceptClientAsync();
        }

        private void OnDestroy()
        {
           
            _receiveThread.Join(); // Wait for the receive thread to exit
            _clientSocket.Close(); // Close the client socket
            _serverSocket.Close(); // Close the server socket
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

                // Start receiving data from the client
                _receiveThread = new Thread(ReceiveData);
                _receiveThread.Start();
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception occurred while accepting client: " + ex.Message);
            }
        }

        private void ReceiveData()
        {
            Debug.Log("A new Thread was created!");

            {
                while (true)
                {

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

                    Message message = JsonConvert.DeserializeObject<Message>(response);
                    Debug.Log(message.Id);
                    
                    
                }
            }
        }
    }
}

public class Message
{
    public string Id { get; set; }
    public string Instruction { get; set; }
    public Coordinates Coordinates { get; set; }


}

public class Coordinates
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    
}