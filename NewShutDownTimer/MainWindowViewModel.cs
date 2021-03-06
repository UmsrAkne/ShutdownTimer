﻿using System;
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

namespace NewShutDownTimer {
    class MainWindowViewModel : BindableBase{

        public Window Window { private get; set; }

        private DateTime startUpDate = DateTime.Now;
        public String ElapsedTimeFromStart {
            get {
                TimeSpan span = DateTime.Now - startUpDate;
                return span.ToString(@"d\.hh\:mm\:ss");
            }
        }

        private DateTime timeForShutdown = DateTime.Now.AddHours(3);
        public String TimeForShutdown {
            get {
                return timeForShutdown.ToString(@"MM/dd HH\:mm\:ss");
            }
        }

        public String RemainingTimeUntilShutDown {
            get {
                return (timeForShutdown - DateTime.Now).ToString(@"hh\:mm\:ss") + " " + getRemainingTimeMeter();
            }
        }

        /// <summary>
        /// 残り時間を大まかに示すメーター（文字列）を取得します。
        /// </summary>
        public String getRemainingTimeMeter(){
            const int meterUnit = 15;
            var remainingTime = timeForShutdown - DateTime.Now;
            int meterLength = (int)remainingTime.TotalMinutes / meterUnit;

            char meterCharacter = '/';
            return new string(meterCharacter, meterLength);
        }

        private Boolean preShutdownNotified = false;

        public DelegateCommand<object> ChangeRemainingTimeCommand { get; private set; }

        public MainWindowViewModel() {

            Timer timer = new Timer(1000);
            timer.Elapsed += (object sender, ElapsedEventArgs e) => {
                RaisePropertyChanged(nameof(ElapsedTimeFromStart));
                RaisePropertyChanged(nameof(RemainingTimeUntilShutDown));

                if (!preShutdownNotified) {
                    if (DateTime.Now.CompareTo(timeForShutdown.AddMinutes(-15)) > 0) {
                        preShutdownNotified = true;
                        Window.Dispatcher.Invoke(() => {
                            Window.Activate();
                            MessageBox.Show(Window, "シャットダウンまで残り15分です");
                            }
                        );
                    }
                }

                if(timeForShutdown.CompareTo(DateTime.Now) < 0) {
                    shutdown();
                    timer.Stop();
                }
            };

            timer.Start();

            ChangeRemainingTimeCommand = new DelegateCommand<object>(
                (object param) => {
                    String buttonTag = (String)param;
                    int additionMinutes = int.Parse(buttonTag);
                    timeForShutdown = timeForShutdown.AddMinutes(additionMinutes);
                    RaisePropertyChanged(nameof(TimeForShutdown));

                    if (timeForShutdown > DateTime.Now.AddMinutes(15)) {
                        preShutdownNotified = false;
                    }
                },
                (object param) => { return true; }
            );

        }

        private void shutdown() {
            // shutdown.exe を実行するコード
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "shutdown.exe";

            //コマンドラインを指定
            psi.Arguments = "/s";

            //ウィンドウを表示しないようにする
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            Process p = Process.Start(psi);
        }
    }
}
