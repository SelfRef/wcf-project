using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WCFReference.Objects
{
  public class Bullet : GameObject
  {
    public Bullet(World world, Texture2D tex = null, Vector2? position = null, float? angle = null) : base(world, null, tex, null, position, angle)
    {
      Body = new Body(world, position, angle.Value);
      TexRect = new Rectangle((int)position.Value.X, (int)position.Value.Y, 10, 10);
    }
  }
}
