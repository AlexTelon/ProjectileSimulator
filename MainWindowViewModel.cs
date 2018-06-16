using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml;
using Otto;
using OttoCore;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace MechanicsSimulator
{
    class MainWindowViewModel : BindableBase
    {
        public static readonly decimal GRAVACC = 9.8067m;
        public static readonly decimal DegToRad = (decimal)(Math.PI / 180);
        public static readonly decimal RadToDeg = (decimal)(180 / Math.PI);

        public int WorldWidth
        {
            get => Get<int>();
            set => Set(value);
        }

        public int WorldHeight
        {
            get => Get<int>();
            set => Set(value);
        }

        public ObservableCollection<HistoryPoint> History { get; set; } = new ObservableCollection<HistoryPoint>();

        public Vector2D Position
        {
            get => Get<Vector2D>();
            set => Set(value);
        }

        public decimal Time
        {
            get => Get<decimal>();
            set => Set(value);
        }

        public decimal MaxTime
        {
            get => Get<decimal>();
            set => Set(value);
        }
        public decimal SpeedUp
        {
            get => Get<decimal>();
            set => Set(value);
        }

        public Vector2D Init_Position
        {
            get => Get<Vector2D>();
            set => Set(value);
        }

        public decimal Init_Angle
        {
            get => Get<decimal>();
            set => Set(value);
        }

        public decimal Init_Velocity
        {
            get => Get<decimal>();
            set => Set(value);
        }

        private bool SimulationOn = false;

        public RelayCommand<object> StartSimulationCommand { get; set; }

        private void OnStartSimulation(object obj)
        {
            History.Clear();
            Time = 0;
            Position.X = Init_Position.X;
            Position.Y = Init_Position.Y;
            SimulationOn = true;
            RunSimulation();
        }

        public RelayCommand<object> StopSimulationCommand { get; set; }

        private void OnStopSimulation(object obj)
        {
            SimulationOn = false;
        }

        public RelayCommand<object> SaveToFileCommand { get; set; }

        private void OnSaveToFile(object obj)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(FilePath))
            {
                foreach (var item in History)
                {
                    file.WriteLine(item.ToString());
                }
            }
        }

        public RelayCommand<object> LoadFileCommand { get; set; }

        private void OnLoadFile(object obj)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Load File";
            dialog.Filter = "CSV file|*.csv";
            if (dialog.ShowDialog() ?? DialogResult.Cancel == DialogResult.OK)
            {
                History.Clear();

                using (System.IO.StreamReader file = new System.IO.StreamReader(dialog.FileName))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        var dataPoint = new HistoryPoint();

                        var data = line.Split(',');
                        dataPoint.TimeStamp = decimal.Parse(data[0], CultureInfo.InvariantCulture);
                        dataPoint.Position.X = decimal.Parse(data[1], CultureInfo.InvariantCulture);
                        dataPoint.Position.Y = decimal.Parse(data[2], CultureInfo.InvariantCulture);

                        History.Add(dataPoint);
                    }
                }
            }
        }
        

        public RelayCommand<object> OpenToFileCommand { get; set; }
        private void OnOpenFile(object obj)
        {
            Process.Start(FilePath);
        }

        public string FilePath { get; set; } = "output.csv";

        public MainWindowViewModel()
        {
            StartSimulationCommand = new RelayCommand<object>(OnStartSimulation);
            StopSimulationCommand = new RelayCommand<object>(OnStopSimulation);
            SaveToFileCommand = new RelayCommand<object>(OnSaveToFile);
            OpenToFileCommand = new RelayCommand<object>(OnOpenFile);
            LoadFileCommand = new RelayCommand<object>(OnLoadFile);


            Init_Position = new Vector2D(0, 0);

            Position = new Vector2D(Init_Position);
            Init_Velocity = 40m;
            Init_Angle = 45;

            MaxTime = 30;
            SpeedUp = 10;

            WorldHeight = 300;
            WorldWidth = 500;

            Position.PropertyChanged += Position_PropertyChanged;
            //Init_Velocity.PropertyChanged += Init_Velocity_PropertyChanged;

            StartSimulationCommand.Execute(null);
        }

        public async Task RunSimulation()
        {
            decimal timestep_s = 0.1m;
            int timestep_ms = (int) (timestep_s * 1000);

            decimal realTimeWait_ms = timestep_ms / SpeedUp;

            //TimeSpan RealtimeWait = new TimeSpan(0, 0, 0, 0, );

            while (SimulationOn)
            {
                await Task.Delay((int)realTimeWait_ms);
                Time += timestep_s;
                UpdatePosition();

                // tmp code just to stop the design view eventually
                if (Time == MaxTime || IsOutOfBounds(Position))
                {
                    SimulationOn = false;
                }
            }
        }

        public bool IsOutOfBounds(Vector2D position)
        {
            return Position.Y < 0 || Position.Y > WorldHeight
                                  || Position.X < 0
                                  || Position.X > WorldWidth;
        }

        /// <summary>
        /// Time in seconds
        /// </summary>
        public void UpdatePosition()
        {
            double angle_rad = (double) (Init_Angle * DegToRad);

            Position.X = Init_Position.X + Time * Init_Velocity * (decimal) Math.Cos(angle_rad); 
            Position.Y = Init_Position.Y + Time * Init_Velocity * (decimal) Math.Sin(angle_rad) - 0.5m * GRAVACC * Time * Time;

            History.Add(new HistoryPoint(new Vector2D(Position), Time));
        }

        private void Position_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "X" || e.PropertyName == "Y")
            {
                OnPropertyChanged(nameof(Position));
            }
        }
        private void Init_Velocity_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "X" || e.PropertyName == "Y")
            {
                OnPropertyChanged(nameof(Init_Velocity));
            }
        }
    }


    class Vector2D : BindableBase
    {
        public decimal X
        {
            get => Get<decimal>();
            set => Set(value);
        }
        public decimal Y
        {
            get => Get<decimal>();
            set => Set(value);
        }

        public Vector2D() { }

        public Vector2D(decimal x, decimal y)
        {
            X = x;
            Y = y;
        }

        public Vector2D(Vector2D original) {
            X = original.X;
            Y = original.Y;
        }

        public override string ToString()
        {
            return "" + X.ToString(CultureInfo.InvariantCulture) + ", " + Y.ToString(CultureInfo.InvariantCulture);
        }
    }

    class HistoryPoint : BindableBase
    {
        public Vector2D Position
        {
            get => Get<Vector2D>();
            set => Set(value);
        }

        public decimal TimeStamp
        {
            get => Get<decimal>();
            set => Set(value);
        }

        public HistoryPoint()
        {
            Position = new Vector2D();
        }

        public HistoryPoint(Vector2D position, decimal timeStamp)
        {
            Position = position;
            TimeStamp = timeStamp;
        }

        public override string ToString()
        {
            return TimeStamp.ToString(CultureInfo.InvariantCulture) + ", " + Position;
        }
    }
}
