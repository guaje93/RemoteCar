using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket _socket; 
        private TurnStateEnum _turnStateEnum = TurnStateEnum.RestServo;
        private RideStateEnum _rideStateEnum = RideStateEnum.RestEngine;

        public TurnStateEnum TurnStateEnum
        {
            get => _turnStateEnum;
            set
            {
                if (value != _turnStateEnum)
                {
                    _turnStateEnum = value;
                    SetTurnRequest(((TurnStateEnum)_turnStateEnum).ToString());
                }
            }
        }

        public RideStateEnum RideStateEnum
        {
            get => _rideStateEnum;
            set
            {
                if (value != _rideStateEnum)
                {
                    _rideStateEnum = value;
                    SetRideRequest(((RideStateEnum)_rideStateEnum).ToString());
                }
            }
        }

        private void SetRideRequest(string parameter)
        {
            _button.Content = parameter;
            var r = new Random();
            Brush brush = new SolidColorBrush(Color.FromRgb((byte)r.Next(1, 255),
                          (byte)r.Next(1, 255), (byte)r.Next(1, 233))); SolidColorBrush solidColorBrush = new SolidColorBrush();
            _button.Background = brush;
            //SocketSendReceive(parameter);
        }
        private void SetTurnRequest(string parameter)
        {
            _button.Content = parameter;
            var r = new Random();
            Brush brush = new SolidColorBrush(Color.FromRgb((byte)r.Next(1, 255),
                          (byte)r.Next(1, 255), (byte)r.Next(1, 233))); SolidColorBrush solidColorBrush = new SolidColorBrush();
            _button.Background = brush;
            //SocketSendReceive(parameter);
        }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
            try
            {
               //_socket = ConnectSocket();
            }
            catch (Exception e)
            {
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
            this.KeyUp += new KeyEventHandler(MainWindow_KeyUp);
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W || e.Key == Key.S)
            {
                RideStateEnum = RideStateEnum.RestEngine;
            }
            else if (e.Key == Key.A || e.Key == Key.D)
            {
                TurnStateEnum = TurnStateEnum.RestServo;
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
            {
                RideStateEnum = RideStateEnum.Forward;
            }
            else if (e.Key == Key.S)
            {
                RideStateEnum = RideStateEnum.Backward;

            }
            else if (e.Key == Key.A)
            {
                TurnStateEnum = TurnStateEnum.Left;
            }
            else if (e.Key == Key.D)
            {
                TurnStateEnum = TurnStateEnum.Right;
            }
        }
        private static Socket ConnectSocket(string server = "192.168.43.113", int port = 54000)
        {
            Socket s = null;
            {
                IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(server), port);

                Socket tempSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.Connect(ipe);

                if (tempSocket.Connected)
                {
                    s = tempSocket;
                }
                else
                {
                }
            }
            return s;
        }

        // This method requests the home page content for the specified server.
        private string SocketSendReceive(string request)
        {
            Byte[] bytesSent = Encoding.ASCII.GetBytes(request);

            if (_socket == null)
                return ("Connection failed");

            // Send request to the server.
            _socket.Send(bytesSent, bytesSent.Length, 0);
            return request;
        }
    }
        public enum TurnStateEnum { Right, Left, RestServo }
        public enum RideStateEnum { Forward, Backward, RestEngine }
}
