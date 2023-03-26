﻿using MyGame.Game.Animators;
using MyGame.Game.Constants;
using MyGame.Game.ECS.Components.Animation;
using MyGame.Game.ECS.Entities;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.Scenes;
using MyGame.Game.StateMachine;
using System.Linq;

namespace MyGame.Game.ECS.Systems
{
    internal class PlayerController : EcsSystem, IEventHandler
    {
        private readonly PlayerEntity _playerEntity;
        private readonly IEventSystem _eventSystem;
        private KeyboardState _lastKeyboardState;

        private GameTime _lastGameTime;

        public PlayerController(IEntityCollection entityCollection, IEventSystem eventSystem)
        {
            _playerEntity = entityCollection.GetEntityOfType<PlayerEntity>();
            _eventSystem = eventSystem;
            _eventSystem.PushHandler(this);

            _playerEntity.Animation.Animator.StateMachine.StateChanged += StateMachine_StateChanged;
        }

        private void StateMachine_StateChanged(object sender, TransitionEventArgs<AnimationNode> e)
        {
            if (IsAttackAnimation(e.CurrentState) && !IsAttackAnimation(e.PreviousState))
            {
                _eventSystem.SendEvent(this, new PlayerAttackEvent(_lastGameTime, _playerEntity, GetAttackDirection(e.CurrentState)));
            }
        }

        public bool OnEvent<T>(object sender, T @event) where T : EventBase
        {
            if (@event.EventGroup != EventGroup.InputEvent)
            {
                return false;
            }

            var animation = _playerEntity.Animation;
            var animator = animation.Animator;

            if (@event is KeyboardEvent keyboardEvent)
            {
                _lastKeyboardState = keyboardEvent.KeyboardState;
                var input = InputHelper.GetInputAxisNormalized(_lastKeyboardState);
                animation.Animator.StateMachine.SetDirectionVector(input);

                // TODO remove later
                if (keyboardEvent.PressedKeys.Contains(Keys.K))
                {
                    animator.StateMachine.SetParameter(AnimationKeys.IsDead, true);
                }

                return true;
            }
            else if (@event is MouseEvent mouseEvent)
            {
                if (mouseEvent.MouseState.LeftButton == ButtonState.Pressed)
                {
                    animator.StateMachine.SetTrigger(AnimationKeys.AttackTrigger);
                }

                return true;
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            _lastGameTime = gameTime;

            var animator = _playerEntity.Animation.Animator;
            if (IsMovable(animator.StateMachine.State))
            {
                var input = InputHelper.GetInputAxisNormalized(_lastKeyboardState);
                _playerEntity.Transform.Position += input * _playerEntity.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        private static bool IsMovable(AnimationNode state)
        {
            return state == PlayerAnimator.WalkDownNode ||
                state == PlayerAnimator.WalkUpNode ||
                state == PlayerAnimator.WalkRightNode || 
                state == PlayerAnimator.WalkLeftNode;
        }

        public static bool IsAttackAnimation(AnimationNode state)
        {
            return state == PlayerAnimator.AttackDownNode ||
                state == PlayerAnimator.AttackUpNode ||
                state == PlayerAnimator.AttackRightNode ||
                state == PlayerAnimator.AttackLeftNode;
        }

        private static PlayerAttackEvent.Direction GetAttackDirection(AnimationNode state)
        {
            if (state == PlayerAnimator.AttackDownNode)
            {
                return PlayerAttackEvent.Direction.Down;
            }
            else if (state == PlayerAnimator.AttackUpNode)
            {
                return PlayerAttackEvent.Direction.Up;
            }
            else if (state == PlayerAnimator.AttackLeftNode)
            {
                return PlayerAttackEvent.Direction.Left;
            }
            else if (state == PlayerAnimator.AttackRightNode)
            {
                return PlayerAttackEvent.Direction.Right;
            }
            throw new ArgumentException($"{nameof(state)} is not attack animation state");
        }
    }
}