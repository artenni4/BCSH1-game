using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.Scenes;
using System.Linq;

namespace MyGame.Game.ECS.Systems
{
    internal class MenuSystem : EcsSystem
    {
        private readonly MyGame _game;
        
        public MenuSystem(IEntityCollection entityCollection, IEventSystem eventSystem, MyGame myGame) : base(entityCollection)
        {
            eventSystem.Subscribe<KeyboardEvent>(OnGameExit);
            _game = myGame;
        }

        private bool OnGameExit(object sender, KeyboardEvent keyboardEvent)
        {
            if (keyboardEvent.PressedKeys.Contains(Keys.Escape))
            {
                _game.Exit();
                return true;
            }
            return false;
        }
    }
}
