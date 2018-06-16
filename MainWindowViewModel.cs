using System.Threading;
using System.Threading.Tasks;
using OttoCore;

namespace MechanicsSimulator
{
    class MainWindowViewModel : BindableBase
    {
        public Position Position
        {
            get => Get<Position>();
            set => Set(value);
        }

        public MainWindowViewModel()
        {
            Position = new Position(0, 0);

            Position.PropertyChanged += Position_PropertyChanged;

            Task.Run(() => RunSimulation());
        }

        public void RunSimulation()
        {
            var timeStep_ms = 100;
            while (true)
            {
                Thread.Sleep(timeStep_ms);
                TimeStep(timeStep_ms);
            }
        }

        /// <summary>
        /// Time in ms
        /// </summary>
        /// <param name="timeStep"></param>
        public void TimeStep(int timeStep)
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
