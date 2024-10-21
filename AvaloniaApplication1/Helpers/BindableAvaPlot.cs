using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using ScottPlot.Avalonia;
using System.Collections.Generic;
using System.Linq;

namespace AvaloniaApplication1.Controls
{
    public class BindableAvaPlot : AvaPlot
    {
        public static readonly StyledProperty<IEnumerable<double>> DataXProperty =
            AvaloniaProperty.Register<BindableAvaPlot, IEnumerable<double>>(nameof(DataX));

        public static readonly StyledProperty<IEnumerable<double>> DataYProperty =
            AvaloniaProperty.Register<BindableAvaPlot, IEnumerable<double>>(nameof(DataY));

        public IEnumerable<double> DataX
        {
            get => GetValue(DataXProperty);
            set => SetValue(DataXProperty, value);
        }

        public IEnumerable<double> DataY
        {
            get => GetValue(DataYProperty);
            set => SetValue(DataYProperty, value);
        }

        static BindableAvaPlot()
        {
            DataXProperty.Changed.AddClassHandler<BindableAvaPlot>((x, e) => x.OnDataChanged());
            DataYProperty.Changed.AddClassHandler<BindableAvaPlot>((x, e) => x.OnDataChanged());
        }

        private void OnDataChanged()
        {
            if (DataX != null && DataY != null)
            {
                Plot.Clear();
                Plot.Add.Scatter(DataX.ToArray(), DataY.ToArray());
                Refresh();
            }
        }
    }
}
