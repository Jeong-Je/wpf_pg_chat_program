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
    public MyServer()
    {
        // 서버시작
        AsyncServerStart();
    }

    public void AsyncServerStart()
    {
        // 서버 포트설정 및 시작
        TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Any, 9999));
        listener.Start();
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("서버를 시작합니다.");
        Console.ResetColor();

        try
        {
            // 데이터를 읽든 못읽든 일단 바로 해당로직이 실행된다(비동기서버)
            while (true)
            {
                // 연결을 요청한 client의 객체를 acceptClient에 저장
                TcpClient acceptClient = listener.AcceptTcpClient();

                // ClientData의 객체를 생성해주고 연결된 클라이언트를 ClientData의 멤버로 설정해준다
                ClientData clientData = new ClientData(acceptClient);

                // BeginRead를 통해 비동기로 읽는다.(읽을데이터가 올때까지 대기하지않고 바로 아랫줄의 while문으로 이동한다)
                clientData.client.GetStream().BeginRead(clientData.readByteData, 0, clientData.readByteData.Length, new AsyncCallback(DataReceived), clientData);
            }
        }
        finally
        {
            listener.Stop();  // Ensure the listener is stopped when exiting the loop
        }
    }

    private void DataReceived(IAsyncResult ar)
    {
        // 콜백메서드입니다.(피호출자가 호출자의 해당 메서드를 실행시켜줍니다)
        // 즉 데이터를 읽었을때 실행됩니다.

        // 콜백으로 받아온 Data를 ClientData로 형변환 해줍니다.
        ClientData callbackClient = ar.AsyncState as ClientData;

        //실제로 넘어온 크기를 받아옵니다
        int bytesRead = callbackClient.client.GetStream().EndRead(ar);

        // 문자열로 넘어온 데이터를 파싱해서 출력해줍니다.
        string readString = Encoding.Default.GetString(callbackClient.readByteData, 0, bytesRead);

        Console.WriteLine($"Received data from {callbackClient.clientName}: " + readString);

        callbackClient.client.GetStream().BeginRead(callbackClient.readByteData, 0, callbackClient.readByteData.Length, new AsyncCallback(DataReceived), callbackClient);

    }
}

class ClientData
{
    public TcpClient client;
    public byte[] readByteData;
    public string clientName;

    public ClientData(TcpClient tcpClient)
    {
        NetworkStream stream = tcpClient.GetStream();

        try
        {
            StreamReader reader = new StreamReader(stream);
            string username = reader.ReadLine();
            this.clientName = username;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"클라이언트 {username}이(가) 연결되었습니다.");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"클라이언트에서 username 수신 실패: {ex.Message}");
        }
        client = tcpClient;
        readByteData = new byte[1024];
    }
}
