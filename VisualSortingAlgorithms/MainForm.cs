using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VisualSortingAlgorithms.Boundary;
using VisualSortingAlgorithms.Control;
using ZedGraph;

namespace VisualSortingAlgorithms
{
    public partial class MainForm : Form
    {
        private App _app = Program._app;
        private BehaviorSubject<int> _stepTrigger = new BehaviorSubject<int>(App.DefaultStepDelay);
        private BehaviorSubject<Unit> _visualizationTick = new BehaviorSubject<Unit>(Unit.Default);

        public MainForm()
        {
            InitializeComponent();
            //var source = Observable.Create<int>((observer) => {
            //    observer.OnNext(1);
            //    observer.OnNext(2);
            //    observer.OnNext(3);
            //    observer.OnCompleted();
            //    return Disposable.Create(() => Console.WriteLine($"Observer has unsubscribed"));
            //}).SelectMany(it =>
            ////var source = Observable.Range(1, 5).SelectMany(it =>
            //{
            //    return new List<string>
            //    {
            //        $"#{it}",
            //        $"#{it + 10}"
            //    };
            //});
            //var trigger = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            //var triggeredSource = source.Zip(trigger, (s, _) => s);
            //triggeredSource.Finally(() =>
            //{
            //    Console.WriteLine("Finally does not work in here");
            //});
            //triggeredSource.Finally(() =>
            //{
            //    Console.WriteLine("Finally");
            //}).Timestamp().Subscribe(timer =>
            //{
            //    Console.WriteLine($"Timer:{timer.Timestamp} {timer.Value}");
            //},
            //ex =>
            //{
            //    Console.WriteLine($"Error {ex}");
            //},
            //() =>
            //{
            //    Console.WriteLine($"Complete");
            //});
            tableLayoutPanel1.Visible = false;
            _app.OnStopVisualization.Subscribe(_ =>
            {
                startButton.Text = "&Start";
                randomButton.Enabled = true;
            });
            checkedListBox1.SetItemChecked(0, true);
            delayNumericUpDown.Value = App.DefaultStepDelay;

            _stepTrigger.Select(t => Observable.Interval(TimeSpan.FromMilliseconds(t))).
                Switch().Subscribe(_ =>
                {
                    Debug.WriteLine("tick-tack ");
                    _visualizationTick.OnNext(Unit.Default);
                });
            //_visualizationTick = Observable.Timer(
            //    TimeSpan.Zero,
            //    TimeSpan.FromMilliseconds(StepDelay));
        }
        private void button1_Click(object sender, EventArgs e)
        {
            switch (startButton.Text)
            {
                case "&Stop":
                    _app.StopVisualization();
                    startButton.Text = "&Start";
                    break;
                case "&Start":
                default:
                    _app.StartVisualization();
                    startButton.Text = "&Stop";
                    randomButton.Enabled = false;
                    break;
            }
        }
        private void delayNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            var stepDelay = (int)delayNumericUpDown.Value;
            _stepTrigger.OnNext(stepDelay);
        }
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var name = checkedListBox1.Items[e.Index] as string;
            if (e.NewValue == CheckState.Checked)
            {
                GraphControl graphControl = GraphControl.Create();
                SortAlgorithm sa = _app.CreateSortAlgorithm(name);
                graphControl.SortAlgorithm = sa;
                graphControl.VisualizationTick = _visualizationTick;
                _app.AddGraph(graphControl);
                mainFlowLayoutPanel.Controls.Add(graphControl);
            }
            else if (e.NewValue == CheckState.Unchecked)
            {
                var gc = _app.FindGraph(name: name);
                _app.RemoveGraph(gc);
                var zcontrol =  gc as ZedGraphControl;
                mainFlowLayoutPanel.Controls.Remove(zcontrol);
            }
            FitGraphs();
        }
        private void bigOGraphCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //_app.ShowBigOGraph();
        }
        private void MainForm_Resize(object sender, EventArgs e)
        {
            FitGraphs();
        }
        private void FitGraphs()
        {
            if (mainFlowLayoutPanel.Controls.Count <= 0)
            {
                return;
            }
            int w = splitContainer.Panel2.Width - 40;
            int h = (splitContainer.Panel2.Height / mainFlowLayoutPanel.Controls.Count) - 6;
            for (int i = 0; i < mainFlowLayoutPanel.Controls.Count; i++)
            {
                var c = mainFlowLayoutPanel.Controls[i];
                c.Width = w;
                c.Height = h;
            }
        }
        private void randomButton_Click(object sender, EventArgs e)
        {
            _app.GenerateRandomSeries();
        }
    }
}
