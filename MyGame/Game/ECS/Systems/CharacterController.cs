using MyGame.Game.Constants;
using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Components.Animation;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using System.Linq;

namespace MyGame.Game.ECS.Systems
{
    

    internal class CharacterController : EcsSystem, IEventHandler
    {
        private readonly EcsEntity _playerEntity;
        private KeyboardState _lastKeyboardState;

        public CharacterController(EcsEntity playerEntity)
        {
            _playerEntity = playerEntity;
        }

        public bool OnEvent<T>(object sender, T @event) where T : EventBase
        {
            if (@event.EventGroup != EventGroup.InputEvent || !_playerEntity.TryGetComponent<Animation>(out var animation))
            {
                return false;
            }

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
                }

                return true;
            }
            return false;
        }

        public override void Update(GameTime gameTime, ICollection<EcsEntity> entities)
        {
            var animator = _playerEntity.TryGetAnimator(out var _);
            if (animator.GetFlag(AnimationFlags.IsMovable, true))
            {
                var playerComponent = _playerEntity.GetComponent<Player>();
                var playerTransform = _playerEntity.GetComponent<Transform>();

                var input = InputHelper.GetInputAxisNormalized(_lastKeyboardState);
                playerTransform.Position += input * playerComponent.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }
}
