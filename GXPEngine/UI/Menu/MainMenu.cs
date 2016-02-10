﻿using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Utils;
using NeonArkanoid.Utility;
using NeonArkanoid.UI.Menu;
using System.Drawing;

namespace NeonArkanoid.UI.Menu
{
    internal class MainMenu : GameObject
    {
        private readonly Button[] _buttons;
        private readonly NeonArkanoidGame _game;
        //private Sprite _header;
        private Menu.Background _background;
        private Menu.Background _header;
        private Color _color;

         private readonly SoundChannel _musicChannel;
         private readonly Sound _selectedSound;

        private int _selection;

        public MainMenu(NeonArkanoidGame game)
        {
            _game = game;
            SetBackground();
            SetHeader();
            _buttons = new[]
            {
                new Button(UtilStrings.SpritesMenu + "Start.png", 2, game.width/2, 50, "Level1"),
                new Button(UtilStrings.SpritesMenu + "Score.png", 2, game.width/2, 150, "HighScores"),
                new Button(UtilStrings.SpritesMenu + "option.png", 2, game.width/2, 250, "Option"),
                new Button(UtilStrings.SpritesMenu + "quit.png", 2, game.width/2, 350, "Exit")
            };
            foreach (var button in _buttons)
            {
                AddChild(button);
            }
            _buttons[0].Selected();
            /**/
            _selectedSound = new Sound(UtilStrings.SoundsMenu + "sound_selected.wav");
            var music = new Sound(UtilStrings.SoundsMenu + "music_menu1.wav", true, true);
            _musicChannel = music.Play();
            /**/
        }
        /**/
        private void SetBackground()
        {
            _background = new Background(UtilStrings.SpritesMenu + "background.png", true);
            AddChild(_background);
        }
        /**/

        /**/
        private void SetHeader()
        {
            _header = new Background(UtilStrings.SpritesMenu + "header.png", true);
            _header.SetOrigin(_header.width/2, _header.height/2);
            _header.SetXY(game.width/2 - 350, 400);
            AddChild(_header);
        }
        /**/

        private void Update()
        {
            if (Input.GetKeyDown(Key.UP) || Input.GetKeyDown(Key.W))  SelectionUp();
            if (Input.GetKeyDown(Key.DOWN) || Input.GetKeyDown(Key.S)) SelectionDown();
            if (Input.GetKeyDown(Key.ENTER) || Input.GetKeyDown(Key.SPACE)) Select();
        }

        private void SelectionDown()
        {
            _selectedSound.Play();
            _buttons[_selection].DeSelect();
            if (_selection < _buttons.Length - 1) _selection++;
            else _selection = 0;
            _buttons[_selection].Selected();
        }

        private void SelectionUp()
        {
            _selectedSound.Play();
            _buttons[_selection].DeSelect();
            if (_selection > 0) _selection--;
            else _selection = _buttons.Length - 1;
            _buttons[_selection].Selected();
        }

        private void Select()
        {
            _game.SetState(_buttons[_selection].Pressed());
        }

        public void StopMusic()
        {
            _musicChannel.Stop();
        }
    }
}