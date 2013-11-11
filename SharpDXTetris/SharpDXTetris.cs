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

        private TimeSpan blockTick = TimeSpan.FromSeconds(1);

        private TimeSpan currentTime = TimeSpan.Zero;


        // Game entities

        private LinkedList<List<TetrisBlock>> blocks;
        private CurrentTetrisBlock currentBlock;

        // View objects
        private Matrix view, projection;
        private Model baseModel, blockModel;


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

            NewGame();

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
            blocks = new LinkedList<List<TetrisBlock>>();
            for (int i = 0; i < 20; i++)
                blocks.AddFirst(new List<TetrisBlock>());

            currentBlock = TetrisBlockGenerator.GetBlock((int)Page.blockstyle.Value);
            currentBlock.Position = new Vector2(0, 0);//20);
        }

        protected override void Update(GameTime gameTime)
        {
            currentTime += gameTime.ElapsedGameTime;
            while (currentTime > blockTick)
            {
                currentTime = currentTime.Subtract(blockTick);
                //currentBlock.Position -= Vector2.UnitY;
            }   

            // Use time in seconds directly
            var time = (float)gameTime.TotalGameTime.TotalSeconds;

            Focus = (int)Page.focus.Value;

            // Update vertical focus
            actualFocus = MathUtil.Lerp(actualFocus, Focus, focusLerpAmount);

            // View matrix
            var eye = new Vector3(0, 50, actualFocus / 1.2f + 200);
            var target = new Vector3(0, MathUtil.Lerp(actualFocus, 50, .5f), 0);
            view = Matrix.LookAtLH(eye, target, Vector3.Up);

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


            baseModel.Draw(GraphicsDevice, Matrix.RotationZ(MathUtil.Pi), view, projection, baseEffect);

            foreach (var block in currentBlock.Blocks)
            {
                var world = Matrix.RotationY(block.Position.X * (float)Math.PI / 5f) * Matrix.Translation(0, 10 * (currentBlock.Position.Y + block.Position.Y), 0) * Matrix.RotationZ(MathUtil.Pi);
                blockModel.ForEach(part => (part.Effect as BasicEffect).DiffuseColor = block.Color.ToVector4());
                blockModel.Draw(GraphicsDevice, world, view, projection);
            }

            

            base.Draw(gameTime);
        }

    }
}
