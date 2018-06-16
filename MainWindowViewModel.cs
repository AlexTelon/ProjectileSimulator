using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Otto;
using OttoCore;

namespace MechanicsSimulator
{
    class MainWindowViewModel : BindableBase
    {
        public static readonly decimal GRAVACC = 9.8067m;

        public Vector2D Position
        {
            get => Get<Vector2D>();
            set => Set(value);
        }

        public decimal Time { get; set; }

        public decimal Init_Angle { get; set; }
        public Vector2D Init_Velocity { get; set; }

        private bool SimulationOn = false;

        public RelayCommand<object> StartSimulationCommand { get; set; }

        private void OnStartSimulation(object obj)
        {
            Time = 0;
            SimulationOn = true;
            Task.Run(() => RunSimulation());
        }

        public RelayCommand<object> StopSimulationCommand { get; set; }

        private void OnStopSimulation(object obj)
        {
            SimulationOn = false;
            Position.X = 0;
            Position.Y = 0;
        }

        public MainWindowViewModel()
        {
            StartSimulationCommand = new RelayCommand<object>(OnStartSimulation);
            StopSimulationCommand = new RelayCommand<object>(OnStopSimulation);

            Position = new Vector2D(0, 0);
            Init_Velocity = new Vector2D(10, 10);

            Position.PropertyChanged += Position_PropertyChanged;
            Init_Velocity.PropertyChanged += Init_Velocity_PropertyChanged; ;
        }

        public void RunSimulation()
        {
            decimal timestep_s = 0.1m;
            int timestep_ms = (int) (timestep_s * 1000);
            while (SimulationOn)
            {
                Thread.Sleep(timestep_ms);
                TimeStep(timestep_s);
            }
        }

        /// <summary>
        /// Time in seconds
        /// </summary>
        /// <param name="timeStep"></param>
        public void TimeStep(decimal timeStep)
        {
            Position.X += timeStep * Init_Velocity.X;
            Position.Y += timeStep * Init_Velocity.Y;
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

        public override string ToString()
        {
            return "" + X + ", " + Y;
        }
    }
}
