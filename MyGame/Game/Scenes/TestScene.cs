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
        private readonly Animation entityAnimation;
        private readonly Image followerTexture;

        public TestScene(GraphicsDeviceManager graphics) : base(graphics)
        {
            var graphicsDevice = graphics.GraphicsDevice;

            // add entities
            EcsEntity entity = new();
            var entityTransfrom = entity.AddComponent<Transform>();
            entityTransfrom.Position = new Vector2(0, 100);
            entityTransfrom.Scale = 5f;

            entityAnimation = entity.AddComponent<Animation>();
            entityAnimation.IsPlaying = true;
            entityAnimation.IsCycled = true;
            entityAnimation.Frames = AnimationHelper.GenerateBoundsForAnimationAtlas(0, 0, 32, 32, 10, 5); //AnimationHelper.GenerateBoundsForAtlasHorizontal(0, 0, 32, 32, 10);
            entityAnimation.Speed = 7f;
            //entityTexture = entity.AddComponent<Image>();
            entity.AddComponent<Player>().Speed = 200f;
            Entities.Add(entity);

            EcsEntity follower = new();
            var followerTransform = follower.AddComponent<Transform>();
            followerTransform.Position = new Vector2(0, -100);
            followerTransform.Scale = 0.7f;

            followerTexture = follower.AddComponent<Image>();
            var playerFollower = follower.AddComponent<FollowPlayer>();
            playerFollower.Player = entity;
            playerFollower.MinDistanceToTarget = 150f;
            playerFollower.Speed = 150f;
            Entities.Add(follower);

            EcsEntity camera = new();
            camera.AddComponent<Transform>();
            camera.AddComponent<TopDownCamera>();
            Entities.Add(camera);

            // add systems
            Systems.Add(new Renderer(graphicsDevice));
            Systems.Add(new AiController());

            var eventSystem = new EventSystem();
            var characterHandler = new CharacterInputHandler(entity);
            eventSystem.PushHandler(characterHandler);
            Systems.Add(eventSystem);
            Systems.Add(characterHandler);
        }

        public override void LoadContent(ContentManager content)
        {
            followerTexture.Texture2D = content.Load<Texture2D>("ball");
            entityAnimation.Texture2D = content.Load<Texture2D>("warrior");
        }
    }
}
