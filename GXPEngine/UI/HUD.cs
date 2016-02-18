using System;
using NeonArkanoid.GXPEngine;
using System.Collections.Generic;
using NeonArkanoid.Level;

namespace GXPEngine.UI
{
    class HUD : GameObject
    {
        private List<HUDheart> _lifeHearts;
        private int _lives;
        private Level _level;

        public HUD(int lives, Level level)
        {
            _lives = lives;
            _level = level;
            CreateHearts();
            _level.AddChildAt(this, 10);
        }

        private void CreateHearts()
        {
            _lifeHearts = new List<HUDheart>();
            for (int i = 0; i < _lives; i++)
            {
                _lifeHearts.Add(new HUDheart(x + 30 + (i * 64), 30));
            }
            foreach (HUDheart heart in _lifeHearts)
            {
                AddChild(heart);
            }

        }

        public void SetHearts(int lives)
        {
            if (_lifeHearts.Count < lives) while (_lifeHearts.Count < lives) AddHeart();
            else if (_lifeHearts.Count > lives) while (_lifeHearts.Count > lives) LoseHeart();
        }

        private void LoseHeart()
        {
            if (_lifeHearts.Count > 0)
            {
                _lifeHearts[_lifeHearts.Count - 1].Break();
                _lifeHearts.RemoveAt(_lifeHearts.Count - 1);
            }
        }

        private void AddHeart()
        {
            _lifeHearts.Add(new HUDheart(x + 30 + ((_lifeHearts.Count) * 64), 30));
            AddChild(_lifeHearts[_lifeHearts.Count - 1]);
        }

        private void Update()
        {
        }
    }
}
