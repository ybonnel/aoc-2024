using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace Main;

public class Day17
{
    private enum OpCode
    {
        ADV = 0,
        BXL = 1,
        BST = 2,
        JNZ = 3,
        BXC = 4,
        OUT = 5,
        BDV = 6,
        CDV = 7,
    }
    
    private record Problem(
        long RegisterA,
        long RegisterB,
        long RegisterC,
        List<int> Instructions
    )
    {
        public override string ToString()
        {
            return $"{RegisterA}, {RegisterB}, {RegisterC} : {string.Join(",", Instructions)}";
        }

        public static Problem Parse(string file)
        {
            using var lines = File.ReadLines(file).GetEnumerator();
            lines.MoveNext();
            var registerA = long.Parse(lines.Current.Split(" ").Last());
            lines.MoveNext();
            var registerB = long.Parse(lines.Current.Split(" ").Last());
            lines.MoveNext();
            var registerC = long.Parse(lines.Current.Split(" ").Last());
            lines.MoveNext();
            lines.MoveNext();
            var instructions = lines.Current.Split(' ')[1].Split(',').Select(int.Parse).ToList();
                
            return new Problem(
                registerA,
                registerB,
                registerC,
                instructions
                );
        }

        public (Problem, string output) Run(bool failFast = false)
        {
            var instructionIndex = 0;
            var registerA = RegisterA;
            var registerB = RegisterB;
            var registerC = RegisterC;
            List<int> output = new();
            
            while (instructionIndex < Instructions.Count)
            {
                var opcode = (OpCode) Instructions[instructionIndex];
                instructionIndex++;
                var operand = Instructions[instructionIndex];
                instructionIndex++;

                var combo = operand switch
                {
                    >= 0 and <= 3 => operand,
                    4 => registerA,
                    5 => registerB,
                    6 => registerC,
                    _ => -1,
                };

                switch (opcode)
                {
                    case OpCode.ADV:
                        /*
                         * The adv instruction (opcode 0) performs division. The numerator is the value in the A register. The denominator is found by raising 2 to the power of the instruction's combo operand. (So, an operand of 2 would divide A by 4 (2^2); an operand of 5 would divide A by 2^B.) The result of the division operation is truncated to an integer and then written to the A register.
                         */
                        registerA /= (long) Math.Pow(2, combo);
                        break;
                    case OpCode.BXL:
                        /*
                         * The bxl instruction (opcode 1) calculates the bitwise XOR of register B and the instruction's literal operand, then stores the result in register B.
                         */
                        registerB ^= operand;
                        break;
                    case OpCode.BST:
                        /*
                         * The bst instruction (opcode 2) calculates the value of its combo operand modulo 8 (thereby keeping only its lowest 3 bits), then writes that value to the B register.
                         */
                        registerB = combo % 8;
                        break;
                    case OpCode.JNZ:
                        /*
                         * The jnz instruction (opcode 3) does nothing if the A register is 0. However, if the A register is not zero, it jumps by setting the instruction pointer to the value of its literal operand; if this instruction jumps, the instruction pointer is not increased by 2 after this instruction.
                         */
                        if (registerA != 0)
                        {
                            instructionIndex = operand;
                        }
                        break;
                    case OpCode.BXC:
                        /*
                         * The bxc instruction (opcode 4) calculates the bitwise XOR of register B and register C, then stores the result in register B. (For legacy reasons, this instruction reads an operand but ignores it.)
                         */
                        registerB ^= registerC;
                        break;
                    case OpCode.OUT:
                        /*
                         * The out instruction (opcode 5) calculates the value of its combo operand modulo 8, then outputs that value. (If a program outputs multiple values, they are separated by commas.)
                         */
                        output.Add((int)(combo % 8));
                        if (failFast && output.Last() != Instructions[output.Count - 1])
                        {
                            return (new Problem(registerA, registerB, registerC, Instructions), string.Join(",", output));
                        }
                        break;
                    case OpCode.BDV:
                        /*
                         * The bdv instruction (opcode 6) works exactly like the adv instruction except that the result is stored in the B register. (The numerator is still read from the A register.)
                         */
                        registerB = registerA / (long) Math.Pow(2, combo);
                        break;
                    case OpCode.CDV:
                        /*
                         * The cdv instruction (opcode 7) works exactly like the adv instruction except that the result is stored in the C register. (The numerator is still read from the A register.)
                         */
                        registerC = registerA / (long) Math.Pow(2, combo);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
            
            return (new Problem(registerA, registerB, registerC, Instructions), string.Join(",", output));
        }
        
        public string SolveStep1()
        {
            var result = Run();
            
            return result.output;
        }

        public long FindNextCandidate(long startCandidate)
        {
            var nextProblem = this with{ RegisterA = startCandidate};
            var expectedOutput = string.Join(",", Instructions);
            var (_, output) = nextProblem.Run();
            
            while (!expectedOutput.EndsWith(output))
            {
                nextProblem = nextProblem with { RegisterA = nextProblem.RegisterA +1 };
                (_, output) = nextProblem.Run();
            }

            return nextProblem.RegisterA;
        }

        public long SolveStep2()
        {
            var nextProblem = this with{ RegisterA = 1};
            var expectedOutput = string.Join(",", Instructions);

            while (!nextProblem.Run().output.Equals(expectedOutput))
            {
                nextProblem = nextProblem with { RegisterA = FindNextCandidate(nextProblem.RegisterA) };
                if (nextProblem.Run().output.Equals(expectedOutput))
                {
                    break;
                }
                nextProblem = nextProblem with { RegisterA = nextProblem.RegisterA * 8 };
            }

            return nextProblem.RegisterA;

        }
    }

    public void Run()
    {
        var problemSample = Problem.Parse("day17_sample.txt");
        var problem = Problem.Parse("day17.txt");

        Console.WriteLine(problemSample.SolveStep1());
        Console.WriteLine(problem.SolveStep1());
        
        Console.WriteLine(problemSample.SolveStep2());        
        Console.WriteLine(problem.SolveStep2());

    }
}