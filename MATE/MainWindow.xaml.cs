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

        public TimeAdvanced ShutdownTime { get; private set; }

        public bool IsCancellationRequested { get; set; } = false;

        public bool IsWarningNeeded { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            setDeadlineButton.IsEnabled = true;
            cancelDeadlineButton.IsEnabled = false;
            isWarningNeeded.IsChecked = false;
            ShutdownTime = new TimeAdvanced();
        }

        private async void setDeadlineButton_Click(object sender, RoutedEventArgs e)
        {
            if(deadlineTime.SelectedTime == null)
            {
                return;
            }

            setDeadlineButton.IsEnabled = false;
            cancelDeadlineButton.IsEnabled = true;

            ShutdownTime.ChangeTime(deadlineTime.SelectedTime.Value);
            Shutdown += OnShutdown;

            locker = new object();
            
            await CheckingCycle();
        }

        private void cancelDeadlineButton_Click(object sender, RoutedEventArgs e)
        {
            Shutdown -= OnShutdown;
            IsCancellationRequested = true;

            setDeadlineButton.IsEnabled = true;
            cancelDeadlineButton.IsEnabled = false;

            isWarningNeeded.IsChecked = false;

            timeLeftProgress.Value = 0;
        }

        private async Task CheckingCycle()
        {
            await Task.Run(async () =>
            {
                //int TEMP_COUNTER = 0;

                var totalSpan = ShutdownTime - new TimeAdvanced().SetTime(DateTime.Now);

                if(totalSpan.Hours < 0)
                {
                    totalSpan.ChangeTime(new TimeSpan(24, 0, 0));
                }

                int timeLeftInMinutes = totalSpan.GetTotalMinutes();

                await Dispatcher.InvokeAsync(() =>
                {
                    timeLeftProgress.Maximum = 100;
                    timeLeftProgress.Value = 0;
                });

                while (/*TEMP_COUNTER < timeLeftInMinutes*/ DateTime.Now.Hour != ShutdownTime.Hours ||
                       DateTime.Now.Minute != ShutdownTime.Minutes)
                {
                    if(IsWarningNeeded)
                    {
                        IsWarningNeeded = false;

                        if (totalSpan.Minutes == 30)
                        {
                            MessageBox.Show("Get ready. It's only 30 minutes left!");
                        }
                        else if(totalSpan.Minutes == 1)
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
                        if(timeLeftInMinutes - 1 == 0)
                        {
                            timeLeftProgress.Value += (int)100 / 1;
                        }
                        else
                        {
                            timeLeftProgress.Value += (int)100 / timeLeftInMinutes - 1;
                        }
                    });

                    //TEMP_COUNTER++;

                    Thread.Sleep(new TimeSpan(0, 1, 0));
                }

                await Dispatcher.InvokeAsync(() =>
                {
                    timeLeftProgress.Value = 100;
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
    }
}
