using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.ECS.Systems.EventSystem
{
    internal class EventSystem : EcsSystem
    {
        /// <summary>
        /// Stack of event handlers.
        /// Every event should be propagated throug the stack until it's handled by one of handlers
        /// </summary>
        public Stack<IEventHandler> EventHandlers { get; } = new();

        public override void Update(GameTime gameTime, IList<EcsEntity> entities)
        {
            foreach (var handler in EventHandlers)
            {
                var keyboardState = Keyboard.GetState();
                if (keyboardState.GetPressedKeyCount() > 0)
                {
                    handler.OnEvent(new KeyboardEvent(keyboardState, gameTime));
                }
            }
        }
    }
}
