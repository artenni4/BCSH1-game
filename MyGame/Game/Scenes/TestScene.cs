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
using MyGame.Game.ECS.Entities;

namespace MyGame.Game.Scenes
{
    internal class TestScene : SceneBase
    {
        public TestScene(GraphicsDeviceManager graphics)
        {
            var graphicsDevice = graphics.GraphicsDevice;

            var configuration = new ConfigurationStorage();
            configuration.SetValue(ConfigurationConstants.ShowBoxColliders, true);
            configuration.SetValue(ConfigurationConstants.ShowAiDebug, true);

            var loggerFactory = LoggerFactory.Create(config =>
            {
                config.SetMinimumLevel(LogLevel.Trace);
                config.AddDebug();
            });
            
            var eventSystem = new EventSystem(loggerFactory.CreateLogger<EventSystem>());

            var player = new PlayerEntity(eventSystem);
            player.Transform.Position = new(0, 100);
            var slime1 = new SlimeEntity(eventSystem);
            slime1.Transform.Position = new(30, -70);
            var slime2 = new SlimeEntity(eventSystem);
            slime2.Transform.Position = new(100, -60);

            // add entities
            AddEntities(
                player,
                slime1,
                //slime2,
                new BushEntity(),
                new CameraEntity());

            // add systems
            AddSystems(
                new Renderer(graphicsDevice, this, configuration), 
                new AiDetectionSystem(this, eventSystem),
                new InputSystem(eventSystem),
                new CollisionSystem(this),
                new FightSystem(this, eventSystem)
                );
        }
    }
}
