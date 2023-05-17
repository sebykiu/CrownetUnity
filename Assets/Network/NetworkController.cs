using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Network.Json;
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
            _serverSocket.Dispose();
            _receiveThread.Interrupt();
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

        private static void ReceiveData()
        {
            Debug.Log("A new Thread was created!");

            {
                while (true)
                {
                    int bytesRead = _clientSocket.Receive(Buffer);
                    Debug.Log("Message Size received: " + bytesRead);


                    var message = Encoding.ASCII.GetString(Buffer, 0, bytesRead);

                    Debug.Log(message);
                    if (message.Contains("<|EOM|>"))
                    {
                        // Extract the portion of the message before the delimiter
                        int endIndex = message.IndexOf("<|EOM|>", StringComparison.Ordinal);
                        string finalMessage = message.Substring(0, endIndex);
                        var deserialize = JsonDeserializer.Deserialize(finalMessage);
                        Debug.Log("[Deserialize]" + deserialize.Coordinates.x);
                        // Clear the buffer and reset the memory stream
                        Array.Clear(Buffer, 0, Buffer.Length);
                    }
                }
            }
        }
    }
}