using MyGame.Game.Constants.Enums;
using MyGame.Game.ECS.Components;
using System.Linq;

namespace MyGame.Game.Helpers
{
    internal static class AnimationHelper
    {
        public static Rectangle[] GenerateBoundsForAtlasHorizontal(int originX, int originY, int rectHeight, int rectWidth, int framesCount, int xGap = 0)
        {
            Rectangle[] bounds = new Rectangle[framesCount];
            for (int i = 0; i < framesCount; i++)
            {
                bounds[i] = new Rectangle(originX + i * (rectWidth + xGap), originY, rectWidth, rectHeight);
            }

            return bounds.ToArray();
        }

        public static Rectangle[][] GenerateBoundsForAnimationAtlas(int xOrigin, int yOrigin, int rectWidth, int rectHeight, int animationsCount, params int[] framesCount)
        {
            if (framesCount.Length != animationsCount)
            {
                throw new ArgumentException($"{nameof(animationsCount)} must be equal to length of {nameof(framesCount)}");
            }

            Rectangle[][] bounds = new Rectangle[animationsCount][];

            for (int i = 0; i < animationsCount; i++)
            {
                int animationFramesCount = framesCount[i];
                bounds[i] = new Rectangle[animationFramesCount];
                for (int j = 0; j < animationFramesCount; j++)
                {
                    bounds[i][j] = new Rectangle(xOrigin + j * rectWidth, yOrigin + i * rectHeight, rectWidth, rectHeight);
                }
            }
            return bounds;
        }

        /// <summary>
        /// Calculates animation duration
        /// </summary>
        public static float GetStateDuration(this Animation animation, AnimationState state)
        {
            return (animation.Frames[animation.MapState(state)].Length - 1) / animation.Speed;
        }

        /// <summary>
        /// Gets frames depending on current animation state
        /// </summary>
        public static Rectangle[] GetStateFrames(this Animation animation)
        {
            return animation.Frames[animation.MapState(animation.State)];
        }

        public static int GetFramesIndex(this Animation animation)
        {
            // calculate frame
            int rectIndex = Convert.ToInt32(animation.TimePlayed.TotalSeconds * animation.Speed);

            // if animation played one cycle
            int framesCount = animation.GetStateFrames().Length;
            if (rectIndex >= framesCount)
            {
                if (animation.IsCycled)
                {
                    return rectIndex % framesCount;
                }
                else
                {
                    return framesCount - 1;
                }
            }
            return rectIndex;
        }

        public static Rectangle GetCurrentBound(this Animation animation)
        {
            return animation.GetStateFrames()[animation.GetFramesIndex()];
        }

        public static SpriteEffects GetSpriteEffect(this AnimationState state)
        {
            return state switch
            {
                AnimationState.IdleLeft or
                AnimationState.WalkLeft or
                AnimationState.AttackLeft or
                AnimationState.DeathLeft or
                AnimationState.HurtLeft => SpriteEffects.FlipHorizontally,
                _ => SpriteEffects.None,
            };
        }

        public static bool IsMoving(this AnimationState state)
        {
            return state == AnimationState.Walk || state == AnimationState.WalkLeft || state == AnimationState.WalkRight ||
                state == AnimationState.WalkUp || state == AnimationState.WalkDown;
        }
    }
}
