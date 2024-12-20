namespace Main;

public class Day9
{
    
    
    private record Problem(
        List<int> Blocks,
        long FreeSpaces
    )
    {
        public static Problem Parse(string file)
        {
            var line = File.ReadLines(file).First();

            var id = 0;

            var isFile = true;
            
            List<int> blocks = new();
            
            var freeSpaces = 0L;

            foreach (var c in line)
            {
                var size = int.Parse(c.ToString());

                var numberToAdd = isFile ? id : -1;

                if (!isFile)
                {
                    freeSpaces+=size;
                }

                for (var i = 0; i < size; i++)
                {
                    blocks.Add(numberToAdd);
                }

                if (isFile)
                {
                    id++;
                }
                isFile = !isFile;
            }
            
            return new Problem(blocks, freeSpaces);
        }

        private List<int> Rearrange()
        {
            var newBlocks = Blocks.ToList();
            var freeSpaces = FreeSpaces;

            var lastFreeSpaceIndex = 0;

            while (freeSpaces > 0)
            {
                // Get and remove the last
                var id = newBlocks.Last();
                newBlocks.RemoveAt(newBlocks.Count - 1);
                if (id != -1)
                {
                    while (newBlocks[lastFreeSpaceIndex] != -1)
                    {
                        lastFreeSpaceIndex++;
                    }
                    newBlocks[lastFreeSpaceIndex] = id;
                }
                freeSpaces--;
            }
            
            return newBlocks;
        }

        private List<int> RearrangeStep2()
        {
            var newBlocks = Blocks.ToList();

            var id = newBlocks.Last();

            while (id > 0)
            {
                var firstIndex = newBlocks.IndexOf(id);
                var lastIndex = newBlocks.LastIndexOf(id);
                
                var size = lastIndex - firstIndex + 1;
                
                for (var index = 0; index < firstIndex; index++)
                {
                    var couldFit = true;
                    for (var i = 0; i < size; i++)
                    {
                        if (newBlocks[index + i] != -1)
                        {
                            couldFit = false;
                            break;
                        }
                    }

                    if (couldFit)
                    {
                        for (var i = 0; i < size; i++)
                        {
                            newBlocks[index + i] = id;
                            newBlocks[firstIndex + i] = -1;
                        }

                        break;
                    }
                    
                }
                
                
                id--;
            }

            return newBlocks;
        }
        
        public long SolveStep1()
        {
            var afterDefrag = Rearrange();

            var checksum = Checksum(afterDefrag);

            return checksum;
        }

        private static long Checksum(List<int> afterDefrag)
        {
            var checksum = 0L;
            for (var i = 0; i < afterDefrag.Count; i++)
            {
                if (afterDefrag[i] == -1)
                {
                    continue;
                }
                checksum += i * afterDefrag[i];
            }

            return checksum;
        }


        public long SolveStep2()
        {
            var afterDefrag = RearrangeStep2();

            var checksum = Checksum(afterDefrag);

            return checksum;
        }

    }

    public void Run()
    {
        var problemSample = Problem.Parse("day9_sample.txt");
        var problem = Problem.Parse("day9.txt");
        
        Console.WriteLine(problemSample.SolveStep1());
        Console.WriteLine(problem.SolveStep1());
        
        Console.WriteLine(problemSample.SolveStep2());
        Console.WriteLine(problem.SolveStep2());
    }
}