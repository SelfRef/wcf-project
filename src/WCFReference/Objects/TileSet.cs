using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WCFReference.Objects
{
    public class TileSet
    {
        public Texture2D Texture { get; set; }
        public Rectangle Slice { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }
        public Vector2? Origin { get; set; }

        public TileSet(Texture2D texture, int columns, int rows, Vector2? origin = null, Rectangle? slice = null)
        {
            Texture = texture;
            Columns = columns;
            Rows = rows;
            Origin = origin;
            Slice = slice ?? new Rectangle(0, 0, Texture.Width, Texture.Height);
        }
        public Rectangle Rectangle(int column, int row)
        {
            return GetRect(Texture, Columns, Rows, column, row, Slice);
        }
        public static Rectangle GetRect(Texture2D texture, int columns, int rows, int column, int row, Rectangle? slice = null)
        {
            Rectangle rect = slice ?? new Rectangle(0, 0, texture.Width, texture.Height);
            return new Rectangle(rect.Width / columns * column, rect.Height / rows * row, rect.Width / columns, rect.Height / rows);
        }
    }
}
