using System.Collections.Generic;
using GXPEngine.GXPEngine;
using GXPEngine.GXPEngine.Utils;
using GXPEngine.UI.Menu;

namespace GXPEngine.Level
{
    internal abstract class LevelBase : GameObject
    {
        protected Sprite Background;

        protected List<GameObject> Enemies;

        protected bool Finished = false;
        protected NeonArkanoidGame Game;
        protected Sound Music;
        protected SoundChannel MusicChannel;
        protected Pause Pause;
        protected bool Paused;
        protected Sprite[] Spritesheet;
        protected int[] Tiles;

        protected LevelBase(NeonArkanoidGame game)
        {
            Game = game;
            Pause = new Pause(game, this);
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(Key.P)) PauseToggle();
            if (!Paused) UpdateUnpaused();
        }

        private void UpdateUnpaused()
        {
        }

        public void PauseToggle()
        {
            Paused = !Paused;
            Pause.Toggle();
        }

        public void StopMusic()
        {
            MusicChannel.Stop();
        }

        public void PauseMusic(bool pause)
        {
            MusicChannel.IsPaused = pause;
        }

        public abstract string GetLevelName();

        public abstract string GetNextLevelName();

        public NeonArkanoidGame GetGame()
        {
            return Game;
        }
    }
}