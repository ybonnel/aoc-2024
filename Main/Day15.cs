using System.Text;
using System.Text.RegularExpressions;

namespace Main;

public class Day15
{
    
    private record Problem(
        List<List<char>> Map,
        List<char> Moves,
        (int, int) RobotPosition
    )
    {
        public static Problem Parse(string file)
        {
            var map = new List<List<char>>();

            var moves = new List<char>();
            
            foreach (var line in File.ReadLines(file)
                                  .Where(line => !string.IsNullOrWhiteSpace(line)))
            {
                if (line.Contains('<') || line.Contains('>') || line.Contains('^') || line.Contains('v'))
                {
                    moves.AddRange(line);
                }
                else
                {
                    map.Add(line.ToList());
                }
            }
            
            for (var y = 0; y < map.Count; y++)
            {
                for (var x = 0; x < map[y].Count; x++)
                {
                    if (map[y][x] == '@')
                    {
                        map[y][x] = '.';
                        return new Problem(map, moves, (x, y));
                    }
                }
            }

            throw new Exception("Robot not found");
        }

        public Problem Move()
        {
            var move = Moves.First();
            var moves = Moves.Skip(1).ToList();
            
            var (x, y) = RobotPosition;
            
            var (xInc, yInc) = move switch
            {
                '<' => (-1, 0),
                '>' => (1, 0),
                '^' => (0, -1),
                'v' => (0, 1),
                _ => throw new Exception($"Invalid move: {move}")
            };
            
            var (freeX, freeY) = (x + xInc, y + yInc);
            while (Map[freeY][freeX] != '.')
            {
                if (Map[freeY][freeX] == '#')
                {
                    return this with{Moves = moves};
                }
                freeX += xInc;
                freeY += yInc;
            }

            while ((freeX, freeY) != (x, y))
            {
                var (nextFreeX, nextFreeY) = (freeX - xInc, freeY - yInc);
                Map[freeY][freeX] = Map[nextFreeY][nextFreeX];
                freeX = nextFreeX;
                freeY = nextFreeY;
            }
            
            return this with{Moves = moves, RobotPosition = (x + xInc, y + yInc)};
        }

        public Problem MoveStep2()
        {
            var move = Moves.First();
            var moves = Moves.Skip(1).ToList();
            
            var (x, y) = RobotPosition;
            
            var (xInc, yInc) = move switch
            {
                '<' => (-1, 0),
                '>' => (1, 0),
                '^' => (0, -1),
                'v' => (0, 1),
                _ => throw new Exception($"Invalid move: {move}")
            };
            
            var newMap = Map.Select(row => row.ToList()).ToList();
            
            var toMove = new List<(int, int, char)> { (x, y, '.') };
            
            while (toMove.Count > 0)
            {
                var (currentX, currentY, car) = toMove.First();
                toMove.RemoveAt(0);
                
                var (freeX, freeY) = (currentX + xInc, currentY + yInc);
                
                if (newMap[freeY][freeX] == '#')
                {
                    return this with{Moves = moves};
                }

                if (newMap[freeY][freeX] == '.')
                {
                    newMap[freeY][freeX] = car;
                } 
                else if (newMap[freeY][freeX] == '[')
                {
                    toMove.Add((freeX, freeY, '['));
                    toMove.Add((freeX+1, freeY, ']'));
                    
                    newMap[freeY][freeX] = car;
                    newMap[freeY][freeX + 1] = '.';
                }
                else if (newMap[freeY][freeX] == ']')
                {
                    toMove.Add((freeX, freeY, ']'));
                    toMove.Add((freeX-1, freeY, '['));
                    
                    newMap[freeY][freeX] = car;
                    newMap[freeY][freeX - 1] = '.';
                }
                else
                {
                    throw new Exception($"Invalid car: {newMap[freeY][freeX]}");
                }
            }
            
            return new Problem(Moves: moves, Map: newMap, RobotPosition: (x + xInc, y + yInc));
        }

        public void PrintMap()
        {
            for (var y = 0; y < Map.Count; y++)
            {
                for (var x = 0; x < Map[y].Count; x++)
                {
                    if ((x, y) == RobotPosition)
                    {
                        Console.Write('@');
                    }
                    else
                    {
                        Console.Write(Map[y][x]);
                    }
                }
                Console.WriteLine();
            }
        }

        public long SolveStep1()
        {
            var currentProblem = new Problem(this.Map.Select(row => row.ToList()).ToList(), this.Moves.ToList(), this.RobotPosition);
            while (currentProblem.Moves.Count > 0)
            {
                currentProblem = currentProblem.Move();
            }

            var sum = 0L;
            
            for (var y = 0; y < currentProblem.Map.Count; y++)
            {
                for (var x = 0; x < currentProblem.Map[y].Count; x++)
                {
                    if (currentProblem.Map[y][x] == 'O')
                    {
                        sum += y * 100 + x;
                    }
                }
            }
            
            return sum;
        }

        public long SolveStep2()
        {
            var newMap = Map.Select(row => row.SelectMany(car =>
                                                              car switch
                                                              {
                                                                  '#' => "##",
                                                                  'O' => "[]",
                                                                  '.' => "..",
                                                                    _ => throw new Exception($"Invalid car: {car}")
                                                              }
                                        ).ToList()).ToList();
            
            var newRobotPosition = (RobotPosition.Item1 * 2, RobotPosition.Item2);
            
            var currentProblem = new Problem(newMap, Moves.ToList(), newRobotPosition);
            
            while (currentProblem.Moves.Count > 0)
            {
                currentProblem = currentProblem.MoveStep2();
            }
            
            currentProblem.PrintMap();
            

            var sum = 0L;
            
            for (var y = 0; y < currentProblem.Map.Count; y++)
            {
                for (var x = 0; x < currentProblem.Map[y].Count; x++)
                {
                    if (currentProblem.Map[y][x] == '[')
                    {
                        sum += y * 100 + x;
                    }
                }
            }

            return sum;
        }
    }

    public void Run()
    {
        var problemSample = Problem.Parse("day15_sample.txt");
        var problem = Problem.Parse("day15.txt");

        Console.WriteLine(problemSample.SolveStep1());
        Console.WriteLine(problem.SolveStep1());

        Console.WriteLine(problemSample.SolveStep2());
        Console.WriteLine(problem.SolveStep2());
    }
}