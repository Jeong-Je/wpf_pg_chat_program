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
using System.Windows.Shapes;

using Npgsql;

namespace WpfChattingProgram
{
    /// <summary>
    /// Interaction logic for RegisterForm.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        TcpClient client = null;
        NetworkStream stream = null;
        public RegisterWindow(TcpClient client)
        {
            this.client = client;
            this.stream = client.GetStream();
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            usernamePlaceholder.Visibility = Visibility.Collapsed;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
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

        private void password2Box_GotFocus(object sender, RoutedEventArgs e)
        {
            password2Placeholder.Visibility = Visibility.Collapsed;
        }

        private void password2Box_LostFocus(object sender, RoutedEventArgs e)
        {
            if (password2Box.Password.Length == 0)
            {
                password2Placeholder.Visibility = Visibility.Visible;
            }
        }


        private void registerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (usernameBox.Text.Length == 0 || passwordBox.Password.Length == 0 || password2Box.Password.Length == 0)
            {
                MessageBox.Show("입력칸을 모두 입력해주세요.", "가입 실패", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (passwordBox.Password != password2Box.Password)
            {
                MessageBox.Show("두 비밀번호가 일치하지 않습니다.", "가입 실패", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                string register = $"{{\"username\":\"{usernameBox.Text}\",\"password\":\"{passwordBox.Password}\",\"register\":\"true\"}}";
                byte[] data = Encoding.UTF8.GetBytes(register);

                stream.Write(data, 0, data.Length);

                byte[] serverRes = new byte[1024];
                int read = stream.Read(serverRes, 0, serverRes.Length);
                string serverMessage = Encoding.UTF8.GetString(serverRes, 0, read);

                if(serverMessage == "Success")
                {
                    MessageBox.Show("계정 등록이 완료되었습니다.", "가입 성공", MessageBoxButton.OK);
                    Close();
                }
                else
                {
                    MessageBox.Show("이미 등록된 계정입니다.","가입 실패",MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
