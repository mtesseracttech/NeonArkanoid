using System;
using System.Drawing;
using NeonArkanoid.GXPEngine;

namespace NeonArkanoid.Physics
{
    public class Vec2
    {
        public static Vec2 zero { get { return new Vec2(0, 0); } }

        public float x = 0;
        public float y = 0;

        public Vec2(float pX = 0, float pY = 0)
        {
            x = pX;
            y = pY;
        }

        public override string ToString()
        {
            return String.Format("({0}, {1})", x, y);
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

        public Vec2 Scale(float scale)
        {
            x *= scale;
            y *= scale;
            return this;
        }

        public float Length()
        {
            float lengthSquared = ((x*x) + (y*y));
            if (lengthSquared != 0f) return Mathf.Sqrt(lengthSquared);
            return 0f;
        }

        public Vec2 Normalize()
        {
            float length = Length();
            if (length != 0f)
            {
                x /= length;
                y /= length;
            }
            return this;
        }

        public Vec2 Normal()
        {
            return new Vec2(-y, x).Normalize();
        }

        public float Dot(Vec2 b)
        {
            return ((this.x * b.x) + (this.y * b.y));
        }

        public Vec2 Reflect(Vec2 normal, float bounciness = 1)
        {
            float vectorPartLenght = Dot(normal);
            Vec2 projectedVector = normal.Clone().Scale(vectorPartLenght);
            return this.Subtract(normal.Clone().Scale((1 + bounciness) * this.Dot(normal)));
        }


        public Vec2 Clone()
        {
            return new Vec2(x, y);
        }

        public void SetXY(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public PointF Vec2toPointF()
        {
            return new PointF(x, y);
        }

        //angles and rotation

        public Vec2 SetAngleRadians(float radians)
        {
            float radius = Length();
            y = radians * (float)Math.Sin(radians);
            x = radius * (float)Math.Cos(radians);
            return this;
        }

        public Vec2 SetAngleDegrees(float degrees)
        {
            float radius = Length();
            x = radius * (float)Math.Cos(degrees * (float)Math.PI / 180);
            y = radius * (float)Math.Sin(degrees * (float)Math.PI / 180);
            return this;
        }

        public double GetAngleRadians()
        {
            return Math.Atan2(y, x);
        }
        public double GetAngleDegrees()
        {
            return Math.Atan2(y, x) * 180 / Math.PI;
        }

        public Vec2 RotateRadians(float radians)
        {
            float x1 = x * (float)Math.Cos(radians) - y * (float)Math.Sin(radians);
            float y1 = x * (float)Math.Sin(radians) - y * (float)Math.Cos(radians);
            x = x1;
            y = y1;
            return this;
        }
        public Vec2 RotateDegrees(float degrees)
        {
            float rad = degrees * (float)Math.PI / 180;
            float x1 = x * (float)Math.Cos(rad) - y * (float)Math.Sin(rad);
            float y1 = x * (float)Math.Sin(rad) - y * (float)Math.Cos(rad);
            x = x1;
            y = y1;
            return this;
        }
        public Vec2 FromPolarDegrees(float degrees)
        {
            float radius = Length();
            x = radius * (float)Math.Cos(degrees * (float)Math.PI / 180);
            y = radius * (float)Math.Sin(degrees * (float)Math.PI / 180);
            return this;
        }
        public Vec2 FromPolarRadians(float radians)
        {
            float radius = Length();
            x = radius * (float)Math.Sin(radians);
            y = radius * (float)Math.Cos(radians);
            return this;
        }


    }
}

