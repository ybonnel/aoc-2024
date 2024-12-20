namespace Main;

public class Day11
{
    private record Problem(
        Dictionary<long, long> stones
    )
    {
        public static Problem Parse(string file)
        {
            var stones = File.ReadLines(file).First().Split(' ').Select(long.Parse)
                             .Select(stone => (stone, 1L))
                             .ToDictionary();

            return new Problem(stones);
        }

        private static List<(long, long)> BlinkOneStone(long stone, long times)
        {
            if (stone == 0)
            {
                return [(1, times)];
            }

            var stoneAsString = stone.ToString();
            var stoneLength = stoneAsString.Length;

            if (stoneLength % 2 == 0)
            {
                var stone1 = long.Parse(stone.ToString()[..(stoneLength / 2)]);
                var stone2 = long.Parse(stone.ToString()[(stoneLength / 2)..]);
                return [(stone1, times), (stone2, times)];
            }

            return [(stone * 2024, times)];
        }

        public Problem Blink()
        {
            var newStones = new Dictionary<long, long>();

            foreach (var (stone, times) in stones.SelectMany(stone =>
                                                                 BlinkOneStone(stone.Key, stone.Value)
                     ))
            {
                newStones.TryAdd(stone, 0);
                newStones[stone] += times;
            }
            
            return new Problem(newStones);
        }

        public long SolveStep1(int times = 25)
        {
            // Blink 25 times
            var currentProblem = this;
            for (var i = 0; i < times; i++)
            {
                currentProblem = currentProblem.Blink();
            }

            return currentProblem.stones.Select(stoneAndTimes => stoneAndTimes.Value).Sum();
        }


        public long SolveStep2()
        {
            return SolveStep1(75);
        }
    }

    public void Run()
    {
        var problemSample = Problem.Parse("day11_sample.txt");
        var problem = Problem.Parse("day11.txt");

        Console.WriteLine(problemSample.SolveStep1());
        Console.WriteLine(problem.SolveStep1());

        Console.WriteLine(problemSample.SolveStep2());
        Console.WriteLine(problem.SolveStep2());
    }
}