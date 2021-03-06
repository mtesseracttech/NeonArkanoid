﻿using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Utils;
using NeonArkanoid.Utility;
using System.Drawing;

namespace NeonArkanoid.UI.Menu
{
    internal class MainMenu : GameObject
    {
        private readonly Button[] _buttons;
        private readonly NeonArkanoidGame _game;
        private Background _background1, _background2;
        private Background _header, _header1;
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
                new Button(UtilStrings.SpritesMenu + "Start.png", 2, 1500, 207, "Level1"),
                new Button(UtilStrings.SpritesMenu + "credits.png", 2, 1498, 370, "Credits"),
                new Button(UtilStrings.SpritesMenu + "quit.png", 2, 1502, 537, "Exit")
            };
            foreach (var button in _buttons)
            {
                AddChild(button);
            }
            _buttons[0].Selected();
            _selectedSound = new Sound(UtilStrings.SoundsMenu + "sound_selected.wav");
            var music = new Sound(UtilStrings.SoundsMenu + "Menu.ogg", true, true);
            _musicChannel = music.Play();
            
        }    
        private void SetBackground()
        {
            _background1 = new Background(UtilStrings.SpritesMenu + "background1.jpg", true,1);
            AddChild(_background1);
            _background2 = new Background(UtilStrings.SpritesMenu + "background.png", false,0);
            AddChild(_background2);
        }

        private void SetHeader()
        {
            _header = new Background(UtilStrings.SpritesMenu + "header1.png", true, 2);
            AddChild(_header);
            _header1 = new Background(UtilStrings.SpritesMenu + "header.png", false, 0);
            AddChild(_header1);

        }

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