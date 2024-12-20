using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace Main;

public class Day16
{
    private enum Direction { Up, Down, Left, Right }
    
    private record Problem(
        List<List<char>> Map,
        (int, int, Direction) RobotPosition
    )
    {
        public static Problem Parse(string file)
        {
            var map = new List<List<char>>();

            var robotPosition = (0, 0, Direction.Right);
            
            foreach (var line in File.ReadLines(file)
                                  .Where(line => !string.IsNullOrWhiteSpace(line)))
            {
                var lineForMap = new List<char>();
                foreach (var car in line)
                {
                    switch (car)
                    {
                        case '#' or '.' or 'E':
                            lineForMap.Add(car);
                            break;
                        case 'S':
                            robotPosition = (lineForMap.Count, map.Count, Direction.Right);
                            lineForMap.Add('.');
                            break;
                        default:
                            throw new Exception($"Invalid character: {car}");
                    }
                }
                map.Add(lineForMap);
            }
            
            return new Problem(map, robotPosition);
        }

        private record Robot(int X, int Y, Direction Direction, long Score, HashSet<(int, int)> History);

        public (long, long) SolveSteps()
        {
            Dictionary<(int, int, Direction), long> alreadyExplored = new();
            Queue<Robot> robots = new();
            robots.Enqueue(new Robot(RobotPosition.Item1, RobotPosition.Item2, RobotPosition.Item3, 0, []));

            var currentMinScore = long.MaxValue;
            var currentRobotsReachEnd = new List<Robot>();

            while (robots.Count > 0)
            {
                var currentRobot = robots.Dequeue();
                var (x, y, direction, score) = (currentRobot.X, currentRobot.Y, currentRobot.Direction, currentRobot.Score);

                if (!alreadyExplored.ContainsKey((x, y, direction)))
                {
                    alreadyExplored[(x, y, direction)] = score;
                } else if (alreadyExplored[(x, y, direction)] > score)
                {
                    alreadyExplored[(x, y, direction)] = score;
                } else if (alreadyExplored[(x, y, direction)] < score)
                {
                    continue;
                }
                
                if (Map[y][x] == '#' || score > currentMinScore)
                {
                    continue;
                }
                
                if (Map[y][x] == 'E')
                {
                    if (score == currentMinScore)
                    {
                        currentRobotsReachEnd.Add(currentRobot);
                    } else if (score < currentMinScore)
                    {
                        currentMinScore = score;
                        currentRobotsReachEnd = new List<Robot> {currentRobot};
                    }
                    continue;
                }

                var (nextX, nextY) = direction switch
                {
                    Direction.Up => (x, y - 1),
                    Direction.Down => (x, y + 1),
                    Direction.Left => (x - 1, y),
                    Direction.Right => (x + 1, y),
                    _ => throw new Exception("Wrong direction")
                };
                
                var nextHistory = new HashSet<(int, int)>(currentRobot.History);
                nextHistory.Add((x, y));
                
                robots.Enqueue(new Robot(nextX, nextY, direction, score + 1, nextHistory));
                
                var (nextDirectionClockWise, nextDirectionCounterClockWise) = direction switch
                {
                    Direction.Up => (Direction.Right, Direction.Left),
                    Direction.Down => (Direction.Left, Direction.Right),
                    Direction.Left => (Direction.Up, Direction.Down),
                    Direction.Right => (Direction.Down, Direction.Up),
                    _ => throw new Exception("Wrong direction")
                };
                robots.Enqueue(new Robot(x, y, nextDirectionClockWise, score + 1000, nextHistory));
                robots.Enqueue(new Robot(x, y, nextDirectionCounterClockWise, score + 1000, nextHistory));
            }


            var allHistories = new HashSet<(int, int)>();
            
            foreach (var robot in currentRobotsReachEnd)
            {
                allHistories.UnionWith(robot.History);
                allHistories.Add((robot.X, robot.Y));
            }
            
            return (currentMinScore, allHistories.Count);
        }

    }

    public void Run()
    {
        var problemSample = Problem.Parse("day16_sample.txt");
        var problem = Problem.Parse("day16.txt");

        Console.WriteLine(problemSample.SolveSteps());
        Console.WriteLine(problem.SolveSteps());
    }
}