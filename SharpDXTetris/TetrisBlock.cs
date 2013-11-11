using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDXTetris
{
    class TetrisBlock
    {
        public Vector2 Position { get; set; }
        public Color Color { get; set; }

        public TetrisBlock(Vector2 position, Color color)
        {
            this.Position = position;
            this.Color = color;
        }
    }

    class CurrentTetrisBlock
    {
        public List<TetrisBlock> Blocks { get; set; }
        public Vector2 Position { get; set; }
    }

    class TetrisBlockGenerator
    {
        private static List<TetrisBlock>[] blocks = new []{
            new List<TetrisBlock>{
                new TetrisBlock(new Vector2(-1,0), Color.Cyan),
                new TetrisBlock(new Vector2(), Color.Cyan),
                new TetrisBlock(new Vector2(1,0), Color.Cyan),
                new TetrisBlock(new Vector2(2,0), Color.Cyan),
            },
            new List<TetrisBlock>{
                new TetrisBlock(new Vector2(-1,-1), Color.Blue),
                new TetrisBlock(new Vector2(-1,0), Color.Blue),
                new TetrisBlock(new Vector2(), Color.Blue),
                new TetrisBlock(new Vector2(1,0), Color.Blue),
            },
            new List<TetrisBlock>{
                new TetrisBlock(new Vector2(-1,0), Color.Orange),
                new TetrisBlock(new Vector2(), Color.Orange),
                new TetrisBlock(new Vector2(1,0), Color.Orange),
                new TetrisBlock(new Vector2(1,-1), Color.Orange),
            },
            new List<TetrisBlock>{
                new TetrisBlock(new Vector2(0,-1), Color.Yellow),
                new TetrisBlock(new Vector2(), Color.Yellow),
                new TetrisBlock(new Vector2(1,-1), Color.Yellow),
                new TetrisBlock(new Vector2(1,0), Color.Yellow),
            },
            new List<TetrisBlock>{
                new TetrisBlock(new Vector2(-1,0), Color.Green),
                new TetrisBlock(new Vector2(), Color.Green),
                new TetrisBlock(new Vector2(0,-1), Color.Green),
                new TetrisBlock(new Vector2(1,-1), Color.Green),
            },
            new List<TetrisBlock>{
                new TetrisBlock(new Vector2(-1,0), Color.Purple),
                new TetrisBlock(new Vector2(), Color.Purple),
                new TetrisBlock(new Vector2(0,-1), Color.Purple),
                new TetrisBlock(new Vector2(1,0), Color.Purple),
            },
            new List<TetrisBlock>{
                new TetrisBlock(new Vector2(-1,-1), Color.Red),
                new TetrisBlock(new Vector2(0,-1), Color.Red),
                new TetrisBlock(new Vector2(), Color.Red),
                new TetrisBlock(new Vector2(1,0), Color.Red),
            },
        };


        private static Random random = new Random();

        public static CurrentTetrisBlock GetBlock()
        {
            return GetBlock(random.Next(blocks.Length));
        }

        public static CurrentTetrisBlock GetBlock(int style)
        {
            return new CurrentTetrisBlock
            {
                Blocks = new List<TetrisBlock>(blocks[style])
            };
        }
    }
}
