using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace Main;

public class Day18
{
    
    private record Problem(
        List<(int, int)> Bytes,
        int size,
        int steps
    )
    {

        public static Problem Parse(string file, int size, int steps)
        {
            return new Problem(
                File.ReadLines(file).Select(line => line.Split(',').Select(int.Parse).ToArray()).Select(x => (x[0], x[1])).ToList(),
                size,
                steps
            );

        }
        
        public long SolveStep1()
        {
            var corruptedBytes = Bytes.Take(steps).ToHashSet();

            HashSet<(int x, int y)> alreadyVisited = new();
            Queue<(int x, int y, int score, List<(int, int)> path)> toExplore = new();
            toExplore.Enqueue((0, 0, 0, [(0, 0)]));
            
            while (toExplore.Count > 0)
            {
                var (x, y, score, path) = toExplore.Dequeue();
                if (x == size && y == size)
                {   
                    return score;
                }
                
                if (!alreadyVisited.Add((x, y)))
                {
                    continue;
                }

                var nextScore = score + 1;
                
                if (x - 1 >= 0 && !corruptedBytes.Contains((x - 1, y)) && !alreadyVisited.Contains((x - 1, y)))
                {
                    toExplore.Enqueue((x - 1, y, nextScore, path.Union([(x - 1, y)]).ToList()));
                }
                if (x + 1 <= size && !corruptedBytes.Contains((x + 1, y)) && !alreadyVisited.Contains((x + 1, y)))
                {
                    toExplore.Enqueue((x + 1, y, nextScore, path.Union([(x + 1, y)]).ToList()));
                }
                if (y - 1 >= 0 && !corruptedBytes.Contains((x, y - 1)) && !alreadyVisited.Contains((x, y - 1)))
                {
                    toExplore.Enqueue((x, y - 1, nextScore, path.Union([(x, y - 1)]).ToList()));
                }
                if (y + 1 <= size && !corruptedBytes.Contains((x, y + 1)) && !alreadyVisited.Contains((x, y + 1)))
                {
                    toExplore.Enqueue((x, y + 1, nextScore, path.Union([(x, y + 1)]).ToList()));
                }
            }

            return -1;

        }
        public string SolveStep2()
        {
            var minSteps = steps + 1;
            var maxSteps = Bytes.Count;
            var testStep = minSteps + (maxSteps - minSteps) / 2;
            var nextProblem = this with { steps = testStep };
            while (minSteps != maxSteps)
            {
                var ok = nextProblem.SolveStep1() != -1;
                if (ok)
                {
                    minSteps = testStep;
                }
                else
                {
                    maxSteps = testStep - 1;
                }
                testStep = minSteps + (maxSteps - minSteps) / 2;
                if (testStep == nextProblem.steps && maxSteps > testStep)
                {
                    testStep++;
                }

                Console.WriteLine($"Trying {testStep}/{Bytes.Count}, {minSteps}-{maxSteps}, {ok}");;
                
                nextProblem = nextProblem with { steps = testStep };
            }

            var problematicByte = nextProblem.Bytes[nextProblem.steps];
            return $"{problematicByte.Item1},{problematicByte.Item2}";
        }
    }

    public void Run()
    {
        var problemSample = Problem.Parse("day18_sample.txt", 6, 12);
        var problem = Problem.Parse("day18.txt", 70, 1024);

        Console.WriteLine(problemSample.SolveStep1());
        Console.WriteLine(problem.SolveStep1());
        
        Console.WriteLine(problemSample.SolveStep2());        
        Console.WriteLine(problem.SolveStep2());

    }
}