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
    /// Interaction logic for UserListWindow.xaml
    /// </summary>

    public partial class UserListWindow : Window
    {
        TcpClient client = null;
        NetworkStream stream = null;

        public UserListWindow(string username)
        {
            InitializeComponent();
            usernameAndIP.Content = $"{username}님 반갑습니다. ({GetLocalIP()})";

            try
            {
                client = new TcpClient();
                client.Connect("127.0.0.1", 9999);

                stream = client.GetStream();
                SendUserNameToServer(username);
            }
            catch
            {
                MessageBox.Show("채팅 서버 연결에 실패하였습니다.", "채팅 서버 접속 실패", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }

            
        }

        public string GetLocalIP()
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
            catch(Exception ex)
            {
                MessageBox.Show($"클라이언트 username 전송 실패: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ChattingRoomWindow chattingRoomWindow = new ChattingRoomWindow(client);
            chattingRoomWindow.ShowDialog();
        }


    }

}
