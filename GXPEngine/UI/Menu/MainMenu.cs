using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Utils;
using NeonArkanoid.Utility;

namespace NeonArkanoid.UI.Menu
{
    internal class MainMenu : GameObject
    {
        private readonly Button[] _buttons;
        private readonly NeonArkanoidGame _game;
        private Sprite _header, _background;
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
                new Button(UtilStrings.SpritesMenu + "button_newgame.png", 2, game.width/2, 350, "Level1"),
                new Button(UtilStrings.SpritesMenu + "button_highscore.png", 2, game.width/2, 450, "HighScores"),
                new Button(UtilStrings.SpritesMenu + "button_credits.png", 2, game.width/2, 550, "Credits"),
                new Button(UtilStrings.SpritesMenu + "button_exit.png", 2, game.width/2, 650, "Exit")
            };

            foreach (var button in _buttons)
            {
                AddChild(button);
            }

            _selectedSound = new Sound(UtilStrings.SoundsMenu + "sound_selected.wav");
            var music = new Sound(UtilStrings.SoundsMenu + "music_menu.mp3", true, true);
            _musicChannel = music.Play();

            _buttons[0].Selected();
        }

        private void SetBackground()
        {
            _background = new Sprite(UtilStrings.SpritesMenu + "background.png");
            AddChild(_background);
        }

        private void SetHeader()
        {
            _header = new Sprite(UtilStrings.SpritesMenu + "header_logo.png");
            _header.SetOrigin(_header.width/2, _header.height/2);
            _header.SetXY(game.width/2, 120);
            AddChild(_header);
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

        public void StopMusic()
        {
            _musicChannel.Stop();
        }
    }
}