using System;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.UI.Menu;

namespace NeonArkanoid
{
    public class NeonArkanoidGame : Game
    {
        private MainMenu _menu;

        private string _state;

        public NeonArkanoidGame() : base(1280, 720, false, false)
        {

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