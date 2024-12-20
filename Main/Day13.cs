using System.Text.RegularExpressions;

namespace Main;

public class Day13
{

    private record SubProblem(
        (long, long) ButtonA,
        (long, long) ButtonB,
        (long, long) Prize
    )
    {
        public long SolveStep1()
        {
            // ButtonA.Item1 * aPressed + ButtonB.Item1 * bPressed = Prize.Item1
            // ButtonA.Item2 * aPressed + ButtonB.Item2 * bPressed = Prize.Item2
            
            var eliminator00 = ButtonB.Item2 * ButtonA.Item1;
            var eliminator01 = ButtonB.Item2 * Prize.Item1;
            var eliminator10 = ButtonB.Item1 * ButtonA.Item2;
            var eliminator11 = ButtonB.Item1 * Prize.Item2;
            
            var aPressed = (eliminator01 - eliminator11) / (eliminator00 - eliminator10);
            var bPressed = (Prize.Item1 - ButtonA.Item1 * aPressed) / ButtonB.Item1;
            
            if (aPressed >= 0 && bPressed >= 0 && 
                ButtonA.Item1 * aPressed + ButtonB.Item1 * bPressed == Prize.Item1 &&
                ButtonA.Item2 * aPressed + ButtonB.Item2 * bPressed == Prize.Item2)
            {
                return aPressed * 3 + bPressed;
            }

            return 0;

        }
    }
    
    private record Problem(
        List<SubProblem> SubProblems
    )
    {
        public static Problem Parse(string file)
        {
            var buttonRegex = new Regex(@"Button\s+[AB]:\s+X\+(\d+),\s+Y\+(\d+)");
            var prizeRegex = new Regex(@"Prize:\s+X\=(\d+),\sY\=(\d+)");
            var lines = File.ReadLines(file)
                            .Where(line => !string.IsNullOrWhiteSpace(line))
                            .ToList();

            var subProblems = new List<SubProblem>();

            while (lines.Count > 0)
            {
                var buttonALine = lines.First();
                lines.RemoveAt(0);
                var buttonBLine = lines.First();
                lines.RemoveAt(0);
                var prizeLine = lines.First();
                lines.RemoveAt(0);
                var buttonAMatch = buttonRegex.Match(buttonALine);
                var buttonBMatch = buttonRegex.Match(buttonBLine);
                var prizeMatch = prizeRegex.Match(prizeLine);
                var buttonA = (int.Parse(buttonAMatch.Groups[1].Value), int.Parse(buttonAMatch.Groups[2].Value));
                var buttonB = (int.Parse(buttonBMatch.Groups[1].Value), int.Parse(buttonBMatch.Groups[2].Value));
                var prize = (int.Parse(prizeMatch.Groups[1].Value), int.Parse(prizeMatch.Groups[2].Value));
                subProblems.Add(new SubProblem(buttonA, buttonB, prize));
            }
            

            return new Problem(subProblems);
        }

        public long SolveStep1()
        {
            return SubProblems.Select(subProblem => subProblem.SolveStep1()).Sum();
        }

        public long SolveStep2()
        {
            return SubProblems.Select(subProblem => subProblem with{ Prize = (subProblem.Prize.Item1 + 10000000000000, subProblem.Prize.Item2 + 10000000000000)})
                              .Select(subProblem => subProblem.SolveStep1())
                              .Sum();
        }
    }

    public void Run()
    {
        var problemSample = Problem.Parse("day13_sample.txt");
        var problem = Problem.Parse("day13.txt");

        Console.WriteLine(problemSample.SolveStep1());
        Console.WriteLine(problem.SolveStep1());

        Console.WriteLine(problemSample.SolveStep2());
        Console.WriteLine(problem.SolveStep2());
    }
}