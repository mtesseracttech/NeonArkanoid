using System;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.UI.Menu;

namespace NeonArkanoid
{
    public class NeonArkanoidGame : Game
    {
        private MainMenu _menu;
        private Level.Level _level;

        private string _state;

        public NeonArkanoidGame() : base(1280, 800, false, false)
        {
            SetState("MainMenu");
        }
        private static void Main()
        {
            new NeonArkanoidGame().Start();
        }

        public void SetState(string state, bool restart = false)
        {
            if (state == _state && !restart) return;
            StopState();
            _state = state;
            StartState();
        }

        private void StopState()
        {
            switch (_state)
            {
                case "MainMenu":
                    _menu.StopMusic();
                    _menu.Destroy();
                    break;
                case "Level1":
                    _level.Destroy();
                    _level = null;
                    break;
            }
        }

        private void StartState()
        {
            switch (_state)
            {
                case "MainMenu":
                    _menu = new MainMenu(this);
                    AddChild(_menu);
                    break;
                case "Level1":
                    _level = new Level.Level("tiger 4.tmx", this);
                    AddChild(_level);
                    break;
                case "Exit":
                    Environment.Exit(0);
                    break;
                default:
                    throw new Exception("You tried to load a non-existant state");
            }
        }


        private void Update()
        {
        }
    }
}