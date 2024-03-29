﻿using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WCFReference.Objects
{
    public class Wheel : GameObject
    {
        public Wheel(World world, Texture2D tx, Vector2 size, float angle) : base(world, null, tx, new Rectangle(0, 0, (int)size.X, (int)size.Y), null, angle)
        {
            FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(size.X), ConvertUnits.ToSimUnits(size.Y), 1, Vector2.Zero, Body);
            //Body.AngularDamping = 1;
            Body.LinearDamping = 5;
            Body.CollidesWith = Category.None;
        }
    }
}
