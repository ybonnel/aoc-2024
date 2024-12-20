using System.Text.RegularExpressions;

namespace Main;

public class Day5
{
    private record Problem(
        Dictionary<int, List<int>> rules,
        List<List<int>> lines
    )
    {
        public static Problem Parse(string file)
        {
            var rules = new Dictionary<int, List<int>>();
            var lines = new List<List<int>>();
            foreach (var line in File.ReadLines(file))
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (line.Contains('|'))
                    {
                        // it's a rule
                        var parts = line.Split('|');
                        var left = int.Parse(parts[0]);
                        var right = int.Parse(parts[1]);
                        var currentList = rules.GetValueOrDefault(left) ?? [];
                        currentList.Add(right);
                        rules[left] = currentList;
                    }
                    else
                    {
                        // it's a message
                        lines.Add(line.Split(',').Select(int.Parse).ToList());
                    }
                }
            }

            return new Problem(rules, lines);
        }

        private bool IsLineValid(List<int> line)
        {
            for (var index =0; index < line.Count; index++) 
            {
                var number = line[index];

                var rule = rules.GetValueOrDefault(number) ?? [];

                // need to verify for each number in rule, it is after in line or not at all.
                if (rule.Select(ruleNumber => line.IndexOf(ruleNumber)).Any(ruleIndex => ruleIndex >= 0 && ruleIndex < index))
                {
                    return false;
                }
            }

            return true;
        }

        private int MiddleOfLine(List<int> line)
        {
            return line[line.Count / 2];
        }

        public long SolveStep1()
        {
            return lines.Where(IsLineValid).Select(MiddleOfLine).Sum();
        }

        public List<int> ReorderToBeValid(List<int> line)
        {
            if (IsLineValid(line))
            {
                return line;
            }
            for (var index =0; index < line.Count; index++) 
            {
                var number = line[index];

                var rule = rules.GetValueOrDefault(number) ?? [];

                // need to verify for each number in rule, it is after in line or not at all.
                foreach (var needAfterIndex in rule)
                {
                    var invalidIndex = line.IndexOf(needAfterIndex);

                    if (invalidIndex >= 0 && invalidIndex < index)
                    {
                        var newLine = line.ToList();
                        newLine[index] = line[invalidIndex];
                        newLine[invalidIndex] = line[index];
                        return ReorderToBeValid(newLine);
                    }
                }
            }
            
            throw new Exception("Should not happen");
            
        }

        public long SolveStep2()
        {
            return lines.Where(line => !IsLineValid(line))
                 .Select(ReorderToBeValid)
                 .Select(MiddleOfLine)
                 .Sum();
        }
    }

    public void Run()
    {
        var problemSample = Problem.Parse("day5_sample.txt");
        var problem = Problem.Parse("day5.txt");
        
        Console.WriteLine(problemSample.SolveStep1());
        Console.WriteLine(problem.SolveStep1());
        
        Console.WriteLine(problemSample.SolveStep2());
        Console.WriteLine(problem.SolveStep2());
    }
}