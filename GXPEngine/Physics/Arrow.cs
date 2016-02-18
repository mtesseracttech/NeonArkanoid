using GXPEngine;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Core;

namespace NeonArkanoid.Physics
{
    public class Arrow : GameObject
    {
        private Vec2 _startPoint;
        private Vec2 _vector;

        public uint Color;
        public uint LineWidth;
        public float Scale;

        public Arrow(Vec2 pStartPoint, Vec2 pVector, float pScale, uint pColor = 0xffffffff, uint pLineWidth = 1)
        {
            _startPoint = pStartPoint;
            _vector = pVector;
            Scale = pScale;

            Color = pColor;
            LineWidth = pLineWidth;
        }

        public Vec2 StartPoint
        {
            set { _startPoint = value ?? Vec2.zero; }
            get { return _startPoint; }
        }

        public Vec2 Vector
        {
            set { _vector = value ?? Vec2.zero; }
            get { return _vector; }
        }

        protected override void RenderSelf(GLContext glContext)
        {
            if (_startPoint == null || _vector == null)
                return;

            var endPoint = _startPoint.Clone().Add(_vector.Clone().Scale(Scale));
            LineSegment.RenderLine(_startPoint, endPoint, Color, LineWidth, true);

            var smallVec = endPoint.Clone().Subtract(_startPoint).Normalize().Scale(-10);
            var left = new Vec2(-smallVec.y, smallVec.x);
            var right = new Vec2(smallVec.y, -smallVec.x);
            left.Add(smallVec).Add(endPoint);
            right.Add(smallVec).Add(endPoint);

            LineSegment.RenderLine(endPoint, left, Color, LineWidth, true);
            LineSegment.RenderLine(endPoint, right, Color, LineWidth, true);
        }
    }
}