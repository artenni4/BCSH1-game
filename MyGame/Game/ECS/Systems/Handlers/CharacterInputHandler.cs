using Microsoft.Xna.Framework;
using MyGame.Game.Constants;
using MyGame.Game.Constants.Enums;
using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.Helpers;
using MyGame.Game.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Game.ECS.Systems.Handlers
{
    internal class CharacterInputHandler : EcsSystem, IEventHandler
    {
        private readonly EcsEntity _playerEntity;
        private readonly StateMachine<EntityAnimationState, EntityAnimationTrigger> _playerStateMachine;

        public CharacterInputHandler(EcsEntity playerEntity)
        {
            _playerEntity = playerEntity;
            _playerStateMachine = new StateMachine<EntityAnimationState, EntityAnimationTrigger>.Builder(EntityAnimationState.IdleRight)
                .State(EntityAnimationState.IdleRight)
                    .TransitionTo(EntityAnimationState.WalkRight).On(EntityAnimationTrigger.RightPressed)
                    .TransitionTo(EntityAnimationState.WalkLeft).On(EntityAnimationTrigger.LeftPressed)
                    .TransitionTo(EntityAnimationState.GestureRight).On(EntityAnimationTrigger.GesturePressed)
                    .TransitionTo(EntityAnimationState.AttackRight).On(EntityAnimationTrigger.AttackPressed)
                    .TransitionTo(EntityAnimationState.DeathRight).On(EntityAnimationTrigger.Died)
                    .OnEnter(OnEnterHandler)
                .State(EntityAnimationState.IdleLeft)
                    .TransitionTo(EntityAnimationState.WalkRight).On(EntityAnimationTrigger.RightPressed)
                    .TransitionTo(EntityAnimationState.WalkLeft).On(EntityAnimationTrigger.LeftPressed)
                    .TransitionTo(EntityAnimationState.GestureLeft).On(EntityAnimationTrigger.GesturePressed)
                    .TransitionTo(EntityAnimationState.AttackLeft).On(EntityAnimationTrigger.AttackPressed)
                    .TransitionTo(EntityAnimationState.DeathLeft).On(EntityAnimationTrigger.Died)
                    .OnEnter(OnEnterHandler)
                .State(EntityAnimationState.WalkRight)
                    .TransitionTo(EntityAnimationState.IdleRight).On(EntityAnimationTrigger.RightReleased)
                    .OnEnter(OnEnterHandler)
                .State(EntityAnimationState.WalkLeft)
                    .TransitionTo(EntityAnimationState.IdleLeft).On(EntityAnimationTrigger.LeftReleased)
                    .OnEnter(OnEnterHandler)
                .State(EntityAnimationState.GestureRight)
                    .TransitionTo(EntityAnimationState.IdleRight).After(TimeSpan.FromSeconds(_playerEntity.GetComponent<Animation>().AnimationDuration))
                    .OnEnter(OnEnterHandler)
                .State(EntityAnimationState.GestureLeft)
                    .TransitionTo(EntityAnimationState.IdleLeft).After(TimeSpan.FromSeconds(_playerEntity.GetComponent<Animation>().AnimationDuration))
                    .OnEnter(OnEnterHandler)
                .Build();
        }

        public void OnEnterHandler(StateMachine<EntityAnimationState, EntityAnimationTrigger>.TransitionInfo transition)
        {
            var animation = _playerEntity.GetComponent<Animation>();
            (animation.State, animation.FlipHorizontally) = MapState(transition.CurrentState);
            animation.TimePlayed = TimeSpan.Zero;
        }

        private static (int, bool) MapState(EntityAnimationState enumState)
        {
            return enumState switch
            {
                EntityAnimationState.IdleLeft => (0, true),
                EntityAnimationState.IdleRight => (0, false),
                EntityAnimationState.GestureLeft => (1, true),
                EntityAnimationState.GestureRight => (1, false),
                EntityAnimationState.WalkLeft => (2, true),
                EntityAnimationState.WalkRight => (2, false),
                EntityAnimationState.AttackLeft => (3, true),
                EntityAnimationState.AttackRight => (3, false),
                EntityAnimationState.DeathLeft => (4, true),
                EntityAnimationState.DeathRight => (4, false),
                _ => (-1, default),
            };
        }

        public bool OnEvent<T>(T @event) where T : EventBase
        {
            if (@event is KeyboardEvent keyboardEvent)
            {
                var playerComponent = _playerEntity.GetComponent<Player>();
                var playerTransform = _playerEntity.GetComponent<Transform>();

                if (keyboardEvent.ReleasedKeys.Contains(InputConstants.Left))
                {
                    _playerStateMachine.Trigger(EntityAnimationTrigger.LeftReleased);
                }
                if (keyboardEvent.ReleasedKeys.Contains(InputConstants.Right))
                {
                    _playerStateMachine.Trigger(EntityAnimationTrigger.RightReleased);
                }
                if (keyboardEvent.KeyboardState.IsKeyDown(InputConstants.Left))
                {
                    _playerStateMachine.Trigger(EntityAnimationTrigger.LeftPressed);
                }
                if (keyboardEvent.KeyboardState.IsKeyDown(InputConstants.Right))
                {
                    _playerStateMachine.Trigger(EntityAnimationTrigger.RightPressed);
                }

                if (keyboardEvent.KeyboardState.IsKeyDown(InputConstants.Gesture))
                {
                    _playerStateMachine.Trigger(EntityAnimationTrigger.GesturePressed);
                }

                var input = InputHelper.GetInputAxisNormalized(keyboardEvent.KeyboardState);
                playerTransform.Position += input * playerComponent.Speed * (float)keyboardEvent.GameTime.ElapsedGameTime.TotalSeconds;

                return true;
            }

            return false;
        }

        public override void Update(GameTime gameTime, ICollection<EcsEntity> entities)
        {
            _playerStateMachine.Update(gameTime.ElapsedGameTime);
        }
    }
}
