using System.Text;
using System.Text.RegularExpressions;

namespace Main;

public class Day14
{
    private record Robot((int, int) position, (int, int) velocity)
    {

        public static Robot Parse(string line)
        {
            var regex = new Regex(@"p=(\d+),(\d+) v=([\-0-9]+),([\-0-9]+)");
            
            var match = regex.Match(line);
            if (!match.Success)
            {
                throw new Exception($"Invalid line: {line}");
            }
            
            var position = (int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
            var velocity = (int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));
            
            return new Robot(position, velocity);
        }
        
        public Robot Move((int, int) widthAndHeight)
        {
            var (x, y) = (position.Item1 + velocity.Item1, position.Item2 + velocity.Item2);
            
            x = x < 0 ? widthAndHeight.Item1 + x : x;
            y = y < 0 ? widthAndHeight.Item2 + y : y;
            
            x = x >= widthAndHeight.Item1 ? x - widthAndHeight.Item1 : x;
            y = y >= widthAndHeight.Item2 ? y - widthAndHeight.Item2 : y;
            
            
            return this with{position = (x, y)};
        }
    }

    
    private record Problem(
        List<Robot> Robots,
        (int, int) WidthAndHeight
    )
    {
        public static Problem Parse(string file, (int, int) widthAndHeigth)
        {
            var robots = File.ReadLines(file)
                            .Where(line => !string.IsNullOrWhiteSpace(line))
                            .Select(Robot.Parse)
                            .ToList();

            return new Problem(robots, widthAndHeigth);
        }

        public long SolveStep1()
        {
            var currentRobots = Robots;
            for (var i = 0; i < 100; i++)
            {
                currentRobots = currentRobots.Select(robot => robot.Move(WidthAndHeight)).ToList();
            }
            
            var middle = (WidthAndHeight.Item1 / 2, WidthAndHeight.Item2 / 2);

            var topLeft = 0L;
            var topRight = 0L;
            var bottomLeft = 0L;
            var bottomRight = 0L;
            
            foreach (var robot in currentRobots)
            {
                if (robot.position.Item1 < middle.Item1 && robot.position.Item2 < middle.Item2)
                {
                    topLeft++;
                }
                else if (robot.position.Item1 > middle.Item1 && robot.position.Item2 < middle.Item2)
                {
                    topRight++;
                }
                else if (robot.position.Item1 < middle.Item1 && robot.position.Item2 > middle.Item2)
                {
                    bottomLeft++;
                }
                else if (robot.position.Item1 > middle.Item1 && robot.position.Item2 > middle.Item2)
                {
                    bottomRight++;
                }
            }
            
            return topLeft * topRight * bottomLeft * bottomRight;
        }

        public long SolveStep2()
        {
            File.Delete("D:\\tree.txt");
            var currentRobots = Robots;
            // Move to 199 seconds
            for (var i = 0; i < 199; i++)
            {
                currentRobots = currentRobots.Select(robot => robot.Move(WidthAndHeight)).ToList();
            }

            var index = 199;
            
            var listOfRobots = new List<(int, List<Robot>)>();

            while (index < 10000)
            {
                // move 101 seconds forward
                for (var i = 0; i < 101; i++)
                {
                    currentRobots = currentRobots.Select(robot => robot.Move(WidthAndHeight)).ToList();
                }

                index += 101;
                
                listOfRobots.Add((index, currentRobots));

                if (listOfRobots.Count == 10)
                {
                    using var file = new StreamWriter($"D:\\tree.txt", append:true);

                    var lineOfSeconds = string.Join(", ", listOfRobots.Select(secondsAndRobots => secondsAndRobots.Item1));
                    
                    file.WriteLine($"After {lineOfSeconds}");
                    Print(listOfRobots.Select(secondsAndRobots => secondsAndRobots.Item2).ToList(), file);

                    file.WriteLine("");

                    listOfRobots.Clear();
                }
                
                
                
            }

            return 0L;
        }
        //199, 300, 401, 502, 603, 704, 805
        //906, 1007, 1108, 1209

        public void Print(List<List<Robot>> listOfRobots, StreamWriter file)
        {
            List<Dictionary<(int, int), int>> listOfpositions = new();
            
            foreach (var robots in listOfRobots)
            {
                var positions = new Dictionary<(int, int), int>();
                foreach (var robot in robots)
                {
                    positions.TryAdd(robot.position, 0);
                    positions[robot.position] += 1;
                }
                listOfpositions.Add(positions);
            }

            for (var y = WidthAndHeight.Item2 - 1; y >= 0; y--)
            {
                var line = new StringBuilder();

                foreach (var positions in listOfpositions)
                {
                    for (var x = 0; x < WidthAndHeight.Item1; x++)
                    {
                        if (positions.TryGetValue((x, y), out var count))
                        {
                            line.Append(count);
                        }
                        else
                        {
                            line.Append('.');
                        }
                    }
                    line.Append("   ");
                }

                file.WriteLine(line);
                
            }
        }
    }

    public void Run()
    {
        var problemSample = Problem.Parse("day14_sample.txt", (11, 7));
        var problem = Problem.Parse("day14.txt", (101, 103));

        Console.WriteLine(problemSample.SolveStep1());
        Console.WriteLine(problem.SolveStep1());

        //Console.WriteLine(problemSample.SolveStep2());
        Console.WriteLine(problem.SolveStep2());
    }
}