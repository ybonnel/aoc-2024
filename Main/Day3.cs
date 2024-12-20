using System.Text.RegularExpressions;

namespace Main;

public class Day3
{

    public long MulLine(string line)
    {
        var regex = new Regex(@"mul\((\d+),(\d+)\)");

        var sum = 0L;
        
        // Get all matches
        var matches = regex.Matches(line);
        
        foreach (Match match in matches)
        {
            var first = long.Parse(match.Groups[1].Value);
            var second = long.Parse(match.Groups[2].Value);
            sum += first * second;
        }

        return sum;
    }

    public long MulLineStep2(string line)
    {
        if (!line.Contains("don't()"))
        {
            return MulLine(line);
        }
        var indexDont = line.IndexOf("don't()");
        var sum = MulLine(line.Substring(0, indexDont));
        
        var secondPartString = line.Substring(indexDont + 7);
        if (secondPartString.Contains("do()"))
        {
            sum += MulLineStep2(secondPartString.Substring(secondPartString.IndexOf("do()") + 4));
        }

        return sum;

    }
    
    public void Run()
    {
        var step1Result = File.ReadLines("day3.txt")
                        .Where(line => !string.IsNullOrWhiteSpace(line))
                        .Select(MulLine)
                        .Sum();

        Console.WriteLine(step1Result);
        var step2Result = MulLineStep2(string.Join(' ', File.ReadLines("day3.txt")
                                                            .Where(line => !string.IsNullOrWhiteSpace(line))));

        Console.WriteLine(step2Result);
    }
}