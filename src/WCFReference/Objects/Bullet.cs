using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WCFReference.Objects
{
  public class Bullet : GameObject, IDisposable
  {
    public Pedestrian Owner { get; set; }
    public Bullet(World world, Texture2D tex, Vector2 position, float angle, Pedestrian owner = null) : base(world, null, tex, null, position, angle)
    {
      Owner = owner;
      var bodyWidth = ConvertUnits.ToSimUnits(5);
      var bodyHeight = ConvertUnits.ToSimUnits(10);
      var bodyPosition = ConvertUnits.ToSimUnits(position);
      Body = BodyFactory.CreateRectangle(world, bodyWidth, bodyHeight, 1, bodyPosition);
      Origin = new Vector2(5, 5);
      Body.BodyType = BodyType.Dynamic;
      Body.CollisionCategories = Category.Cat2;
      Body.UserData = this;
      Body.IsBullet = true;
      Body.Mass = 0.005f;
      Body.Restitution = 0.5f;
      Body.LinearDamping = 1;
      Body.AngularDamping = 2;
      Body.Rotation = angle;
      Body.LinearVelocity = MathUtils.Mul(new Rot(angle), new Vector2(0, -10));

      Timer removeSelf = new Timer((object o) => Dispose());
      removeSelf.Change(2000, Timeout.Infinite);
    }

    public void Dispose()
    {
      Body.Dispose();
    }
  }
}
