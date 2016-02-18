using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Utils;
using NeonArkanoid.Level;
using NeonArkanoid.Utility;

namespace NeonArkanoid.UI.Menu
{
    internal class Pause : GameObject
    {
        private readonly Button[] _buttons;
        private readonly NeonArkanoidGame _game;
        private readonly Level.Level _level;
        private bool _open;
        private readonly Sound _openSound;
        private readonly Sound _selectedSound;
        private int _selection;

        public Pause(NeonArkanoidGame game, Level.Level level)
        {
            _level = level;
            _game = game;

            x = -500;

            var background = new Sprite(UtilStrings.SpritesPause + "background_pause.png");
            background.SetOrigin(0, background.height/2);
            background.y = game.height/2;

            var header = new Sprite(UtilStrings.SpritesPause + "header_pause.png");
            header.SetOrigin(header.width/2, header.height/2);
            header.SetXY(background.width/2, 200);

            _buttons = new[]
            {
                new Button(UtilStrings.SpritesPause + "button_resume.png", 2, background.width/2, 350, "Resume"),
                new Button(UtilStrings.SpritesPause + "button_restart.png", 2, background.width/2, 450,
                    _level.GetLevelName()),
                new Button(UtilStrings.SpritesPause + "button_exit.png", 2, background.width/2, 550, "MainMenu")
            };

            AddChild(background);
            AddChild(header);
            foreach (var button in _buttons)
            {
                AddChild(button);
            }

            _selectedSound = new Sound(UtilStrings.SoundsMenu + "sound_selected.wav");
            _openSound = new Sound(UtilStrings.SoundsMenu + "sound_pause.wav");
        }


        private void Update()
        {
            Opener();
        }

        private void Opener()
        {
            if (_open)
            {
                if (x < 0) x += 50;
                _buttons[_selection].Selected();
                if (Input.GetKeyDown(Key.UP) || Input.GetKeyDown(Key.W)) SelectionUp();
                if (Input.GetKeyDown(Key.DOWN) || Input.GetKeyDown(Key.S)) SelectionDown();
                if (Input.GetKeyDown(Key.ENTER) || Input.GetKeyDown(Key.SPACE)) Select();
            }
            else
            {
                if (x >= -500) x -= 50;
            }
        }

        private void SelectionDown()
        {
            _selectedSound.Play();
            _buttons[_selection].DeSelect();
            if (_selection < _buttons.Length - 1) _selection++;
            else _selection = 0;
        }

        private void SelectionUp()
        {
            _selectedSound.Play();
            _buttons[_selection].DeSelect();
            if (_selection > 0) _selection--;
            else _selection = _buttons.Length - 1;
        }


        public void Toggle()
        {
            _open = !_open;
            _buttons[_selection].DeSelect();
            _openSound.Play();
            if (_open)
            {
                //_level.PauseMusic(true);
                _selection = 0;
            }
            //else _level.PauseMusic(false);
        }

        private void Select()
        {
            switch (_selection)
            {
                case 0:
                    //_level.PauseToggle();
                    break;
                case 1:
                    _game.SetState(_buttons[_selection].Pressed(), true);
                    break;
                case 2:
                    _game.SetState(_buttons[_selection].Pressed());
                    break;
            }
        }
    }
}