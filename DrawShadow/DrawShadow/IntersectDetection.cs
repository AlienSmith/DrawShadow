using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawShadow
{
    class IntersectDetection
    {
        public static float CurrentT2;
        public static float IntersetDetection(Trace Ray, Trace Line) {
            //Debug.Assert(Ray.mytraceType == TraceType.Ray && Line.mytraceType == TraceType.Line, "Wrong Trace Type");
            
            float x = Ray.StartPoint.X;
            float y = Ray.StartPoint.Y;
            float xdo = Ray.Extend.X;
            float ydo = Ray.Extend.Y;

            float x1 = Line.StartPoint.X;
            float y1 = Line.StartPoint.Y;
            float x1do = Line.Extend.X;
            float y1do = Line.Extend.Y;

            float T2 = (y1 * xdo - y * xdo + x * ydo - x1 * ydo) / (x1do * ydo - y1do * xdo);
            if (T2 >= 1 || T2 <= 0) {
                return 1000;
            }
            float T1 = (y1+y1do*T2-y)/ydo;
            if (T1 < 0) {
                return 1000;
            }
            CurrentT2 = T2;
            //Debug.WriteLine(T1);
            return T1;
        }
    }
}
