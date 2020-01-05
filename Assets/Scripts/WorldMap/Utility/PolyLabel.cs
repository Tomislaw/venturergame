using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PolyLabel
{
    public class PolyLabel
    {
        public static PolyLabel _polyLabel = null;

        public static PolyLabel GetPolyLabel()
        {
            if (_polyLabel == null)
                _polyLabel = new PolyLabel();

            return _polyLabel;
        }

        private PolyLabel()
        {
        }

        private Queue queue = new Queue();

        private float CompareMax(Cell a, Cell b)
        {
            return b.max - a.max;
        }

        private Cell GetCentroidCell(List<Vector2> polygon)
        {
            float area = 0;
            float x = 0;
            float y = 0;
            List<Vector2> points = polygon;

            for (int i = 0, len = points.Count, j = len - 1; i < len; j = i++)
            {
                Vector2 a = points[i];
                Vector2 b = points[j];
                float f = a.x * b.y - b.x * a.y;
                x += (a.x + b.x) * f;
                y += (a.y + b.y) * f;
                area += f * 3;
            }
            if (area == 0)
                return new Cell(points[0].x, points[0].y, 0, polygon);

            return new Cell(x / area, y / area, 0, polygon);
        }

        public Vector2 GetPolyLabel(List<Vector2> polygon, float precision = 1.0f, bool debug = false)
        {
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            for (int i = 0; i < polygon.Count; i++)
            {
                Vector2 p = polygon[i];
                if (i == 0 || p.x < minX) minX = p.x;
                if (i == 0 || p.y < minY) minY = p.y;
                if (i == 0 || p.x > maxX) maxX = p.x;
                if (i == 0 || p.y > maxY) maxY = p.y;
            }

            float width = maxX - minX;
            float height = maxY - minY;
            float cellSize = Math.Min(width, height);
            float h = cellSize / 2;

            Queue<Cell> cellQueue = new Queue<Cell>();
            //    var cellQueue = new Queue(null, compareMax);

            if (cellSize == 0)
                return new Vector2(minX, minY);

            for (var x = minX; x < maxX; x += cellSize)
            {
                for (var y = minY; y < maxY; y += cellSize)
                {
                    cellQueue.Enqueue(new Cell(x + h, y + h, h, polygon));
                }
            }

            Cell bestCell = GetCentroidCell(polygon);

            Cell bBoxCell = new Cell(minX + width / 2, minY + height / 2, 0, polygon);
            if (bBoxCell.d > bestCell.d)
                bestCell = bBoxCell;

            int numProbes = cellQueue.Count;

            while (cellQueue.Count != 0)
            {
                // pick the most promising cell from the queue
                var cell = cellQueue.Dequeue();

                // update the best cell if we found a better one
                if (cell.d > bestCell.d)
                {
                    bestCell = cell;
                    if (debug)
                        Console.WriteLine("found best {0} after {1} probes", Math.Round(1e4 * cell.d) / 1e4, numProbes);
                }

                // do not drill down further if there's no chance of a better solution
                if (cell.max - bestCell.d <= precision)
                    continue;

                // split the cell into four cells
                h = cell.h / 2;
                cellQueue.Enqueue(new Cell(cell.x - h, cell.y - h, h, polygon));
                cellQueue.Enqueue(new Cell(cell.x + h, cell.y - h, h, polygon));
                cellQueue.Enqueue(new Cell(cell.x - h, cell.y + h, h, polygon));
                cellQueue.Enqueue(new Cell(cell.x + h, cell.y + h, h, polygon));
                numProbes += 4;
            }

            if (debug)
            {
                Console.WriteLine("num probes: " + numProbes);
                Console.WriteLine("best distance: " + bestCell.d);
            }

            return new Vector2(bestCell.x, bestCell.y);
        }

        private class Cell
        {
            public float x, y, h, d, max;

            public Cell(float x, float y, float h, List<Vector2> polygon)
            {
                this.x = x;
                this.y = y;
                this.h = h;
                this.d = PointToPolygonDist(x, y, polygon);
                this.max = Convert.ToSingle(this.d + this.h * Math.Sqrt(2));
            }

            private float PointToPolygonDist(float x, float y, List<Vector2> polygon)
            {
                bool inside = false;
                float minDistSq = float.PositiveInfinity;

                for (int i = 0, len = polygon.Count, j = len - 1; i < len; j = i++)
                {
                    Vector2 a = polygon[i];
                    Vector2 b = polygon[j];

                    if ((a.y > y != b.y > y) && (x < (b.x - a.x) * (y - a.y) / (b.y - a.y) + a.x))
                        inside = !inside;

                    minDistSq = Math.Min(minDistSq, GetSeqDistSq(x, y, a, b));
                }

                return Convert.ToSingle((inside ? 1 : -1) * Math.Sqrt(minDistSq));
            }

            private float GetSeqDistSq(float px, float py, Vector2 a, Vector2 b)
            {
                float x = a.x;
                float y = a.y;
                float dx = b.x - x;
                float dy = b.y - y;

                if (dx != 0 || dy != 0)
                {
                    var t = ((px - x) * dx + (py - y) * dy) / (dx * dx + dy * dy);

                    if (t > 1)
                    {
                        x = b.x;
                        y = b.y;
                    }
                    else if (t > 0)
                    {
                        x += dx * t;
                        y += dy * t;
                    }
                }

                dx = px - x;
                dy = py - y;

                return dx * dx + dy * dy;
            }
        }
    }
}