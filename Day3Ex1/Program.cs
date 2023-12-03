// See https://aka.ms/new-console-template for more information

using System.Text;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);

var grid = lines.Select(l => l.ToArray()).ToArray();
var (i, j) = (0, 0);
var rowLength = grid[0].Length;
var gridLength = grid.Length;
var sum = 0;
while (i < gridLength)
{
    var currentChar = grid[i][j];
    if (char.IsDigit(currentChar))
    {
        var initialJ = j;
        var digits = currentChar.ToString();
        while (IsValidRowIndex(++j))
        {
            currentChar = grid[i][j];
            if (char.IsDigit(currentChar)) digits += currentChar;
            else break;
        }

        var hasAdjacent = false;
        for (var jj = initialJ; jj <= j; jj++)
        {
            hasAdjacent = hasAdjacent || HasAdjacentSpecialChar(i, jj);
        }

        if (hasAdjacent)
        {
            var value = int.Parse(digits);
            sum += value;
            Console.WriteLine($"{value} is valid and sum is now {sum}");
        }
        else
        {
            Console.WriteLine($"{digits} is **not** valid");
        }
    }

    if (j < rowLength - 1) j++;
    else
    {
        i++;
        j = 0;
    }
}


Console.WriteLine(sum);
return;

bool HasAdjacentSpecialChar(int i, int j)
{
    for (var ii = i - 1; ii <= i + 1; ii++)
    {
        for (var jj = j - 1; jj <= j + 1; jj++)
        {
            if (!IsValidColumnIndex(ii) || !IsValidRowIndex(jj)) continue;
            if (!(grid![ii][jj] == '.' || char.IsDigit(grid[ii][jj])))
            // if (char.IsSymbol(grid![ii][jj]))
            {
                return true;
            }
        }
    }

    return false;
}

bool IsValidRowIndex(int index) => index >= 0 && index < rowLength;
bool IsValidColumnIndex(int index) => index >= 0 && index < gridLength;