using System.Text.RegularExpressions;

namespace Main;

public class Day7
{
    
    private record Equation(
        long result,
        List<long> numbers
    )
    {
        
        public bool CanBeTrue(bool withConcat)
        {
            if (numbers.Count == 1)
            {
                return numbers[0] == result;
            }
            
            var first = numbers[0];
            var second = numbers[1];
            var rest = numbers.GetRange(2, numbers.Count - 2);
            
            var withPlus = this with { numbers = new List<long> {first + second}.Concat(rest).ToList() };
            if (withPlus.CanBeTrue(withConcat))
            {
                return true;
            }
            
            var withMultiply = this with { numbers = new List<long> {first * second}.Concat(rest).ToList() };

            if (withMultiply.CanBeTrue(withConcat))
            {
                return true;
            }
            
            if (withConcat)
            {
                var withConcatenation = this with { numbers = new List<long> {long.Parse(first.ToString() + second)}.Concat(rest).ToList() };
                if (withConcatenation.CanBeTrue(withConcat))
                {
                    return true;
                }
            }

            return false;
        }
    }
    
    private record Problem(
        List<Equation> Equations
    )
    {
        public static Problem Parse(string file)
        {
            var lines = File.ReadLines(file)
                            .Where(line => !string.IsNullOrWhiteSpace(line))
                            .Select(line =>
                            {
                                var result = line.Split(':').First();
                                var numbers = line.Split(':').Last().Split(' ')
                                                  .Where(number => !string.IsNullOrWhiteSpace(number))
                                                  .Select(long.Parse).ToList();
                                
                                return new Equation(long.Parse(result), numbers);
                            })
                            .ToList();
            
            return new Problem(lines);
        }
        
        public long SolveStep1()
        {
            return Equations.Where(equation => equation.CanBeTrue(false))
                            .Select(equation => equation.result)
                            .Sum();
        }


        public long SolveStep2()
        {
            return Equations.Where(equation => equation.CanBeTrue(true))
                            .Select(equation => equation.result)
                            .Sum();
        }

    }

    public void Run()
    {
        var problemSample = Problem.Parse("day7_sample.txt");
        var problem = Problem.Parse("day7.txt");
        
        Console.WriteLine(problemSample.SolveStep1());
        Console.WriteLine(problem.SolveStep1());
        
        Console.WriteLine(problemSample.SolveStep2());
        Console.WriteLine(problem.SolveStep2());
    }
}