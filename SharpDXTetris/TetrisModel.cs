using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDXTetris
{
    class TetrisModel
    {
        #region

        private const int Rows = 20;
        private const int Columns = 10;

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
            current.Position = new Vector2(1, Rows - 3);

            foreach (var b in current)
                rows.ElementAt((int)b.Y)[(int)b.X] = current.Color;
        }

        #endregion


        internal void Tick(Vector2 move)
        {
            foreach (var b in current)
                rows.ElementAt((int)b.Y)[(int)b.X % Columns] = null;

            current.Position += move;

            bool collision = false;
            List<Vector2> changes = new List<Vector2>();
            foreach (var b in current)
            {
                // Check Y-position
                if ((int)b.Y < 0)
                {
                    // Collision
                    collision = true;
                    break;
                }

                // Check block-collsion
                var row = rows.ElementAt((int)b.Y);
                if (row[(int)b.X % Columns].HasValue)
                {
                    // Collision
                    collision = true;
                    break;
                }

                row[(int)b.X % Columns] = current.Color;
                changes.Add(b);
            }

            // Reverse the changes
            if (collision)
            {
                // Roll back the changes made halfway
                foreach (var change in changes)
                    rows.ElementAt((int)change.Y)[(int)change.X % Columns] = null;
                
                // Reverse positionchange
                current.Position -= move;
                
                // Set back the colors.
                foreach (var b in current)
                    rows.ElementAt((int)b.Y)[(int)b.X % Columns] = current.Color;

                // New block
                NewBlock();
            }

        }

    }
}
