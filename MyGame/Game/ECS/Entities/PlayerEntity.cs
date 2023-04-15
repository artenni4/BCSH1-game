﻿using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Content;
using MyGame.Game.Animators;
using MyGame.Game.Constants;
using MyGame.Game.ECS.Components;
using MyGame.Game.ECS.Components.Animation;
using MyGame.Game.ECS.Components.Collider;
using MyGame.Game.ECS.Systems.EventSystem;
using MyGame.Game.ECS.Systems.EventSystem.Events;
using MyGame.Game.StateMachine;
using System.Diagnostics;
using System.Linq;

namespace MyGame.Game.ECS.Entities
{
    internal class PlayerEntity : EcsEntity
    {
        public const float AttackDamage = 20f;
        public const float AttackRadius = 30f;

        public float Speed { get; set; } = 100f;

        public Transform Transform { get; }
        public BoxCollider BoxCollider { get; }
        public Animation Animation { get; }
        public EntityHealth EntityHealth { get; }

        private readonly IEventSystem _eventSystem;
        private KeyboardState _lastKeyboardState;
        private GameTime _lastGameTime;

        public PlayerEntity(IEventSystem eventSystem)
        {
            Transform = AddComponent<Transform>();

            BoxCollider = AddComponent<BoxCollider>();
            BoxCollider.Box = new Rectangle(17, 22, 15, 19);

            Animation = AddComponent<Animation>();
            Animation.Animator = new PlayerAnimator();

            EntityHealth = AddComponent<EntityHealth>();
            EntityHealth.MaxHealth = 100f;
            EntityHealth.HealthPoints = 100f;

            _eventSystem = eventSystem;
            _eventSystem.Subscribe<KeyboardEvent>(OnKeyboardEvent);
            _eventSystem.Subscribe<MouseEvent>(OnMouseEvent);

            Animation.Animator.StateMachine.StateChanged += StateMachine_StateChanged;
        }

        public override void LoadContent(ContentManager contentManager)
        {
            Animation.Texture2D = contentManager.Load<Texture2D>("sprites/characters/player");
        }

        private void StateMachine_StateChanged(object sender, TransitionEventArgs<AnimationNode> e)
        {
            if (IsAttackAnimation(e.CurrentState) && !IsAttackAnimation(e.PreviousState))
            {
                _eventSystem.Emit(this, new MeleeAttackEvent(_lastGameTime, this, GetAttackDirection(e.CurrentState), AttackRadius, AttackDamage));
            }
        }

        private bool OnKeyboardEvent(object sender, KeyboardEvent keyboardEvent)
        {
            _lastKeyboardState = keyboardEvent.KeyboardState;
            var handled = false;

            var input = InputHelper.GetInputAxisNormalized(_lastKeyboardState);
            handled |= input != Vector2.Zero; // if player moves
            Animation.Animator.StateMachine.SetDirectionVector(input);

            // TODO remove later
            if (keyboardEvent.PressedKeys.Contains(Keys.K))
            {
                Animation.Animator.StateMachine.SetParameter(AnimationKeys.IsDead, true);
                handled = true;
            }
            return handled;
        }

        private bool OnMouseEvent(object sender, MouseEvent mouseEvent)
        {
            if (mouseEvent.MouseState.LeftButton == ButtonState.Pressed)
            {
                Animation.Animator.StateMachine.SetTrigger(AnimationKeys.AttackTrigger);
                return true;
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            _lastGameTime = gameTime;

            var animator = Animation.Animator;
            if (IsMovable(animator.StateMachine.State))
            {
                var input = InputHelper.GetInputAxisNormalized(_lastKeyboardState);
                Transform.Position += input * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
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

        private static MeleeAttackEvent.Direction GetAttackDirection(AnimationNode state)
        {
            if (state == PlayerAnimator.AttackDownNode)
            {
                return MeleeAttackEvent.Direction.Down;
            }
            else if (state == PlayerAnimator.AttackUpNode)
            {
                return MeleeAttackEvent.Direction.Up;
            }
            else if (state == PlayerAnimator.AttackLeftNode)
            {
                return MeleeAttackEvent.Direction.Left;
            }
            else if (state == PlayerAnimator.AttackRightNode)
            {
                return MeleeAttackEvent.Direction.Right;
            }
            throw new ArgumentException($"{nameof(state)} is not attack animation state");
        }
    }
}
