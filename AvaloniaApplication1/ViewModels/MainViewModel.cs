using System.Collections.ObjectModel;

namespace AvaloniaApplication1.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<double> DataX { get; }
        public ObservableCollection<double> DataY { get; }

        public MainViewModel()
        {
            DataX = new ObservableCollection<double> { 1, 2, 3, 4, 5 };
            DataY = new ObservableCollection<double> { 1, 4, 9, 16, 25 };
        }

        public void UpdateData()
        {
            DataX.Add(6);
            DataY.Add(36);
        }
    }
}
