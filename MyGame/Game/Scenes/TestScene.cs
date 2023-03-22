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
using MyGame.Game.Constants.Enums;
using MyGame.Game.StateMachine;
using MyGame.Game.Configuration;
using MyGame.Game.Constants;
using Microsoft.Extensions.Logging;
using MyGame.Game.ECS.Components.Animation;
using MyGame.Game.Animators;

namespace MyGame.Game.Scenes
{
    internal class TestScene : SceneBase
    {
        private readonly Animation playerAnimation;
        private readonly Animation slimeAnimation;

        public TestScene(GraphicsDeviceManager graphics) : base(graphics)
        {
            var graphicsDevice = graphics.GraphicsDevice;

            var configuration = new ConfigurationStorage();
            configuration.SetValue(ConfigurationConstants.ShowBoxColliders, true);
            configuration.SetValue(ConfigurationConstants.ShowAiDebug, true);

            // add entities
            EcsEntity player = new();
            var playerTransform = player.AddComponent<Transform>();
            playerTransform.Position = new Vector2(0f, 100f);

            var playerBox = player.AddComponent<BoxCollider>();
            playerBox.Box = new Rectangle(19, 23, 12, 17);

            playerAnimation = player.AddComponent<Animation>();
            playerAnimation.Animator = new PlayerAnimator();
            player.AddComponent<Player>().Speed = 100f;
            Entities.Add(player);

            EcsEntity slime = new();
            var slimeTransform = slime.AddComponent<Transform>();
            slimeTransform.Position = new Vector2(0, -70);
            slimeTransform.ZIndex = 1f;

            var slimeBox = slime.AddComponent<BoxCollider>();
            slimeBox.Box = new Rectangle(10, 13, 13, 10);

            slimeAnimation = slime.AddComponent<Animation>();
            slimeAnimation.Animator = new SlimeAnimator();

            var slimeDetector = slime.AddComponent<PlayerDetector>();
            slimeDetector.Player = player;
            slimeDetector.MaxDistanceToTarget = 100f;
            var slimeLogic = slime.AddComponent<MeleeEnemyLogic>();
            slimeLogic.StateMachine = new StateMachineBuilder<AiState>()
                .State(AiState.WalkAround)
                    .TransitionTo(AiState.ChasePlayer).OnTrigger(AiTriggers.PlayerDetected)
                .State(AiState.ChasePlayer)
                    .TransitionTo(AiState.WalkAround).OnTrigger(AiTriggers.PlayerLost)
                .BuildStateMachine(AiState.WalkAround);
            slimeLogic.Speed = 40f;
            Entities.Add(slime);


            EcsEntity camera = new();
            camera.AddComponent<Transform>();
            camera.AddComponent<TopDownCamera>();
            Entities.Add(camera);

            // add systems
            var loggerFactory = LoggerFactory.Create(config =>
            {
                config.SetMinimumLevel(LogLevel.Trace);
                config.AddDebug();
            });

            var eventSystem = new EventSystem(loggerFactory.CreateLogger<EventSystem>());
            var aiController = new AiController();
            var characterHandler = new CharacterController(player);
            eventSystem.PushHandler(aiController);
            eventSystem.PushHandler(characterHandler);

            Systems.Add(new Renderer(graphicsDevice, configuration));
            Systems.Add(new AiDetectionSystem(eventSystem));
            Systems.Add(new InputSystem(eventSystem));
            Systems.Add(aiController);
            Systems.Add(characterHandler);
        }

        public override void LoadContent(ContentManager content)
        {
            playerAnimation.Texture2D = content.Load<Texture2D>("sprites/characters/player");
            slimeAnimation.Texture2D = content.Load<Texture2D>("sprites/characters/slime1");
        }
    }
}
