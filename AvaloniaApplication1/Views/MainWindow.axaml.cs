using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;

namespace AvaloniaApplication1.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void ButtonClicked(object source, RoutedEventArgs args)
        {
            if (double.TryParse(celsius.Text, out double C))
            {
                var F = C * (9d / 5d) + 32;
                fahrenheit.Text = F.ToString("0.0");
            }
        }
    }
}