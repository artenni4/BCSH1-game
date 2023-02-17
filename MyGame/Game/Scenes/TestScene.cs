using MyGame.Game.ECS.Components;
using MyGame.Game.ECS;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using Microsoft.Xna.Framework.Content;
using MyGame.Game.ECS.Systems;
using MyGame.Game.Helpers;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.Handlers;

namespace MyGame.Game.Scenes
{
    internal class TestScene : SceneBase
    {
        private Animation entityAnimation;
        //private Image entityTexture;

        public TestScene(GraphicsDeviceManager graphics) : base(graphics)
        {
            var graphicsDevice = graphics.GraphicsDevice;

            // add entities
            EcsEntity entity = new();
            var entityTransfrom = entity.AddComponent<Transform>();
            entityTransfrom.Position = new Vector2(0, 100);
            entityTransfrom.Scale = 10f;

            entityAnimation = entity.AddComponent<Animation>();
            entityAnimation.PreviousStart = TimeSpan.Zero;
            entityAnimation.IsCycled = true;
            entityAnimation.Frames = AnimationHelper.GenerateBoundsForAtlasHorizontal(0, 0, 32, 32, 10);
            entityAnimation.Speed = 5f;
            //entityTexture = entity.AddComponent<Image>();
            entity.AddComponent<Player>().Speed = 500f;
            Entities.Add(entity);

            EcsEntity camera = new();
            camera.AddComponent<Transform>();
            camera.AddComponent<TopDownCamera>();
            Entities.Add(camera);

            // add systems
            Systems.Add(new Renderer(graphicsDevice));

            var eventSystem = new EventSystem();
            eventSystem.PushHandler(new CharacterInputHandler(entity));
            Systems.Add(eventSystem);
        }

        public override void LoadContent(ContentManager content)
        {
            //entityTexture.Texture2D = content.Load<Texture2D>("ball");
            entityAnimation.Texture2D = content.Load<Texture2D>("warrior");
        }
    }
}
