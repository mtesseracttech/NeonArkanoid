using System;
using System.Drawing;

namespace NeonArkanoid.Physics
{
    public class Vec2
    {
        public static Vec2 temp = new Vec2();

        public float x;
        public float y;

        public Vec2(float pX = 0, float pY = 0)
        {
            x = pX;
            y = pY;
        }

        public static Vec2 zero
        {
            get { return new Vec2(0, 0); }
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", x, y);
        }

        public Vec2 Add(Vec2 other)
        {
            x += other.x;
            y += other.y;
            return this;
        }

        public Vec2 Subtract(Vec2 other)
        {
            x -= other.x;
            y -= other.y;
            return this;
        }

        public float Length()
        {
            return (float) Math.Sqrt(x*x + y*y);
        }

        public Vec2 Normalize()
        {
            if (x == 0 && y == 0)
            {
                return this;
            }
            return Scale(1/Length());
        }

        public Vec2 Clone()
        {
            return new Vec2(x, y);
        }

        public Vec2 Zero()
        {
            x = y = 0;
            return this;
        }

        public Vec2 Scale(float scalar)
        {
            x *= scalar;
            y *= scalar;
            return this;
        }

        public Vec2 Set(float pX, float pY)
        {
            x = pX;
            y = pY;
            return this;
        }

        public Vec2 SetVec(Vec2 vec)
        {
            x = vec.x;
            y = vec.y;
            return this;
        }

        public Vec2 Normal()
        {
            if (x == 0 && y == 0)
            {
                return this;
            }
            return new Vec2(-y, x).Scale(1/Length());
        }

        public float Dot(Vec2 other)
        {
            return x*other.x + y*other.y;
        }

        public Vec2 Reflect(Vec2 normal, float bounciness)
        {
            var vectorPartLength = Dot(normal);
            var projectedVector = normal.Clone().Scale(vectorPartLength);
            return Subtract(projectedVector.Scale(1 + bounciness));
        }

        //angles and rotation

        public Vec2 SetAngleRadians(float radians)
        {
            var radius = Length();
            y = radius*(float) Math.Sin(radians);
            x = radius*(float) Math.Cos(radians);
            return this;
        }

        public Vec2 SetAngleDegrees(float degrees)
        {
            var radius = Length();
            x = radius*(float) Math.Cos(degrees*(float) Math.PI/180);
            y = radius*(float) Math.Sin(degrees*(float) Math.PI/180);
            return this;
        }

        public double GetAngleRadians()
        {
            return Math.Atan2(y, x);
        }

        public double GetAngleDegrees()
        {
            return Math.Atan2(y, x)*180/Math.PI;
        }

        public Vec2 RotateRadians(float radians)
        {
            var x1 = x*(float) Math.Cos(radians) - y*(float) Math.Sin(radians);
            var y1 = x*(float) Math.Sin(radians) - y*(float) Math.Cos(radians);
            x = x1;
            y = y1;
            return this;
        }

        public Vec2 RotateDegrees(float degrees)
        {
            var rad = degrees*(float) Math.PI/180;
            var x1 = x*(float) Math.Cos(rad) - y*(float) Math.Sin(rad);
            var y1 = x*(float) Math.Sin(rad) - y*(float) Math.Cos(rad);
            x = x1;
            y = y1;
            return this;
        }

        public Vec2 FromPolarDegrees(float degrees)
        {
            var radius = Length();
            x = radius*(float) Math.Cos(degrees*(float) Math.PI/180);
            y = radius*(float) Math.Sin(degrees*(float) Math.PI/180);
            return this;
        }

        public Vec2 FromPolarRadians(float radians)
        {
            var radius = Length();
            y = radius*(float) Math.Sin(radians);
            x = radius*(float) Math.Cos(radians);
            return this;
        }

        public PointF Vec2ToPointF()
        {
            return new PointF(x, y);
        }
    }
}