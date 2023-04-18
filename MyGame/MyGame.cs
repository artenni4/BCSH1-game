using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Game;
using MyGame.Game.Constants;
using MyGame.Game.ECS;
using MyGame.Game.ECS.Components;
using MyGame.Game.Factories;
using MyGame.Game.Scenes;
using System.IO;
using System.Xml.Linq;

namespace MyGame
{
    public class MyGame : XnaGame
    {
        private readonly GraphicsDeviceManager _graphics;
        private ISceneManager sceneManager;

        public MyGame()
        {
            _graphics = new(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef,
                PreferredBackBufferWidth = 1920,
                PreferredBackBufferHeight = 1080
            };

            Content.RootDirectory = PersistenceConstants.ContentRootDirectory;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            sceneManager = new SceneManager(this);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            sceneManager.LoadScene("test-map");
        }

        protected override void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                sceneManager.Update(gameTime);
                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            if (IsActive)
            {
                sceneManager.Draw(gameTime);
                base.Draw(gameTime);
            }
        }
    }
}
