using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
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

        public MainForm()
        {
            InitializeComponent();
            //var source = Observable.Range(1, 5).SelectMany(it =>
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
            //triggeredSource.Take(10).Finally(() =>
            //{
            //    Console.WriteLine("Finally");
            //}).Timestamp().Subscribe(timer =>
            //{
            //    Console.WriteLine($"Timer:{timer.Timestamp} {timer.Value}");
            //});

            _app.OnStopVisualization += _app_OnStopVisualization1;
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
                break;
            }
            GraphControl graphControl = GraphControl.Create();
            SortAlgorithm sa = _app.CreateSortAlgorithm("Merge");
            graphControl.SortAlgorithm = sa;
            _app.AddGraph(graphControl);
            splitContainer.Panel2.Controls.Add(graphControl);
            FitGraphs();
        }

        private void _app_OnStopVisualization1()
        {
            startButton.Text = "&Start";
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
                    break;
            }
        }
        private void delayNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _app.SetStepDelay((int)delayNumericUpDown.Value);
        }
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var name = checkedListBox1.Items[e.Index] as string;
            if (e.NewValue == CheckState.Checked)
            {
                GraphControl graphControl = GraphControl.Create();
                SortAlgorithm sa = _app.CreateSortAlgorithm(name);
                graphControl.SortAlgorithm = sa;
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
            //var it = _app.SortAlgorithms.
        }
        private void MainForm_Resize(object sender, EventArgs e)
        {
            FitGraphs();
        }
        private void FitGraphs()
        {
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
