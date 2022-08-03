namespace NewShutDownTimer
{
    using System.Windows;

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += This_Loaded;
        }

        private void This_Loaded(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)DataContext).Window = this;
        }
    }
}
