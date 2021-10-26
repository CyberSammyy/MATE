using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System;
using System.Threading;

namespace MATE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private object locker;

        public delegate Task InTimeCheckerDelegate();

        public event InTimeCheckerDelegate Shutdown;
        public DateTime ShutdownTime { get; private set; }
        public TimeSpan TimeLeftToShutdown { get; set; }
        public bool IsCancellationRequested { get; set; } = false;
        public bool IsWarningNeeded { get; set; }
        public bool IsNotificationModeEnabled { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            setDeadlineButton.IsEnabled = true;
            cancelDeadlineButton.IsEnabled = false;
            isWarningNeeded.IsChecked = false;
            isShutDownRequired.IsChecked = false;
        }

        private async void setDeadlineButton_Click(object sender, RoutedEventArgs e)
        {
            if(deadlineTime.SelectedTime == null)
            {
                return;
            }

            setDeadlineButton.IsEnabled = false;
            cancelDeadlineButton.IsEnabled = true;

            ShutdownTime = deadlineTime.SelectedTime.Value;

            if(ShutdownTime.Hour == 0)
            {
                ShutdownTime = ShutdownTime.AddHours(24);
            }

            TimeLeftToShutdown = ShutdownTime - DateTime.Now;

            if(TimeLeftToShutdown.Hours < 0 || TimeLeftToShutdown.Minutes < 0 || TimeLeftToShutdown.Seconds < 0)
            {
                var newTimeSpan = TimeLeftToShutdown.Add(new TimeSpan(24, 0, 0));
                TimeLeftToShutdown = newTimeSpan;
            }

            Shutdown += OnShutdown;

            locker = new object();
            
            await CheckingCycle();
        }

        private void cancelDeadlineButton_Click(object sender, RoutedEventArgs e)
        {
            Shutdown -= OnShutdown;

            deadlineTime.SelectedTime = null;

            IsCancellationRequested = true;

            setDeadlineButton.IsEnabled = true;
            cancelDeadlineButton.IsEnabled = false;

            isWarningNeeded.IsChecked = false;

            timeLeftProgress.ClearValue(UidProperty);
        }

        private async Task CheckingCycle()
        {
            await Task.Run(async () =>
            {
                int timeLeftInSeconds = (int)TimeLeftToShutdown.TotalSeconds;

                await Dispatcher.InvokeAsync(() =>
                {
                    timeLeftProgress.Maximum = timeLeftInSeconds;
                    timeLeftProgress.Value = 0;
                });

                for(int timeCounter = timeLeftInSeconds; timeCounter >= 0; timeCounter--)
                {
                    if(IsWarningNeeded)
                    {
                        if (timeCounter == 1800)
                        {
                            MessageBox.Show("Get ready. It's only 30 minutes left!");
                        }
                        else if(timeCounter == 60)
                        {
                            MessageBox.Show("Get ready. It's only 1 minute left!");
                        }
                    }

                    if(IsCancellationRequested)
                    {
                        return;
                    }

                    await Dispatcher.InvokeAsync(() =>
                    {
                        if (timeCounter - 1 == 0)
                        {
                            timeLeftProgress.Value++;
                        }
                        else
                        {
                            timeLeftProgress.Value++;
                        }
                    });

                    await Task.Delay(new TimeSpan(0, 0, 1));
                }

                await Dispatcher.InvokeAsync(() =>
                {
                    timeLeftProgress.Value = timeLeftInSeconds;
                });

                await Shutdown();
            });
        }

        //private async Task OnShutdown()
        //{
        //    await Task.Run(() =>
        //    {
        //        var psi = new ProcessStartInfo("shutdown", "-f /s /t 0");
        //        psi.CreateNoWindow = true;
        //        psi.UseShellExecute = false;
        //
        //        Process.Start(psi);
        //    });
        //
        //    await Dispatcher.InvokeAsync(() =>
        //    {
        //        cancelDeadlineButton.IsEnabled = false;
        //        setDeadlineButton.IsEnabled = true;
        //        timeLeftProgress.Value = 0;
        //    });
        //}

        private async Task OnShutdown()
        {
            var psi = new ProcessStartInfo("shutdown", "/s /t 0");
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            MessageBox.Show("ЫЫЫЫЫЫЫЫЫЫЫЫ");

            await Dispatcher.InvokeAsync(() =>
            {
                cancelDeadlineButton.IsEnabled = false;
                setDeadlineButton.IsEnabled = true;
                timeLeftProgress.Value = 0;
            });
        
            Shutdown -= OnShutdown;
        }

        private void isWarningNeeded_Checked(object sender, RoutedEventArgs e)
        {
            IsWarningNeeded = true;
        }

        private void isWarningNeeded_Unchecked(object sender, RoutedEventArgs e)
        {
            IsWarningNeeded = false;
        }

        private void isShutDownRequired_Checked(object sender, RoutedEventArgs e)
        {
            IsNotificationModeEnabled = false;
        }

        private void isShutDownRequired_Unchecked(object sender, RoutedEventArgs e)
        {
            IsNotificationModeEnabled = true;
        }
    }
}
