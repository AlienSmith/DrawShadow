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
        public DrawPolygon() {
            Traces = new List<Trace>();
            Polygons = new List<Polygon>();
            PolyInfos = new List<List<Vector2>>();
        }
        public void LoadContent(SpriteBatch spriteBatch) {
            this.spriteBatch = spriteBatch;
        }
        public void Draw() {
            foreach (Polygon p in Polygons) {
                MonoGame.Extended.ShapeExtensions.DrawPolygon(this.spriteBatch, new Vector2(0, 0), p, Color.Black, 1);
            }
            foreach (Trace t in Traces) {
                MonoGame.Extended.ShapeExtensions.DrawLine(this.spriteBatch, t.StartPoint, Trace.EndPoint(t), Color.Red, 1);
            }
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
            float T1 = 1000;
            float T1do = 1000;
            foreach (Trace ray in Traces) {
                ray.Extend.Normalize();
                foreach (List<Vector2> info in PolyInfos) {
                    this.CurrentLines = Trace.PolyGonToLines(info);
                    foreach (Trace line in this.CurrentLines)
                    {
                       T1do =  ray.IntersectPosition(line);
                        if (T1do < T1) {
                            T1 = T1do;
                        }
                    }
                }
                ray.Extend *= T1;
                ray.mytraceType = TraceType.Line;
                T1 = 1000;
                T1do = 1000;
            }
        }
        public void DrawShaow() {

        }
    }
}
