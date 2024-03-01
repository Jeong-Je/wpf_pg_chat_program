using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfChattingProgram
{
    /// <summary>
    /// Interaction logic for ChattingRoomWindow.xaml
    /// </summary>
    public partial class ChattingRoomWindow : Window
    {
        TcpClient client = null;
        NetworkStream stream = null;
        public ChattingRoomWindow(string username)
        {
            InitializeComponent();
            usernameTextBox.Text = username;
            userIPTextBox.Text = GetLocalIP();

            try
            {
                client = new TcpClient();
                client.Connect("127.0.0.1", 9999);

                stream = client.GetStream();
                SendUserNameToServer(username);

                receiveDataFromServer();
            }
            catch
            {
                MessageBox.Show("채팅 서버 연결에 실패하였습니다.", "채팅 서버 접속 실패", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private string GetLocalIP()
        {
            string IP = string.Empty;
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            IP = host.AddressList[1].ToString();
            return IP;
        }
        private void SendUserNameToServer(string username)
        {
            try
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine(username);
                writer.Flush();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"클라이언트 username 전송 실패: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async Task receiveDataFromServer()
        {
            try
            {
                StreamReader streamReader = new StreamReader(stream);

                while (true)
                {
                    string newData = await streamReader.ReadLineAsync();
                    if (newData.Substring(0, 3) == "%0%")
                    {
                        UpdateUserList(newData.Substring(3));
                    }
                    else
                    {
                        UpdateNewChat(newData);
                    }

                    // Introduce a small delay to avoid busy-waiting
                    await Task.Delay(10);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to receive chat from the server: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void UpdateNewChat(string newChat)
        {
            chatLogListBox.Items.Add(newChat);
        }

        private void UpdateUserList(string userListString)
        {
            List<string> userList = new List<string>();
            userList = userListString.Split(',').ToList();

            userListListBox.Items.Clear();

            foreach (string username in userList)
            {
                userListListBox.Items.Add(username);
            }

        }

        private void message_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                sendMessage();
            }
        }

        private async void sendMessage()
        {
            byte[] byteData = Encoding.Default.GetBytes(message.Text);

            await client.GetStream().WriteAsync(byteData, 0, byteData.Length);

            message.Text = "";
        }    
    }
}
