﻿using System;
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
                TcpClient client = listener.AcceptTcpClient();

                // ClientData의 객체를 생성해주고 연결된 클라이언트를 ClientData의 멤버로 설정해준다
                ClientData clientData = new ClientData(client, userList);

                // 현재 연결된 클라이언트 리스트에 추가
                connectedClients.Add(client);

                // 클라이언트들에게 새로 추가된 클라이언트 리스트 전송
                BroadcastUserList();

                // 클라이언트발 메세지들 관리
                _ = HandleClient(clientData);
            }
        }
        finally
        {
            listener.Stop();  // Ensure the listener is stopped when exiting the loop
        }
    }

    private void BroadcastUserList()
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
                client.GetStream().Write(userListBytes, 0, userListBytes.Length);
            }
            catch
            {

            }

        }
    }

    private async Task HandleClient(ClientData clientData)
    {
        NetworkStream stream = clientData.tcpClient.GetStream();
        byte[] buffer = new byte[1024];
        int read;

        try
        {
            while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, read);

                // 서버 콘솔 로그에 기록
                Console.WriteLine($"[대화] {DateTime.Now.ToString("yyyy년 MM월 dd일 hh:mm:ss")} {clientData.clientName} ({clientData.clientIP}): " + message);

                string usernameANDmessage = $"{clientData.clientName} > {message}";
                BroadcastMessage(usernameANDmessage);
                stream.Flush();
            }
        }
        catch (IOException)
        {
            // 클라리언트 접속이 끊긴 경우
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"[종료] {DateTime.Now.ToString("yyyy년 MM월 dd일 hh:mm:ss")} 클라이언트 {clientData.clientName}이(가) 연결을 끊었습니다. ({clientData.clientIP})");
            Console.ResetColor();

            userList.Remove(clientData.clientName);
            connectedClients.Remove(clientData.tcpClient);
            clientData.tcpClient.Close();

            BroadcastUserList();
        }
    }


    private void BroadcastMessage(string message)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        foreach (var client in connectedClients)
        {
            client.GetStream().Write(buffer, 0, buffer.Length);
        }
    }
}

class ClientData
{
    public TcpClient tcpClient;
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
            Console.WriteLine($"클라이언트에서 username 수신 실패: {ex.Message}");
        }
        this.tcpClient = tcpClient;
        readByteData = new byte[1024];
    }
}