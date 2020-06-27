﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Prism.Mvvm;

namespace NewShutDownTimer {
    class MainWindowViewModel : BindableBase{

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
                return "Remaining " + (timeForShutdown - DateTime.Now).ToString(@"hh\:mm\:ss");
            }
        }

        public MainWindowViewModel() {

            Timer timer = new Timer(1000);
            timer.Elapsed += (object sender, ElapsedEventArgs e) => {
                RaisePropertyChanged(nameof(ElapsedTimeFromStart));
                RaisePropertyChanged(nameof(RemainingTimeUntilShutDown));

                if(timeForShutdown.CompareTo(DateTime.Now) < 0) {
                    shutdown();
                    timer.Stop();
                }
            };

            timer.Start();
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
