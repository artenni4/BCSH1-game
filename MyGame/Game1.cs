using MyGame.Game;
using MyGame.Game.ECS;
using MyGame.Game.ECS.Components;
using MyGame.Game.Scenes;

namespace MyGame
{
    public class Game1 : XnaGame
    {
        private readonly GraphicsDeviceManager _graphics;
        private TestScene scene;

        public Game1()
        {
            _graphics = new(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef,
                PreferredBackBufferWidth = 1920,
                PreferredBackBufferHeight = 1080
            };

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            scene = new(_graphics);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            scene.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                scene.Update(gameTime);
                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            if (IsActive)
            {
                scene.Draw(gameTime);
                base.Draw(gameTime);
            }
        }
    }
}
