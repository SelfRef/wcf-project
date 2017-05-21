using System;
using System.Collections.Generic;
using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WCFReference.ServerObjects;

namespace WCFReference.Objects
{
    public class Car : GameObject, IPlayable
    {
        public enum Type
        {
            Type1,
            Type2,
            Type3,
            Type4
        }
        public enum CarColor
        {
            Color1,
            Colro2,
            Color3
        }
        public List<Wheel> Wheels;

        public bool CanEnter { get; set; } = true;
        public ServerObject Inside { get; set; } = null;
        public float Speed { get; set; } = 1.2f;

        public bool mouse = false;
        private float curve = 0;
        private float curveLast = 0;
        private float curveSpeed = 0.05f;

        public Car(World world, Texture2D tex, Vector2 position, float angle, Type? type, CarColor? color, Texture2D wheelTex, Vector2? wheelSize) : base(world, null, tex, null, position, angle)
        {
            int start;
            int width;
            switch (type)
            {
                case Type.Type1:
                    start = 0;
                    width = 49;
                    break;
                case Type.Type2:
                    start = 170;
                    width = 47;
                    break;
                case Type.Type3:
                    start = 290;
                    width = 54;
                    break;
                case Type.Type4:
                    start = 452;
                    width = 40;
                    break;
                default:
                    start = 0;
                    width = 49;
                    break;
            }

            if (tex != null)
            {
                TexRect = TileSet.GetRect(tex, 3, 1, (int)color, 0, new Rectangle(start, 0, width * 3, 100));

                uint[] texData = new uint[(int)Size.X * (int)Size.Y];
                Texture.GetData(0, TexRect, texData, 0, texData.Length);
                Vertices vert = PolygonTools.CreatePolygon(texData, 49);
                vert.Scale(ConvertUnits.ToSimUnits(new Vector2(0.96f)));
                List<Vertices> vertList = Triangulate.ConvexPartition(vert, TriangulationAlgorithm.Bayazit);

                FixtureFactory.AttachCompoundPolygon(vertList, 1, Body); 
            }

            Wheels = new List<Wheel>(4)
            {
                new Wheel(world, wheelTex, wheelSize ?? new Vector2(7, 15), angle),
                new Wheel(world, wheelTex, wheelSize ?? new Vector2(7, 15), angle),
                new Wheel(world, wheelTex, wheelSize ?? new Vector2(7, 15), angle),
                new Wheel(world, wheelTex, wheelSize ?? new Vector2(7, 15), angle)
            };
            JointFactory.CreateRevoluteJoint(world, Body, Wheels[0].Body, ConvertUnits.ToSimUnits(new Vector2(3, 20)), ConvertUnits.ToSimUnits(Wheels[0].Origin));
            JointFactory.CreateRevoluteJoint(world, Body, Wheels[1].Body, ConvertUnits.ToSimUnits(new Vector2(Size.X-3, 20)), ConvertUnits.ToSimUnits(Wheels[1].Origin));

            JointFactory.CreateWeldJoint(world, Body, Wheels[2].Body, ConvertUnits.ToSimUnits(new Vector2(3, Size.Y - 17)), ConvertUnits.ToSimUnits(Wheels[2].Origin));
            JointFactory.CreateWeldJoint(world, Body, Wheels[3].Body, ConvertUnits.ToSimUnits(new Vector2(Size.X-3, Size.Y - 17)), ConvertUnits.ToSimUnits(Wheels[3].Origin));

            Origin = new Vector2(Origin.X, 20);
            Body.Restitution = 0.2f;
            //Body.Mass = 1500;
            //Body.LinearDamping = 1f;
            //Body.AngularDamping = 10;
        }

        public override void Update(Controll ctrl)
        {
            if(ctrl.Front)
            {
                Wheels[0].Body.ApplyForce(MathUtils.Mul(new Rot(Wheels[0].Angle), new Vector2(0, -Speed)), ConvertUnits.ToSimUnits(Wheels[0].Position));
                Wheels[1].Body.ApplyForce(MathUtils.Mul(new Rot(Wheels[1].Angle), new Vector2(0, -Speed)), ConvertUnits.ToSimUnits(Wheels[1].Position));
            }
            else if(ctrl.Back)
            {
                Wheels[0].Body.ApplyForce(MathUtils.Mul(new Rot(Wheels[0].Angle), new Vector2(0, Speed)), ConvertUnits.ToSimUnits(Wheels[0].Position));
                Wheels[1].Body.ApplyForce(MathUtils.Mul(new Rot(Wheels[1].Angle), new Vector2(0, Speed)), ConvertUnits.ToSimUnits(Wheels[1].Position));
            }

            if(ctrl.Brake) foreach (var i in Wheels)
            {
                Vector2 tmp = MathUtils.MulT(new Rot(i.Angle), i.Body.LinearVelocity);
                tmp.Y -= tmp.Y * 0.1f;
                i.Body.LinearVelocity = MathUtils.Mul(new Rot(i.Angle), tmp);
            }

            if (ctrl.AnyMove) mouse = false;
            if (!mouse)
            {
                if (ctrl.Right) { if (curve < 1) curve += curveSpeed; }
                else if (ctrl.Left) { if (curve > -1) curve -= curveSpeed; }
                else if (curve != 0) curve -= curveSpeed * Math.Sign(curve);
            }
            Wheels[0].Angle = Angle + curve;
            Wheels[1].Angle = Angle + curve;

            if (ctrl.Front) Wheels[0].Body.LinearVelocity = MathUtils.Mul(new Rot(curve), Wheels[0].Body.LinearVelocity);
            else if (ctrl.Back) Wheels[0].Body.LinearVelocity = MathUtils.Mul(new Rot(-curve), Wheels[0].Body.LinearVelocity);
            if (ctrl.Front) Wheels[1].Body.LinearVelocity = MathUtils.Mul(new Rot(curve), Wheels[1].Body.LinearVelocity);
            else if (ctrl.Back) Wheels[1].Body.LinearVelocity = MathUtils.Mul(new Rot(-curve), Wheels[1].Body.LinearVelocity);

            curveLast = curve;

            foreach (var i in Wheels)
            {
                i.Body.LinearDamping = 5;

                Vector2 tmp = MathUtils.MulT(new Rot(i.Angle), i.Body.LinearVelocity);
                tmp.Y += Math.Abs(tmp.X * 0.2f) * Math.Sign(tmp.Y);
                tmp.X -= tmp.X * 0.5f;
                i.Body.LinearVelocity = MathUtils.Mul(new Rot(i.Angle), tmp);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2? velSnap = null)
        {
            foreach (var i in Wheels)
            {
                spriteBatch.Draw(i.Texture, null, new Rectangle(i.Position.ToPoint(), i.Size.ToPoint()), null, new Vector2(0.5f, 0.5f), i.Angle, null, Color.Black);
            }

            spriteBatch.Draw(Texture, Position, null, TexRect, Origin, Angle);
        }
    }
}
