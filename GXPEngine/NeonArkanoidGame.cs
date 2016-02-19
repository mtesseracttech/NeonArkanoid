using System;
using System.Collections.Generic;
using System.Linq;
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
        private GameOver _gameOver;
        private string[] _levelNames;
        private string _state;
        private int _score = 100;

        public NeonArkanoidGame() : base(1280, 800, false, false)
        {
            PopulateLevelNames();
            SetState("MainMenu");
        }

        private void PopulateLevelNames()
        {
            _levelNames = new []
            {
                "Level01",
                "Level02",
                "Level03",
                "Level04",
                "Level05",
                "Level06",
                "Level07",
                "Level08",
                "Level09",
                "Level10"
            };
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
                case "Level01":
                    _level.StopMusic();
                    _level.Destroy();
                    _level = null;
                    break;
                case "Level02":
                    _level.StopMusic();
                    _level.Destroy();
                    _level = null;
                    break;
                case "Level03":
                    _level.StopMusic();
                    _level.Destroy();
                    _level = null;
                    break;
                case "Level04":
                    _level.StopMusic();
                    _level.Destroy();
                    _level = null;
                    break;
                case "Level05":
                    _level.StopMusic();
                    _level.Destroy();
                    _level = null;
                    break;
                case "Level06":
                    _level.StopMusic();
                    _level.Destroy();
                    _level = null;
                    break;
                case "Level07":
                    _level.StopMusic();
                    _level.Destroy();
                    _level = null;
                    break;
                case "Level08":
                    _level.StopMusic();
                    _level.Destroy();
                    _level = null;
                    break;
                case "Level09":
                    _level.StopMusic();
                    _level.Destroy();
                    _level = null;
                    break;
                case "Level10":
                    _level.StopMusic();
                    _level.Destroy();
                    _level = null;
                    break;
                case "Credits":
                    _credits.StopMusic();
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
                case "Level01":
                    _level = new Level.Level("level01 mirror.tmx", this);
                    AddChild(_level);
                    break;
                case "Level02":
                    _level = new Level.Level("level02 diamonds.tmx", this);
                    AddChild(_level);
                    break;
                case "Level03":
                    _level = new Level.Level("level03 eye.tmx", this);
                    AddChild(_level);
                    break;
                case "Level04":
                    _level = new Level.Level("level04 invader.tmx", this);
                    AddChild(_level);
                    break;
                case "Level05":
                    _level = new Level.Level("level05 squashing bugs.tmx", this);
                    AddChild(_level);
                    break;
                case "Level06":
                    _level = new Level.Level("level06 bear.tmx", this);
                    AddChild(_level);
                    break;
                case "Level07":
                    _level = new Level.Level("level07 cardinal.tmx", this);
                    AddChild(_level);
                    break;
                case "Level08":
                    _level = new Level.Level("level08 Tiger.tmx", this);
                    AddChild(_level);
                    break;
                case "Level09":
                    _level = new Level.Level("level09 butterfly.tmx", this);
                    AddChild(_level);
                    break;
                case "Level10":
                    _level = new Level.Level("level10 wolf.tmx", this);
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
        {
            if (Input.GetKeyDown(Key.Q)) NextLevel();
        }

        public void StartWinScreen()
        {
            _winScreen = new WinScreen(this);
            AddChild(_winScreen);
        }

        public void StartGameOver()
        {
            _gameOver = new GameOver(this);
            AddChild(_gameOver);
        }

        public void AddToScore(int score)
        {
            _score += score;
        }

        public int GetScore()
        {
            return _score;
        }

        
        public void NextLevel()
        {
            for (int i = 0; i < _levelNames.Length; i++)
            {
                if (_levelNames[i] == _state)
                {
                    if (i < _levelNames.Length - 1)
                    {
                        SetState(_levelNames[i + 1]);
                        break;
                    }
                    else
                    {
                        SetState("Credits");
                        break;
                    }
                }
            }
        }
        
    }
}