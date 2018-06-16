using System.Threading;
using System.Threading.Tasks;
using Otto;
using OttoCore;

namespace MechanicsSimulator
{
    class MainWindowViewModel : BindableBase
    {
        public static readonly decimal GRAVACC = 9.8067m;

        public Position Position
        {
            get => Get<Position>();
            set => Set(value);
        }

        public decimal Time { get; set; }

        public decimal Init_Velocity { get; set; }

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
        }

        public MainWindowViewModel()
        {
            StartSimulationCommand = new RelayCommand<object>(OnStartSimulation);
            StopSimulationCommand = new RelayCommand<object>(OnStopSimulation);

            Position = new Position(0, 0);

            Position.PropertyChanged += Position_PropertyChanged;
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
            Position.X++;
            Position.Y++;
        }

        private void Position_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "X" || e.PropertyName == "Y")
            {
                OnPropertyChanged(nameof(Position));
            }
        }
    }

    
    class Position : BindableBase
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

        public Position(decimal x, decimal y)
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
