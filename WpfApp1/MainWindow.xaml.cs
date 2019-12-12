using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private double _leftHandPosition_X;
        private double _rightHandPosition_Y;
        private KinectSensor _sensor;
        private Socket _socket;
        private bool remoteFlag;
        private bool counterStarted;
        private TurnStateEnum _turnStateEnum = TurnStateEnum.RestServo;
        private RideStateEnum _rideStateEnum = RideStateEnum.RestEngine;

        public TurnStateEnum TurnStateEnum
        {
            get => _turnStateEnum;
            set
            {
                if (remoteFlag)
                    if (value != _turnStateEnum)
                    {
                        _turnStateEnum = value;
                        //  SetTurnRequest(((TurnStateEnum)_turnStateEnum).ToString());
                    }
            }
        }

        public RideStateEnum RideStateEnum
        {
            get => _rideStateEnum;
            set
            {
                if (remoteFlag)
                    if (value != _rideStateEnum)
                    {
                        _rideStateEnum = value;
                        //  SetRideRequest(((RideStateEnum)_rideStateEnum).ToString());
                    }
            }
        }


        private double KinnectXValueMapper(double leftHandPosition_X)
        {
            if (leftHandPosition_X < -0.25)
                leftHandPosition_X = -0.25;
            if (leftHandPosition_X > 0)
                leftHandPosition_X = 0;
            return (0.25 + leftHandPosition_X) * 2200;
        }

        private double KinnectYValueMapper(double leftHandPosition_Y)
        {
            if (leftHandPosition_Y < 0)
                leftHandPosition_Y = 0;
            if (leftHandPosition_Y > 0.5)
                leftHandPosition_Y = 0.5;
            return 450 - (leftHandPosition_Y) * 900;
        }

        public double LeftHandPosition_X
        {
            get => _leftHandPosition_X;
            set
            {
                _leftHandPosition_X = value;
                Thickness margin = leftHandButton.Margin;
                margin.Left = KinnectXValueMapper(_leftHandPosition_X);
                leftHandButton.Margin = margin;
                if (KinnectXValueMapper(_leftHandPosition_X) < 75)
                    TurnStateEnum = TurnStateEnum.Left;

                else if (KinnectXValueMapper(_leftHandPosition_X) > 475)
                    TurnStateEnum = TurnStateEnum.Right;

                else
                    TurnStateEnum = TurnStateEnum.RestServo;
            }
        }

        public double RightHandPosition_Y
        {
            get => _rightHandPosition_Y; set
            {
                _rightHandPosition_Y = value;
                Thickness margin = rightHandButton.Margin;
                margin.Top = KinnectYValueMapper(_rightHandPosition_Y);
                rightHandButton.Margin = margin;
                if (KinnectYValueMapper(_rightHandPosition_Y) < 75)
                {
                    RideStateEnum = RideStateEnum.Forward;
                }
                else if (KinnectYValueMapper(_rightHandPosition_Y) > 375)
                {
                    RideStateEnum = RideStateEnum.Backward;
                }
                else
                {
                    RideStateEnum = RideStateEnum.RestEngine;
                }
            }
        }

        private void SetRideRequest(string parameter)
        {
            //SocketSendReceive(parameter);
        }
        private void SetTurnRequest(string parameter)
        {
            //SocketSendReceive(parameter);
        }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindowLoaded;
            try
            {
                // _socket = ConnectSocket();
            }
            catch (Exception e)
            {


            }
            finally
            {
                //   _socket.Dispose();
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

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            var sensorStatus = new KinectSensorChooser();
            sensorStatus.KinectChanged += KinectSensorChooserKinectChanged;
            kinectChooser.KinectSensorChooser = sensorStatus;
            sensorStatus.Start();
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);

        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                remoteFlag = !remoteFlag;
            }
        }


        private void KinectSensorChooserKinectChanged(object sender, KinectChangedEventArgs e)
        {
            if (_sensor != null)
                _sensor.SkeletonFrameReady -= KinectSkeletonFrameReady;
            _sensor = e.NewSensor;

            if (_sensor == null) return;

            _sensor.SkeletonStream.Enable();
            _sensor.SkeletonFrameReady += KinectSkeletonFrameReady;

        }
        private void KinectSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            var skeletons = new Skeleton[0];

            using (var skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);

                    if (!counterStarted && !remoteFlag && KinnectXValueMapper(LeftHandPosition_X) > 225 && KinnectXValueMapper(LeftHandPosition_X) < 325 && KinnectYValueMapper(RightHandPosition_Y) > 175 && KinnectYValueMapper(RightHandPosition_Y) < 275)
                    {
                        var task = new Task(() =>
                        {
                            counterStarted = !counterStarted;
                            for (int i = 3; i > 0; i--)
                            {
                                topButton.Dispatcher.Invoke(
                                    new Action(() => { topButton.Content = i.ToString(); }));
                                Thread.Sleep(1000);
                            }
                            remoteFlag = true;
                            counterStarted = !counterStarted;
                            topButton.Dispatcher.Invoke(
                                    new Action(() =>
                                    {
                                        topButton.Content = "";
                                    }));
                        });
                        task.Start();
                    }
                }

                else
                {
                    TurnStateEnum = TurnStateEnum.RestServo;
                    RideStateEnum = RideStateEnum.RestEngine;
                }
            }

            if (skeletons.Length == 0) { return; }

            var skel = skeletons.FirstOrDefault(x => x.TrackingState == SkeletonTrackingState.Tracked);

            if (skel == null) { return; }

            var rightHand = skel.Joints[JointType.WristRight];
            RightHandPosition_Y = Math.Round(rightHand.Position.Y, 2);

            var leftHand = skel.Joints[JointType.WristLeft];
            LeftHandPosition_X = Math.Round(leftHand.Position.X, 2);
        }
    }

    public enum TurnStateEnum { Right, Left, RestServo }
    public enum RideStateEnum { Forward, Backward, RestEngine }
}
