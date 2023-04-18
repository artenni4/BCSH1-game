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
            Name = "TestScene";

            var graphicsDevice = graphics.GraphicsDevice;
            var configuration = new ConfigurationStorage();

            var loggerFactory = LoggerFactory.Create(config =>
            {
                config.SetMinimumLevel(LogLevel.Trace);
                config.AddDebug();
            });
            
            var eventSystem = new EventSystem(loggerFactory.CreateLogger<EventSystem>());
            var damageSystem = new DamageSystem(this, eventSystem);

            // add entities
            AddEntities(
                new PlayerEntity(eventSystem), // new(0, 100)
                new SlimeEntity(eventSystem), // new(30, -70), SlimeEntity.Strength.Weak
                new SlimeEntity(eventSystem), // new(100, -60), SlimeEntity.Strength.Average
                new StumpEntity(), //new(-100, 30)
                new BushEntity(), //new(-50, -10)
                new CameraEntity(this));

            // add systems
            AddSystems(
                new Renderer(graphicsDevice, this, configuration), 
                new AiDetectionSystem(this, eventSystem),
                new InputSystem(eventSystem, this),
                new CollisionSystem(this, eventSystem),
                new AnimationSystem(this, eventSystem),
                new VisualEffectsSystem(this, eventSystem),
                damageSystem,
#if DEBUG
                new DebugInputHandler(eventSystem, this, configuration),
#endif
                new AttackSystem(this, damageSystem, eventSystem),
                new SimplePhysicsSystem(this)
                );
        }
    }
}
