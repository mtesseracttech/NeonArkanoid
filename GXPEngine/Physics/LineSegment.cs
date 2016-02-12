using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Core;
using NeonArkanoid.GXPEngine.OpenGL;

namespace NeonArkanoid.Physics
{
    /// <summary>
    ///     Implements an OpenGL line
    /// </summary>
    public class LineSegment : GameObject
    {
        public uint Color;
        public Vec2 End;
        public uint LineWidth;
        public Vec2 Start;
        private GameObject _owner;

        //stay in this object's coordinate space or interpret vectors as screen coordinates?
        public bool UseGlobalCoords;

        public LineSegment(float pStartX, float pStartY, float pEndX, float pEndY, uint pColor = 0xffffffff,
            uint pLineWidth = 1, bool pGlobalCoords = false)
            : this(new Vec2(pStartX, pStartY), new Vec2(pEndX, pEndY), pColor, pLineWidth)
        {
        }

        public LineSegment(Vec2 pStart, Vec2 pEnd, GameObject owner, uint pColor = 0xffffffff, uint pLineWidth = 1, bool pGlobalCoords = false)
        {
            _owner = owner;
            Start = pStart;
            End = pEnd;
            Color = pColor;
            LineWidth = pLineWidth;
            UseGlobalCoords = pGlobalCoords;
        }

        public LineSegment(Vec2 pStart, Vec2 pEnd, uint pColor = 0xffffffff, uint pLineWidth = 1,
            bool pGlobalCoords = false)
        {
            Start = pStart;
            End = pEnd;
            Color = pColor;
            LineWidth = pLineWidth;
            UseGlobalCoords = pGlobalCoords;
        }

        public GameObject GetOwner()
        {
            return _owner;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														RenderSelf()
        //------------------------------------------------------------------------------------------------------------------------
        protected override void RenderSelf(GLContext glContext)
        {
            if (game != null && Start != null && End != null)
            {
                RenderLine(Start, End, Color, LineWidth);
            }
        }

        public static void RenderLine(Vec2 pStart, Vec2 pEnd, uint pColor = 0xffffffff, uint pLineWidth = 1,
            bool pGlobalCoords = false)
        {
            RenderLine(pStart.x, pStart.y, pEnd.x, pEnd.y, pColor, pLineWidth, pGlobalCoords);
        }

        public static void RenderLine(float pStartX, float pStartY, float pEndX, float pEndY, uint pColor = 0xffffffff,
            uint pLineWidth = 1, bool pGlobalCoords = false)
        {
            if (pGlobalCoords) GL.LoadIdentity();
            GL.Disable(GL.TEXTURE_2D);
            GL.LineWidth(pLineWidth);
            GL.Color4ub((byte) ((pColor >> 16) & 0xff), (byte) ((pColor >> 8) & 0xff), (byte) ((pColor) & 0xff),
                (byte) ((pColor >> 24) & 0xff));
            float[] vertices = {pStartX, pStartY, pEndX, pEndY};
            GL.EnableClientState(GL.VERTEX_ARRAY);
            GL.VertexPointer(2, GL.FLOAT, 0, vertices);
            GL.DrawArrays(GL.LINES, 0, 2);
            GL.DisableClientState(GL.VERTEX_ARRAY);
            GL.Enable(GL.TEXTURE_2D);
        }
    }
}