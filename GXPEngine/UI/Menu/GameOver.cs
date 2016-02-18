using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Utils;
using TweenEngine;
using TweenGameUTE;

namespace NeonArkanoid.UI.Menu
{
    class GameOver : GameObject
    {
        private NeonArkanoidGame _game;
        Sprite mySprite, mySprite2;
        TweenManager tm;
        Timeline myTimelime = Timeline.CreateSequence();
        Timeline myTimelime2 = Timeline.CreateSequence();

        public GameOver(NeonArkanoidGame game) : base()
        {
            _game = game;
            tm = new TweenManager();
            Tween.RegisterAccessor(typeof(Sprite), new SpriteAccessor());

            mySprite = new Sprite("../assets/sprite/ui/GO1.png");
            mySprite.SetOrigin(mySprite.width / 2, mySprite.height / 2);
            AddChild(mySprite);

            mySprite2 = new Sprite("../assets/sprite/ui/GO2.png");
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
        }
    }
}
