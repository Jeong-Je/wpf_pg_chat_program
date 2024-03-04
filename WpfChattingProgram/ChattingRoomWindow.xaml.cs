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

        public ChattingRoomWindow(string username, TcpClient client, NetworkStream stream)
        {
            this.client = client;
            this.stream = stream;

            InitializeComponent();
            usernameTextBox.Text = username;
            userIPTextBox.Text = GetPublicIP();

            try
            {               
                _ = receiveDataFromServer(client);
            }
            catch
            {
                MessageBox.Show("채팅 서버 연결에 실패하였습니다.", "채팅 서버 접속 실패", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private string GetPublicIP()
        {
            string PublicIP;
            string apiUrl = "https://api64.ipify.org?format=json";

            try
            {
                string jsonResponse;
                using (WebClient client = new WebClient())
                {
                    jsonResponse = client.DownloadString(apiUrl);
                }

                PublicIP = jsonResponse.Split('"')[3];
            }
            catch
            {
                PublicIP = "null";
            }

            return PublicIP;


        }

        private async Task receiveDataFromServer(TcpClient client)
        {
            byte[] buffer = new byte[1024];
            int read;

            while((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                string message = Encoding.UTF8.GetString(buffer);
               
                if(message.Substring(0,3) == "%0%")
                {
                    int EndUsernameIdx = message.IndexOf("\n");
                    UpdateUserList(message.Substring(3, EndUsernameIdx));
                }
                else
                {
                    UpdateNewChat(message);
                }
                Array.Clear(buffer, 0x0, buffer.Length);
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
