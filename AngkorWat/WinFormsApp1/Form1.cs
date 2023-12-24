using AngkorWat.Algorithms.Strategies;
using AngkorWat.Components;
using AngkorWat.IO.HTTP;
using AngkorWat.IO.JSON;
using AngkorWat.Phases;
using ScottPlot;
using ScottPlot.Drawing.Colormaps;
using ScottPlot.Plottable;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
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
        ///public IShipStrategy FiringStrategy { get; private set; } = new FireAtWillStrategy();
        public IShipStrategy FiringStrategy { get; private set; } = new FireAtWillStrategy();


        private DataTable ShipTable { get; set; }

        public Form1()
        {
            InitializeComponent();

            // Add the FormsPlot
            formsPlot1 = new()
            {
                Dock = DockStyle.Left,
                Width = 1400,
            };

            formsPlot1.MouseClick += formsPlot1_MouseClicked;

            ShipTable = new DataTable();

            ShipTable.Columns.Add("Fire", typeof(string));
            ShipTable.Columns.Add("HP", typeof(string));
            ShipTable.Columns.Add("Speed", typeof(string));
            ShipTable.Columns.Add("Dir", typeof(string));
            ShipTable.Columns.Add("X", typeof(int));
            ShipTable.Columns.Add("Y", typeof(int));
            ShipTable.Columns.Add("Id", typeof(int));

            dataGridView1.DataSource = ShipTable;
            dataGridView1.Columns[0].Width = 30;
            dataGridView1.Columns[1].Width = 40;
            dataGridView1.Columns[2].Width = 30;
            dataGridView1.Columns[3].Width = 30;
            dataGridView1.Columns[4].Width = 30;
            dataGridView1.Columns[5].Width = 30;
            dataGridView1.Columns[6].Width = 30;

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

        private async void formsPlot1_MouseClicked(object? sender, MouseEventArgs e)
        {
            double x = formsPlot1.Plot.GetCoordinateX(e.X);
            double y = formsPlot1.Plot.GetCoordinateY(e.Y);

            int tx = (int)x;
            int ty = (int)(data.Map.SizeY - y);

            await Phase1.RequestLongScan(tx, ty);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await Phase1.LoadScan(data);
            UpdateShipTable(data);

            SetStartingView();

            RefreshView();
        }

        private void UpdateShipTable(Data data)
        {
            foreach (var ship in data.CurrentScan.MyShips)
            {
                if (TryGetTableRow(ship, out var row))
                {
                    row["Fire"] = ship.CannonCooldownLeft == 0 ? "R!" : ship.CannonCooldownLeft.ToString();
                    row["HP"] = $"{ship.HP}/{ship.MaxHP}";
                    row["Speed"] = $"{ship.Speed}/{ship.MaxSpeed}";
                    row["X"] = ship.X;
                    row["Y"] = ship.Y;
                    row["Dir"] = ship.RawDirection;
                }
                else
                {
                    row = ShipTable.NewRow();

                    row["Id"] = ship.ShipId;

                    ShipTable.Rows.Add(row);
                }
            }

            int index = 0;
            int iteration = 0;

            while (index < ShipTable.Rows.Count)
            {
                iteration++;
                var trow = ShipTable.Rows[index];

                if (!data.CurrentScan.MyShips.Any(s => s.ShipId == (int)trow["Id"]))
                {
                    ShipTable.Rows.RemoveAt(index);
                }
                else
                {
                    index++;
                }
                if (iteration > 1000)
                {
                    throw new StackOverflowException();
                }
            }
        }

        private bool TryGetTableRow(Ship ship, [MaybeNullWhen(false)][NotNullWhen(true)] out DataRow? row)
        {
            for (int i = 0; i < ShipTable.Rows.Count; i++)
            {
                var trow = ShipTable.Rows[i];

                if ((int)trow["Id"] == ship.ShipId)
                {
                    row = trow;
                    return true;
                }
            }

            row = null;
            return false;
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            await Phase1.LoadScan(data);

            UpdateShipTable(data);

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

            if (IsZoneVisible && data.CurrentScan.Zone is not null &&
                data.CurrentScan.Zone.Radius > 0)
            {
                var circle = formsPlot1.Plot.AddCircle(
                    data.CurrentScan.Zone.X,
                    data.Map.SizeY - data.CurrentScan.Zone.Y,
                    data.CurrentScan.Zone.Radius,
                    color: Color.Aqua,
                    lineStyle: LineStyle.Solid);

                ///circle.Color = Color.FromArgb(50, Color.Aqua);
            }

            label2.Text = $"Ship number: {data.CurrentScan.MyShips.Count(e => e.HP > 0)}";

            formsPlot1.Refresh();

            dataGridView1.Refresh();
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