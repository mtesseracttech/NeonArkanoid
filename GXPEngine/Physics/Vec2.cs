using System;
using GXPEngine.Core;

namespace GXPEngine
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


    }
}

