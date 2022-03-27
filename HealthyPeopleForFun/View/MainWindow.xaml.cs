using System.Windows;
using HealthyPeopleForFun.ViewModel;

namespace HealthyPeopleForFun.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            PositionWindow();
        }

        private void PositionWindow()
		{
            var workArea = SystemParameters.WorkArea;
            Left = workArea.Right - Width - 10;
            Top = workArea.Bottom - Height - 10;
        }
	}
}
