using System;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Utils;
using NeonArkanoid.Utility;

namespace NeonArkanoid.UI.Menu
{
    public class Credits : GameObject
    {
        private readonly NeonArkanoidGame _game;
        private readonly Button[] _buttons;
        private Background _background1;
        private readonly SoundChannel _musicChannel;
        private readonly Sound _selectedSound;

        private int _selection;

        public Credits(NeonArkanoidGame game) : base()
        {
            _game = game;
            SetBackground();

            _buttons = new[]
           {
                new Button(UtilStrings.SpritesMenu + "/Credits/back.png", 2, 1500, 680, "MainMenu"),
            };
            foreach (var button in _buttons)
            {
                AddChild(button);
            }
            _buttons[0].Selected();
            _selectedSound = new Sound(UtilStrings.SoundsMenu + "sound_selected.wav");

        }

        private void SetBackground()
        {
            _background1 = new Background(UtilStrings.SpritesMenu + "/Credits/Credit.png", false, 0);
            AddChild(_background1);
        }

        private void Update()
        {
            if (Input.GetKeyDown(Key.UP) || Input.GetKeyDown(Key.W)) SelectionUp();
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
    }
}
