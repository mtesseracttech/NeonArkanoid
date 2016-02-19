using NeonArkanoid.GXPEngine;
using NeonArkanoid.Physics;
using NeonArkanoid.Utility;

namespace GXPEngine.Object
{
    internal class Spark : AnimationSprite
    {
        private int[] _defaultFrames = {0, 1, 2, 3, 4, 5, 6};
        private int _currentDefaultFrame;

        public Spark(Vec2 position) : base(UtilStrings.SpritesObject + "spark.png", 7, 1)
        {
            SetOrigin(width/2, height/2);
            SetXY(position.x, position.y);
            currentFrame = 0;
        }

        private void Update()
        {
            BreakFrames();
        }

        private void BreakFrames()
        {
            if (_currentDefaultFrame < _defaultFrames.Length * 5 - 1) _currentDefaultFrame++;
            else Destroy();
            currentFrame = _defaultFrames[_currentDefaultFrame / 5];
        }
    }
}