using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using System.Collections.Generic;
using System.Diagnostics;

namespace DrawShadow
{
    public struct PolygongEntity {
        public static Vector2 StartPoint = new Vector2(333, 333);
    }
    class DrawPolygon
    {
        public List<Trace> Traces;
        public SpriteBatch spriteBatch;
        public List<Polygon> Polygons;
        public List<List<Vector2>> PolyInfos;
        private List<Trace> CurrentLines;// store the information of the currentpolygon
        public List<Trace> Edge = new List<Trace>();
        public TraceType previousType = TraceType.undefined;
        public List<List<Trace>> Ploywithshadow = new List<List<Trace>>();
        public List<Trace> ClosestPoly = new List<Trace>();
        public List<Trace> PreviousClosestPoly = new List<Trace>();
        public Trace PreviousTrace;
        public List<Trace[]> Shadows = new List<Trace[]>(); // stored int the oreder of S1 E1 E2 S2
        public DrawPolygon() {
            Traces = new List<Trace>();
            Polygons = new List<Polygon>();
            PolyInfos = new List<List<Vector2>>();
        }
        public void LoadContent(SpriteBatch spriteBatch) {
            this.spriteBatch = spriteBatch;
        }
        public void AddPolygon(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4) {
            List<Vector2> Polygon = new List<Vector2>();
            Polygon.Add(p1);
            Polygon.Add(p2);
            Polygon.Add(p3);
            Polygon.Add(p4);
            PolyInfos.Add(Polygon);
            Polygons.Add(new Polygon(Polygon));
        }
        public void AddLine(Trace Line) {
            Debug.Assert(Line.mytraceType == TraceType.Line, "entered a ray when a line is needed");
            this.Traces.Add(Line);
        }
        public void Update() {
            this.InitializeUpdate();
            this.FindInterSection();
            this.UpdateShadows();
        }
        public void UpdateShadows()
        {
            float T1;
            float MinT1 = 1000;
            Vector2 StartPoint = new Vector2(0, 0);
            Vector2 EndPoint = new Vector2(0, 0);
            Vector2 StartPoint1 = new Vector2(0, 0);
            Vector2 EndPoint1 = new Vector2(0, 0);
            Trace[] sides = new Trace[2];
            for (int i = 0; i < Edge.Count/2; i++) {
                Trace Ray = Edge[2*i];
                foreach (Trace line in Ploywithshadow[2*i])
                {
                    T1 = Ray.IntersectPosition(line);
                    if (T1 < MinT1) {
                        MinT1 = T1;
                        if (IntersectDetection.CurrentT2 < 0.5)
                        {
                            StartPoint = line.StartPoint;
                        }
                        else {
                            StartPoint = line.StartPoint + line.Extend;
                        }
                    }
                    
                }
                Ray.Extend.Normalize();
                sides[0] = new Trace(TraceType.Line, StartPoint, Ray.Extend);
                T1 = 1000;
                MinT1 = 1000;
                Ray = Edge[2 * i+1];
                foreach (Trace line in Ploywithshadow[2 * i+1])
                {
                    T1 = Ray.IntersectPosition(line);
                    if (T1 < MinT1)
                    {
                        MinT1 = T1;
                        if (IntersectDetection.CurrentT2 < 0.5)
                        {
                            StartPoint1 = line.StartPoint;
                        }
                        else
                        {
                            StartPoint1 = line.StartPoint + line.Extend;
                        }
                    }

                }
                Ray.Extend.Normalize();
                sides[1] = new Trace(TraceType.Line, StartPoint1, Ray.Extend);
                this.Shadows.Add(sides);
            }
        }


        
        public void InitializeUpdate() {
            Edge = new List<Trace>();
            previousType = TraceType.undefined;
            ClosestPoly = new List<Trace>();
            Shadows = new List<Trace[]>();
        }
        public void FindInterSection() {
            float T1 = 1000;
            float T1do = 1000;
            foreach (Trace ray in Traces)
            {
                ray.Extend.Normalize();
                foreach (List<Vector2> info in PolyInfos)
                {
                    this.CurrentLines = Trace.PolyGonToLines(info);
                    foreach (Trace line in this.CurrentLines)
                    {
                        T1do = ray.IntersectPosition(line);
                        if (T1do < T1)
                        {
                            T1 = T1do;
                            this.ClosestPoly = CurrentLines;
                        }
                    }
                }
                ray.Extend *= T1;
                if (T1 < 1000)
                {
                    ray.mytraceType = TraceType.Line;
                    if (previousType == TraceType.Ray)
                    {
                        Edge.Add(ray);
                        Ploywithshadow.Add(ClosestPoly);
                        previousType = ray.mytraceType;
                    }
                    else
                    {
                        PreviousTrace = ray;
                        PreviousClosestPoly = ClosestPoly;
                    }
                }
                else
                {
                    ray.mytraceType = TraceType.Ray;
                    if (previousType == TraceType.Line)
                    {
                        Edge.Add(PreviousTrace);
                        Ploywithshadow.Add(PreviousClosestPoly);
                        previousType = ray.mytraceType;
                    }
                }
                if (previousType == TraceType.undefined)
                {
                    previousType = ray.mytraceType;
                }
                T1 = 1000;
                T1do = 1000;
            }
        }
        public void Draw()
        {
            /*
            foreach (Trace t in Traces) {
                MonoGame.Extended.ShapeExtensions.DrawLine(this.spriteBatch, t.StartPoint, Trace.EndPoint(t), Color.Red, 1);
            }
            foreach (Trace t in Edge)
            {
                MonoGame.Extended.ShapeExtensions.DrawLine(this.spriteBatch, t.StartPoint, Trace.EndPoint(t), Color.Blue, 1);
            }*/
            foreach (Polygon p in Polygons)
            {
                MonoGame.Extended.ShapeExtensions.DrawPolygon(this.spriteBatch, new Vector2(0, 0), p, Color.Black, 1);
            }
            foreach (Trace[] polygon in Shadows)
            {
                DrawShaows(polygon);
            }
        }
        public void DrawShaows(Trace[] twoside) {
            
            MonoGame.Extended.ShapeExtensions.DrawPolygon(this.spriteBatch, new Vector2(0, 0), new Polygon(new Vector2[] {
                twoside[0].StartPoint,
                twoside[0].StartPoint + twoside[0].Extend*1000,
                twoside[1].StartPoint + twoside[1].Extend*1000,
                twoside[1].StartPoint}), Color.Black, 6);
            for (int i = 0; i < 1000 / 3; i++)
            {
                MonoGame.Extended.ShapeExtensions.DrawLine(this.spriteBatch,
                        twoside[0].StartPoint + twoside[0].Extend * 3*i,
                        twoside[1].StartPoint + twoside[1].Extend * 3*i,
                         Color.Black, 3);
            }
            
        }
        
    }
}
