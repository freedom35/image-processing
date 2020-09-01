using System;

namespace Freedom35.ImageProcessing
{
    internal struct ZoneData
    {
        public double Distance
        {
            get;
            private set;
        }

        public byte Threshold
        {
            get;
            set;
        }

        public int X
        {
            get;
            set;
        }

        public int Y
        {
            get;
            set;
        }

        public int CenterX
        {
            get;
            set;
        }

        public int CenterY
        {
            get;
            set;
        }

        public void CalculateDistance(int x, int y)
        {
            Distance = Math.Sqrt(Math.Pow(Math.Abs(x - CenterX), 2) + Math.Pow(Math.Abs(y - CenterY), 2));
        }
    }
}
