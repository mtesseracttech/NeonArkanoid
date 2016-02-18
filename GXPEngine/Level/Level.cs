using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using GXPEngine.Utility;
using GXPEngine.UI;
using NeonArkanoid.Utility;
using GXPEngine.Utility.TiledParser;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Utils;
using NeonArkanoid.Physics;
using TiledParser;
using Polygon = NeonArkanoid.Physics.Polygon;


namespace NeonArkanoid.Level
{
    internal class Level : GameObject
    {
        private readonly Vec2 _acceleration = new Vec2(0, 0.1f); //Gravity
        private readonly Ball _ball;
        private PrivateFontCollection _fonts;
        private readonly NeonArkanoidGame _game;
        private readonly float maxSpeed = 10;

        private readonly string _levelName; //useless for now
        private readonly Map _map;

        private Font _Myfont;
        private Paddle _paddle;
        private List<Polygon> _polyList;
        private List<Ball> _bouncerBalls;
        private List<Polygon> _bumperList;
        private List<LineSegment> _borderList;
        private AnimationSprite _background;
        private Canvas _drawingField;

        private SoundChannel _musicChannel;
        private Sound _music;
        private Sound _hitPoly;
        private Sound _hitPaddle;

        private AnimationSprite[] _bumperSprites;
        private SolidBrush _brushTime;
        private SolidBrush _brushScore;

        private Color _colorTime;
        private Color _colorScore;

        private readonly HUD _hud;

        private int _lifes = 5;
        private int _oldLifes;
        private int _endTimer;
        private bool _gameEnded;

        private float _leftXBoundary;
        private float _rightXBoundary;
        private float _bottomYBoundary;

        private int _invincibilityTimer;
        private bool _stuckToPaddle;

        private int _score;
        private int _oldScore;
        private int _timerMinutes;
        private int _timerSeconds;
        private float _topYBoundary;
        private readonly float height;

        private int seconds, minutes;

        private readonly float width;

        private float _currentFrame;
        private readonly float _currentSpeed = 5f; // change the speed of animation


        public Level(string filename, NeonArkanoidGame game) //: base(game.width, game.height)
        {
            width = game.width;
            height = game.height;

            SetBackground();
            _hud = new HUD(_lifes,this);
            SetPolyField();
            SetTextBoxSettings();
            BoundaryCreator();
            SetSounds();

            _gameEnded = false;
            _levelName = filename.Remove(filename.Length - 4);
            //Console.WriteLine(_levelName);
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

            _ball = new Ball(30, new AnimationSprite(UtilStrings.SpritesPlayer + "ball.png", 5, 1), new Vec2(200, 200));
            AddChild(_ball);
            _stuckToPaddle = true;

            _paddle = new Paddle(this, new Vec2(game.width/2, game.height - 100));
            AddChild(_paddle);
            Redraw();
        }

        private void SetSounds()
        {
            _hitPoly = new Sound(UtilStrings.SoundsLevel + "1_tile hit.wav");
            _hitPaddle = new Sound(UtilStrings.SoundsLevel + "1_pedal hit.wav");
            _music = new Sound(UtilStrings.SoundsLevel + "8.wav", true, true);
            _musicChannel = _music.Play();

        }

        private void SetTextBoxSettings()
        {
            _fonts = new PrivateFontCollection();
            _fonts.AddFontFile("agency_fb.ttf");
            _Myfont = new Font(_fonts.Families[0], 30);
            _colorTime = Color.FromArgb(255, Color.White);
            _brushTime = new SolidBrush(_colorTime);
            _colorScore = Color.FromArgb(100, ColorUtils.UIntToColor(0xFF4CDDFF));
            _brushScore = new SolidBrush(_colorScore);
        }

        private void SetPolyField()
        {
            _drawingField = new Canvas(game.width, game.height);
            _drawingField.graphics.SmoothingMode = SmoothingMode.HighQuality;
            AddChild(_drawingField);
        }

        private void SetBackground()
        {
            _background = new AnimationSprite(UtilStrings.SpritesBack + "background game.png", 7, 3);
            AddChild(_background);
        }

        private void AnimationForBackground()
        {
            _currentFrame += _currentSpeed / 50;
            _currentFrame %= _background.frameCount;
            _background.SetFrame((int)_currentFrame);
        }

        private void AnimtaionForBumperRound()
        {

            _currentFrame += _currentSpeed / 50;
            _currentFrame %= _bumperSprites[0].frameCount;
            _bumperSprites[0].SetFrame((int)_currentFrame);
            _currentFrame %= _bumperSprites[1].frameCount;
            _bumperSprites[1].SetFrame((int)_currentFrame);

        }

        public void Update()
        {
            if (_polyList.Count > 0) //IN THIS BLOCK, ALL THE CODE THAT HAPPENS WHILE THE GAME PLAYS FITS IN
            {

                Tickers();
                RenderVisuals();
                Controls();
                LimitBallSpeed();
                ApplyForces();
                CollisionDetections();
                ExceptionalMovement();
                DebugInfo();
            }
            else
            {
                ReturnTime();
                ReturnScore();
                ReturnLifes();
                EndRound();
            }
            AnimationForBackground();
            AnimtaionForBumperRound();
            BallLoseLife();

        }

        private void ExceptionalMovement()
        {
            if (_stuckToPaddle)
            {
                _ball.Acceleration = Vec2.zero;
                _ball.Velocity = Vec2.zero;
                _ball.Position.SetXY(_paddle.Position.x, _paddle.Position.y - _ball.radius - 1);
                _ball.Step();
            }
        }

        private void Tickers()
        {
            _timerSeconds++;
            if (_invincibilityTimer > 0) _invincibilityTimer--;
        }

        private void RenderVisuals()
        {
            CheckTimer();
            CheckScore();

            SpriteEffects();
        }

        private void SpriteEffects()
        {
            BallEffects();
        }

        private void BallEffects()
        {
            if (_invincibilityTimer > 0) _ball.GetSprite().alpha = 0.5f;
            else _ball.GetSprite().alpha = 1f;
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
            foreach (var polygon in _bumperList) AddChild(polygon);
            _bumperSprites = new[]
            {
                new AnimationSprite(UtilStrings.SpritesObject + "bumper double.png", 17, 1),
                new AnimationSprite(UtilStrings.SpritesObject + "bumper double.png", 17, 1)
            };


            _bumperSprites[0].SetXY(0, game.height - _bumperSprites[0].height);
            _bumperSprites[1].SetXY(game.width - _bumperSprites[1].width, game.height - _bumperSprites[1].height);
            _bumperSprites[1].Mirror(true, false);
            foreach (var bumperSprite in _bumperSprites) AddChild(bumperSprite);
        }

        private void AddBouncerBalls()
        {
            _bouncerBalls = new List<Ball>();
            _bouncerBalls.Add(new Ball(45, new AnimationSprite(UtilStrings.SpritesObject + "bumper round.png", 19,1), new Vec2(100, 100)));
            _bouncerBalls.Add(new Ball(45, new AnimationSprite(UtilStrings.SpritesObject + "bumper round.png", 19, 1), new Vec2(game.width - 100, 100)));
            foreach (var bouncerBall in _bouncerBalls) AddChild(bouncerBall);
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
            var time = minutes.ToString("00") + ":" + seconds.ToString("00");

            _drawingField.graphics.DrawString(time, _Myfont, _brushTime, new PointF(game.width / 2, 20));
            _drawingField.graphics.DrawString(_score.ToString("0000"), _Myfont, _brushScore, new PointF(game.width / 2 + 2, 60));
            _drawingField.graphics.DrawString(_lifes.ToString(), _Myfont, _brushTime, new PointF(game.width / 8, 20));
        }

        private void LoseLife(bool returnToPaddle = true)
        {
            if (_invincibilityTimer <= 0)
            {
                _lifes--;
                _invincibilityTimer = 60;
                if(returnToPaddle) _stuckToPaddle = true;
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
            //Console.WriteLine(_invincibilityTimer);
        }

        public Canvas GetPolyField()
        {
            return _drawingField;
        }

        private void ApplyForces()
        {
            if (!_stuckToPaddle)
            {
                _ball.Acceleration = _acceleration;
                _ball.Velocity.Add(_ball.Acceleration);
                _ball.Position.Add(_ball.Velocity);
                _ball.Step();
            }
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
                                _hitPoly.Play();
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
                        _hitPaddle.Play();
                        //What happens when the ball hits the pedal
                    }
                }


                //Temporary collisions with the borders of the game
                foreach (var line in _borderList)
                {
                    if (LineCollisionTest(line, 1f))
                    {
                        _hitPoly.Play();
                        //What happens when the ball hits a border  
                    }
                }
                foreach (var ball in _bouncerBalls)
                {
                    if (BallCollisionTest(ball, 1f))
                    {
                        //What happens when the ball bounces against a bouncer ball
                        //ADDING THE SCORE
                        //_score += 10;
                        LoseLife();
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
                            _hitPoly.Play();
                        }
                    }
                }

                if (_ball.Position.y > game.height + _ball.radius*2)//Not really a collision, but still :)
                {
                    LoseLife(); 
                }


                //AND BEFORE THIS ONE
            }
        }

        private void EndRound()
        {
            
            //Triggers the end of the game and sets counter until game pops to different state/does something
            if (_gameEnded == false)
            {
                _game.StartWinScreen();//calls the win screen
                _endTimer = Time.now;
                _gameEnded = true;
            }
            //Sets the game to the main menu after the set time is over
            if (_gameEnded && _endTimer + 5000 < Time.now)
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
            if (Input.GetKeyDown(Key.B)) _stuckToPaddle = !_stuckToPaddle;
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
            if (_stuckToPaddle)
            {
                if (Input.GetKeyDown(Key.F))
                {
                    _stuckToPaddle = false;
                    _ball.Velocity = new Vec2(0, 10);
                }
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
            //_bottomYBoundary = height - border;

            _borderList = new List<LineSegment>();
            CreateVisualXBoundary(_leftXBoundary);
            CreateVisualXBoundary(_rightXBoundary);
            CreateVisualYBoundary(_topYBoundary);
            //CreateVisualYBoundary(_bottomYBoundary);
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

        private void CheckTimer()
        {
            bool hasChanged = false;
            int oldSeconds = seconds;
            int oldMinutes = minutes;

            seconds = Mathf.Floor(_timerSeconds/120);

            if (seconds > 59)
            {
                _timerSeconds = 0;
                minutes++;
            }
            if (minutes > 59) _timerMinutes = 0;

            if (seconds != oldSeconds || minutes != oldMinutes)
            {
                Redraw();
            }
        }

        private void CheckScore()
        {
            if (_oldScore != _score)
            {
                Redraw();
                _oldScore = _score;
            }

        }

        private void EndRoundGameOver()
        {

            //Triggers the end of the game and sets counter until game pops to different state/does something
            if (_gameEnded == false)
            {
                _game.StartGameOver();
                _endTimer = Time.now;
                _gameEnded = true;
            }
            //Sets the game to the main menu after the set time is over
            if (_gameEnded && _endTimer + 5000 < Time.now)
            {
                _game.SetState("MainMenu");

            }
        }

        private void BallLoseLife()
        {
            if (_lifes < 0)
            {
                EndRoundGameOver();
                
            }
            else
            {
                _hud.SetHearts(_lifes);
            }
        }

        public void StopMusic()
        {
            _musicChannel.Stop();
        }

    }
}
 