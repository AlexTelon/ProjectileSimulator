using System.Globalization;
using OttoCore;

namespace MechanicsSimulator
{
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
