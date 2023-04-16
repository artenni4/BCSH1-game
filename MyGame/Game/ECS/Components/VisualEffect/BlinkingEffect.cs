namespace MyGame.Game.ECS.Components.VisualEffect
{
    internal class BlinkingEffect : EffectComponent
    {
        public Color OriginalColor { get; set; } = Color.White;
        public Color BlinkColor { get; set; }
        public TimeSpan BlinkInterval { get; set; }

        public Color GetCurrentColor(TimeSpan currentTime)
        {
            if (currentTime - TimeStarted > Duration)
            {
                return OriginalColor;
            }

            // Calculate the elapsed time since the blinking started, modulo the blink interval
            TimeSpan elapsedTime = currentTime - TimeStarted;
            TimeSpan elapsedTimeModInterval = TimeSpan.FromTicks(elapsedTime.Ticks % BlinkInterval.Ticks);

            // Determine the current color based on the elapsed time
            if (elapsedTimeModInterval <= TimeSpan.FromTicks(BlinkInterval.Ticks / 2))
            {
                return OriginalColor;
            }
            else
            {
                return BlinkColor;
            }
        }
    }
}
