using System.Text.RegularExpressions;

namespace Main;

public class Day8
{
    
    private record Problem(
        Dictionary<char, List<(int, int)>> AntennasPositions,
        int Width,
        int Height
    )
    {
        public static Problem Parse(string file)
        {
            var lines = File.ReadLines(file)
                            .Where(line => !string.IsNullOrWhiteSpace(line))
                            .ToList();
            
            var antennasPositions = new Dictionary<char, List<(int, int)>>();
            
            var width = lines[0].Length;
            var height = lines.Count;
            
            for (var y = 0; y < lines.Count; y++)
            {
                var line = lines[y];
                for (var x = 0; x < line.Length; x++)
                {
                    var antenna = line[x];
                    if (antenna != '.')
                    {
                        if (!antennasPositions.ContainsKey(antenna))
                        {
                            antennasPositions[antenna] = [];
                        }
                        antennasPositions[antenna].Add((x, y));
                    }
                }
            }
            
            return new Problem(antennasPositions, width, height);
        }
        
        public long SolveStep1()
        {
            var antinodePositions = new HashSet<(int, int)>();
            
            foreach (var (car, positions) in AntennasPositions)
            {
                // for each pair, we need to find antinode position
                for (var i = 0; i < positions.Count; i++)
                {
                    for (var j = i + 1; j < positions.Count; j++)
                    {
                        var (x1, y1) = positions[i];
                        var (x2, y2) = positions[j];

                        var xa = x1 + (x1 - x2);
                        var ya = y1 + (y1 - y2);
                        if (xa >= 0 && xa < Width && ya >= 0 && ya < Height)
                        {
                            antinodePositions.Add((xa, ya));
                        }
                        
                        var xb = x2 + (x2 - x1);
                        var yb = y2 + (y2 - y1);
                        if (xb >= 0 && xb < Width && yb >= 0 && yb < Height)
                        {
                            antinodePositions.Add((xb, yb));
                        }
                    }
                }
                
            }

            return antinodePositions.Count;
        }


        public long SolveStep2()
        {
            var antinodePositions = new HashSet<(int, int)>();
            
            foreach (var (car, positions) in AntennasPositions)
            {
                if (positions.Count > 1)
                {
                    foreach (var (x, y) in positions)
                    {
                        antinodePositions.Add((x, y));
                    }
                }
                // for each pair, we need to find antinode position
                for (var i = 0; i < positions.Count; i++)
                {
                    for (var j = i + 1; j < positions.Count; j++)
                    {
                        var (x1, y1) = positions[i];
                        var (x2, y2) = positions[j];

                        var xa = x1 + (x1 - x2);
                        var ya = y1 + (y1 - y2);
                        while (xa >= 0 && xa < Width && ya >= 0 && ya < Height)
                        {
                            antinodePositions.Add((xa, ya));
                            xa += (x1 - x2);
                            ya += (y1 - y2);
                        }
                        
                        var xb = x2 + (x2 - x1);
                        var yb = y2 + (y2 - y1);
                        while (xb >= 0 && xb < Width && yb >= 0 && yb < Height)
                        {
                            antinodePositions.Add((xb, yb));
                            xb += (x2 - x1);
                            yb += (y2 - y1);
                        }
                    }
                }
                
            }

            return antinodePositions.Count;
        }

    }

    public void Run()
    {
        var problemSample = Problem.Parse("day8_sample.txt");
        var problem = Problem.Parse("day8.txt");
        
        Console.WriteLine(problemSample.SolveStep1());
        Console.WriteLine(problem.SolveStep1());
        
        Console.WriteLine(problemSample.SolveStep2());
        Console.WriteLine(problem.SolveStep2());
    }
}