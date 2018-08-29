using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawShadow
{
    class ShadowManager
    {
        public static float drawingOffset = 20;
        public Game1 game;
        public List<Light> lightballs= new List<Light>();
        public SpriteBatch lightSpriteBathch;
        Vector2 lightPosition = new Vector2(0, 0);
        List<DrawPolygon> lights = new List<DrawPolygon>();
        public List<Hull> hulls = new List<Hull>();
        public List<Trace> DrakLine = new List<Trace>();
        public ShadowManager(Game1 game) {
            this.game = game;
            Light light = new Light(game);
            light.Size = new Vector2(500, 500);
            light.position = new Vector2(200, 200);
            lightballs.Add(light);
            Light light1 = new Light(game);
            light1.Size = new Vector2(1000, 1000);
            light1.position = new Vector2(100, 100);
            lightballs.Add(light1);
            Light light2 = new Light(game);
            light2.Size = new Vector2(200, 200);
            light2.position = new Vector2(700, 100);
            lightballs.Add(light2);
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
            foreach (Light light in lightballs)
            {
                light.LoadContent();
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
            this.CreateDetectLineUpdate();
            lightballs[0].position = MousePoint;
        }
        public void Draw() {
            foreach (DrawPolygon light in lights)
            {
                light.Draw();
            }
        }
        public void DrawLight() {
            foreach (Light light in lightballs)
            {
                light.draw(lightSpriteBathch);
            }
            this.Drawblack();
        }
        public void CreateDetectLineUpdate()
        {
            this.SortLight();
            int i = 0;
            Vector2 extend = new Vector2(Game1.WINDOWSIZE.X, 0);
            Vector2 start = new Vector2(0, 0);
            List<Light> currentLights = new List<Light>();
            while (i < Game1.WINDOWSIZE.Y)
            {
                i += Light.steps;
                start = new Vector2(0, i);
                foreach (Light light in lightballs) {
                    if ((light.position.Y - light.Size.Y / 2  +10) < i && i< (light.position.Y + light.Size.Y / 2 -10)) {
                        currentLights.Add(light);
                    }
                }
                if (currentLights.Count > 0) {
                    if (currentLights.Count > 1)
                    {
                        this.sort(currentLights);
                        Debug.WriteLine(currentLights[0].order + "  " + currentLights[1].order +" "+ currentLights.Count);
                    }
                    foreach (Light light in currentLights) {
                        extend = new Vector2(light.position.X - light.Size.X/2 + ShadowManager.drawingOffset - start.X, 0);
                        DrakLine.Add(new Trace(TraceType.Line, start, extend));
                        start = new Vector2(light.position.X + light.Size.X/2 - ShadowManager.drawingOffset, i);
                    }
                    extend = new Vector2(Game1.WINDOWSIZE.X - start.X, 0);
                }
                this.DrakLine.Add(new Trace(TraceType.Line,start,extend));
                extend = new Vector2(Game1.WINDOWSIZE.X, 0);
                currentLights = new List<Light>();
                start = new Vector2(0, 0);
            }
        }
        public void Drawblack() {
            foreach (Trace darklin in this.DrakLine) {
                MonoGame.Extended.ShapeExtensions.DrawLine(lightSpriteBathch, darklin.StartPoint, darklin.StartPoint + darklin.Extend, Color.Black, Light.steps);
            }
            this.DrakLine = new List<Trace>();
            
        }
        public void SortLight()
        {
            float tempfloat = 0;
            Light currentlight;
            List<Light> LightsList = new List<Light>();
            List<float> Lightdistance = new List<float>();
            foreach (Light light in lightballs)
            {
                LightsList.Add(light);
                Lightdistance.Add(light.position.X - light.Size.X / 2);
            }
            for (int m = 0; m < LightsList.Count; m++)
            {
                for (int n = m + 1; n < LightsList.Count; n++)
                {
                    if (Lightdistance[m] < Lightdistance[n])
                    {
                        tempfloat = Lightdistance[m];
                        Lightdistance[m] = Lightdistance[n];
                        Lightdistance[n] = tempfloat;
                        currentlight = LightsList[m];
                        LightsList[m] = LightsList[n];
                        LightsList[n] = currentlight;
                    }
                }
            }
            for (int i = 0; i < LightsList.Count; i++)
            {
                LightsList[i].order = i;
            }
        }
        public void sort(List<Light> lights) {
            Light currentLight;
            for (int i = 0; i < lights.Count; i++) {
                for (int l = i+1; l < lights.Count; l++) {
                    if (lights[i].order < lights[l].order) {
                        currentLight = lights[i];
                        lights[i] = lights[l];
                        lights[l] = currentLight;
                    }
                }
            }
        }
    }
}
