using Microsoft.Xna.Framework;
using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Game.ECS.Systems.Handlers
{
    internal class CharacterInputHandler : IEventHandler
    {
        private readonly EcsEntity _playerEntity;

        public CharacterInputHandler(EcsEntity playerEntity)
        {
            _playerEntity = playerEntity;
        }

        public void OnEvent<T>(T @event) where T : EventBase
        {
            if (@event.EventGroup != EventGroup.InputEvent)
            {
                return;
            }

            if (@event is KeyboardEvent keyboardEvent)
            {
                var playerComponent = _playerEntity.GetComponent<Player>();
                var playerTransform = _playerEntity.GetComponent<Transform>();

                var input = InputHelper.GetInputAxisNormalized(keyboardEvent.KeyboardState);
                playerTransform.Position += input * playerComponent.Speed * (float)keyboardEvent.GameTime.ElapsedGameTime.TotalSeconds;

                @event.IsHandled = true;
            }
        }
    }
}
