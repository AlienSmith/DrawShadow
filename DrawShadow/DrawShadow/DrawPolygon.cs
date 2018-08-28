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
        public List<Hull> Hulls = new List<Hull>();
        public TraceType previousType = TraceType.undefined;
        public List<List<Trace>> Ploywithshadow = new List<List<Trace>>();
        public List<Trace> ClosestPoly = new List<Trace>();
        public List<Trace> PreviousClosestPoly = new List<Trace>();
        public Trace PreviousTrace;
        public List<Trace[]> Shadows = new List<Trace[]>(); // stored int the oreder of S1 E1 E2 S
        public bool backword = false;
        public bool start = true;
        public DrawPolygon() {
            Traces = new List<Trace>();
        }
        public void LoadContent(SpriteBatch spriteBatch) {
            this.spriteBatch = spriteBatch;
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
            List<Trace> Line;
            Trace tempLine;
            foreach(Hull hull in Hulls) {
                Line = new List<Trace>();
                tempLine = new Trace(TraceType.undefined);
                foreach (Trace ray in hull.Bound)
                {
                        foreach (Trace line in hull.Sides)
                        {
                            T1 = ray.IntersetDetection(line);
                            if (T1 < MinT1) {
                                MinT1 = T1;
                                tempLine = line;
                            }
                        }
                        MinT1 = 1000;
                        Line.Add(tempLine);
                }
                if (hull.Bound.Count == 1 )
                {
                   
                }
                else if (hull.Bound.Count == 2)
                {
                    if (Line[0].StartPoint == Line[1].StartPoint)
                    {
                        this.Shadows.Add(this.OnesideEdgeTrace(hull.Bound[0], Line[0]));
                    }
                    else
                    {
                        Trace[] shadow = new Trace[2];
                        Trace oneray = hull.Bound[0];
                        Trace oneline = Line[0];
                        shadow[0] = this.EdgeTrace(oneray, oneline);
                        oneray = hull.Bound[1];
                        oneline = Line[1];
                        shadow[1] = this.EdgeTrace(oneray, oneline);
                        this.Shadows.Add(shadow);

                    }
                }
                
            }
        }
        public Trace[] OnesideEdgeTrace(Trace ray,Trace line) {
            Trace[] shadow = new Trace[2];
            Vector2 start = line.StartPoint;
            Vector2 End = line.StartPoint - ray.StartPoint;
            End.Normalize();
            shadow[0] = new Trace(TraceType.Line, start, End);
            start = line.StartPoint + line.Extend;
            End = start - ray.StartPoint;
            End.Normalize();
            shadow[1] = new Trace(TraceType.Line, start, End);
            return shadow;
        }
        public Trace EdgeTrace(Trace ray,Trace line) {
            Vector2 StartPoint;
            if (ray.Extend.Dot(line.Extend) > 0)
            {
                StartPoint =  line.StartPoint + line.Extend;
            }
            else {
                StartPoint = line.StartPoint;
            }
            Vector2 extend = StartPoint - ray.StartPoint;
            extend.Normalize();
            return new Trace(TraceType.Line, StartPoint, extend);
        }

        public void InitializeHull() {
            Debug.WriteLine(this.Hulls.Count + "Hulls in the shadow");
        }
        public void InitializeUpdate() {
            previousType = TraceType.undefined;
            ClosestPoly = new List<Trace>();
            Shadows = new List<Trace[]>();
            Ploywithshadow = new List<List<Trace>>();
            this.backword = false;
            this.start = true;
            foreach (Hull h in Hulls) {
                h.Update();
            }
        }
        public void FindInterSection() {
            float T1 = 1000;
            float T1do = 1000;
            int HullId = -1;
            int currentId = -1;
            int previousId = -1;
            foreach (Trace ray in Traces)
            {
                ray.Extend.Normalize();
                foreach (Hull hull in Hulls)
                {
                    HullId = hull.id;
                    foreach (Trace line in hull.Sides)
                    {
                        T1do = ray.IntersetDetection(line);
                        if (T1do < T1)
                        {
                            T1 = T1do;
                            currentId = HullId;
                        }
                    }

                }
                ray.Extend *= T1;
                if (T1 < 1000)
                {
                    ray.mytraceType = TraceType.Line;
                    if (previousType == TraceType.Ray)
                    {
                        Hulls[currentId].Bound.Add(ray);
                        previousType = ray.mytraceType;
                        PreviousTrace = ray;
                        previousId = currentId;
                    }
                    else
                    {
                        if (previousId != currentId) {
                            Hulls[currentId].Bound.Add(ray);
                            Hulls[previousId].Bound.Add(PreviousTrace);
                        }
                        PreviousTrace = ray;                          
                        previousId = currentId;
                    }
                    
                }
                else
                {
                    ray.mytraceType = TraceType.Ray;
                    if (previousType == TraceType.Line)
                    {
                        Hulls[previousId].Bound.Add(PreviousTrace);
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
            
            foreach (Trace t in Traces) {
                MonoGame.Extended.ShapeExtensions.DrawLine(this.spriteBatch, t.StartPoint, Trace.EndPoint(t), Color.Red, 1);
            }
            foreach (Hull h in Hulls)
            {
                this.DrawHull(h);
                foreach (Trace t in h.Bound)
                {
                    MonoGame.Extended.ShapeExtensions.DrawLine(this.spriteBatch, t.StartPoint, Trace.EndPoint(t), Color.Green, 1);
                }
            }
            foreach (Trace[] polygon in Shadows)
            {
                DrawShaows(polygon);
            }
           
            // Debug.WriteLine(Shadows.Count);
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
        public void DrawHull(Hull hull) {
            Polygon poly = new Polygon(Trace.HullTOpoint(hull));
            MonoGame.Extended.ShapeExtensions.DrawPolygon(this.spriteBatch, new Vector2(0, 0), poly, Color.Black, 1);
        }
        
    }
}
