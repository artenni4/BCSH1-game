using MyGame.Game.ECS.Entities;
using MyGame.Game.ECS.Systems;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.Configuration;
using Microsoft.Extensions.Logging;

namespace MyGame.Game.Scenes
{
    internal class TestScene : SceneBase
    {
        public TestScene(GraphicsDeviceManager graphics)
        {
            var graphicsDevice = graphics.GraphicsDevice;
            var configuration = new ConfigurationStorage();

            var loggerFactory = LoggerFactory.Create(config =>
            {
                config.SetMinimumLevel(LogLevel.Trace);
                config.AddDebug();
            });
            
            var eventSystem = new EventSystem(loggerFactory.CreateLogger<EventSystem>());

            // add entities
            AddEntities(
                new PlayerEntity(eventSystem, new(0, 100)),
                new SlimeEntity(eventSystem, new(30, -70), SlimeEntity.Strength.Weak),
                new SlimeEntity(eventSystem, new(100, -60), SlimeEntity.Strength.Average),
                new StumpEntity(),
                new BushEntity(),
                new CameraEntity());

            // add systems
            AddSystems(
                new Renderer(graphicsDevice, this, configuration), 
                new AiDetectionSystem(this, eventSystem),
                new InputSystem(eventSystem),
                new CollisionSystem(this),
#if DEBUG
                new DebugInputHandler(eventSystem, configuration),
#endif
                new FightSystem(this, eventSystem)
                );
        }
    }
}
