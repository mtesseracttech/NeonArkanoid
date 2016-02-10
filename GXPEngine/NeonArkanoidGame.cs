using System;
using GXPEngine.Utility.TiledParser;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.UI.Menu;
using TiledParser;

namespace NeonArkanoid
{
    public class NeonArkanoidGame : Game
    {
        private MainMenu _menu;

        private string _state;

        public NeonArkanoidGame() : base(1280, 800, false, false)
        {
            //Sprite Test = new Sprite(Utility.UtilStrings.SpritesDebug + "colors.png");
            //Test.SetXY(100, 100);
            //AddChild(Test);
            SetState("MainMenu");
        }
        private static void Main()
        {
            /**
            TMXParser tmxParser = new TMXParser();
            tmxParser.Parse("Polytest.tmx");
            Console.Read();
            /**/
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