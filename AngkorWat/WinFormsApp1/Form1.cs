using AngkorWat.Algorithms.Strategies;
using AngkorWat.Components;
using AngkorWat.IO.HTTP;
using AngkorWat.IO.JSON;
using AngkorWat.Phases;
using ScottPlot;
using ScottPlot.Drawing.Colormaps;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        readonly Data data;

        readonly ScottPlot.FormsPlot formsPlot1;

        private readonly Color[] colors = {
                Color.GhostWhite,
                Color.DarkGreen,
                Color.Green,
                Color.DarkRed,
                Color.Red,
                Color.Wheat,
            };
        private readonly ScottPlot.Drawing.Colormap cmap;

        private Directions? GlobalFleetDirection { get; set; } = null;
        private int? GlobalSpeed { get; set; } = null;

        public IShipStrategy MovingStrategy { get; private set; } = new DoNothingStrategy();
        public IShipStrategy FiringStrategy { get; private set; } = new FireAtWillStrategy();

        /// <summary>
        /// private DataTable 
        /// </summary>

        public Form1()
        {
            InitializeComponent();

            // Add the FormsPlot
            formsPlot1 = new()
            {
                Dock = DockStyle.Left,
                Width = 1400,
            };

            formsPlot1.Configuration.Zoom = false;
            formsPlot1.Configuration.Pan = false;

            Controls.Add(formsPlot1);
            cmap = new(colors);

            HttpHelper.SetApiKey("05d74cb4-7f21-424f-afc6-9dc573c82974");

            var rawMap = IOHelper.ReadInputData<RawMap>("map.json");

            var map = new Map(rawMap);

            map.PrintInfo();

            data = new Data(map);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await Phase1.LoadScan(data);

            SetStartingView();

            RefreshView();
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            await Phase1.LoadScan(data);

            var commands = IShipStrategy.GenerateEmpty(data);

            FiringStrategy.UpdateCommands(data, commands);
            MovingStrategy.UpdateCommands(data, commands);

            await Phase1.SendCommand(data, commands);

            RefreshView();
        }

        #region View stuff
        private double ViewX { get; set; } = 0.0d;
        private double ViewY { get; set; } = 0.0d;
        private double ViewScale { get; set; } = 100.0d;
        private bool IsFireRangeVisible { get; set; } = false;
        private bool IsZoneVisible { get; set; } = false;
        private void SetStartingView()
        {
            if (data.CurrentScan.MyShips.Any(e => e.HP > 0))
            {
                ViewX = data.CurrentScan.MyShips.Average(e => e.X);
                ViewY = data.CurrentScan.MyShips.Average(e => e.Y);
            }
            else
            {
                ViewX = data.Map.SizeX / 2.0d;
                ViewY = data.Map.SizeY / 2.0d;
            }
        }

        private AxisLimits SetStartAxis(Data data)
        {
            return new AxisLimits(
                ViewX - ViewScale,
                ViewX + ViewScale,
                data.Map.SizeY - (ViewY + ViewScale),
                data.Map.SizeY - (ViewY - ViewScale));
        }

        private void RefreshView()
        {
            formsPlot1.Plot.Clear();

            var t = formsPlot1.Plot.AddHeatmap(data.Map.Tiles, cmap);
            t.UseParallel = true;

            var axisLimits = SetStartAxis(data);
            formsPlot1.Plot.SetAxisLimits(axisLimits);

            if (IsFireRangeVisible)
            {
                foreach (var ship in data.CurrentScan.MyShips)
                {
                    if (ship.HP == 0)
                    {
                        continue;
                    }

                    var circle = formsPlot1.Plot.AddCircle(
                        ship.X,
                        data.Map.SizeY - ship.Y,
                        ship.CannonRadius, color: Color.FromArgb(0, Color.Pink),
                        lineStyle: LineStyle.None);

                    circle.HatchColor = Color.FromArgb(0, Color.Pink);
                    circle.Color = Color.FromArgb(50, Color.Pink);
                    circle.HatchStyle = ScottPlot.Drawing.HatchStyle.StripedUpwardDiagonal;
                }
            }

            if (IsZoneVisible && data.CurrentScan.Zone is not null)
            {
                var circle = formsPlot1.Plot.AddCircle(
                    data.CurrentScan.Zone.X,
                    data.Map.SizeY - data.CurrentScan.Zone.Y,
                    data.CurrentScan.Zone.Radius,
                    color: Color.FromArgb(0, Color.Aqua),
                    lineStyle: LineStyle.None);

                circle.Color = Color.FromArgb(50, Color.Aqua);
            }

            formsPlot1.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ZoomViewOut();
        }

        private void ZoomViewOut()
        {
            ViewScale *= 2;

            RefreshView();
        }



        private void button3_Click(object sender, EventArgs e)
        {
            ZoomViewIn();
        }

        private void ZoomViewIn()
        {
            ViewScale /= 2;

            RefreshView();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MoveViewRight();
        }

        private void MoveViewRight()
        {
            ViewX += ViewScale / 2.0d;

            RefreshView();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MoveViewDown();
        }

        private void MoveViewDown()
        {
            ViewY += ViewScale / 2.0d;

            RefreshView();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MoveViewUp();
        }

        private void MoveViewUp()
        {
            ViewY -= ViewScale / 2.0d;

            RefreshView();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MoveViewLeft();
        }

        private void MoveViewLeft()
        {
            ViewX -= ViewScale / 2.0d;

            RefreshView();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    MoveViewUp();
                    break;
                case Keys.S:
                    MoveViewDown();
                    break;
                case Keys.A:
                    MoveViewLeft();
                    break;
                case Keys.D:
                    MoveViewRight();
                    break;
                case Keys.PageUp:
                    ZoomViewOut();
                    break;
                case Keys.PageDown:
                    ZoomViewIn();
                    break;
                default:
                    break;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            IsFireRangeVisible = checkBox1.Checked;
            RefreshView();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            IsZoneVisible = checkBox2.Checked;
            RefreshView();
        }

        #endregion

        #region Global control

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var strategy = (string)comboBox2.Items[comboBox2.SelectedIndex];

            switch (strategy)
            {
                case "None":
                    MovingStrategy = new DoNothingStrategy();
                    break;
                case "Diagonaling":
                    MovingStrategy = new DiagonalingStrategy(data);
                    break;
                case "Stop":
                    MovingStrategy = new StopStrategy();
                    break;
                case "Group":
                    MovingStrategy = new GroupStrategy();
                    break;
                default:
                    break;
            }
        }

        #endregion

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MovingStrategy is not GroupStrategy groupStrategy)
            {
                return;
            }

            var direction = DirectionHelper.GetFromString(comboBox1.Text);

            groupStrategy.Direction = direction;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (MovingStrategy is not GroupStrategy groupStrategy)
            {
                return;
            }

            groupStrategy.TargetSpeed = (int)numericUpDown1.Value;
        }
    }
}