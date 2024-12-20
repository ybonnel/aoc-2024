using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace Main;

public class Day19
{
    
    private record Problem(
        List<string> availableTowels,
        List<string> wantedDesign
    )
    {

        public static Problem Parse(string file)
        {
            var availableTowels = File.ReadLines(file).First().Split(", ").ToList();
            var wantedDesign = File.ReadLines(file).Skip(2).ToList();
            
            return new Problem(
                availableTowels,
                wantedDesign
            );

        }

        public bool IsDesignPossible(string design)
        {
            HashSet<string> alreadyExplored = new();
            Queue<string> toExplore = new();
            toExplore.Enqueue("");
            
            while (toExplore.Count > 0)
            {
                var currentTry = toExplore.Dequeue();
                if (currentTry == design)
                {
                    return true;
                }
                if (!alreadyExplored.Add(currentTry))
                {
                    continue;
                }

                foreach (var towel in availableTowels)
                {
                    if (design.StartsWith(currentTry + towel))
                    {
                        toExplore.Enqueue(currentTry + towel);
                    }
                }
            }

            return false;


        }

        public long PossibleArrangements(string design)
        {
            List<(string, long)> toExplore = new();
            toExplore.Add(("", 1L));

            var sum = 0L;
            
            while (toExplore.Count > 0)
            {
                var newToExplore = new Dictionary<string, long>();
                foreach (var (pattern, count) in toExplore)
                {
                    foreach (var towel in availableTowels)
                    {
                        var newPattern = pattern + towel;
                        if (design.StartsWith(newPattern))
                        {
                            if (newPattern == design)
                            {
                                sum += count;
                            }
                            else
                            {
                                newToExplore.TryAdd(newPattern, 0);
                                newToExplore[newPattern] += count;
                            }
                        }
                    }
                    
                    
                }
                
                toExplore = newToExplore.Select(keyValue => (keyValue.Key, keyValue.Value)).ToList();
            }

            return sum;
            
            
        }
        
        public long SolveStep1()
        {
            return wantedDesign.Count(IsDesignPossible);

        }
        public long SolveStep2()
        {
            return wantedDesign.Sum(PossibleArrangements);
        }
    }

    public void Run()
    {
        var problemSample = Problem.Parse("day19_sample.txt");
        var problem = Problem.Parse("day19.txt");

        Console.WriteLine(problemSample.SolveStep1());
        Console.WriteLine(problem.SolveStep1());
        
        Console.WriteLine(problemSample.SolveStep2());        
        Console.WriteLine(problem.SolveStep2());

    }
}