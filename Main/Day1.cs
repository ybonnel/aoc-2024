namespace Main;

public class Day1
{
    public void Run()
    {
        var lines = File.ReadLines("day1.txt")
                        .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(input =>
                        {
                            var fields = input.Split(' ');
                            return (long.Parse(fields.First()), long.Parse(fields.Last()));

                        })
            .ToList();
        
        var firstList = lines.Select(x => x.Item1).Order().ToList();
        var secondList = lines.Select(x => x.Item2).Order().ToList();

        long sum = 0;

        long step2 = 0;
        
        for (var i = 0; i < firstList.Count; i++)
        {
            sum += Math.Abs(firstList[i] - secondList[i]);

            var similarity = secondList.Count(second => second == firstList[i]);
            step2 += similarity * firstList[i];
        }
        
        Console.WriteLine(sum);

        // Step 2
        Console.WriteLine(step2);
    }
    
}