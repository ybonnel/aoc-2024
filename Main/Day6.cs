using System.Text.RegularExpressions;

namespace Main;

public class Day6
{

    private enum State
    {
        EMPTY,
        OBSTACLE,
    }
    
    private enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
    }

    private record Guard(int X, int Y, Direction Direction);
    
    private record Problem(
        List<List<State>> Board,
        Guard Guard
    )
    {
        public static Problem Parse(string file)
        {
            var lines = File.ReadLines(file)
                            .Where(line => !string.IsNullOrWhiteSpace(line))
                            .ToList();
            
            var board = new List<List<State>>();
            Guard guard = null;

            for (var y = 0; y < lines.Count; y++)
            {
                var line = lines[y];
                var row = new List<State>();
                for (var x = 0; x < line.Length; x++)
                {
                    var state = line[x] switch
                    {
                        '#' => State.OBSTACLE,
                        _ => State.EMPTY
                    };
                    row.Add(state);

                    if (line[x] == '^')
                    {
                        guard = new Guard(x, y, Direction.UP);
                    }
                }
                board.Add(row);
            }
            

            return new Problem(board, guard);
        }
        
        private Guard? NextPosition(Guard guard, List<List<State>> board)
        {
            var (nextX, nextY) = guard.Direction switch
            {
                Direction.UP => (guard.X, guard.Y - 1),
                Direction.DOWN => (guard.X, guard.Y + 1),
                Direction.LEFT => (guard.X - 1, guard.Y),
                Direction.RIGHT => (guard.X + 1, guard.Y),
                _ => throw new InvalidOperationException()
            };

            if (nextX < 0 || nextX >= board[0].Count || nextY < 0 || nextY >= board.Count)
            {
                return null;
            }

            if (board[nextY][nextX] == State.OBSTACLE)
            {
                return guard with
                {
                    Direction = guard.Direction switch
                    {
                        Direction.UP => Direction.RIGHT,
                        Direction.RIGHT => Direction.DOWN,
                        Direction.DOWN => Direction.LEFT,
                        Direction.LEFT => Direction.UP,
                        _ => throw new ArgumentOutOfRangeException()
                    }
                };
            }

            return new Guard(nextX, nextY, guard.Direction);
        }
        
        public long SolveStep1()
        {
            var currentGuard = Guard;
            var positions = new HashSet<(int X, int Y)>();
            while (true)
            {
                positions.Add((currentGuard.X, currentGuard.Y));
                var next = NextPosition(currentGuard, Board);
                if (next == null)
                {
                    break;
                }
                currentGuard = next;
            }

            return positions.Count;
        }

        public bool IsLoop()
        {
            var currentGuard = Guard;
            var guardPosition = new HashSet<(int X, int Y, Direction direction)>();
            while (true)
            {
                if (guardPosition.Contains((currentGuard.X, currentGuard.Y, currentGuard.Direction)))
                {
                    return true;
                }
                guardPosition.Add((currentGuard.X, currentGuard.Y, currentGuard.Direction));
                var next = NextPosition(currentGuard, Board);
                if (next == null)
                {
                    return false;
                }
                currentGuard = next;
            }
        }

        public long SolveStep2()
        {
            long sum = 0;
            for (var y = 0; y < Board.Count; y++)
            {
                for (var x = 0; x < Board[y].Count; x++)
                {
                    if (Board[y][x] == State.OBSTACLE || (x, y) == (Guard.X, Guard.Y))
                    {
                        continue;
                    }
                    
                    // Replace current position with obstacle
                    var oldState = Board[y][x];
                    Board[y][x] = State.OBSTACLE;
                    
                    if (IsLoop())
                    {
                        sum++;
                    }
                    
                    Board[y][x] = oldState;
                }
            }

            return sum;
        }

    }

    public void Run()
    {
        var problemSample = Problem.Parse("day6_sample.txt");
        var problem = Problem.Parse("day6.txt");
        
        Console.WriteLine(problemSample.SolveStep1());
        Console.WriteLine(problem.SolveStep1());
        
        Console.WriteLine(problemSample.SolveStep2());
        Console.WriteLine(problem.SolveStep2());
    }
}