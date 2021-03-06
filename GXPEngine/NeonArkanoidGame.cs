using System;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Utils;
using NeonArkanoid.UI.Menu;

namespace NeonArkanoid
{
    public class NeonArkanoidGame : Game
    {
        private Level.Level _level;
        private Credits _credits;
        private MainMenu _menu;
        private WinScreen _winScreen;
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
                    _menu = null;
                    break;
                case "Level1":
                    _level.Destroy();
                    _level = null;
                    break;
                case "Level2":
                    _level.Destroy();
                    _level = null;
                    break;
                case "Credits":
                    _credits.Destroy();
                    _credits = null;
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
                case "Level2":
                    _level = new Level.Level("rocket.tmx", this);
                    AddChild(_level);
                    break;
                case "Credits":
                    _credits = new Credits(this);
                    AddChild(_credits);
                    break;
                case "Exit":
                    Environment.Exit(0);
                    break;
                default:
                    throw new Exception("You tried to load a non-existant state");
            }
        }

        private void Update()
        { }

        public void StartWinScreen()
        {
            _winScreen = new WinScreen(this);
            AddChild(_winScreen);
        }
    }
}