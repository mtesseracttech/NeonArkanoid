using System;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Utils;
using NeonArkanoid.Utility;
using TweenEngine;
using TweenGameUTE;


namespace NeonArkanoid.UI.Menu

{
    public class WinScreen : GameObject
    {
        private NeonArkanoidGame _game;
        Sprite mySprite, mySprite2;
        TweenManager tm;
        Timeline myTimelime = Timeline.CreateSequence();
        Timeline myTimelime2 = Timeline.CreateSequence();
        private Sound _sound;
        public WinScreen(NeonArkanoidGame game) : base()
        {
            _game = game;
            _sound = new Sound(UtilStrings.SoundsMenu + "Win.wav");
            _sound.Play();
            tm = new TweenManager();
            Tween.RegisterAccessor(typeof(Sprite), new SpriteAccessor());

            mySprite = new Sprite("../assets/sprite/ui/W1.png");
            mySprite.SetOrigin(mySprite.width / 2, mySprite.height / 2);
            AddChild(mySprite);  

            mySprite2 = new Sprite("../assets/sprite/ui/W2.png");
            mySprite2.SetOrigin(0, 0);
            AddChild(mySprite2);

            myTimelime.Push(Tween.To(mySprite, SpriteAccessor.XY, 4000).Target(_game.width / 2, _game.height / 2).Ease(TweenEquations.easeOutBounce));
            myTimelime.Start(tm);
            myTimelime2.Push(Tween.From(mySprite2, SpriteAccessor.XY, 4000).Target(_game.width, _game.height).Ease(TweenEquations.easeOutBounce));
            myTimelime2.Start(tm);
        }

        public void Update()
        {
            tm.Update(Time.deltaTime);
            /*
           if (mySprite2.x <_game.width /14  && mySprite2.y < _game.height / 14)
           {
               //mySprite.alpha -= 0.009f;
               //mySprite2.alpha -= 0.009f;

               if (mySprite2.alpha <= -0.5 && mySprite.alpha <= -0.5)
               {
                   mySprite2.alpha = 0;
                   mySprite.alpha = 0;
                   mySprite2.visible = false;
                   mySprite.visible = false;

                   if (mySprite.visible == false && mySprite2.visible == false)
                   {
                       mySprite.alpha += 1f;
                       mySprite.alpha +=1f ;
                   }
                   /**/
        }
    }
}
