using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.ServiceModel.Discovery;
using System.Windows;
using WCFReference;
using System.Timers;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPFClient.View.ViewModel
{
    public class DiscoveryServersViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<EndpointDiscoveryMetadata> FoundServers { get; set; } = new ObservableCollection<EndpointDiscoveryMetadata>();
        public int Count
        {
            get
            {
                return FoundServers.Count;
            }
        }
        public Visibility DataGridVisibility { get; set; }
        private double _progress;
        public double Progress
        {
            get
            {
                return _progress / MaxProgress;
            }
            set
            {
                _progress = value;
            }
        }

        public double MaxProgress { get; set; }
        public bool Done { get; set; } = true;
        private SolidColorBrush _barColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CC119EDA"));
        public SolidColorBrush BarColor
        {
            get
            {
                return _barColor;
            }
            set
            {
                _barColor = value;
                PropertyChanged(this, new PropertyChangedEventArgs("BarColor"));
            }
        }

        private DiscoveryClient dc = new DiscoveryClient(new UdpDiscoveryEndpoint());
        private Stopwatch sw;
        private Timer tm;
        private FindCriteria fc = new FindCriteria(typeof(IWCFService));
        public DiscoveryServersViewModel()
        {
            Progress = 0;
            fc.Duration = TimeSpan.FromSeconds(3);
            MaxProgress = fc.Duration.TotalMilliseconds;
            dc.FindProgressChanged += (s, e) =>
            {
                FoundServers.Add(e.EndpointDiscoveryMetadata);
                PropertyChanged(this, new PropertyChangedEventArgs("Count"));
            };
            dc.FindCompleted += (s, e) =>
            {
                if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                {
                    Done = true;
                    tm.Stop();
                    sw.Reset();
                    Progress = MaxProgress;
                    PropertyChanged(this, new PropertyChangedEventArgs("Progress"));
                    PropertyChanged(this, new PropertyChangedEventArgs("Done"));

                    SolidColorBrush scb = BarColor;
                    ColorAnimation ca = new ColorAnimation(Colors.White, TimeSpan.FromMilliseconds(200));
                    ca.AutoReverse = true;
                    ca.EasingFunction = new ExponentialEase();
                    scb.BeginAnimation(SolidColorBrush.ColorProperty, ca);
                    BarColor = scb;
                }
            };
            Search();
        }

        public void Dispose()
        {
            FoundServers.Clear();
            tm.Dispose();
        }

        public void Search()
        {
            Done = false;
            dc.FindAsync(fc);
            sw = new Stopwatch();
            tm = new Timer(10);
            tm.Elapsed += (s, e) =>
            {
                Progress = sw.ElapsedMilliseconds;
                PropertyChanged(this, new PropertyChangedEventArgs("Progress"));
            };
            sw.Start();
            tm.Start();
        }
    }
}
