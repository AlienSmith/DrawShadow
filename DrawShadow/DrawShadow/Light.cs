using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended;
namespace DrawShadow
{
    
    class Light
    {
        public int order = 0;
        public static int steps = 4;
        public static String LightMask = "LightMask";
        public static int Denominator = 64;
        public static Vector2 origin = new Vector2(32, 32);
        public Texture2D lightmask; // this the texture here
        public Vector2 position = new Vector2(0, 0);
        public Vector2 Size = new Vector2(0, 0);
        RectangleF[] dark;
        public Game1 game;
        public Light(Game1 game) {
            this.game = game;
        }
        public Vector2 GetSize() {
            return this.Size;
        }
        public static RectangleF[] drawsideOneLight(Vector2 position, Vector2 Size) {
            RectangleF[] rectangles = new RectangleF[4];
            Vector2 p1 = new Vector2(position.X - Size.X / 2,position.Y - Size.Y/2);
            Vector2 p2 = new Vector2(position.X + Size.X / 2, position.Y - Size.Y / 2);
            Vector2 p3 = new Vector2(position.X + Size.X / 2, position.Y + Size.Y / 2);
            Vector2 p4 = new Vector2(position.X - Size.X / 2, position.Y + Size.Y / 2);
            rectangles[0] = new RectangleF(0,0,p1.X,p4.Y);
            rectangles[1] = new RectangleF(p1.X,0,Game1.WINDOWSIZE.X - p1.X,p1.Y);
            rectangles[2] = new RectangleF(p2.X, p2.Y, Game1.WINDOWSIZE.X - p2.X, Game1.WINDOWSIZE.Y - p2.Y);
            rectangles[3] = new RectangleF(0, p4.Y, p2.X, Game1.WINDOWSIZE.Y - p4.Y);
            return rectangles;
        }
        public void LoadContent() {
            this.lightmask = game.Content.Load<Texture2D>(Light.LightMask);
        }
        public void update() {
            //this.dark = Light.drawsideOneLight(this.position, this.Size);
        }
        public void draw(SpriteBatch lightsprite) {
            lightsprite.Draw(texture: lightmask, origin: Light.origin, scale: this.Size/Light.Denominator, position: this.position);
            /*for (int i = 0; i < this.dark.Length; i++) {
                MonoGame.Extended.ShapeExtensions.FillRectangle(lightsprite,this.dark[i], Color.Black);
            }*/
        }       
    }
}
