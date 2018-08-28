using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawShadow
{
    class ShadowManager
    {
        List<DrawPolygon> lights = new List<DrawPolygon>();
        public List<Hull> hulls = new List<Hull>();
        public void AddPolygon(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            List<Vector2> Polygon = new List<Vector2>();
            Polygon.Add(p1);
            Polygon.Add(p2);
            Polygon.Add(p3);
            Polygon.Add(p4);
            this.hulls.Add(new Hull(Trace.PolyGonToLines(Polygon)));
        }
        public void addLight(float angle,Vector2 offset) {
            DrawPolygon light = new DrawPolygon();
            light.Traces = TraceHelperClass.Range(angle, MathHelper.Pi * 2, (MathHelper.Pi / 360), offset);
            this.lights.Add(light);
        }
        public void InitiateLight() {
            foreach (DrawPolygon light in lights)
            {
                light.Hulls = this.hulls;
            }
        }
        public void loadContent(SpriteBatch spriteBatch) {
            foreach (DrawPolygon light in lights) {
                light.LoadContent(spriteBatch);
            }
        }
        public void Update() {
            Vector2 MousePoint = new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y);
            
            foreach (DrawPolygon light in lights)
            {
                foreach (Trace tra in light.Traces)
                {
                    tra.StartPoint = MousePoint;
                }
                light.Update();
            }
        }
        public void Draw() {
            foreach (DrawPolygon light in lights)
            {
                light.Draw();
            }
        }
    }
}
