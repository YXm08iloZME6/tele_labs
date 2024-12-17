using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

[Serializable]
public class UPDMessage{
    public bool IsCheck;
    public int Length;
    public byte[]? Message;
}

class Program{

    public static void Main(string[] args) {
        string? localIP;
        int localPort = 11000;

        string? serverIP;
        int serverPort = 11001;

        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0)) {
            socket.Connect("8.8.8.8", 65530);
            IPEndPoint? endPoint1 = socket.LocalEndPoint as IPEndPoint;
            localIP = endPoint1?.Address.ToString();
            serverIP = endPoint1?.Address.ToString();
        }
        
        // Client
        UdpClient client = new UdpClient();
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

        string text_message = "Тестируем!";
        UPDMessage message = new UPDMessage() {IsCheck = true, Length = text_message.Length, Message = Encoding.ASCII.GetBytes(text_message)};
        string json = JsonSerializer.Serialize(message);
        byte[] data = Encoding.UTF8.GetBytes(json);
        client.Send(data, endPoint);

        // Server
        UdpClient server = new UdpClient(serverPort);

        while (true) {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] response = server.Receive(ref endPoint);
            string ser = Encoding.UTF8.GetString(response);

            UPDMessage msg = JsonSerializer.Deserialize<UPDMessage>(ser);
            Console.WriteLine($"Received: IsCheck = {msg.IsCheck}, Message = {msg.Message}");

            server.Send(new byte[1] {1}, 1, remoteEP);
        }


    }
}