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

using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace SslServerDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 6000);
            listener.Start();

            Console.WriteLine("Waiting for a client to connect...");
            TcpClient client = listener.AcceptTcpClient();

            _sslStream = new SslStream(client.GetStream(), true);

            try
            {
                serverCertificate = new X509Certificate(Environment.CurrentDirectory + @"\" + "server.pfx", "1");
                _sslStream.AuthenticateAsServer(serverCertificate, false, SslProtocols.Tls, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
                return;
            }

            while (true)
            {
                string receivedMessage = ReadMessage(_sslStream);
                Console.WriteLine("received:" + receivedMessage);
                byte[] message = Encoding.UTF8.GetBytes("Success.<EOF>");
                _sslStream.Write(message);
                _sslStream.Flush();
            }
        }

        static X509Certificate serverCertificate = null;
        private static SslStream _sslStream;



        static string ReadMessage(SslStream sslStream)
        {
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do
            {
                bytes = sslStream.Read(buffer, 0, buffer.Length);
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
                if (messageData.ToString().IndexOf("<EOF>") != -1)
                {
                    break;
                }
            } while (bytes != 0);

            return messageData.ToString();
        }

        static void ProcessClient(TcpClient client)
        {

            SslStream sslStream = new SslStream(
                client.GetStream(), true);

            try
            {
                sslStream.AuthenticateAsServer(serverCertificate, true, SslProtocols.Tls, true);

                Console.WriteLine("Waiting for client message...");
                string messageData = ReadMessage(sslStream);
                Console.WriteLine("Received: {0}", messageData);

                byte[] message = Encoding.UTF8.GetBytes("已收到信息.<EOF>");
                sslStream.Write(message);
                sslStream.Flush();
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                }
                Console.WriteLine("Authentication failed - closing the connection.");
                sslStream.Close();
                client.Close();
                return;
            }
            finally
            {
                sslStream.Close();
                client.Close();
            }
        }
    }
}
