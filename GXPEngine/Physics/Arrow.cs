using System;

namespace GXPEngine
{
	public class Arrow:GameObject
	{
		private Vec2 _startPoint;
		private Vec2 _vector;
		public float scale;

		public uint color = 0xffffffff;
		public uint lineWidth = 1;

		public Arrow (Vec2 pStartPoint, Vec2 pVector, float pScale, uint pColor = 0xffffffff, uint pLineWidth = 1)
		{
			_startPoint = pStartPoint;
			_vector = pVector;
			scale = pScale;

			color = pColor;
			lineWidth = pLineWidth;
		}

		public Vec2 startPoint {
			set {
				_startPoint = value ?? Vec2.zero;
			}
			get {
				return _startPoint;
			}
		}

		public Vec2 vector {
			set {
				_vector = value ?? Vec2.zero;
			}
			get {
				return _vector;
			}
		}

		protected override void RenderSelf (GXPEngine.Core.GLContext glContext)
		{
			if (_startPoint == null || _vector == null)
				return;
				
			Vec2 endPoint = _startPoint.Clone().Add (_vector.Clone().Scale (scale));
			LineSegment.RenderLine (_startPoint, endPoint, color, lineWidth, true);

			Vec2 smallVec = endPoint.Clone().Subtract(_startPoint).Normalize ().Scale (-10);
			Vec2 left = new Vec2 (-smallVec.y, smallVec.x);
			Vec2 right = new Vec2 (smallVec.y, -smallVec.x);
			left.Add (smallVec).Add (endPoint);
			right.Add (smallVec).Add (endPoint);

			LineSegment.RenderLine (endPoint, left, color, lineWidth, true);
			LineSegment.RenderLine (endPoint, right, color, lineWidth, true);
		}
	}
}

