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
        private ISceneManager sceneManager;
        private GraphicsDeviceManager graphics;

        public MyGame()
        {
            graphics = new GraphicsDeviceManager(this);
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = true;
            Window.AllowAltF4 = true;
            IsMouseVisible = true;
            Content.RootDirectory = PersistenceConstants.ContentRootDirectory;

            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.DisplayMode.Width;
            graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.DisplayMode.Height;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            sceneManager = new SceneManager(this);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            sceneManager.LoadScene("level1");
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
