using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class RegisterForm : Window
    {
        NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=postgres;User Id=postgres;Password=postgres;");
        public RegisterForm()
        {
            InitializeComponent();
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DB 연결 실패", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
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
            if(passwordBox.Password.Length == 0)
            {
                passwordPlaceholder.Visibility= Visibility.Visible;
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
                password2Placeholder.Visibility= Visibility.Visible;
            }
        }


        private void registerBtn_Click(object sender, RoutedEventArgs e)
        {
            if(usernameBox.Text.Length == 0 || passwordBox.Password.Length == 0 || password2Box.Password.Length == 0) {
                MessageBox.Show("입력칸을 모두 입력해주세요.", "가입 실패", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (passwordBox.Password != password2Box.Password)
            {
                MessageBox.Show("두 비밀번호가 일치하지 않습니다.", "가입 실패", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                using var cmd = new NpgsqlCommand();
                cmd.Connection = conn;

                cmd.CommandText = ($"INSERT INTO users_table(username, password) VALUES('{usernameBox.Text}', '{passwordBox.Password}')");

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("가입 성공", "가입 완료", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                } catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "가입 실패", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
