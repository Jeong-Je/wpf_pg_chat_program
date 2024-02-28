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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;

namespace WpfChattingProgram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=postgres;User Id=postgres;Password=postgres;");
        public MainWindow()
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
            string sql = $"SELECT password FROM users_table WHERE username='{usernameBox.Text}'";
            using var cmd = new NpgsqlCommand(sql, conn);

            using NpgsqlDataReader reader = cmd.ExecuteReader();
            if (usernameBox.Text.Length == 0 || passwordBox.Password.Length == 0)
            {
                MessageBox.Show("입력칸을 모두 입력해주세요.", "로그인 실패", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (reader.Read())
            {
                string passwordFromDatabase = reader.GetString(0);
                if (passwordFromDatabase == passwordBox.Password)
                {
                    MessageBox.Show("로그인 성공!", "로그인 성공", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show("회원정보가 일치하지 않습니다.", "로그인 실패", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("회원정보가 일치하지 않습니다.", "로그인 실패", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void registerBtn_Click(object sender, RoutedEventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
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

