using System.Threading.Tasks;
using System.Windows;
using KarkatAnimation.Manager;
using KarkatAnimation.WebServer;

namespace KarkatAnimation.WPF.Windows
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView
    {
        private readonly AnimationManager _animationManager;
        private readonly KarkatServer _karkatServer;

        public MainView()
        {
            InitializeComponent();

            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;

            _animationManager = new AnimationManager(CurrentSampleVolume);
            _karkatServer = new KarkatServer();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _animationManager.StartMonitoring();
            Task.Factory.StartNew(() => _karkatServer.StartServer());
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _animationManager.StopMonitoring();
            _karkatServer.StopServer();
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            new SettingsControl().ShowDialog();
        }
    }
}
