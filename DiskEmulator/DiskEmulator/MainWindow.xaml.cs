using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiskEmulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int Radius = 15;
        private const byte HandshakeValue = 23;
        private const float MaxYAngle = 20.0f;
        private const float MaxXAngle = 20.0f;

        private Point Center => new((double)Target.GetValue(Canvas.LeftProperty) + Radius, (double)Target.GetValue(Canvas.TopProperty) + Radius);
        private readonly Socket Socket;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += (_, _) =>
            {
                Target.SetValue(Canvas.LeftProperty, (DrawArea.ActualWidth / 2) - Radius);
                Target.SetValue(Canvas.TopProperty, (DrawArea.ActualHeight / 2) - Radius);
            };

            MouseMove += (_, e) =>
            {
                var pos = e.GetPosition(DrawArea);

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Target.SetValue(Canvas.LeftProperty, pos.X - Radius);
                    Target.SetValue(Canvas.TopProperty, pos.Y - Radius);
                }
            };

            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var networkThread = new Thread(EnableConnection);
            networkThread.Start();

            Closing += (_, _) =>
            {
                Socket.Close();
                networkThread.Join();
            };
        }

        private void EnableConnection()
        {
            try
            {
                Socket.Bind(new IPEndPoint(IPAddress.Any, 9998));
                Socket.Listen(10);

                while (true)
                {
                    var handler = Socket.Accept();
                    new Thread(() =>
                    {
                        byte[] handshakeData = [HandshakeValue];
                        _ = handler.Send(handshakeData);
                        int bytesRead = handler.Receive(handshakeData);

                        if (handshakeData[0] != HandshakeValue)
                        {
                            handler.Close();
                            throw new SocketException();
                        }

                        while (handler.Connected)
                        {
                            Point pos;
                            Size size;
                            _ = Dispatcher.Invoke(() => pos = Center);
                            _ = Dispatcher.Invoke(() => size = new(DrawArea.RenderSize.Width / 2, DrawArea.RenderSize.Height / 2));

                            var x = (float)((size.Width - (float)pos.X) / size.Width * MaxXAngle * Math.PI / 180.0f);
                            var xBytes = BitConverter.GetBytes(x);
                            if (handler.Connected)
                            {
                                _ = handler.Send(xBytes);
                            }

                            var y = (float)((size.Height - (float)pos.Y) / size.Height * MaxYAngle * Math.PI / 180.0f);
                            var yBytes = BitConverter.GetBytes(y);
                            if (handler.Connected)
                            {
                                _ = handler.Send(yBytes);
                            }

                            var z = 0.0f;
                            var zBytes = BitConverter.GetBytes(z);
                            if (handler.Connected)
                            {
                                _ = handler.Send(zBytes);
                            }
                        }
                    }).Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private bool TargetContains(Point p)
            => Math.Pow(p.X - Center.X, 2) + Math.Pow(p.Y - Center.Y, 2) <= Radius * Radius;
    }
}