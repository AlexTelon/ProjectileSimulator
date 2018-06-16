using System.Globalization;
using OttoCore;

namespace MechanicsSimulator
{
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

        public Vector2D(Vector2D original)
        {
            X = original.X;
            Y = original.Y;
        }

        public override string ToString()
        {
            return "" + X.ToString(CultureInfo.InvariantCulture) + ", " + Y.ToString(CultureInfo.InvariantCulture);
        }
    }
}
