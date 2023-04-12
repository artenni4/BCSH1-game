using MyGame.Game.Configuration;
using MyGame.Game.Constants;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Game.ECS.Systems
{
    internal class DebugInputHandler : EcsSystem, IEventHandler
    {
        private readonly IEventSystem _eventSystem;
        private readonly IConfiguration _configuration;

        public DebugInputHandler(IEventSystem eventSystem, IConfiguration configuration)
        {
            _eventSystem = eventSystem;
            _configuration = configuration;

            _eventSystem.PushHandler(this);
        }

        public bool OnEvent<T>(object sender, T @event) where T : EventBase
        {
            if (@event is KeyboardEvent keyboardEvent)
            {
                if (keyboardEvent.PressedKeys.Contains(Keys.B))
                {
                    var showBoxColliders = _configuration.GetValue<bool>(ConfigurationConstants.ShowBoxColliders);
                    _configuration.SetValue(ConfigurationConstants.ShowBoxColliders, !showBoxColliders);
                    return true;
                }
                if (keyboardEvent.PressedKeys.Contains(Keys.I))
                {
                    var showAiDebug = _configuration.GetValue<bool>(ConfigurationConstants.ShowAiDebug);
                    _configuration.SetValue(ConfigurationConstants.ShowAiDebug, !showAiDebug);
                    return true;
                }
            }
            return false;
        }
    }
}
