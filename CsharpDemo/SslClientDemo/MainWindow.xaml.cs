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

using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace SslClientDemo
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
            try
            {
                TcpClient client = new TcpClient("127.0.0.1", 6000);
                Console.WriteLine("Client connected.");
                _sslStream = new SslStream(
                   client.GetStream(),
                   false,
                   new RemoteCertificateValidationCallback(ValidateServerCertificate),
                   null
                   );


                X509CertificateCollection certs = new X509CertificateCollection();
                X509Certificate cert = X509Certificate.CreateFromCertFile(System.Environment.CurrentDirectory + @"\" + "client.cer");
                certs.Add(cert);
                //验证证书
                try
                {
/*
注意事项

  1.服务端验证方法AuthenticateAsServer的参数clientCertificateRequired如果为true,那在客户端也要安装server.pfx.

  2.客户端验证方法AuthenticateAsClient的参数targetHost对应证书中Common Name,也就是受颁发者.
*/
                    _sslStream.AuthenticateAsClient("test", certs, SslProtocols.Tls, false);
                }
                catch (AuthenticationException )
                {
                    client.Close();

                    //Console.WriteLine("Exception: {0}", e.Message);
                    //if (e.InnerException != null)
                    //{
                    //    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                    //}
                    //Console.WriteLine("Authentication failed - closing the connection.");
                    //client.Close();
                    //Console.ReadLine();
                    return;
                }

                //开始读取消息
                Task.Factory.StartNew(() =>
                {
                    ReadMessage(_sslStream);
                });

                Console.WriteLine("按Q退出程序");
                string message = "";
                message = Console.ReadLine() + "<EOF>";
                while (message != "Q")
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(message);
                    _sslStream.Write(bytes);
                    _sslStream.Flush();
                    Console.WriteLine("send:" + message);
                    message = Console.ReadLine() + "<EOF>";
                }

                client.Close();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        private static SslStream _sslStream;



        public static void ReadMessage(SslStream sslStream)
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
                if (messageData.ToString().IndexOf("<EOF>", StringComparison.Ordinal) != -1)
                {
                    break;
                }
            } while (bytes != 0);

            string message = messageData.ToString().Replace("<EOF>", "");
            Console.WriteLine("recevied:" + message);
            ReadMessage(sslStream);
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            if (sslpolicyerrors == SslPolicyErrors.None)
                return true;
            Console.WriteLine("Certificate error: {0}", sslpolicyerrors);
            return false;
        }
    }
}
