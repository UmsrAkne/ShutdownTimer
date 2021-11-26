namespace NewShutDownTimer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Timers;
    using System.Windows;
    using Prism.Commands;
    using Prism.Mvvm;

    public class MainWindowViewModel : BindableBase
    {
        private DateTime startUpDate = DateTime.Now;
        private DateTime timeForShutdown = DateTime.Now.AddHours(3);
        private bool preShutdownNotified = false;
        private DelegateCommand<object> changeRemainingTimeCommand;

        public MainWindowViewModel()
        {
            Timer timer = new Timer(1000);
            timer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                RaisePropertyChanged(nameof(ElapsedTimeFromStart));
                RaisePropertyChanged(nameof(RemainingTimeUntilShutDown));

                if (!preShutdownNotified)
                {
                    if (DateTime.Now.CompareTo(timeForShutdown.AddMinutes(-15)) > 0)
                    {
                        preShutdownNotified = true;
                        Window.Dispatcher.Invoke(() =>
                        {
                            Window.Activate();
                            MessageBox.Show(Window, "シャットダウンまで残り15分です");
                        });
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
                TimeSpan span = DateTime.Now - startUpDate;
                return span.ToString(@"d\.hh\:mm\:ss");
            }
        }

        public string TimeForShutdown => timeForShutdown.ToString(@"MM/dd HH\:mm\:ss");

        public string RemainingTimeUntilShutDown
        {
            get => (timeForShutdown - DateTime.Now).ToString(@"hh\:mm\:ss") + " " + GetRemainingTimeMeter();
        }

        public DelegateCommand<object> ChangeRemainingTimeCommand
        {
            get => changeRemainingTimeCommand ?? (changeRemainingTimeCommand = new DelegateCommand<object>((param) =>
            {
                string buttonTag = (string)param;
                int additionMinutes = int.Parse(buttonTag);
                timeForShutdown = timeForShutdown.AddMinutes(additionMinutes);
                RaisePropertyChanged(nameof(TimeForShutdown));

                if (timeForShutdown > DateTime.Now.AddMinutes(15))
                {
                    preShutdownNotified = false;
                }
            }));
        }

        /// <summary>
        /// 残り時間を大まかに示すメーター（文字列）を取得します。
        /// </summary>
        public string GetRemainingTimeMeter()
        {
            const int MeterUnit = 15;
            var remainingTime = timeForShutdown - DateTime.Now;
            int meterLength = (int)remainingTime.TotalMinutes / MeterUnit;

            char meterCharacter = '/';
            return new string(meterCharacter, meterLength);
        }

        private void Shutdown()
        {
            /// shutdown.exe を実行するコード
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "shutdown.exe";

            ///コマンドラインを指定
            psi.Arguments = "/s";

            ///ウィンドウを表示しないようにする
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            Process p = Process.Start(psi);
        }
    }
}
