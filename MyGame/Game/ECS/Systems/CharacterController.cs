using Microsoft.Xna.Framework;
using MyGame.Game.Constants;
using MyGame.Game.Constants.Enums;
using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.Helpers;
using MyGame.Game.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Game.ECS.Systems
{
    internal class CharacterController : EcsSystem, IEventHandler
    {
        private readonly EcsEntity _playerEntity;
        private readonly StateMachine<AnimationState, PlayerAnimationTrigger> _playerStateMachine;

        private KeyboardState _lastKeyboardState;

        public CharacterController(EcsEntity playerEntity)
        {
            _playerEntity = playerEntity;
            _playerStateMachine = new StateMachine<AnimationState, PlayerAnimationTrigger>.Builder(AnimationState.IdleRight)
                .State(AnimationState.IdleRight)
                    .TransitionTo(AnimationState.WalkRight).OnTrigger(PlayerAnimationTrigger.RightPressed)
                    .TransitionTo(AnimationState.WalkLeft).OnTrigger(PlayerAnimationTrigger.LeftPressed)
                    .TransitionTo(AnimationState.WalkUp).OnTrigger(PlayerAnimationTrigger.UpPressed)
                    .TransitionTo(AnimationState.WalkDown).OnTrigger(PlayerAnimationTrigger.DownPressed)
                    .TransitionTo(AnimationState.AttackRight).OnTrigger(PlayerAnimationTrigger.AttackPressed)
                    .TransitionTo(AnimationState.DeathRight).OnTrigger(PlayerAnimationTrigger.Died)
                .State(AnimationState.IdleLeft)
                    .TransitionTo(AnimationState.WalkRight).OnTrigger(PlayerAnimationTrigger.RightPressed)
                    .TransitionTo(AnimationState.WalkLeft).OnTrigger(PlayerAnimationTrigger.LeftPressed)
                    .TransitionTo(AnimationState.WalkUp).OnTrigger(PlayerAnimationTrigger.UpPressed)
                    .TransitionTo(AnimationState.WalkDown).OnTrigger(PlayerAnimationTrigger.DownPressed)
                    .TransitionTo(AnimationState.AttackLeft).OnTrigger(PlayerAnimationTrigger.AttackPressed)
                    .TransitionTo(AnimationState.DeathLeft).OnTrigger(PlayerAnimationTrigger.Died)
                .State(AnimationState.IdleUp)
                    .TransitionTo(AnimationState.WalkRight).OnTrigger(PlayerAnimationTrigger.RightPressed)
                    .TransitionTo(AnimationState.WalkLeft).OnTrigger(PlayerAnimationTrigger.LeftPressed)
                    .TransitionTo(AnimationState.WalkUp).OnTrigger(PlayerAnimationTrigger.UpPressed)
                    .TransitionTo(AnimationState.WalkDown).OnTrigger(PlayerAnimationTrigger.DownPressed)
                    .TransitionTo(AnimationState.AttackUp).OnTrigger(PlayerAnimationTrigger.AttackPressed)
                    .TransitionTo(AnimationState.DeathLeft).OnTrigger(PlayerAnimationTrigger.Died)
                .State(AnimationState.IdleDown)
                    .TransitionTo(AnimationState.WalkRight).OnTrigger(PlayerAnimationTrigger.RightPressed)
                    .TransitionTo(AnimationState.WalkLeft).OnTrigger(PlayerAnimationTrigger.LeftPressed)
                    .TransitionTo(AnimationState.WalkUp).OnTrigger(PlayerAnimationTrigger.UpPressed)
                    .TransitionTo(AnimationState.WalkDown).OnTrigger(PlayerAnimationTrigger.DownPressed)
                    .TransitionTo(AnimationState.AttackDown).OnTrigger(PlayerAnimationTrigger.AttackPressed)
                    .TransitionTo(AnimationState.DeathRight).OnTrigger(PlayerAnimationTrigger.Died)
                .State(AnimationState.WalkRight)
                    .TransitionTo(AnimationState.IdleRight).OnTrigger(PlayerAnimationTrigger.RightReleased)
                    .TransitionTo(AnimationState.AttackRight).OnTrigger(PlayerAnimationTrigger.AttackPressed)
                .State(AnimationState.WalkLeft)
                    .TransitionTo(AnimationState.IdleLeft).OnTrigger(PlayerAnimationTrigger.LeftReleased)
                    .TransitionTo(AnimationState.AttackLeft).OnTrigger(PlayerAnimationTrigger.AttackPressed)
                .State(AnimationState.WalkUp)
                    .TransitionTo(AnimationState.IdleUp).OnTrigger(PlayerAnimationTrigger.UpReleased)
                    .TransitionTo(AnimationState.AttackUp).OnTrigger(PlayerAnimationTrigger.AttackPressed)
                .State(AnimationState.WalkDown)
                    .TransitionTo(AnimationState.IdleDown).OnTrigger(PlayerAnimationTrigger.DownReleased)
                    .TransitionTo(AnimationState.AttackDown).OnTrigger(PlayerAnimationTrigger.AttackPressed)
                .State(AnimationState.AttackLeft)
                    .TransitionTo(AnimationState.IdleLeft)
                        .After(TimeSpan.FromSeconds(_playerEntity.GetComponent<Animation>().GetAnimationDuration(AnimationState.AttackLeft)))
                .State(AnimationState.AttackRight)
                    .TransitionTo(AnimationState.IdleRight)
                        .After(TimeSpan.FromSeconds(_playerEntity.GetComponent<Animation>().GetAnimationDuration(AnimationState.AttackRight)))
                .State(AnimationState.AttackUp)
                    .TransitionTo(AnimationState.IdleUp)
                        .After(TimeSpan.FromSeconds(_playerEntity.GetComponent<Animation>().GetAnimationDuration(AnimationState.AttackUp)))
                .State(AnimationState.AttackDown)
                    .TransitionTo(AnimationState.IdleDown)
                        .After(TimeSpan.FromSeconds(_playerEntity.GetComponent<Animation>().GetAnimationDuration(AnimationState.AttackDown)))
                .State(AnimationState.DeathLeft).DeadEnd()
                .State(AnimationState.DeathRight).DeadEnd()
                .GlobalOnEnter(OnEnterHandler)
                .Build();
        }

        private void OnEnterHandler(StateMachine<AnimationState, PlayerAnimationTrigger>.TransitionInfo transition)
        {
            var animation = _playerEntity.GetComponent<Animation>();
            animation.State = transition.CurrentState;
            animation.IsCycled = transition.CurrentState != AnimationState.DeathLeft && transition.CurrentState != AnimationState.DeathRight;
            animation.TimePlayed = TimeSpan.Zero;
        }

        public bool OnEvent<T>(T @event) where T : EventBase
        {
            bool handled = false;

            if (@event is KeyboardEvent keyboardEvent)
            {
                _lastKeyboardState = keyboardEvent.KeyboardState;
                if (keyboardEvent.ReleasedKeys.Contains(InputConstants.Left))
                {
                    handled |= _playerStateMachine.Trigger(PlayerAnimationTrigger.LeftReleased);
                }
                if (keyboardEvent.ReleasedKeys.Contains(InputConstants.Right))
                {
                    handled |= _playerStateMachine.Trigger(PlayerAnimationTrigger.RightReleased);
                }
                if (keyboardEvent.ReleasedKeys.Contains(InputConstants.Up))
                {
                    handled |= _playerStateMachine.Trigger(PlayerAnimationTrigger.UpReleased);
                }
                if (keyboardEvent.ReleasedKeys.Contains(InputConstants.Down))
                {
                    handled |= _playerStateMachine.Trigger(PlayerAnimationTrigger.DownReleased);
                }

                if (keyboardEvent.KeyboardState.IsKeyDown(InputConstants.Left))
                {
                    handled |= _playerStateMachine.Trigger(PlayerAnimationTrigger.LeftPressed);
                }
                if (keyboardEvent.KeyboardState.IsKeyDown(InputConstants.Right))
                {
                    handled |= _playerStateMachine.Trigger(PlayerAnimationTrigger.RightPressed);
                }
                if (keyboardEvent.KeyboardState.IsKeyDown(InputConstants.Up))
                {
                    handled |= _playerStateMachine.Trigger(PlayerAnimationTrigger.UpPressed);
                }
                if (keyboardEvent.KeyboardState.IsKeyDown(InputConstants.Down))
                {
                    handled |= _playerStateMachine.Trigger(PlayerAnimationTrigger.DownPressed);
                }


                // TODO remove later
                if (keyboardEvent.PressedKeys.Contains(Keys.K))
                {
                    handled |= _playerStateMachine.Trigger(PlayerAnimationTrigger.Died);
                }
            }
            else if (@event is MouseEvent mouseEvent)
            {
                if (mouseEvent.MouseState.LeftButton == ButtonState.Pressed)
                {
                    handled |= _playerStateMachine.Trigger(PlayerAnimationTrigger.AttackPressed);
                }
            }
            return handled;
        }

        public override void Update(GameTime gameTime, ICollection<EcsEntity> entities)
        {
            _playerStateMachine.Update(gameTime.ElapsedGameTime);

            var state = _playerStateMachine.State;
            if (state.IsMoving())
            {
                var playerComponent = _playerEntity.GetComponent<Player>();
                var playerTransform = _playerEntity.GetComponent<Transform>();

                var input = InputHelper.GetInputAxisNormalized(_lastKeyboardState);
                playerTransform.Position += input * playerComponent.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }
}
