using MyGame.Game.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyGame.Game.Helpers
{
    internal static class InputHelper
    {
        public static Vector2 GetInputAxis(KeyboardState keyboardState)
        {
            float x = 0, y = 0;
            if (keyboardState.IsKeyDown(InputConstants.Up)) y++;
            if (keyboardState.IsKeyDown(InputConstants.Down)) y--;
            if (keyboardState.IsKeyDown(InputConstants.Left)) x--;
            if (keyboardState.IsKeyDown(InputConstants.Right)) x++;

            return new Vector2(x, y);
        }

        public static Vector2 GetInputAxisNormalized(KeyboardState keyboardState)
        {
            var result = GetInputAxis(keyboardState);
            if (result != Vector2.Zero)
            {
                result.Normalize();
            }
            return result;
        }
    }
}
