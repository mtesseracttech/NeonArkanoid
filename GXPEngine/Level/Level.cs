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
using NeonArkanoid.Utility;
using TiledParser;
using Polygon = NeonArkanoid.Physics.Polygon;

namespace NeonArkanoid.Level
{
    internal class Level : Canvas
    {
        private readonly  Tweener _tweener = new Tweener();
        private readonly Ball _ball;
        private readonly NeonArkanoidGame _game;
        private readonly Map _map;
        private readonly Paddle _paddle;
        private readonly List<Polygon> _polyList;
        private readonly Vec2 acceleration = new Vec2(0, 0.1f); //Gravity
        private readonly float maxspeed = 10;

        private Background _background1;

        private float _bottomYBoundary;

        private List<LineSegment> _borderList;
        private List<Ball> _bouncerBalls;
        private List<Polygon> _bumperList;

        private Canvas _go1 = new Canvas("../assets/sprite/ui/GO1.png");
        private Canvas _go2 = new Canvas("../assets/sprite/ui/GO2.png");

        private int _endTimer;
        private bool _gameEnded;

        private int seconds, minutes;
        private int _timerSeconds = 0;
        private int _timerMinutes = 0;


        private int _score = 0;
        private int _lifes = 5;
        
        private float _leftXBoundary;
        private float _rightXBoundary;
        private float _topYBoundary;

        private readonly string _levelName; //useless for now

        public Level(string filename, NeonArkanoidGame game) : base(game.width, game.height)
        {
            _gameEnded = false;
            BoundaryCreator();
            _levelName = filename.Remove(filename.Length - 4);
            Console.WriteLine(_levelName);
            _game = game;
            graphics.SmoothingMode = SmoothingMode.HighQuality;

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

            _ball = new Ball(30, new Vec2(game.width/2, game.height/2));
            AddChild(_ball);

            _paddle = new Paddle(this, new Vec2(game.width/2, game.height - 100));
            AddChild(_paddle);
            
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
                        new Vec2(game.width,game.height),
                        new Vec2(game.width-50, game.height)
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
                                //var chars = property.Property.Value.ToLower().ToCharArray().ToList();
                                //chars.Insert(2, 'f');
                                //chars.Insert(2, 'f');
                                //polyColor = Convert.ToUInt32(new string(chars.ToArray()), 16);
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
            graphics.Clear(Color.Black);
            foreach (var polygon in _polyList)
            {
                polygon.DrawOnCanvas();
            }
            foreach (var polygon in _bumperList)
            {
                polygon.DrawOnCanvas();
            }

        }

        public void Update()
        {
            
            if (_polyList.Count > 0) //IN THIS BLOCK, ALL THE CODE THAT HAPPENS WHILE THE GAME PLAYS FITS IN
            {
                _timerSeconds ++;
                _timerMinutes ++;
                Redraw();
                DrawTimer();
                DrawScore();
                DrawLifes();
                Controls();
                //LimitBallSpeed();
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
            if (UtilitySettings.DebugMode) Console.WriteLine(_ball.Velocity.Length());
        }

        private void ApplyForces()
        {
            _ball.Acceleration = acceleration;
            _ball.Velocity.Add(_ball.Acceleration);
            _ball.Position.Add(_ball.Velocity);
            _ball.Step();
        }

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
                        _score  +=10;
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
            if (Input.GetKey(Key.UP)) _ball.Velocity.y += -1;
            if (Input.GetKey(Key.DOWN)) _ball.Velocity.y += 1;

            if (Input.GetKey(Key.LEFT)) _ball.Velocity.x += -1;
            if (Input.GetKey(Key.RIGHT)) _ball.Velocity.x += 1;

            if (Input.GetKeyDown(Key.R)) if (_polyList.Count > 0) RemovePolyAt(0);

            if (Input.GetKeyDown(Key.T)) _game.SetState("Level1", true);

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
            if (_ball.Velocity.x > maxspeed) _ball.Velocity.x = maxspeed;
            if (_ball.Velocity.x < -maxspeed) _ball.Velocity.x = -maxspeed;
            if (_ball.Velocity.y > maxspeed) _ball.Velocity.y = maxspeed;
            if (_ball.Velocity.y < -maxspeed) _ball.Velocity.y = -maxspeed;
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

        /**
        private void SetBackground()
        {
            _background1 = new Background(UtilStrings.SpritesMenu + "background4.jpg", true);
            AddChild(_background1);
        }
        /**/

        private void DrawTimer()
        {
            seconds = Mathf.Floor(_timerSeconds / 60);
            minutes = Mathf.Floor(_timerMinutes/3600);
            if (seconds > 59)
            {
                _timerSeconds = 0;
            }
            if (minutes > 59)
            {
                _timerMinutes = 0;
            }
            string time = minutes.ToString("00") + ":" + seconds.ToString("00");



            var brush = new SolidBrush(Color.FromArgb(255, 255, 255, 255));

            var fonts = new PrivateFontCollection();
            fonts.AddFontFile("agency_fb.ttf");

            var myFont = new Font((FontFamily)fonts.Families[0], 30);
            graphics.DrawString(time, myFont, brush, new PointF(game.width/2, 20));
        }

        private void DrawScore()
        {

            var brush = new SolidBrush(Color.FromArgb(255, 4, 255, 255));

            var fonts = new PrivateFontCollection();
            fonts.AddFontFile("agency_fb.ttf");

            var myFont = new Font((FontFamily)fonts.Families[0], 30);
            
            graphics.DrawString(_score.ToString("0000"), myFont, brush, new PointF(game.width/2 + 5, 60));
        }

        private void DrawLifes()
        {
            var brush = new SolidBrush(Color.FromArgb(255, 255, 20 , 20));

            var fonts = new PrivateFontCollection();
            fonts.AddFontFile("agency_fb.ttf");

            var myFont = new Font((FontFamily)fonts.Families[0], 30);

            graphics.DrawString(_lifes.ToString(), myFont, brush, new PointF(game.width / 8, 20));
        }
    }
}