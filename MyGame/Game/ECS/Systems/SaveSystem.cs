using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Game.ECS.Systems
{
    [Serializable]
    internal class SaveSystem : EcsSystem
    {
        private readonly IEventSystem _eventSystem;
        private readonly ISceneManager _sceneManager;

        public SaveSystem(IEntityCollection entityCollection, IEventSystem eventSystem, ISceneManager sceneProvider) : base(entityCollection)
        {
            _eventSystem = eventSystem;
            _sceneManager = sceneProvider;

            _eventSystem.Subscribe<KeyboardEvent>(OnKeyboardEvent);
        }

        private bool OnKeyboardEvent(object sender, KeyboardEvent keyboardEvent)
        {
            if (keyboardEvent.ReleasedKeys.Contains(Keys.F5))
            {
                _sceneManager.SaveGame("tmp-save");
                return true;
            }
            else if (keyboardEvent.ReleasedKeys.Contains(Keys.F6))
            {
                _sceneManager.LoadGame("tmp-save");
                return true;
            }
            return false;
        }
    }
}
