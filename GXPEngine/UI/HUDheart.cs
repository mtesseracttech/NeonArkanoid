using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Utils;
using NeonArkanoid.Utility;

namespace GXPEngine.UI
{
    class HUDheart : AnimationSprite
    {
        private int _currentCreateFrame, _currentDefaultFrame, _currentBreakFrame;
        private int[] _breakFrames = {0,1,2,3,4,5};
        private int[] _createFrames = {5,4,3,2,1,0};
        private int[] _defaultFrames = {6, 7, 8, 9,10};

        private int _state;
        public HUDheart(float x, float y) : base("../assets/sprite/ui/heart.png",11,1)
        {
            SetXY(x, y);
            _state = 0;
        }

        void Update()
        {
            switch (_state)
            {
                case 0:
                    CreationFrames();
                    break;
                case 1:
                    DefaultFrames();
                    break;
                case 2:
                    BreakFrames();
                    break;
            }
        }

        public void Break()
        {
            _currentBreakFrame = _breakFrames[0];
            _state = 2;
        }

        public void Create()
        {
            _currentCreateFrame = _createFrames[0];
            _state = 0;
        }

        private void CreationFrames()
        {
            if (_currentCreateFrame < _createFrames.Length * 10 - 1) _currentCreateFrame++;
            else _state = 1;
            currentFrame = _createFrames[_currentCreateFrame / 10];
        }

        private void DefaultFrames()
        {
            if (_currentDefaultFrame < _defaultFrames.Length * 20 - 1) _currentDefaultFrame++;
            else _currentDefaultFrame = 0;
            currentFrame = _defaultFrames[_currentDefaultFrame / 20];
        }

        private void BreakFrames()
        {
            if (_currentBreakFrame < _breakFrames.Length * 5 - 1) _currentBreakFrame++;
            else Destroy();
            currentFrame = _breakFrames[_currentBreakFrame / 5];
        }
    }
}
