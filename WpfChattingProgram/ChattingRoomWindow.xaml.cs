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

namespace WpfChattingProgram
{
    /// <summary>
    /// Interaction logic for ChattingRoomWindow.xaml
    /// </summary>
    public partial class ChattingRoomWindow : Window
    {
        TcpClient client = null;
        public ChattingRoomWindow(TcpClient client)
        {
            this.client = client;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            sendMessage();
        }

        private void message_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                sendMessage();
            }
        }

        private void sendMessage()
        {
            byte[] byteData = Encoding.Default.GetBytes(message.Text);

            client.GetStream().Write(byteData, 0, byteData.Length);

            message.Text = "";
        }
    }
}
