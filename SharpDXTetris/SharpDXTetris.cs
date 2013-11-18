using System;
using System.Text;
using SharpDX;


namespace SharpDXTetris
{
    // Use these namespaces here to override SharpDX.Direct3D11
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;
    using System.Collections.Generic;

    /// <summary>
    /// Simple SharpDXTetris game using SharpDX.Toolkit.
    /// </summary>
    public class SharpDXTetris : Game
    {
        private GraphicsDeviceManager graphicsDeviceManager;

        public MainPage Page { get; set; }

        // Game settings

        public float Speed { get; set; }

        public float Focus { get; set; }
        private float actualFocus, focusLerpAmount = .08f;

        public float Rotation { get { return tetrisModel.Rotation; } }
        private float actualRotation, rotationLerpAmount = .08f;

        // Time between Tick() in tetris.
        private TimeSpan blockTick = TimeSpan.FromSeconds(.25f);
        private TimeSpan currentTime = TimeSpan.Zero;

        private PointerManager pointerManager;
        private KeyboardManager keyboardManager;

        private uint pointId;
        private Vector2? pointStart;

        // Game entities

        private TetrisModel tetrisModel; 

        // View objects
        private Matrix view, projection;
        private Model baseModel, blockModel;

        private Effect blockEffect;

        // Scale our models.
        private Matrix mirrorXZ = Matrix.Scaling(1, -1, -1);


        /// <summary>
        /// Initializes a new instance of the <see cref="SharpDXTetris" /> class.
        /// </summary>
        public SharpDXTetris()
        {
            // Creates a graphics manager. This is mandatory.
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            graphicsDeviceManager.PreferredBackBufferWidth = 1920;
            graphicsDeviceManager.PreferredBackBufferHeight = 1080;

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // Modify the title of the window
            Window.Title = "SharpDXTetris";

            // Variable initialization
            Speed = .1f;

            tetrisModel = new TetrisModel();

            pointerManager = new PointerManager(this);
            keyboardManager = new KeyboardManager(this);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            projection = Matrix.PerspectiveFovLH(MathUtil.DegreesToRadians(45f), 
                (float)GraphicsDevice.BackBuffer.Width / 
                (float)GraphicsDevice.BackBuffer.Height, 
                1f, 1000);

            baseModel = Content.Load<Model>(@"Models/base");
            blockModel = Content.Load<Model>(@"Models/block");
            BasicEffect.EnableDefaultLighting(baseModel);
            BasicEffect.EnableDefaultLighting(blockModel);

            blockEffect = Content.Load<Effect>(@"Shaders/BlockEffect");

            base.LoadContent();
        }

        internal void NewGame()
        {
            tetrisModel = new TetrisModel();
        }

        protected override void Update(GameTime gameTime)
        {
            // Update our model at appropriate time.
            currentTime += gameTime.ElapsedGameTime;
            while (currentTime > blockTick)
            {
                currentTime = currentTime.Subtract(blockTick);
                tetrisModel.Update(-Vector2.UnitY);
            }

            #region Input

            var state = pointerManager.GetState();

            var output = new StringBuilder();
            foreach (var point in state.Points)
                output.AppendLine(string.Format("Id:{0} Type:{1} Position:{2} Device:{3}", 
                    point.PointerId, point.EventType, point.Position, point.DeviceType));
            Page.output.Text = output.ToString();

            // No input
            if (!pointStart.HasValue)
            {
                // Find any touch moves?
                var point = state.Points.Find(p => p.DeviceType == PointerDeviceType.Touch && p.EventType != PointerEventType.CaptureLost);
                if (point != null)
                {
                    pointId = point.PointerId;
                    pointStart = point.Position;
                }
            }
            else
            {
                // Get our current point
                var point = state.Points.Find(p => p.PointerId == pointId);

                var diff = point.Position - pointStart.Value;
                if (Math.Abs(diff.X) > .2f)
                {
                    pointStart = point.Position;
                    tetrisModel.Update(diff.X > 0 ? Vector2.UnitX : -Vector2.UnitX);
                }
                
                
                // Released?
                if (point.EventType == PointerEventType.Released ||
                    point.EventType == PointerEventType.CaptureLost)
                {
                    pointStart = null;
                }
            }

            if (keyboardManager.GetState().IsKeyDown(Keys.Left))
                Left();
            if (keyboardManager.GetState().IsKeyDown(Keys.Right))
                Right();

            #endregion

            //TODO: Remove adjustable focus
            Focus = (int)Page.focus.Value;

            #region Update View

            // Update vertical focus
            actualFocus = MathUtil.Lerp(actualFocus, Focus, focusLerpAmount);

            // Update rotation
            actualRotation = MathUtil.Lerp(actualRotation, Rotation, rotationLerpAmount);

            // View matrix
            var eye = Vector3.TransformCoordinate(new Vector3(0, 50, -actualFocus / 1.2f - 200), Matrix.RotationY(actualRotation));
            var target = new Vector3(0, MathUtil.Lerp(actualFocus, 50, .5f), 0);
            view = Matrix.LookAtLH(eye, target, Vector3.Up);

            #endregion

            base.Update(gameTime);
        }

        internal void Left()
        {
            tetrisModel.Update(-Vector2.UnitX);
        }

        internal void Right()
        {
            tetrisModel.Update(Vector2.UnitX);
        }

        BasicEffect baseEffect;

        protected override void Draw(GameTime gameTime)
        {
            // Use time in seconds directly
            var time = (float)gameTime.TotalGameTime.TotalSeconds;

            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (baseEffect == null)
            {
                baseEffect = new BasicEffect(GraphicsDevice)
                {
                    DiffuseColor = Color.Salmon.ToVector4()
                };
                baseEffect.EnableDefaultLighting();
            }

            // Draw base model.
            baseModel.Draw(GraphicsDevice, mirrorXZ, view, projection, baseEffect);

            for (int row = 0; row < TetrisModel.Rows; row++)
            {
                for (int col = 0; col < TetrisModel.Columns; col++)
                {
                    var block = tetrisModel.BlockAt(row, col);
                    if (block.HasValue)
                    {
                        var world = Matrix.RotationY(col * MathUtil.TwoPi / (float)TetrisModel.Columns) * Matrix.Translation(0, -10 * row, 0) * mirrorXZ;
                        blockModel.ForEach(part => (part.Effect as BasicEffect).DiffuseColor = block.Value.ToVector4());
                        blockModel.Draw(GraphicsDevice, world, view, projection);
                    }
                }
            }

            //foreach (var block in tetrisModel.Blocks)
            //{
            //    var world = Matrix.RotationY(block.Position.X * (float)Math.PI / 5f) * Matrix.Translation(0, 10 * (currentBlock.Position.Y + block.Position.Y), 0) * Matrix.RotationZ(MathUtil.Pi);
            //    blockModel.ForEach(part => (part.Effect as BasicEffect).DiffuseColor = block.Color.ToVector4());
            //    blockModel.Draw(GraphicsDevice, world, view, projection);
            //}

            
            base.Draw(gameTime);
        }
    }
}
