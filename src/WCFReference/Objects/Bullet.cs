using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WCFReference.Objects
{
  public class Bullet : GameObject
  {
    public Bullet(World world, Texture2D tex, Vector2 position, float angle) : base(world, null, tex, null, position, angle)
    {
      TexRect = new Rectangle((int)position.X, (int)position.Y, 10, 10);
      Body = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(10), ConvertUnits.ToSimUnits(10), 1, position);
      Body.Rotation = angle;
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2? velSnap = null)
    {
      spriteBatch.Draw(Texture, destinationRectangle: TexRect, rotation: Angle);
    }
  }
}
