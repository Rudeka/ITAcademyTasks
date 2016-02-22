using System;
using System.Collections.Generic;
using System.Linq;

namespace ShortestPathFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            var dimension = new AreaDimension() { Hight = 1000, Width = 1000 };
            var startPoint = new CoOrds() { X = 0, Y = 0 };
            var targetPoint = new CoOrds() { X = 8, Y = 8 };
            var knightMovements = new KnightMovements();

            var finder = new ShortestPathFinder(dimension, startPoint, targetPoint, knightMovements);
            finder.FindTargetPoint();
        }

        public class ShortestPathFinder
        {
            private AreaDimension dimension;
            private CoOrds startPointCoords;
            private CoOrds targetPointCoords;
            private IMovements allowedMovements;

            public ShortestPathFinder(AreaDimension dimension, CoOrds startPointCoords, CoOrds targetPointCoords, IMovements allowedMovements)
            {
                this.dimension = dimension;
                this.startPointCoords = startPointCoords;
                this.targetPointCoords = targetPointCoords;
                this.allowedMovements = allowedMovements;
            }

            private bool IsTargetPoint(CoOrds checkingCoOrds)
            {
                return checkingCoOrds == targetPointCoords;
            }

            public void FindTargetPoint()
            {
                var checkedPoints = new bool[dimension.Hight, dimension.Width];
                var uncheckedPoints = new Queue<CoOrds>();
                var parent = new Dictionary<CoOrds, CoOrds>();

                uncheckedPoints.Enqueue(startPointCoords);
                checkedPoints[startPointCoords.X, startPointCoords.Y] = true;

                var stepsCounter = startPointCoords.Steps;

                while (uncheckedPoints.Count > 0)
                {
                    var currentPoint = uncheckedPoints.Dequeue();

                    var maxSteps = allowedMovements.MaxSteps;
                    var movementX = allowedMovements.MovementX;
                    var movementY = allowedMovements.MovementY;

                    stepsCounter = currentPoint.Steps + 1;

                    for (var i = 0; i < maxSteps; i++)
                    {
                        var stepX = currentPoint.X + movementX[i];
                        var stepY = currentPoint.Y + movementY[i];

                        if (stepX >= 0
                            && stepX < dimension.Width
                            && stepY >= 0
                            && stepY < dimension.Hight)
                        {
                            if (!checkedPoints[stepX, stepY])
                            {
                                var newUncheckedPoint = new CoOrds() { X = stepX, Y = stepY, Steps = stepsCounter };
                                parent.Add(newUncheckedPoint, currentPoint);
                                uncheckedPoints.Enqueue(newUncheckedPoint);
                                checkedPoints[stepX, stepY] = true;

                                if (IsTargetPoint(newUncheckedPoint))
                                {
                                    var path = GetPath(parent, targetPointCoords, startPointCoords);


                                    Console.WriteLine("Target point has been found and it took {0} steps", currentPoint.Steps);
                                    Console.WriteLine(path);
                                    Console.ReadLine();


                                }
                            }
                        }
                    }
                }
            }

            public string GetPath(Dictionary<CoOrds, CoOrds> parentTrack, CoOrds targetPoint, CoOrds startPoint)
            {
                var pathArray = new List<CoOrds>();
                
                if (parentTrack.ContainsKey(targetPoint))
                {
                    var backTrack = targetPoint;
                    do
                    {
                        pathArray.Add(backTrack);
                        backTrack = parentTrack[backTrack];
                    } while (backTrack != startPoint);
                    pathArray.Reverse();
                }

                var pathMessage = pathArray.Aggregate("My moves: ", (current, point) => current + (point.X + "," + point.Y + " => "));

                return pathMessage;
            }
        }
    }



    public interface IMovements
    {
        int[] MovementX { get; }
        int[] MovementY { get; }
        int MaxSteps { get; }
    }

    public class KnightMovements : IMovements
    {
        public int[] MovementX { get; } = { -1, 1, -2, 2, -2, 2, -1, 1 };
        public int[] MovementY { get; } = { -2, -2, -1, -1, 1, 1, 2, 2 };
        public int MaxSteps { get; } = 8;
    }

    public struct AreaDimension
    {
        public int Hight;
        public int Width;
    }

    public struct CoOrds : IEquatable<CoOrds>
    {
        public bool Equals(CoOrds other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is CoOrds && Equals((CoOrds)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public static bool operator ==(CoOrds left, CoOrds right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CoOrds left, CoOrds right)
        {
            return !left.Equals(right);
        }

        public int X;
        public int Y;
        public int Steps;

        public CoOrds(int x, int y, int steps = 0)
        {
            X = x;
            Y = y;
            Steps = steps;
        }

    }
}
