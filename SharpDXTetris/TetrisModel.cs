using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDXTetris
{
    static class RowExtension
    {
        public static Color? Get(this LinkedList<Color?[]> rows, Vector2 coord)
        {
            return rows.ElementAt((int)coord.Y)[(int)coord.X];
        }

        public static void Set(this LinkedList<Color?[]> rows, Vector2 coord, Color? value)
        {
            rows.ElementAt((int)coord.Y)[(int)coord.X] = value;
        }
    }

    class TetrisModel
    {
        #region

        public const int Rows = 20;
        public const int Columns = 10;

        private LinkedList<Color?[]> rows;

        private TetrisBlock current;



        //public IEnumerable<TetrisBlock> Blocks
        //{
        //    get
        //    {
        //        return rows.SelectMany(row => row)
        //            .Concat(
        //            current.Blocks.Select(block => { block.Position += current.Position; return block; }));
        //    }
        //}

        public Color? BlockAt(int row, int column)
        {
            return rows.ElementAt(row)[column];
        }


        #endregion

        #region 

        public TetrisModel()
        {
            rows = new LinkedList<Color?[]>();
            for (int i = 0; i < Rows; i++)
                rows.AddLast(new Color?[Columns]);

            NewBlock();
        }

        private void NewBlock()
        {
            current = TetrisBlockGenerator.GetBlock();
            current.Position = new Vector2(0, Rows - 3);

            foreach (var b in current)
                rows.ElementAt((int)b.Y)[(int)b.X] = current.Color;
        }

        #endregion


        internal void Tick(Vector2 move)
        {
            var next = current.Clone();
            next.Position += move;

            var collision = false;
            foreach (var subBlock in next.Except(current))
            {
                if (subBlock.Y < 0 || rows.Get(subBlock).HasValue)
                {
                    collision = true;
                    break;
                }
            }

            if (collision)
            {
                NewBlock();
                return;
            }

            foreach (var subBlock in current.Except(next))
                rows.Set(subBlock, null);
            foreach (var subBlock in next.Except(current))
                rows.Set(subBlock, current.Color);
            current = next;
        }

    }
}
