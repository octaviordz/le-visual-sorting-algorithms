using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisualSortingAlgorithms.Control;
using System.Reactive.Linq;

namespace VisualSortingAlgorithms.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Control.App _app = App._app;
        //public ObservableCollection<SortAlgorithmViewModel> SortAlgorithmList { get; set; }
        private BehaviorSubject<int> _stepTrigger = new BehaviorSubject<int>(Control.App.DefaultStepDelay);
        private BehaviorSubject<Unit> _visualizationTick = new BehaviorSubject<Unit>(Unit.Default);

        public MainWindow()
        {
            InitializeComponent();

            //SortAlgorithmList = new ObservableCollection<SortAlgorithmViewModel>
            //{
            //    new SortAlgorithmViewModel{ Checked = false,  Name = "Merge" },
            //    new SortAlgorithmViewModel{ Checked = false,  Name = "Quick" },
            //    new SortAlgorithmViewModel{ Checked = false,  Name = "Bubble" },
            //};
            mergeCheckBox.IsChecked = true;
            //sortAlgorithmListListBox.DataContext = SortAlgorithmList;
            //var cbox = sortAlgorithmListListBox.ItemTemplate.FindName("sortAlgorithmNameCheckBox", null);

            //SortAlgorithmList[0].Item1
            _app.OnStopVisualization.Subscribe(_ =>
            {
                startButton.Visibility = Visibility.Visible;
                stopButton.Visibility = Visibility.Collapsed;
            });
            
            //delayNumericUpDown.Value = Control.App.DefaultStepDelay;
            _stepTrigger.Select(t => Observable.Interval(TimeSpan.FromMilliseconds(t))).Switch().Subscribe(_ =>
            {
                _visualizationTick.OnNext(Unit.Default);
            });
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            Debug.WriteLine(checkBox.Content);
            string name = SortingAlgorithm.GetName(checkBox);
            GraphControl graphControl = new GraphControl
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Height = 200,
            };
            SortAlgorithm sa = _app.CreateSortAlgorithm(name);
            graphControl.SortAlgorithm = sa;
            graphControl.VisualizationTick = _visualizationTick;
            _app.AddGraph(graphControl);
            mainContentPanel.Children.Add(graphControl);
            FitGraphs();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            Debug.WriteLine(checkBox.Content);

            string name = SortingAlgorithm.GetName(checkBox);
            var gc = _app.FindGraph(name: name);
            _app.RemoveGraph(gc);
            var control = gc as GraphControl;
            mainContentPanel.Children.Remove(control);
            FitGraphs();
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            _app.StartVisualization();
            startButton.Visibility = Visibility.Collapsed;
            stopButton.Visibility = Visibility.Visible;
        }
        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            _app.StopVisualization();
            startButton.Visibility = Visibility.Visible;
            stopButton.Visibility = Visibility.Collapsed;
        }
        private void FitGraphs()
        {
            if (mainContentPanel.Children.Count <= 0)
            {
                return;
            }
            if (rightContent.ActualHeight <= 0)
            {
                return;
            }
            double w = rightContent.Width - 40.0;
            double h = (rightContent.ActualHeight / mainContentPanel.Children.Count);
            for (int i = 0; i < mainContentPanel.Children.Count; i++)
            {
                var c = (ContentControl)mainContentPanel.Children[i];
                c.Height = h;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FitGraphs();
        }

        private void SliderDelay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var stepDelay = (int)e.NewValue;
            _stepTrigger.OnNext(stepDelay);
        }

        private void SliderDelay_TouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
        }
    }
}
