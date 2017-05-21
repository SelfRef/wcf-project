using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using WPFClient.Model;
using WPFClient.View;
using System.ServiceModel.Discovery;
using System.Collections.ObjectModel;
using GameWindow.GameScenes;

namespace WPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public Dictionary<string, string> Prots { get; set; }
        public string Domain { get; set; }
        public int Port { get; set; }
        public string Addr { get; set; }
        public string UserName { get; set; }
        public Connection Connection { get; private set; }
        public Collection<EndpointDiscoveryMetadata> FoundServers2 { get; set; }

        private SolidColorBrush playBtnColor;
        private SolidColorBrush connectBtnColor;
        Task gameThread;
        SceneManager game;

        public MainWindow()
        {
            InitializeComponent();
            Connection = new Connection();

            Domain = "localhost";
            Port = 9999;
            Addr = "Service";
            UserName = "User";
            Prots = new Dictionary<string, string>() { { "TCP", "net.tcp" } };

            connectBtn.Click += (s, e) => Connect();
            sendBtn.Click += (s, e) => SendMsg();
            Connection.Disconnected += (s, e) => Dispatcher.Invoke(UIDisconnected);
            Connection.Disconnected += (s, e) => game?.ExitGame();
            Connection.Connected += (s, e) => Dispatcher.Invoke(UIConnected);
        }

        private void UIDisconnected()
        {

            connectBtnColor = (SolidColorBrush)connectBtn.Background;
            connectBtnColor.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(Color.FromRgb(0, 180, 0), TimeSpan.FromMilliseconds(500)));
            connectBtn.Background = connectBtnColor;
            connectBtn.Content = "Połącz";
            playBtn.BeginAnimation(WidthProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() });
            output.AppendText(String.Format("<{0}> Rozłączono!\n", DateTime.Now.ToLongTimeString()));

            protBox.IsEnabled = true;
            domainBox.IsEnabled = true;
            portBox.IsEnabled = true;
            addrBox.IsEnabled = true;
            loginBox.IsEnabled = true;
            passBox.IsEnabled = true;
        }
        private void UIConnected()
        {
            output.AppendText(String.Format("<{0}> Połączono!\n", DateTime.Now.ToLongTimeString()));
            connectBtn.Content = "Rozłącz";
            playBtn.BeginAnimation(WidthProperty, new DoubleAnimation(330, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() });
            connectBtnColor = (SolidColorBrush)connectBtn.Background;
            connectBtnColor.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(Color.FromRgb(255, 0, 0), TimeSpan.FromMilliseconds(500)));
            connectBtn.Background = connectBtnColor;

            protBox.IsEnabled = false;
            domainBox.IsEnabled = false;
            portBox.IsEnabled = false;
            addrBox.IsEnabled = false;
            loginBox.IsEnabled = false;
            passBox.IsEnabled = false;
        }

        private void inputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (sender as TextBox).IsKeyboardFocused) SendMsg();
        }

        private void SendMsg()
        {
            if (Connection.IsConnected)
            {
                if (inputBox.Text.Length != 0)
                {
                    Connection.Channel.ConsoleWrite(inputBox.Text);
                    output.AppendText(String.Format("<{0}> {1}: {2}\n", DateTime.Now.ToLongTimeString(), UserName, inputBox.Text));
                    inputBox.Clear();
                }
                else Msg.Warning(Msg.WarningMsgs.EmptyInput, MessageBoxButton.OK);
            }
            else if (Msg.Warning(Msg.WarningMsgs.NotConnected, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Connect();
                if (Connection.IsConnected) SendMsg();
            }
        }

        private void output_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            textbox.ScrollToEnd();
        }

        private void Connect()
        {
            if (Connection.IsConnected) Connection.Close();
            else Connection.Login(Prots[protBox.SelectedValue.ToString()] + "://" + Domain + ":" + Port.ToString() + "/" + Addr, UserName, passBox.Password);
        }

        private void playBtn_Click(object sender, RoutedEventArgs e)
        {
            gameThread = new Task(() =>
            {
                using (game = new SceneManager(1200, 800, Connection))
                {
                    Dispatcher.Invoke(() => WindowState = WindowState.Minimized);
                    game.Exiting += (s, f) =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            playBtn.IsEnabled = true;
                            playBtnColor = (SolidColorBrush)playBtn.Background;
                            playBtnColor.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(Color.FromRgb(0, 180, 0), TimeSpan.FromMilliseconds(500)));
                            playBtn.Background = playBtnColor;

                            try
                            {
                                Connection.Channel.GameOpen(false);
                            }
                            catch (ObjectDisposedException) { }

                            WindowState = WindowState.Normal;
                            Connection.Close();
                        });
                    };
                    game.Run();
                }
            });
            gameThread.Start();
            playBtn.IsEnabled = false;
            playBtnColor = (SolidColorBrush)playBtn.Background;
            playBtnColor.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(Colors.Gray, TimeSpan.FromMilliseconds(500)));
            playBtn.Background = playBtnColor;
        }
    }
}
