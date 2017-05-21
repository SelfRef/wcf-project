using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

namespace WCFReference.Objects
{

    public class Map
    {
        public struct CollisionData
        {
            public int Gid { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }
        }
        public enum DrawLevel
        {
            Bottom,
            Top,
            All
        }

        public TmxMap TmxMap { get; set; } = new TmxMap("Content/Maps/map0.tmx");
        public Dictionary<Body, CollisionData> Collisions { get; private set; }

        public List<Texture2D> Tilesets;

        public static Texture2D GetDot(GraphicsDevice graphicsDevice)
        {
            Texture2D dot = new Texture2D(graphicsDevice, 1, 1);
            dot.SetData(new Color[] { Color.White });
            return dot;
        }

        public void DrawCollisions(World world = null, SpriteBatch spriteBatch = null)
        {
            Collisions = new Dictionary<Body, CollisionData>();

            foreach (var tileset in TmxMap.Tilesets)
            {
                foreach (var tile in tileset.Tiles)
                {
                    if (tile.ObjectGroups.Count > 0)
                    {
                        foreach (var layer in TmxMap.Layers)
                        {
                            foreach (var item in layer.Tiles)
                            {
                                if (item.Gid == (tile.Id + tileset.FirstGid))
                                {
                                    var obj = tile.ObjectGroups[0].Objects[0];
                                    Vector2 position = new Vector2(item.X * tileset.TileWidth + (float)obj.X + (float)obj.Width / 2, item.Y * tileset.TileHeight + (float)obj.Y + (float)obj.Height / 2);

                                    if(world != null)
                                    {
                                        Body body = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits((float)obj.Width), ConvertUnits.ToSimUnits((float)obj.Height), 1);
                                        body.BodyType = BodyType.Static;
                                        body.Position = ConvertUnits.ToSimUnits(position);
                                        Collisions.Add(body, new CollisionData() { Gid = item.Gid, Width = (float)obj.Width, Height = (float)obj.Height });
                                    }

                                    if(spriteBatch != null) if (spriteBatch != null) spriteBatch.Draw(GetDot(spriteBatch.GraphicsDevice), new Rectangle((int)position.X, (int)position.Y, (int)obj.Width, (int)obj.Height), new Color(Color.Red, 0.5f));
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DrawCollisions(SpriteBatch spriteBatch)
        {
            if (Collisions == null)
            {
                throw new NullReferenceException("Tilesets are not loaded yet.");
            }

            foreach (var body in Collisions)
            {
                spriteBatch.Draw(GetDot(spriteBatch.GraphicsDevice), new Rectangle((int)body.Key.Position.X, (int)body.Key.Position.Y, (int)body.Value.Width, (int)body.Value.Height), new Color(Color.Red, 0.5f));
            }
        }

        public void Draw(SpriteBatch spriteBatch, DrawLevel drawLevel = DrawLevel.All)
        {
            if(Tilesets == null)
            {
                throw new NullReferenceException("Tilesets are not loaded yet.");
            }
            else foreach (var layer in TmxMap.Layers.Where(i =>
            {
                if (drawLevel == DrawLevel.Bottom) return !i.Name.EndsWith("_Top");
                else if (drawLevel == DrawLevel.Top) return i.Name.EndsWith("_Top");
                else return true;
            }))
            {
                foreach (var tile in layer.Tiles)
                {
                    if (tile.Gid > 0)
                    {
                        int tsNo = 0;
                        for (int j = 0; j < TmxMap.Tilesets.Count; j++) if (TmxMap.Tilesets[j].FirstGid <= tile.Gid) tsNo = j; else break;
                        int tileWidth = TmxMap.Tilesets[tsNo].TileWidth;
                        int tileHeight = TmxMap.Tilesets[tsNo].TileHeight;
                        //List<TmxAnimationFrame> list;
                        //try
                        //{
                        //    list = map.Tilesets[tsNo].Tiles[tile.Gid - map.Tilesets[tsNo].FirstGid].AnimationFrames;
                        //}
                        //catch (ArgumentOutOfRangeException) { }
                        int col = (tile.Gid - TmxMap.Tilesets[tsNo].FirstGid) % (Tilesets[tsNo].Width / tileWidth);
                        int row = (tile.Gid - TmxMap.Tilesets[tsNo].FirstGid) / (Tilesets[tsNo].Width / tileWidth);
                        Rectangle source = new Rectangle(col * tileWidth, row * tileHeight, tileWidth, tileHeight);

                        spriteBatch.Draw(Tilesets[tsNo], new Vector2(tile.X * tileWidth, tile.Y * tileHeight), source, Color.White);
                    }
                }
            }
        }

        public void LoadTextures(ContentManager contentManager)
        {
            Tilesets = new List<Texture2D>();
            for (int i = 0; i < TmxMap.Tilesets.Count; i++)
            {
                Tilesets.Add(contentManager.Load<Texture2D>($"Maps/Assets/{TmxMap.Tilesets[i].Name}"));
            }
        }
    }
}
