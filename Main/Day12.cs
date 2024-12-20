namespace Main;

public class Day12
{
    private record Problem(
        List<List<char>> Map
    )
    {
        public static Problem Parse(string file)
        {
            var map = File.ReadLines(file)
                          .Where(line => !string.IsNullOrWhiteSpace(line))
                          .Select(line => line.ToList())
                          .ToList();

            return new Problem(map);
        }

        public char Get(int x, int y)
        {
            if (x < 0 || x >= Map.First().Count || y < 0 || y >= Map.Count)
            {
                return ' ';
            }

            return Map[y][x];
        }

        public HashSet<(int, int)> GetRegion(int x, int y)
        {
            var explored = new HashSet<(int, int)>();
            var toExplore = new Queue<(int, int)>();
            var region = new HashSet<(int, int)>();
            var regionValue = Get(x, y);

            toExplore.Enqueue((x, y));

            while (toExplore.Any())
            {
                var (currentX, currentY) = toExplore.Dequeue();
                if (Get(currentX, currentY) != regionValue || !explored.Add((currentX, currentY)))
                {
                    continue;
                }


                region.Add((currentX, currentY));
                toExplore.Enqueue((currentX - 1, currentY));
                toExplore.Enqueue((currentX + 1, currentY));
                toExplore.Enqueue((currentX, currentY - 1));
                toExplore.Enqueue((currentX, currentY + 1));
            }

            return region;
        }

        public (long, long) AreaAndPerimeterOfRegion(HashSet<(int, int)> region)
        {
            var area = region.Count;
            var regionValue = Get(region.First().Item1, region.First().Item2);
            var perimeter = region.Select(xAndY =>
                {
                    var perimeter = 0;
                    var (x, y) = xAndY;
                    if (Get(x - 1, y) != regionValue)
                    {
                        perimeter++;
                    }

                    if (Get(x + 1, y) != regionValue)
                    {
                        perimeter++;
                    }

                    if (Get(x, y - 1) != regionValue)
                    {
                        perimeter++;
                    }

                    if (Get(x, y + 1) != regionValue)
                    {
                        perimeter++;
                    }

                    return perimeter;
                }
            ).Sum();

            return (area, perimeter);
        }

        public long SolveStep1()
        {
            var regions = CalculateRegions();

            return regions.Select(AreaAndPerimeterOfRegion)
                          .Select(areaAndPerimeter => areaAndPerimeter.Item1 * areaAndPerimeter.Item2)
                          .Sum();
        }

        private List<HashSet<(int, int)>> CalculateRegions()
        {
            var alreadyExplored = new HashSet<(int, int)>();
            var regions = new List<HashSet<(int, int)>>();

            for (var y = 0; y < Map.Count; y++)
            {
                for (var x = 0; x < Map.First().Count; x++)
                {
                    if (alreadyExplored.Contains((x, y)))
                    {
                        continue;
                    }

                    var region = GetRegion(x, y);

                    alreadyExplored.UnionWith(region);

                    regions.Add(region);
                }
            }

            return regions;
        }

        public enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }

        public record Border(int X, int Y, Direction Direction)
        {
            public bool IsSameLine(Border other)
            {
                if (this.Direction != other.Direction)
                {
                    return false;
                }

                return this.Direction switch
                {
                    Direction.Left => this.X == other.X,
                    Direction.Right => this.X == other.X,
                    Direction.Up => this.Y == other.Y,
                    Direction.Down => this.Y == other.Y,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public record Side(int startX, int startY, int endX, int endY, Direction Direction);


        public (long, long) AreaAndSidesOfRegion(HashSet<(int, int)> region)
        {
            var area = region.Count;
            var regionValue = Get(region.First().Item1, region.First().Item2);

            var borders = region.SelectMany(xAndY =>
                {
                    var borders = new List<Border>();
                    var (x, y) = xAndY;
                    if (Get(x - 1, y) != regionValue)
                    {
                        borders.Add(new Border(x, y, Direction.Left));
                    }

                    if (Get(x + 1, y) != regionValue)
                    {
                        borders.Add(new Border(x, y, Direction.Right));
                    }

                    if (Get(x, y - 1) != regionValue)
                    {
                        borders.Add(new Border(x, y, Direction.Up));
                    }

                    if (Get(x, y + 1) != regionValue)
                    {
                        borders.Add(new Border(x, y, Direction.Down));
                    }

                    return borders;
                }
            ).ToList();

            var numberOfSides = 0L;

            while (borders.Count > 0)
            {
                var border = borders.First();


                // Get all border in same line and remove them
                var sameLineBorders = borders.Where(otherBorder => border.IsSameLine(otherBorder)).ToList();
                borders.RemoveAll(otherBorder => border.IsSameLine(otherBorder));

                var allCoord = sameLineBorders.Select(b => b.Direction switch
                {
                    Direction.Left => b.Y,
                    Direction.Right => b.Y,
                    Direction.Up => b.X,
                    Direction.Down => b.X,
                    _ => throw new ArgumentOutOfRangeException()
                }).Order().ToList();


                var currentCoord = -1;

                foreach (var coord in allCoord)
                {
                    if (currentCoord == -1)
                    {
                        currentCoord = coord;
                        continue;
                    }

                    if (coord - currentCoord > 1)
                    {
                        numberOfSides++;
                    }

                    currentCoord = coord;
                }

                numberOfSides++;
            }

            return (area, numberOfSides);
        }

        public long SolveStep2()
        {
            var regions = CalculateRegions();
            return regions.Select(AreaAndSidesOfRegion)
                          .Select(areaAndSides => areaAndSides.Item1 * areaAndSides.Item2)
                          .Sum();
        }
    }

    public void Run()
    {
        var problemSample = Problem.Parse("day12_sample.txt");
        var problem = Problem.Parse("day12.txt");

        Console.WriteLine(problemSample.SolveStep1());
        Console.WriteLine(problem.SolveStep1());

        Console.WriteLine(problemSample.SolveStep2());
        Console.WriteLine(problem.SolveStep2());
    }
}