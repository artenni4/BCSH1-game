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
                    .TransitionTo(EntityAnimationState.WalkRight).OnTrigger(EntityAnimationTrigger.RightPressed)
                    .TransitionTo(EntityAnimationState.WalkLeft).OnTrigger(EntityAnimationTrigger.LeftPressed)
                    .TransitionTo(EntityAnimationState.GestureRight).OnTrigger(EntityAnimationTrigger.GesturePressed)
                    .TransitionTo(EntityAnimationState.AttackRight).OnTrigger(EntityAnimationTrigger.AttackPressed)
                    .TransitionTo(EntityAnimationState.DeathRight).OnTrigger(EntityAnimationTrigger.Died)
                .State(EntityAnimationState.IdleLeft)
                    .TransitionTo(EntityAnimationState.WalkRight).OnTrigger(EntityAnimationTrigger.RightPressed)
                    .TransitionTo(EntityAnimationState.WalkLeft).OnTrigger(EntityAnimationTrigger.LeftPressed)
                    .TransitionTo(EntityAnimationState.GestureLeft).OnTrigger(EntityAnimationTrigger.GesturePressed)
                    .TransitionTo(EntityAnimationState.AttackLeft).OnTrigger(EntityAnimationTrigger.AttackPressed)
                    .TransitionTo(EntityAnimationState.DeathLeft).OnTrigger(EntityAnimationTrigger.Died)
                .State(EntityAnimationState.WalkRight)
                    .TransitionTo(EntityAnimationState.IdleRight).OnTrigger(EntityAnimationTrigger.RightReleased)
                    .TransitionTo(EntityAnimationState.AttackRight).OnTrigger(EntityAnimationTrigger.AttackPressed)
                .State(EntityAnimationState.WalkLeft)
                    .TransitionTo(EntityAnimationState.IdleLeft).OnTrigger(EntityAnimationTrigger.LeftReleased)
                    .TransitionTo(EntityAnimationState.AttackLeft).OnTrigger(EntityAnimationTrigger.AttackPressed)
                .State(EntityAnimationState.GestureRight)
                    .TransitionTo(EntityAnimationState.IdleRight).After(TimeSpan.FromSeconds(_playerEntity.GetComponent<Animation>().AnimationDuration))
                .State(EntityAnimationState.GestureLeft)
                    .TransitionTo(EntityAnimationState.IdleLeft).After(TimeSpan.FromSeconds(_playerEntity.GetComponent<Animation>().AnimationDuration))
                .State(EntityAnimationState.AttackLeft)
                    .TransitionTo(EntityAnimationState.IdleLeft).After(TimeSpan.FromSeconds(_playerEntity.GetComponent<Animation>().AnimationDuration))
                .State(EntityAnimationState.AttackRight)
                    .TransitionTo(EntityAnimationState.IdleRight).After(TimeSpan.FromSeconds(_playerEntity.GetComponent<Animation>().AnimationDuration))
                .State(EntityAnimationState.DeathLeft).DeadEnd()
                .State(EntityAnimationState.DeathRight).DeadEnd()
                .GlobalOnEnter(OnEnterHandler)
                .GlobalOnUpdate(OnUpdateHandler)
                .Build();
        }

        private void OnEnterHandler(StateMachine<EntityAnimationState, EntityAnimationTrigger>.TransitionInfo transition)
        {
            var animation = _playerEntity.GetComponent<Animation>();
            (animation.State, animation.FlipHorizontally) = MapState(transition.CurrentState);
            animation.IsCycled = (transition.CurrentState != EntityAnimationState.DeathLeft && transition.CurrentState != EntityAnimationState.DeathRight);
            animation.TimePlayed = TimeSpan.Zero;
        }

        private void OnUpdateHandler(EntityAnimationState state, TimeSpan timeSpan)
        {
            var playerComponent = _playerEntity.GetComponent<Player>();
            var playerTransform = _playerEntity.GetComponent<Transform>();

            // TODO allow go up and down
            //var input = InputHelper.GetInputAxisNormalized(keyboardEvent.KeyboardState);
            float x = 0;
            if (state == EntityAnimationState.WalkLeft)
            {
                x = -1;
            }
            else if (state == EntityAnimationState.WalkRight)
            {
                x = 1f;
            }
            playerTransform.Position += new Vector2(x, 0) * playerComponent.Speed * (float)timeSpan.TotalSeconds;
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
                _ => throw new ApplicationException("Bad animation state mapping"),
            };
        }

        public bool OnEvent<T>(T @event) where T : EventBase
        {
            bool handled = false;

            if (@event is KeyboardEvent keyboardEvent)
            {
                if (keyboardEvent.ReleasedKeys.Contains(InputConstants.Left))
                {
                    handled |= _playerStateMachine.Trigger(EntityAnimationTrigger.LeftReleased);
                }
                if (keyboardEvent.ReleasedKeys.Contains(InputConstants.Right))
                {
                    handled |= _playerStateMachine.Trigger(EntityAnimationTrigger.RightReleased);
                }
                if (keyboardEvent.KeyboardState.IsKeyDown(InputConstants.Left))
                {
                    handled |= _playerStateMachine.Trigger(EntityAnimationTrigger.LeftPressed);
                }
                if (keyboardEvent.KeyboardState.IsKeyDown(InputConstants.Right))
                {
                    handled |= _playerStateMachine.Trigger(EntityAnimationTrigger.RightPressed);
                }

                if (keyboardEvent.KeyboardState.IsKeyDown(InputConstants.Gesture))
                {
                    handled |= _playerStateMachine.Trigger(EntityAnimationTrigger.GesturePressed);
                }

                // TODO remove later
                if (keyboardEvent.KeyboardState.IsKeyDown(Keys.K))
                {
                    handled |= _playerStateMachine.Trigger(EntityAnimationTrigger.Died);
                }
            }

            if (@event is MouseEvent mouseEvent)
            {
                if (mouseEvent.MouseState.LeftButton == ButtonState.Pressed)
                {
                    handled |= _playerStateMachine.Trigger(EntityAnimationTrigger.AttackPressed);
                }
            }
            return handled;
        }

        public override void Update(GameTime gameTime, ICollection<EcsEntity> entities)
        {
            _playerStateMachine.Update(gameTime.ElapsedGameTime);
        }
    }
}
