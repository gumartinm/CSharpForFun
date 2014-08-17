using System;

namespace jsonparser.currentweather
{
    public class Rain
    {
        private double? threeHours;

        public void set3h(double three)
        {
            this.threeHours = three;
        }

        public double? get3h()
        {
            return this.threeHours;
        }
    }
}

