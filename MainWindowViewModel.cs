using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Otto;
using OttoCore;

namespace MechanicsSimulator
{
    class MainWindowViewModel : BindableBase
    {
        public static readonly decimal GRAVACC = 9.8067m;
        public static readonly decimal DegToRad = (decimal)(Math.PI / 180);
        public static readonly decimal RadToDeg = (decimal)(180 / Math.PI);

        public ObservableCollection<Vector2D> History { get; set; } = new ObservableCollection<Vector2D>();

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

        public Vector2D Init_Velocity
        {
            get => Get<Vector2D>();
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

        public MainWindowViewModel()
        {
            StartSimulationCommand = new RelayCommand<object>(OnStartSimulation);
            StopSimulationCommand = new RelayCommand<object>(OnStopSimulation);

            Init_Position = new Vector2D(0, 50);

            Position = new Vector2D(Init_Position);
            Init_Velocity = new Vector2D(10, 10);
            Init_Angle = 45;

            MaxTime = 10;
            SpeedUp = 3;

            Position.PropertyChanged += Position_PropertyChanged;
            Init_Velocity.PropertyChanged += Init_Velocity_PropertyChanged;

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
                if (Time == MaxTime)
                {
                    SimulationOn = false;
                }
            }
        }

        /// <summary>
        /// Time in seconds
        /// </summary>
        public void UpdatePosition()
        {
            double angle_rad = (double) (Init_Angle * DegToRad);

            Position.X = Init_Position.X + Time * Init_Velocity.X * (decimal) Math.Cos(angle_rad); 
            Position.Y = Init_Position.Y + Time * Init_Velocity.Y * (decimal) Math.Sin(angle_rad) - 0.5m * GRAVACC * Time * Time;

            History.Add(new Vector2D(Position));
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
            return "" + X + ", " + Y;
        }
    }
}
