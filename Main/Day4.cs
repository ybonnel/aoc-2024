using System.Text.RegularExpressions;

namespace Main;

public class Day4
{
    private static bool IsThisLetter(List<List<char>> board, int i, int j, char letter)
    {
        return i >= 0 && i < board.Count && j >= 0 && j < board[i].Count && board[i][j] == letter;
    }
    
    public void Run()
    {
        List<List<char>> boardSample = File.ReadLines("day4_sample.txt")
                              .Where(line => !string.IsNullOrWhiteSpace(line))
                              .Select(line => line.ToList()).ToList();

        var step1Sample = SolveStep1(boardSample);

        Console.WriteLine(step1Sample);
        
        List<List<char>> board = File.ReadLines("day4.txt")
                                     .Where(line => !string.IsNullOrWhiteSpace(line))
                                     .Select(line => line.ToList()).ToList();
        
        var step1 = SolveStep1(board);
        
        Console.WriteLine(step1);
        
        var step2Sample = SolveStep2(boardSample);
        
        Console.WriteLine(step2Sample);
        
        var step2 = SolveStep2(board);
        
        Console.WriteLine(step2);

    }
    
    private static long SolveStep2(List<List<char>> board)
    {
        var sum = 0L;
        for (int i = 0; i < board.Count; i++)
        {
            for (int j = 0; j < board[i].Count; j++)
            {
                if (board[i][j] == 'A')
                {
                    // need to look for XMAS on each diagonal and each direction
                    // First look at diagonal top left to bottom right
                    if (IsThisLetter(board, i - 1, j - 1, 'M') && IsThisLetter(board, i + 1, j + 1, 'S')
                        || IsThisLetter(board, i - 1, j - 1, 'S') && IsThisLetter(board, i + 1, j + 1, 'M'))
                    {
                        // Second look at diagonal top right to bottom left
                        if (IsThisLetter(board, i - 1, j + 1, 'M') && IsThisLetter(board, i + 1, j - 1, 'S')
                            || IsThisLetter(board, i - 1, j + 1, 'S') && IsThisLetter(board, i + 1, j - 1, 'M'))
                        {
                            sum++;
                        }
                    }
                }
            }
        }

        return sum;
    }

    private static long SolveStep1(List<List<char>> board)
    {
        var sum = 0L;
        for (int i = 0; i < board.Count; i++)
        {
            for (int j = 0; j < board[i].Count; j++)
            {
                if (board[i][j] == 'X')
                {
                    // begin of XMAS word.
                    // check horizontally forward
                    if (IsThisLetter(board, i, j + 1, 'M') && IsThisLetter(board, i, j + 2, 'A') && IsThisLetter(board, i, j + 3, 'S'))
                    {
                        sum++;
                    }
                    // check vertically forward
                    if (IsThisLetter(board, i + 1, j, 'M') && IsThisLetter(board, i + 2, j, 'A') && IsThisLetter(board, i + 3, j, 'S'))
                    {
                        sum++;
                    }
                    // check diagonally top left bottom right
                    if (IsThisLetter(board, i + 1, j + 1, 'M') && IsThisLetter(board, i + 2, j + 2, 'A') && IsThisLetter(board, i + 3, j + 3, 'S'))
                    {
                        sum++;
                    }
                    // check diagonally top right bottom left
                    if (IsThisLetter(board, i + 1, j - 1, 'M') && IsThisLetter(board, i + 2, j - 2, 'A') && IsThisLetter(board, i + 3, j - 3, 'S'))
                    {
                        sum++;
                    }
                    // check diagonally bottom right top left
                    if (IsThisLetter(board, i - 1, j + 1, 'M') && IsThisLetter(board, i - 2, j + 2, 'A') && IsThisLetter(board, i - 3, j + 3, 'S'))
                    {
                        sum++;
                    }
                    // check diagonally bottom left top right
                    if (IsThisLetter(board, i - 1, j - 1, 'M') && IsThisLetter(board, i - 2, j - 2, 'A') && IsThisLetter(board, i - 3, j - 3, 'S'))
                    {
                        sum++;
                    }
                    // check horizontally backward
                    if (IsThisLetter(board, i, j - 1, 'M') && IsThisLetter(board, i, j - 2, 'A') && IsThisLetter(board, i, j - 3, 'S'))
                    {
                        sum++;
                    }
                    // check vertically backward
                    if (IsThisLetter(board, i - 1, j, 'M') && IsThisLetter(board, i - 2, j, 'A') && IsThisLetter(board, i - 3, j, 'S'))
                    {
                        sum++;
                    }
                }
            }
        }

        return sum;
    }
}