namespace Main;

public class Day10
{
    private record Problem(
        List<List<int>> Map
    )
    {
        public static Problem Parse(string file)
        {
            var map = File.ReadLines(file)
                          .Where(line => !string.IsNullOrWhiteSpace(line))
                          .Select(line => line.Select(car => int.Parse(car.ToString())).ToList())
                          .ToList();

            return new Problem(map);
        }

        public int Get(int x, int y)
        {
            if (x < 0 || x >= Map.First().Count || y < 0 || y >= Map.Count)
            {
                return -1;
            }

            return Map[y][x];
        }

        public long HowManyPath(int x, int y, bool distinctPath = false)
        {
            var explored = new HashSet<(int, int)>();
            var toExplore = new Queue<(int, int)>();
            var paths = 0L;
            toExplore.Enqueue((x, y));

            while (toExplore.Any())
            {
                var (currentX, currentY) = toExplore.Dequeue();
                if (!distinctPath && !explored.Add((currentX, currentY)))
                {
                    continue;
                }

                var currentValue = Get(currentX, currentY);
                if (currentValue == 9)
                {
                    paths++;
                    continue;
                }

                // first left
                var leftValue = Get(currentX - 1, currentY);
                if (leftValue == currentValue + 1)
                {
                    toExplore.Enqueue((currentX - 1, currentY));
                }

                var rightValue = Get(currentX + 1, currentY);
                if (rightValue == currentValue + 1)
                {
                    toExplore.Enqueue((currentX + 1, currentY));
                }

                var upValue = Get(currentX, currentY - 1);
                if (upValue == currentValue + 1)
                {
                    toExplore.Enqueue((currentX, currentY - 1));
                }

                var downValue = Get(currentX, currentY + 1);
                if (downValue == currentValue + 1)
                {
                    toExplore.Enqueue((currentX, currentY + 1));
                }
            }

            return paths;
        }

        public long SolveStep1(bool distinctPath = false)
        {
            var sum = 0L;
            for (var y = 0; y < Map.Count; y++)
            {
                for (var x = 0; x < Map.First().Count(); x++)
                {
                    if (Map[y][x] == 0)
                    {
                        sum += HowManyPath(x, y, distinctPath);
                    }
                }
            }


            return sum;
        }


        public long SolveStep2()
        {
            return SolveStep1(true);
        }
    }

    public void Run()
    {
        var problemSample = Problem.Parse("day10_sample.txt");
        var problem = Problem.Parse("day10.txt");

        Console.WriteLine(problemSample.SolveStep1());
        Console.WriteLine(problem.SolveStep1());

        Console.WriteLine(problemSample.SolveStep2());
        Console.WriteLine(problem.SolveStep2());
    }
}