using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;


class Program
{ 
    static void Main(string[] args)
    {
        MyServer a = new MyServer();
    }
}

class MyServer
{
    List<string> userList = new List<string>();
    List<TcpClient> connectedClients = new List<TcpClient>();
 
    public MyServer()
    {
        // 서버시작
        AsyncServerStart();
    }

    public async void AsyncServerStart()
    {
        // 서버 포트설정 및 시작
        TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Any, 9999));
        listener.Start();
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine($"{DateTime.Now.ToString("yyyy년 MM월 dd일 hh:mm:ss")} 서버를 시작합니다.");
        Console.ResetColor();


        try
        {
            // 데이터를 읽든 못읽든 일단 바로 해당로직이 실행된다(비동기서버)
            while (true)
            {
                // 연결을 요청한 client의 객체를 acceptClient에 저장
                TcpClient acceptClient = listener.AcceptTcpClient();

                // ClientData의 객체를 생성해주고 연결된 클라이언트를 ClientData의 멤버로 설정해준다
                ClientData clientData = new ClientData(acceptClient, userList);

                connectedClients.Add(acceptClient);
                BroadcastUserList();

                // BeginRead를 통해 비동기로 읽는다.(읽을데이터가 올때까지 대기하지않고 바로 아랫줄의 while문으로 이동한다)
                clientData.client.GetStream().BeginRead(clientData.readByteData, 0, clientData.readByteData.Length,  new AsyncCallback(ar => DataReceived(ar, acceptClient)), clientData);
            }
        }
        finally
        {
            listener.Stop();
        }
    }

    private async void BroadcastUserList()
    {
        string userListString = "%0%" + string.Join(",", userList) + '\n';

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"[상황] {DateTime.Now.ToString("yyyy년 MM월 dd일 hh:mm:ss")} 현재 접속자 수 : {connectedClients.Count}명");
        Console.ResetColor();

        foreach (var client in connectedClients)
        {
            try
            {
                byte[] userListBytes = Encoding.Default.GetBytes(userListString);
                await client.GetStream().WriteAsync(userListBytes, 0, userListBytes.Length);
            }catch
            {

            }

        }
    }

    private void DataReceived(IAsyncResult ar, TcpClient acceptClient)
    {
        // 콜백메서드입니다.(피호출자가 호출자의 해당 메서드를 실행시켜줍니다)
        // 즉 데이터를 읽었을때 실행됩니다.

        // 콜백으로 받아온 Data를 ClientData로 형변환 해줍니다.
        ClientData callbackClient = ar.AsyncState as ClientData;


        try
        {
            //실제로 넘어온 크기를 받아옵니다
            int bytesRead = callbackClient.client.GetStream().EndRead(ar);

            // 문자열로 넘어온 데이터를 파싱해서 출력해줍니다.
            string readString = Encoding.Default.GetString(callbackClient.readByteData, 0, bytesRead);

            sendMessageToClient(callbackClient.clientName, readString);

            Console.WriteLine($"[대화] {DateTime.Now.ToString("yyyy년 MM월 dd일 hh:mm:ss")} {callbackClient.clientName}: " + readString);


            callbackClient.client.GetStream().BeginRead(callbackClient.readByteData, 0, callbackClient.readByteData.Length, new AsyncCallback(ar => DataReceived(ar, acceptClient)), callbackClient);
        } catch 
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"[종료] {DateTime.Now.ToString("yyyy년 MM월 dd일 hh:mm:ss")} 클라이언트 {callbackClient.clientName}이(가) 연결을 끊었습니다. ({callbackClient.clientIP})");
            Console.ResetColor();

            userList.Remove(callbackClient.clientName);
            connectedClients.Remove(acceptClient);
            acceptClient.Close();
            BroadcastUserList();
        }
    }

    private async void sendMessageToClient(string username, string message)
    {
        string usernameAndMessage = username + "> " + message;
        Console.WriteLine(usernameAndMessage);

        foreach (var client in connectedClients)
        {
            try
            {
                byte[] messages = Encoding.Default.GetBytes(usernameAndMessage);
                await client.GetStream().WriteAsync(messages, 0, messages.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message to client: {ex.Message}");
                // Handle exceptions if needed
            }
        }
    }


}

class ClientData
{
    public TcpClient client;
    public byte[] readByteData;
    public string clientName;

    public IPAddress clientIP;

    public ClientData(TcpClient tcpClient, List<string> userList)
    {
        NetworkStream stream = tcpClient.GetStream();
        try
        {
            clientIP = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;

            StreamReader reader = new StreamReader(stream);
            string username = reader.ReadLine();
            this.clientName = username;

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"[접속] {DateTime.Now.ToString("yyyy년 MM월 dd일 hh:mm:ss")} 클라이언트 {username}이(가) 연결되었습니다. ({clientIP})");
            Console.ResetColor();

            userList.Add(username);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[실패] {DateTime.Now.ToString("yyyy년 MM월 dd일 hh:mm:ss")} 클라이언트 연결 실패: {ex.Message}");
        }
        client = tcpClient;
        readByteData = new byte[1024];
    }
}
