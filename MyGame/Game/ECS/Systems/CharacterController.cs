using MyGame.Game.Animators;
using MyGame.Game.Constants;
using MyGame.Game.ECS.Components.Animation;
using MyGame.Game.ECS.Entities;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.Scenes;
using System.Linq;

namespace MyGame.Game.ECS.Systems
{
    internal class CharacterController : EcsSystem, IEventHandler
    {
        private readonly PlayerEntity _playerEntity;
        private readonly IEventSystem _eventSystem;
        private KeyboardState _lastKeyboardState;

        public CharacterController(IEntityCollection entityCollection, IEventSystem eventSystem)
        {
            _playerEntity = entityCollection.GetEntityOfType<PlayerEntity>();
            _eventSystem = eventSystem;
            _eventSystem.PushHandler(this);
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
                animation.Animator.SetDirectionVector(input);

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
                    _eventSystem.SendEvent(this, new PlayerAttackEvent(mouseEvent.GameTime, _playerEntity));
                }

                return true;
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            var animator = _playerEntity.Animation.Animator;
            if (IsMovable(animator))
            {
                var input = InputHelper.GetInputAxisNormalized(_lastKeyboardState);
                _playerEntity.Transform.Position += input * _playerEntity.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        private static bool IsMovable(IAnimator animator)
        {
            var animState = (PlayerAnimator.PlayerAnimation)animator.StateMachine.State.AnimationState;
            return animState == PlayerAnimator.PlayerAnimation.WalkDown || 
                animState == PlayerAnimator.PlayerAnimation.WalkRight || 
                animState == PlayerAnimator.PlayerAnimation.WalkUp;

        }
    }
}
