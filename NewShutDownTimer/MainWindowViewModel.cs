﻿namespace NewShutDownTimer
{
    using System;
    using System.Diagnostics;
    using System.Timers;
    using System.Windows;
    using Prism.Commands;
    using Prism.Mvvm;

    public class MainWindowViewModel : BindableBase
    {
        private readonly DateTime startUpDate = DateTime.Now;

        private TimeSpan baseRemainingTimeSpan = new TimeSpan(3, 0, 0);
        private DelegateCommand<object> changeRemainingTimeCommand;
        private DateTime lastGotDateTime = DateTime.Now;
        private bool preShutdownNotified;
        private TimeSpan remainingTimeSpan = new TimeSpan(3, 0, 0);
        private DateTime timeForShutdown = DateTime.Now.AddHours(3);

        public MainWindowViewModel()
        {
            var timer = new Timer(1000);
            timer.Elapsed += (_, e) =>
            {
                RemainingTimeSpan -= DateTime.Now - lastGotDateTime;
                lastGotDateTime = DateTime.Now;
                RaisePropertyChanged(nameof(ElapsedTimeFromStart));
                RaisePropertyChanged(nameof(RemainingTimeUntilShutDown));

                if (!preShutdownNotified)
                {
                    if (DateTime.Now.CompareTo(timeForShutdown.AddMinutes(-15)) > 0)
                    {
                        preShutdownNotified = true;
                        if (Window != null)
                        {
                            Window.Dispatcher.Invoke(() =>
                            {
                                Window.Activate();
                                MessageBox.Show(Window, "シャットダウンまで残り15分です");
                            });
                        }
                    }
                }

                if (timeForShutdown.CompareTo(DateTime.Now) < 0)
                {
                    Shutdown();
                    timer.Stop();
                }
            };

            timer.Start();
        }

        public Window Window { private get; set; }

        public string ElapsedTimeFromStart
        {
            get
            {
                var span = DateTime.Now - startUpDate;
                return span.ToString(@"d\.hh\:mm\:ss");
            }
        }

        public string TimeForShutdown => timeForShutdown.ToString(@"MM/dd HH\:mm\:ss");

        public string RemainingTimeUntilShutDown => (timeForShutdown - DateTime.Now).ToString(@"hh\:mm\:ss");

        public DelegateCommand<object> ChangeRemainingTimeCommand =>
            changeRemainingTimeCommand ?? (changeRemainingTimeCommand = new DelegateCommand<object>((param) =>
            {
                var buttonTag = (string)param;
                var additionMinutes = int.Parse(buttonTag);
                timeForShutdown = timeForShutdown.AddMinutes(additionMinutes);
                RaisePropertyChanged(nameof(TimeForShutdown));

                RemainingTimeSpan += new TimeSpan(0, additionMinutes, 0);
                baseRemainingTimeSpan = remainingTimeSpan;

                if (timeForShutdown > DateTime.Now.AddMinutes(15))
                {
                    preShutdownNotified = false;
                }
            }));

        public double RemainingTimeRatio => RemainingTimeSpan.TotalSeconds / baseRemainingTimeSpan.TotalSeconds;

        private TimeSpan RemainingTimeSpan
        {
            get => remainingTimeSpan;
            set
            {
                SetProperty(ref remainingTimeSpan, value);
                RaisePropertyChanged(nameof(RemainingTimeRatio));
            }
        }

        private void Shutdown()
        {
            // shutdown.exe を実行するコード
            var psi = new ProcessStartInfo
            {
                FileName = "shutdown.exe",

                // コマンドラインを指定
                Arguments = "/s",

                // ウィンドウを表示しないようにする
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            Process.Start(psi);
        }
    }
}