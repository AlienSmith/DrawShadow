using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawShadow
{
    class TraceHelperClass
    {
        public static List<Trace> Range(float start, float end, float step,Vector2 Offset) {
            List<Trace> result = new List<Trace>();
            float current = start;
            Vector2 startpoint = new Vector2(0, 0);
            while (current <= end) {
                result.Add(new Trace(TraceType.Ray, startpoint, DegreeToVector(current)));
                current += step;
            }
            return result;
        }
        public static Vector2 DegreeToVector(float degree) {
            Vector2 result = new Vector2((float)Math.Cos(degree), (float)Math.Sin(degree));
            //Debug.WriteLine(result);
            return result;
        }
        
    }
}
