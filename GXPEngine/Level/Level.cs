using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using Glide;
using GXPEngine.Utility.TiledParser;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Utils;
using NeonArkanoid.Physics;
using NeonArkanoid.UI.Menu;
using TiledParser;
using Polygon = NeonArkanoid.Physics.Polygon;

namespace NeonArkanoid.Level
{
    internal class Level : GameObject
    {
        private readonly Ball _ball;
        private readonly NeonArkanoidGame _game;

        private readonly string _levelName; //useless for now
        private readonly Map _map;
        private readonly Paddle _paddle;
        private readonly List<Polygon> _polyList;
        private readonly Tweener _tweener = new Tweener();
        private readonly Vec2 _acceleration = new Vec2(0, 0.1f); //Gravity
        private readonly float maxSpeed = 10;

        private AnimationSprite _background;
        private readonly PrivateFontCollection _fonts;
        private  SolidBrush _brushTime,_brushScore;
        private readonly Font _Myfont;
        private Color _colorTime, _colorScore;

        private Canvas _drawingField;

        private List<LineSegment> _borderList;

        private float _bottomYBoundary;
        private List<Ball> _bouncerBalls;
        private List<Polygon> _bumperList;

        //private Canvas _go1 = new Canvas("../assets/sprite/ui/GO1.png");
        //private Canvas _go2 = new Canvas("../assets/sprite/ui/GO2.png");

        private int _endTimer;
        private bool _gameEnded;

        private float _leftXBoundary;
        private readonly int _lifes = 5;
        private float _rightXBoundary;

        private float width;
        private float height;

        private int _score;
        private int _timerMinutes;
        private int _timerSeconds;
        private float _topYBoundary;

        private int seconds, minutes;

        public Level(string filename, NeonArkanoidGame game) //: base(game.width, game.height)
        {
            width = game.width;
            height = game.height;

            SetBackground();
            SetPolyField();

            _fonts = new PrivateFontCollection();
            _fonts.AddFontFile("agency_fb.ttf");
            _Myfont = new Font((FontFamily)_fonts.Families[0], 30);

            _colorTime = Color.FromArgb(255, Color.White);
            _brushTime = new SolidBrush(_colorTime);
            _colorScore = Color.FromArgb(50, Color.DeepSkyBlue);
            _brushScore = new SolidBrush(_colorScore);

            BoundaryCreator();

            _gameEnded = false;
            _levelName = filename.Remove(filename.Length - 4);
            Console.WriteLine(_levelName);
            _game = game;

            var tmxParser = new TMXParser();
            _map = tmxParser.Parse(filename);

            for (var i = 0; i < _map.ObjectGroup.Length; i++)
            {
                if (_map.ObjectGroup[i].Name.ToLower() == "polygons" || _map.ObjectGroup[i].Name.ToLower() == "polygon")
                {
                    _polyList = new List<Polygon>();
                    CreatePolygons(_map.ObjectGroup[i]);
                }
            }
            foreach (var polygon in _polyList)
            {
                AddChild(polygon);
            }

            AddBouncerBalls();
            AddBumpers();

            _ball = new Ball(30, new Vec2(200, 200));
            AddChild(_ball);

            _paddle = new Paddle(this, new Vec2(game.width/2, game.height - 100));
            AddChild(_paddle);
        }

        private void SetPolyField()
        {
            _drawingField = new Canvas(game.width, game.height);
            _drawingField.graphics.SmoothingMode = SmoothingMode.HighQuality;
            AddChild(_drawingField);
        }

        private void SetBackground()
        {
            _background = new AnimationSprite("../assets/sprite/background/background game.png", 21, 1);
            AddChild(_background);
        }

        public void Update()
        {
            _timerSeconds++;
            _timerMinutes++;

            if (_polyList.Count > 0) //IN THIS BLOCK, ALL THE CODE THAT HAPPENS WHILE THE GAME PLAYS FITS IN
            {
                Redraw();
                DrawScore();
                DrawLifes();
                Controls();
                LimitBallSpeed();
                ApplyForces();
                CollisionDetections();
                DebugInfo();

            }
            else
            {
                ReturnTime();
                ReturnScore();
                ReturnLifes();
                EndRound();
            }
        }

        private void AddBumpers()
        {
            _bumperList = new List<Polygon>();
            _bumperList.Add(
                new Polygon(
                    new[]
                    {
                        new Vec2(0, game.height - 200),
                        new Vec2(0, game.height),
                        new Vec2(50, game.height)
                    }, 0xFF00FFFF, this, 20, 0, 0));
            _bumperList.Add(
                new Polygon(
                    new[]
                    {
                        new Vec2(game.width, game.height - 200),
                        new Vec2(game.width, game.height),
                        new Vec2(game.width - 50, game.height)
                    }, 0xFF00FFFF, this, 20, 0, 0));
            _bumperList.Add(
                new Polygon(
                    new[]
                    {
                        new Vec2(0, game.height - 50),
                        new Vec2(0, game.height),
                        new Vec2(200, game.height)
                    }, 0xFF00FFFF, this, 20, 0, 0));
            _bumperList.Add(new Polygon(
                new[]
                {
                    new Vec2(game.width, game.height - 50),
                    new Vec2(game.width, game.height),
                    new Vec2(game.width - 200, game.height)
                }, 0xFF00FFFF, this, 20, 0, 0));
            foreach (var polygon in _bumperList)
            {
                AddChild(polygon);
            }
        }

        private void AddBouncerBalls()
        {
            _bouncerBalls = new List<Ball>();
            _bouncerBalls.Add(new Ball(50, new Vec2(100, 100)));
            _bouncerBalls.Add(new Ball(50, new Vec2(game.width - 100, 100)));
            for (var i = 0; i < _bouncerBalls.Count; i++)
            {
                _bouncerBalls[i].BallColor = Color.Green;
                AddChild(_bouncerBalls[i]);
            }
        }

        private void CreatePolygons(ObjectGroup objectGroup)
        {
            foreach (var tiledObject in objectGroup.TiledObjects)
            {
                if (tiledObject.Polygon != null)
                {
                    var pointArr = tiledObject.Polygon.Points.Split(' ');
                    var vectorArray = new Vec2[pointArr.Length];
                    for (var i = 0; i < pointArr.Length; i++)
                    {
                        var pointCoords = pointArr[i].Split(',');
                        vectorArray[i] = new Vec2(
                            float.Parse(pointCoords[0], CultureInfo.InvariantCulture.NumberFormat),
                            float.Parse(pointCoords[1], CultureInfo.InvariantCulture.NumberFormat));
                    }

                    uint polyColor = 0;
                    if (tiledObject.Properties != null)
                    {
                        foreach (var property in tiledObject.Properties)
                        {
                            if (property.Property.Name.ToLower() == "colour" ||
                                property.Property.Name.ToLower() == "color")
                            {
                                polyColor = Convert.ToUInt32(property.Property.Value, 16) + 0xFF000000;
                            }
                        }
                        var poly = new Polygon(vectorArray, polyColor, this, tiledObject.ID, tiledObject.X,
                            tiledObject.Y);
                        poly.SetXY(tiledObject.X, tiledObject.Y);
                        _polyList.Add(poly);
                    }
                }
            }
        }

        public void RemovePoly(Polygon poly)
        {
            poly.Destroy();
            _polyList.Remove(poly);
            Redraw();
        }

        public void RemovePolyAt(int index)
        {
            _polyList[index].Destroy();
            _polyList.RemoveAt(index);
            Redraw();
        }

        public void Redraw()
        {
            _drawingField.graphics.Clear(Color.FromArgb(0x00000000));
            foreach (var polygon in _polyList)
            {
                polygon.DrawOnCanvas();
            }
            foreach (var polygon in _bumperList)
            {
                polygon.DrawOnCanvas();
            }
            DrawTimer();
        }

        private int ReturnLifes()
        {
            return _lifes;
        }
        private int ReturnTime()
        {
            return _timerSeconds | _timerMinutes;
        }
        private int ReturnScore()
        {
            return _score;
        }

        private void DebugInfo()
        {
            //if (UtilitySettings.DebugMode) Console.WriteLine(_ball.Velocity.Length());
        }

        public Canvas GetPolyField()
        {
            return _drawingField;
        }

        private void ApplyForces()
        {
            _ball.Acceleration = _acceleration;
            _ball.Velocity.Add(_ball.Acceleration);
            _ball.Position.Add(_ball.Velocity);
            _ball.Step();
        }

        /// <summary>
        ///     In this block of code, the different collisions are checked.
        ///     The preceding for-loop cuts the velocity into pieces to make collision scanning on higher speeds more accurate.
        ///     New collisionchecks are added by scanning the objects of the following types (or that contain these types):
        ///     -Ball
        ///     -LineSegment
        ///     Place a single object in an if/else-block and let the corresponding collisiontest return a value,
        ///     based on that, add the actions that follow the collision in the if/else block.
        /// </summary>
        private void CollisionDetections()
        {
            //Ball velocity gets choppped into pieces to make sure that the hit detection works on higher speeds
            for (var i = 0; i < _ball.Velocity.Length(); i++)
            {
                _ball.Position.Add(_ball.Velocity.Clone().Normalize());
                _ball.Step();

                //ALL COLLISION DETECTIONS COME AFTER THIS POINT

                //Collisions with the vector polygons
                if (_polyList.Count > 0)
                {
                    for (var p = 0; p < _polyList.Count; p++)
                    {
                        for (var l = 0; l < _polyList[p].GetLines().Length; l++)
                        {
                            if (LineCollisionTest(_polyList[p].GetLines()[l], 0.5f))
                            {
                                //ADDING THE SCORE
                                _score += 1;
                                //What happens when the ball hits a polygon
                                _polyList[p].RemovePoly();
                                break; //Needed to avoid ArgumentOutOfRangeException
                            }
                        }
                    }
                }


                //Collisions with the paddle
                foreach (var line in _paddle.GetLines())
                {
                    if (LineCollisionTest(line, 1f))
                    {
                        //What happens when the ball hits the pedal
                    }
                }


                //Temporary collisions with the borders of the game
                foreach (var line in _borderList)
                {
                    if (LineCollisionTest(line, 1f))
                    {
                        //What happens when the ball hits a border  
                    }
                }
                foreach (var ball in _bouncerBalls)
                {
                    if (BallCollisionTest(ball, 1f))
                    {
                        //What happens when the ball bounces against a bouncer ball
                        //ADDING THE SCORE
                        _score += 10;
                    }
                }

                foreach (var polygon in _bumperList)
                {
                    foreach (var lineSegment in polygon.GetLines())
                    {
                        if (LineCollisionTest(lineSegment, 1f))
                        {
                            _ball.Velocity.Normalize().Scale(20);
                            //ADDING THE SCORE
                            _score += 3;
                        }
                    }
                }
                //AND BEFORE THIS ONE
            }
        }

        private void EndRound()
        {
            //Triggers the end of the game and sets counter until game pops to different state/does something
            if (_gameEnded == false)
            {
                _endTimer = Time.now;
                _gameEnded = true;
            }
            //Sets the game to the main menu after the set time is over
            if (_gameEnded && _endTimer + 2000 < Time.now)
            {
                _game.SetState("MainMenu");
            }
        }

        private void Controls()
        {
            if (Input.GetKey(Key.UP)) _ball.Velocity.y += -1; //FOR DEBUG ONLY
            if (Input.GetKey(Key.DOWN)) _ball.Velocity.y += 1; //FOR DEBUG ONLY
            if (Input.GetKey(Key.LEFT)) _ball.Velocity.x += -1; //FOR DEBUG ONLY
            if (Input.GetKey(Key.RIGHT)) _ball.Velocity.x += 1; //FOR DEBUG ONLY
            if (Input.GetKeyDown(Key.R)) if (_polyList.Count > 0) RemovePolyAt(0); //FOR DEBUG ONLY
            if (Input.GetKeyDown(Key.T)) _game.SetState("Level1", true); //FOR DEBUG ONLY

            if (Input.GetKey(Key.D))
            {
                _paddle.Position.x += 10;
                _paddle.Step();
            }
            if (Input.GetKey(Key.A))
            {
                _paddle.Position.x -= 10;
                _paddle.Step();
            }
        }

        /// <summary>
        ///     Handles Collisions and reflection between the ball and a given line.
        /// </summary>
        /// <param name="line">
        ///     Is the Line that the ball will be checked against.
        /// </param>
        /// <param name="reflectionStrength">
        ///     Is the amount of reflection that will be given to the ball,
        ///     1 is a perfect bounce, 0 makes the ball not bounce at all.
        /// </param>
        /// <returns>Returns true when a collision was actually detected</returns>
        private bool LineCollisionTest(LineSegment line, float reflectionStrength)
        {
            var lineVector = line.End.Clone().Subtract(line.Start);
            var lineVectorNormalized = lineVector.Clone().Normalize();
            var lineNormal = lineVector.Clone().Normal();
            var lineLength = lineVector.Length();
            var ballToLine = _ball.Position.Clone().Subtract(line.Start);
            var ballDistance = ballToLine.Dot(lineNormal);
            if (Math.Abs(ballDistance) < _ball.radius)
            {
                var ballDistanceAlongLine = ballToLine.Dot(lineVectorNormalized);
                if (ballDistanceAlongLine < 0) ballDistanceAlongLine = 0;
                if (ballDistanceAlongLine > lineLength) ballDistanceAlongLine = lineLength;
                var closestPointOnLine = line.Start.Clone().Add(lineVectorNormalized.Scale(ballDistanceAlongLine));
                var difference = _ball.Position.Clone().Subtract(closestPointOnLine);
                if (difference.Length() < _ball.radius)
                {
                    var normal = difference.Clone().Normalize();
                    var separation = _ball.radius - difference.Length();
                    _ball.Position.Add(normal.Clone().Scale(separation));
                    _ball.Velocity.Reflect(normal, reflectionStrength);
                    _ball.Step();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Handles Collisions and reflection between the ball and a given ball.
        /// </summary>
        /// <param name="ball">
        ///     Is a ball that the ball will be checked against.
        /// </param>
        /// <param name="reflectionStrength">
        ///     Is the amount of reflection that will be given to the ball,
        ///     1f is a perfect bounce, 0f makes the ball not bounce at all.
        /// </param>
        /// <returns>Returns true when a collision was actually detected</returns>
        private bool BallCollisionTest(Ball ball, float reflectionStrength)
        {
            if (_ball.Position.Clone().Subtract(ball.Position).Length() <= _ball.radius + ball.radius)
            {
                var delta = _ball.Position.Clone().Subtract(ball.Position);
                var distToCap = delta.Clone().Length();
                var deltaNormalized = delta.Clone().Normalize();
                _ball.Position.SetVec(
                    _ball.Position.Add(deltaNormalized.Clone().Scale(-distToCap + _ball.radius + ball.radius)));
                _ball.Velocity.Reflect(deltaNormalized, reflectionStrength);
                _ball.Step();
                return true;
            }
            return false;
        }

        private void LimitBallSpeed()
        {
            if (_ball.Velocity.Length() > maxSpeed) _ball.Velocity.Normalize().Scale(maxSpeed);
        }

        public string GetLevelName()
        {
            return _levelName;
        }

        public void BoundaryCreator()
        {
            float border = 1;
            _leftXBoundary = border;
            _rightXBoundary = width - border;
            _topYBoundary = border;
            _bottomYBoundary = height - border;

            _borderList = new List<LineSegment>();
            CreateVisualXBoundary(_leftXBoundary);
            CreateVisualXBoundary(_rightXBoundary);
            CreateVisualYBoundary(_topYBoundary);
            CreateVisualYBoundary(_bottomYBoundary);
            foreach (var lineSegment in _borderList)
            {
                AddChild(lineSegment);
            }
        }

        private void CreateVisualXBoundary(float xBoundary)
        {
            _borderList.Add(new LineSegment(xBoundary, 0, xBoundary, height, 0xffffffff, 1));
        }

        private void CreateVisualYBoundary(float yBoundary)
        {
            _borderList.Add(new LineSegment(0, yBoundary, width, yBoundary, 0xffffffff, 1));
        }

        private void DrawTimer()
        {
            seconds = Mathf.Floor(_timerSeconds/60);
            minutes = Mathf.Floor(_timerMinutes/3600);
            if (seconds > 59)
            {
                _timerSeconds = 0;
            }
            if (minutes > 59)
            {
                _timerMinutes = 0;
            }
            var time = minutes.ToString("00") + ":" + seconds.ToString("00");

            _drawingField.graphics.DrawString(time, _Myfont, _brushTime, new PointF(game.width/2, 20));
        }

        private void DrawScore()
        {
            _drawingField.graphics.DrawString(_score.ToString("0000"), _Myfont, _brushScore, new PointF(game.width/2 + 2, 60));   
        }

        private void DrawLifes()
        {
            _drawingField.graphics.DrawString(_lifes.ToString(), _Myfont, _brushTime, new PointF(game.width / 8, 20));
           
        }
    }
}