using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiskEmulator;

public partial class MainWindow : Window
{
    private const int Radius = 15;
    private const byte HandshakeValue = 23;
    private const float MaxYAngle = 20.0f;
    private const float MaxXAngle = 20.0f;

    private static int _packetNum = 0;

    private bool _finish = false;

    private Point Center => new(
        (double)Target.GetValue(Canvas.LeftProperty) + Radius,
        (double)Target.GetValue(Canvas.TopProperty) + Radius);

    private Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

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

        var networkThread = new Thread(EnableConnection);
        networkThread.Start();

        Closing += (_, _) =>
        {
            _socket.Close();
            _finish = true;
            networkThread.Join();
        };
    }

    private void EnableConnection()
    {
        while (!_finish)
        {
            try
            {
                _socket.Bind(new IPEndPoint(IPAddress.Any, 9998));
                _socket.Listen(10);

                while (!_finish)
                {
                    var handler = _socket.Accept();
                    new Thread(() =>
                    {
                        try
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
                                Point pos = default;
                                Size size = default;
                                Dispatcher.Invoke(() =>
                                {
                                    pos = Center;
                                    size = new Size(DrawArea.RenderSize.Width / 2, DrawArea.RenderSize.Height / 2);
                                });

                                var y = (float)((size.Height - (float)pos.Y) / size.Height * MaxYAngle * Math.PI / 180.0f);
                                var x = (float)((size.Width - (float)pos.X) / size.Width * MaxXAngle * Math.PI / 180.0f);
                                var z = 0.0f;

                                var numBytes = BitConverter.GetBytes(_packetNum);
                                var yBytes = BitConverter.GetBytes(y);
                                var xBytes = BitConverter.GetBytes(x);
                                var zBytes = BitConverter.GetBytes(z);

                                if (handler.Connected)
                                {
                                    _ = handler.Send(numBytes);
                                    _ = handler.Send(yBytes);
                                    _ = handler.Send(xBytes);
                                    _ = handler.Send(zBytes);
                                    _packetNum++;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка в потоке: {ex.Message}");
                            handler.Close();
                        }
                    }).Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка подключения: {e.Message}");
                try
                {
                    _socket.Close();
                    _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
                catch (Exception innerEx)
                {
                    Console.WriteLine($"Не удалось перезапустить сокет: {innerEx.Message}");
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
