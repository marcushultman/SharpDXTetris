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

        // Time between Tick() in tetris.
        private TimeSpan blockTick = TimeSpan.FromSeconds(1);
        private TimeSpan currentTime = TimeSpan.Zero;

        private PointerManager pointerManager;
        private Vector2? touchStart;
        private uint touchID;

        // Game entities

        private TetrisModel tetrisModel; 

        // View objects
        private Matrix view, projection;
        private Model baseModel, blockModel;

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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            projection = Matrix.PerspectiveFovLH(MathUtil.DegreesToRadians(45f), (float)GraphicsDevice.BackBuffer.Width / (float)GraphicsDevice.BackBuffer.Height, 
                1f, 1000);

            baseModel = Content.Load<Model>(@"Models/base");
            blockModel = Content.Load<Model>(@"Models/block");
            BasicEffect.EnableDefaultLighting(baseModel);
            BasicEffect.EnableDefaultLighting(blockModel);

            base.LoadContent();
        }

        internal void NewGame()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            // Update our model at appropriate time.
            currentTime += gameTime.ElapsedGameTime;
            while (currentTime > blockTick)
            {
                currentTime = currentTime.Subtract(blockTick);
                tetrisModel.Tick(-Vector2.UnitY);
            }

            //var state = pointerManager.GetState();
            //if (state.Points.Count > 0 && !touchStart.HasValue)
            //{
            //    touchStart = state.Points[0].Position;
            //    touchID = state.Points[0].PointerId;
            //}
            
            //if (touchStart.HasValue){
            //    var current = state.Points.Find(point => point.PointerId == touchID).Position;
            //    var diff = current - touchStart;
            //    if (diff.Value.Length() > 100)
            //    {
            //        tetrisModel.Tick(diff.Value.X > 0 ? Vector2.UnitX : -Vector2.UnitX);
            //        touchStart = null;
            //    }
            //}

            var state = pointerManager.GetState();
            var presses = state.Points.FindAll(p => p.EventType == PointerEventType.Pressed);

            if (presses.Count > 0)
            {
                ;
            }



            //TODO: Remove adjustable focus
            Focus = (int)Page.focus.Value;

            #region Update View

            // Update vertical focus
            actualFocus = MathUtil.Lerp(actualFocus, Focus, focusLerpAmount);

            // View matrix
            var eye = new Vector3(0, 50, -actualFocus / 1.2f - 200);
            var target = new Vector3(0, MathUtil.Lerp(actualFocus, 50, .5f), 0);
            view = Matrix.LookAtLH(eye, target, Vector3.Up);

            #endregion

            base.Update(gameTime);
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

            #region Coordinate indicators

            baseEffect.DiffuseColor = Color.Red.ToVector4();
            baseModel.Draw(GraphicsDevice, Matrix.Scaling(.1f) * Matrix.Translation(50, 0, 0), view, projection, baseEffect);

            baseEffect.DiffuseColor = Color.Green.ToVector4();
            baseModel.Draw(GraphicsDevice, Matrix.Scaling(.1f) * Matrix.Translation(0, 50, 0), view, projection, baseEffect);

            baseEffect.DiffuseColor = Color.Blue.ToVector4();
            baseModel.Draw(GraphicsDevice, Matrix.Scaling(.1f) * Matrix.Translation(0, 0, 50), view, projection, baseEffect);

            #endregion


            for (int row = 0; row < 20; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    var block = tetrisModel.BlockAt(row, col);
                    if (block.HasValue)
                    {
                        var world = Matrix.RotationY(col * (float)Math.PI / 5f) * Matrix.Translation(0, -10 * row, 0) * mirrorXZ;
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
