using System;
using System.Timers;
using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WCFReference.Objects
{
    public class Pedestrian : GameObject
    {
        Timer sprites;
        TileSet tileSet;
        int sprite = 0;
        public Pedestrian(World world, float radius, TileSet tileset, Vector2 position, float angle) : base(world, BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(radius), 1), tileset?.Texture, null, position, angle)
        {
            tileSet = tileset;
            Body.LinearDamping = 10;
            Body.AngularDamping = 20;
            sprites = new Timer(300);
            sprites.Elapsed += (s, e) =>
            {
                if (sprite == 0) sprite = 1;
                else if (sprite == 1) sprite = 2;
                else if (sprite == 2) sprite = 1;
            };
        }

        public override void Update(Controll ctrl)
        {
            Body.ApplyForce(MathUtils.Mul(new Rot(Angle), new Vector2(0, (ctrl.Front ? -0.5f : 0) + (ctrl.Back ? 0.2f : 0))));
            Body.ApplyTorque((ctrl.Left ? -0.02f : 0) + (ctrl.Right ? 0.02f : 0));
        }

        public void Update(Controll ctrl, float angle)
        {
            Angle = angle;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2? velSnap = null)
        {
            Vector2 veloc = velSnap ?? Body.LinearVelocity;
            float vel = MathUtils.MulT(new Rot(Angle), veloc).Y;
            if (Math.Abs(vel) >= 0.01f && sprites.Enabled == false) sprites.Start();
            else if (Math.Abs(vel) < 0.01f && sprites.Enabled == true) { sprites.Stop(); sprite = 0; }
            spriteBatch.Draw(Texture, Position, null, tileSet.Rectangle(sprite, 0), tileSet.Origin, Angle, new Vector2(0.5f));
        }
    }
}
