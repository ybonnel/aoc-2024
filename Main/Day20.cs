using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace Main;

public class Day20
{
    private record Problem(
        List<List<char>> Map,
        (int x, int y) Start,
        (int x, int y) End,
        int MinToSave
    )
    {
        public static Problem Parse(string file, int minToSave)
        {
            var lines = File.ReadLines(file).ToList();
            var start = (0, 0);
            var end = (0, 0);

            for (var y = 0; y < lines.Count; y++)
            {
                for (var x = 0; x < lines[y].Length; x++)
                {
                    var c = lines[y][x];
                    if (c == 'S')
                    {
                        start = (x, y);
                    }
                    else if (c == 'E')
                    {
                        end = (x, y);
                    }
                }
            }

            var map = lines.Select(line => line.Replace('S', '.').Replace('E', '.').ToCharArray().ToList()).ToList();

            return new Problem(
                map,
                start,
                end,
                minToSave
            );
        }

        public char Get(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Map[0].Count || y >= Map.Count)
            {
                return '#';
            }

            return Map[y][x];
        }
        
        public Dictionary<(int, int), int> FindAllShorttenPathTo((int, int) target)
        {
            Dictionary<(int, int), int> distances = new();
            Queue<(int x, int y, int score)> toExplore = new();
            toExplore.Enqueue((target.Item1, target.Item2, 0));

            while (toExplore.Count > 0)
            {
                var (x, y, score) = toExplore.Dequeue();
                if (Get(x, y) == '#')
                {
                    continue;
                }

                if (distances.ContainsKey((x, y)))
                {
                    continue;
                }

                distances[(x, y)] = score;

                toExplore.Enqueue((x + 1, y, score + 1));
                toExplore.Enqueue((x - 1, y, score + 1));
                toExplore.Enqueue((x, y + 1, score + 1));
                toExplore.Enqueue((x, y - 1, score + 1));
            }

            return distances;
        }
        

        public int FindPathWithoutCheat((int x, int y) startPosition, (int x, int y) endPosition)
        {
            HashSet<(int x, int y)> alreadyExplored = new();
            Queue<(int x, int y, int score)> toExplore = new();
            toExplore.Enqueue((startPosition.x, startPosition.y, 0));

            while (toExplore.Count > 0)
            {
                var (x, y, score) = toExplore.Dequeue();
                if (Get(x, y) == '#')
                {
                    continue;
                }

                if (!alreadyExplored.Add((x, y)))
                {
                    continue;
                }

                if ((x, y) == endPosition)
                {
                    return score;
                }

                toExplore.Enqueue((x + 1, y, score + 1));
                toExplore.Enqueue((x - 1, y, score + 1));
                toExplore.Enqueue((x, y + 1, score + 1));
                toExplore.Enqueue((x, y - 1, score + 1));
            }

            return -1;
        }

        public int SolveStep1(int maxManathan = 2)
        {
            var distToStart = FindAllShorttenPathTo(Start);
            var distToEnd = FindAllShorttenPathTo(End);
            
            var shorttenPathWithoutCheating = distToEnd[Start];

            var sum = 0;
            
            for (var y = 0; y < Map.Count; y++)
            {
                for (var x = 0; x < Map[y].Count; x++)
                {
                    if (Get(x, y) == '.')
                    {
                        // Look to all . in the manhattan distance
                        for (var y2 = Math.Max(0, y-maxManathan); y2 <= Math.Min(Map.Count-1, y+maxManathan); y2++)
                        {
                            var distY = Math.Abs(y2 - y);
                            for (var x2 = Math.Max(0, x-(maxManathan-distY)); x2 <= Math.Min(Map[y].Count-1, x+(maxManathan-distY)); x2++)
                            {
                                var manhattan = Math.Abs(x2 - x) + Math.Abs(y2 - y);
                                
                                if (Get(x2, y2) == '#')
                                {
                                    continue;
                                }
                                
                                var distanceToStart = distToStart[(x, y)];
                                var distanceToEnd = distToEnd[(x2, y2)];
                                
                                var distance = distanceToStart + manhattan + distanceToEnd;
                                
                                var save = shorttenPathWithoutCheating - distance;
                                
                                if (save >= MinToSave)
                                {
                                    sum++;
                                }
                            }
                        }
                        
                        
                    }
                }
            } 

            

            return sum;
        }
        public long SolveStep2()
        {
            return SolveStep1(20);
        }
    }

    public void Run()
    {
        var problemSample = Problem.Parse("day20_sample.txt", 1);
        var problemSampleForStep2 = Problem.Parse("day20_sample.txt", 50);
        var problem = Problem.Parse("day20.txt", 100);

        Console.WriteLine(problemSample.SolveStep1());
        Console.WriteLine(problem.SolveStep1());

        Console.WriteLine(problemSampleForStep2.SolveStep2());
        Console.WriteLine(problem.SolveStep2());
    }
}