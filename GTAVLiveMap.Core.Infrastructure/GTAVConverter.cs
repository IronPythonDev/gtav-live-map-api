using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure
{
    public class LatLng
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Vector3
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
    }

    public static class GTAVConverter
    {
        public static double MapCenterX = 3753;
        public static double MapCenterY = 5527;

        public static LatLng GetLatLngFromVector2(Vector3 vector)
        {
            double x = (vector.x / 1.515) * -1;
            double y = vector.y / 1.52;

            return new LatLng
            {
                lat = -(MapCenterY - y),
                lng = MapCenterX - x
            };
        }
    }
}
