using SharpDX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpDXTetris
{
    class TetrisBlock : IEnumerable<Vector2>
    {
        private List<Vector2> blocks;
        public Vector2 Position { get; set; }
        public Color Color { get; set; }

        public TetrisBlock Clone()
        {
            return new TetrisBlock
            {
                blocks = new List<Vector2>(this.blocks),
                Position = this.Position,
                Color = this.Color
            };
        }

        IEnumerator<Vector2> IEnumerable<Vector2>.GetEnumerator()
        {
            foreach (var block in blocks)
            {
                Monitor.Enter(this);
                lock (this)
                {
                    var absolutePos = Position + block;

                    while (absolutePos.X < 0)
                        absolutePos.X += TetrisModel.Columns;

                    if (absolutePos.X >= TetrisModel.Columns)
                        absolutePos.X %= TetrisModel.Columns;

                    yield return absolutePos;
                }
                Monitor.Exit(this);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this as IEnumerable<Vector2>).GetEnumerator();
        }


        public class TetrisBlockGenerator
        {
            private static TetrisBlock[] blocks = new[]{
                new TetrisBlock{
                    blocks = new List<Vector2>{
                        new Vector2(-1,0),
                        new Vector2(),
                        new Vector2(1,0),
                        new Vector2(2,0)
                    },
                    Color = Color.Cyan
                },
                new TetrisBlock{
                    blocks = new List<Vector2>{
                        new Vector2(-1,-1),
                        new Vector2(-1,0),
                        new Vector2(),
                        new Vector2(1,0)
                    },
                    Color = Color.Blue
                },
                new TetrisBlock{
                    blocks = new List<Vector2>{
                        new Vector2(-1,0),
                        new Vector2(),
                        new Vector2(1,0),
                        new Vector2(1,-1)
                    },
                    Color = Color.Orange
                },
                new TetrisBlock{
                    blocks = new List<Vector2>{
                        new Vector2(0,-1),
                        new Vector2(),
                        new Vector2(1,-1),
                        new Vector2(1,0)
                    },
                    Color = Color.Yellow
                },
                new TetrisBlock{
                    blocks = new List<Vector2>{
                        new Vector2(-1,0),
                        new Vector2(),
                        new Vector2(0,-1),
                        new Vector2(1,-1)
                    },
                    Color = Color.Green
                },
                new TetrisBlock{
                    blocks = new List<Vector2>{
                        new Vector2(-1,0),
                        new Vector2(),
                        new Vector2(0,-1),
                        new Vector2(1,0)
                    },
                    Color = Color.Purple
                },
                new TetrisBlock{
                    blocks = new List<Vector2>{
                        new Vector2(-1,-1),
                        new Vector2(0,-1),
                        new Vector2(),
                        new Vector2(1,0)
                    },
                    Color = Color.Red
                },
            };

            private static Random random = new Random();

            public static TetrisBlock GetBlock()
            {
                return GetBlock(random.Next(blocks.Length));
            }

            public static TetrisBlock GetBlock(int style)
            {
                return blocks[style].Clone();
            }
        }
    }
}
