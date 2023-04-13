using Microsoft.Extensions.DependencyInjection;
using MyGame.Game;
using MyGame.Game.ECS;
using MyGame.Game.ECS.Components;
using MyGame.Game.Factories;
using MyGame.Game.Scenes;
using System.IO;
using System.Xml.Linq;

namespace MyGame
{
    public class Game1 : XnaGame
    {
        private readonly GraphicsDeviceManager _graphics;
        private SceneBase scene;

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
            var serviceProvider = new ServiceCollection().AddGameServices(_graphics.GraphicsDevice).BuildServiceProvider();
            var sceneFactory = new SceneFactory(serviceProvider.GetService<IServiceScopeFactory>());
            var map = XDocument.Load("Content/maps/test-map.xml");
            scene = sceneFactory.CreateScene(map);
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
