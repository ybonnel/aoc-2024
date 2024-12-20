namespace Main;

public class Day2
{

    public bool IsValid(List<int> line)
    {
        // line is valid if it's decreasing or increasing globally in if step are lower or equal 3
        var increasing = line[0] < line[1];
        
        for (var i = 1; i < line.Count; i++)
        {
            var first = line[i-1];
            var second = line[i];
            var stepIncreasing = first < second;
            var valid = stepIncreasing == increasing && Math.Abs(first - second) <= 3 && first != second;
            if (!valid)
            {
                return false;
            }
            
        }

        return true;
    }

    public bool IsValidWithOneRemoval(List<int> line)
    {
        for (var i = 0; i < line.Count; i++)
        {
            var copy = new List<int>(line);
            copy.RemoveAt(i);
            if (IsValid(copy))
            {
                return true;
            }
        }

        return false;
    }
    
    public void Run()
    {
        var lines = File.ReadLines("day2.txt")
                        .Where(line => !string.IsNullOrWhiteSpace(line))
                        .Select(input =>
                                    input.Split(' ').Select(int.Parse).ToList()
                        )
                        .ToList();

        var validLinesStep1 = lines.Count(IsValid);
        
        Console.WriteLine(validLinesStep1);
        
        var notValidLines = lines.Where(line => !IsValid(line)).ToList();
        
        // For each line try every possible remove possible to check if one works
        var validLinesStep2 = validLinesStep1;
        validLinesStep2 += notValidLines.Count(IsValidWithOneRemoval);
        
        Console.WriteLine(validLinesStep2);
    }
}