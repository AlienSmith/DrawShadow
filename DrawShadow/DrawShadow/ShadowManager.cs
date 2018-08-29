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
        public Game1 game;
        public Light light;
        public SpriteBatch lightSpriteBathch;
        Vector2 lightPosition = new Vector2(0, 0);
        List<DrawPolygon> lights = new List<DrawPolygon>();
        public List<Hull> hulls = new List<Hull>();
        public ShadowManager(Game1 game) {
            this.game = game;
            light = new Light(game);
            light.Size = new Vector2(1000, 1000);
            light.position = new Vector2(200, 200);
        }
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
                light.HulloffSet = Hull.count;
                light.Hulls = this.CopyHulls();            
            }
            lights[0].drawHulls = true;
        }
        public List<Hull> CopyHulls() {

            List<Hull> NEWhulls = new List<Hull>();
            foreach (Hull hull in this.hulls) {
                NEWhulls.Add(new Hull(hull.Sides));
            }
            return NEWhulls;
        }
        public void loadContent(SpriteBatch spriteBatch,SpriteBatch LspriteBatch) {
            this.lightSpriteBathch = LspriteBatch;
            foreach (DrawPolygon light in lights) {
                light.LoadContent(spriteBatch);
            }
            light.LoadContent();
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
            light.position = MousePoint;
            light.update();
        }
        public void Draw() {
            foreach (DrawPolygon light in lights)
            {
                light.Draw();
            }
        }
        public void DrawLight() {
            light.draw(lightSpriteBathch);
        }
    }
}
