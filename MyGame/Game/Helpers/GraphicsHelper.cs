using MyGame.Game.Constants;
using MyGame.Game.ECS;
using MyGame.Game.ECS.Components.Animation;
using MyGame.Game.ECS.Entities;
using MyGame.Game.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Game.Helpers
{
    internal static class GraphicsHelper
    {
        #region Animation Helper
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

        public static void SetDirectionVector(this IRealTimeFSM<AnimationNode> stateMachine, Vector2 direction)
        {
            stateMachine.SetParameter(AnimationKeys.XDirection, direction.X);
            stateMachine.SetParameter(AnimationKeys.YDirection, direction.Y);
        }

        public static void RemoveDirectionVector(this IRealTimeFSM<AnimationNode> stateMachine)
        {
            stateMachine.SetParameter(AnimationKeys.XDirection, 0f);
            stateMachine.SetParameter(AnimationKeys.YDirection, 0f);
        }

        /// <summary>
        /// If current animation frame is last in sequence
        /// </summary>
        /// <param name="animator"></param>
        /// <returns></returns>
        public static bool IsLastAnimationFrame(this IAnimator animator)
        {
            return animator.GetFrameIndex() >= animator.StateMachine.State.AnimationFrames.Length - 1;
        }

        /// <summary>
        /// If current animation frame is first in sequence
        /// </summary>
        /// <param name="animator"></param>
        /// <returns></returns>
        public static bool IsFirstAnimationFrame(this IAnimator animator)
        {
            return animator.GetFrameIndex() <= 1;
        }
        #endregion

        #region Drawing Primitives
        static Texture2D _pixel = null;

        private static void InitPixel(SpriteBatch spriteBatch)
        {
            if (_pixel is null)
            {
                _pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _pixel.SetData(new Color[] { Color.White });
            }
        }

        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rectangle, Color color, int thickness)
        {
            InitPixel(spriteBatch);

            spriteBatch.Draw(_pixel, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height + thickness), color);
            spriteBatch.Draw(_pixel, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + thickness, thickness), color);
            spriteBatch.Draw(_pixel, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, thickness, rectangle.Height + thickness), color);
            spriteBatch.Draw(_pixel, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + thickness, thickness), color);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 startPos, Vector2 endPos, Color color, int thickness)
        {
            InitPixel(spriteBatch);

            // Create a texture as wide as the distance between two points and as high as
            // the desired thickness of the line.
            var distance = (int)Vector2.Distance(startPos, endPos);
            if (distance <= 0)
            {
                return;
            }

            // Rotate about the beginning middle of the line.
            Vector2 tangent = endPos - startPos;
            var rotation = (float)Math.Atan2(tangent.Y, tangent.X);
            var origin = new Vector2(0, thickness / 2);

            spriteBatch.Draw(_pixel, startPos, null, color, rotation, origin, new Vector2(tangent.Length(), thickness), SpriteEffects.None, 0.0f);
        }
        #endregion
    }
}
