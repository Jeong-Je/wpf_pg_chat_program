using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;

namespace WpfChattingProgram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        TcpClient client = new TcpClient();
        NetworkStream stream = null;
        public LoginWindow()
        {
            client.Connect("127.0.0.1", 9999);
            InitializeComponent();           
        }

        private void usernameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            usernamePlaceholder.Visibility = Visibility.Collapsed;
        }

        private void usernameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (usernameBox.Text.Length == 0)
            {
                usernamePlaceholder.Visibility = Visibility.Visible;
            }

        }

        private void passwordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            passwordPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void passwordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Password.Length == 0)
            {
                passwordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            stream = client.GetStream();

            if (usernameBox.Text.Length == 0 || passwordBox.Password.Length == 0)
            {
                MessageBox.Show("입력칸을 모두 입력해주세요.", "로그인 실패", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                string auth = $"{{\"username\":\"{usernameBox.Text}\",\"password\":\"{passwordBox.Password}\"}}";
                byte[] authData = Encoding.UTF8.GetBytes(auth);
                stream.Write(authData, 0, authData.Length);

                byte[] serverAuth = new byte[1024];
                int read = stream.Read(serverAuth, 0, serverAuth.Length);
                string serverMessage = Encoding.UTF8.GetString(serverAuth, 0, read);
                
                if(serverMessage == "Success")
                {
                    ChattingRoomWindow chattingRomm = new ChattingRoomWindow(usernameBox.Text, client, stream);
                    Close();
                    chattingRomm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("회원정보가 일치하지 않습니다.", "로그인 실패", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }            
        }

        private void registerBtn_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerForm = new RegisterWindow(client);
            registerForm.Owner = this;
            try
            {
                registerForm.ShowDialog();
            }
            catch (Exception ex)
            {

            }

        }
    }
}

