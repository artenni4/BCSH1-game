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
            playerTransform.Position = new Vector2(0, 100);

            var playerBox = player.AddComponent<BoxCollider>();
            playerBox.Box = new Rectangle(19, 23, 12, 17);

            playerAnimation = player.AddComponent<Animation>();
            playerAnimation.IsPlaying = true;
            playerAnimation.IsCycled = true;
            playerAnimation.Frames = AnimationHelper.GenerateBoundsForAnimationAtlas(0, 0, 48, 48, 10, 6, 6, 6, 6, 6, 6, 4, 4, 4, 3);
            playerAnimation.MapState = state => state switch
            {
                AnimationState.None or AnimationState.IdleDown => 0,
                AnimationState.IdleRight or AnimationState.IdleLeft => 1,
                AnimationState.IdleUp => 2,
                AnimationState.WalkDown => 3,
                AnimationState.WalkRight or AnimationState.WalkLeft => 4,
                AnimationState.WalkUp => 5,
                AnimationState.AttackDown => 6,
                AnimationState.AttackRight or AnimationState.AttackLeft => 7,
                AnimationState.AttackUp => 8,
                AnimationState.DeathRight or AnimationState.DeathLeft => 9,
                _ => throw new ApplicationException("Bad animation state mapping"),
            };
            playerAnimation.Speed = 7f;
            player.AddComponent<Player>().Speed = 100f;
            Entities.Add(player);

            EcsEntity slime = new();
            var slimeTransform = slime.AddComponent<Transform>();
            slimeTransform.Position = new Vector2(0, -70);
            slimeTransform.ZIndex = 1f;

            var slimeBox = slime.AddComponent<BoxCollider>();
            slimeBox.Box = new Rectangle(10, 13, 13, 10);

            slimeAnimation = slime.AddComponent<Animation>();
            slimeAnimation.IsPlaying = true;
            slimeAnimation.IsCycled = true;
            slimeAnimation.Frames = AnimationHelper.GenerateBoundsForAnimationAtlas(0, 0, 32, 32, 5, 4, 6, 7, 3, 5);
            slimeAnimation.IsMoving = anim =>
            {
                if (anim.State == AnimationState.Walk)
                {
                    int fi = anim.GetFramesIndex();
                    return fi >= 1 && fi <= 4;
                }
                return false;
            };
            slimeAnimation.MapState = state => state switch
            {
                AnimationState.None or AnimationState.Idle => 0,
                AnimationState.Walk => 1,
                AnimationState.Attack => 2,
                AnimationState.Hurt => 3,
                AnimationState.Death => 4,
                _ => throw new ApplicationException("Bad animation state mapping"),
            };
            slimeAnimation.Speed = 7f;

            var slimeDetector = slime.AddComponent<PlayerDetector>();
            slimeDetector.Player = player;
            slimeDetector.MaxDistanceToTarget = 100f;
            var slimeLogic = slime.AddComponent<MeleeEnemyLogic>();
            slimeLogic.Speed = 40f;
            slimeLogic.StateMachine = new StateMachine<AiState, AiStateTrigger>.Builder(AiState.WalkAround)
                .State(AiState.WalkAround)
                    .TransitionTo(AiState.ChasePlayer).OnTrigger(AiStateTrigger.PlayerDetected)
                .State(AiState.ChasePlayer)
                    .TransitionTo(AiState.WalkAround).OnTrigger(AiStateTrigger.PlayerLost)
                .Build();
            Entities.Add(slime);


            EcsEntity camera = new();
            camera.AddComponent<Transform>();
            camera.AddComponent<TopDownCamera>();
            Entities.Add(camera);

            // add systems
            var eventSystem = new EventSystem();
            var aiController = new AiController();
            var characterHandler = new CharacterController(player);
            eventSystem.PushHandler(characterHandler);
            eventSystem.PushHandler(aiController);

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
