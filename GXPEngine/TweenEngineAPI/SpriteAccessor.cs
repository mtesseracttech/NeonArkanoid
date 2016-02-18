using System;
using GXPEngine;
using NeonArkanoid.GXPEngine;
using TweenEngine;

namespace TweenGameUTE
{

	public class SpriteAccessor : TweenAccessor<Sprite>
	{
		public const int X = 1;
		public const int Y = 2;
		public const int XY = 3;

		override public int GetValues (Sprite target, int tweenType, float[] returnValues)
		{
			switch (tweenType) {
			case X:
				returnValues [0] = target.x;
				return 1;
			case Y:
				returnValues [0] = target.y;
				return 1;
			case XY:
				returnValues [0] = target.x;
				returnValues [1] = target.y;
				return 2;
			default:
				return 0;
			}
		}

		override public void SetValues (Sprite target, int tweenType, float[] newValues)
		{
			switch (tweenType) {
			case X:
				target.x = newValues [0];
				break;
			case Y:
				target.y = newValues [1];
				break;
			case XY:
				target.x = newValues [0];
				target.y = newValues [1];
				break;
			default:
				break;
			}
		}
	}
}