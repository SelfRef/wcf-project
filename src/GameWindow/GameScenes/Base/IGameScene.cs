using Microsoft.Xna.Framework;

namespace WCFReference.GameScenes.Base
{
    interface IGameScene
    {
        void Initialize();
        void LoadContent();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}
