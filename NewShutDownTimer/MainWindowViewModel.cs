using System;
using System.Collections.Generic;
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

        public MainWindowViewModel() {

            Timer timer = new Timer(1000);
            timer.Elapsed += (object sender, ElapsedEventArgs e) => {
                RaisePropertyChanged(nameof(ElapsedTimeFromStart)); 
            };

            timer.Start();
        }
    }
}
