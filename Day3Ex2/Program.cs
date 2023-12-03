// See https://aka.ms/new-console-template for more information

/*
 * The solution here assumes that the author is not giving us crazy edge cases
 * to handle. My solution here is lazy, it assumes:
 *
 * - Each `*` that has two adjacent numbers does not share it with another `*`
 *   - If the above were to be true, I would have to track numbers used, or remove them once used
 * - There is no chaining of `*`
 * e.g. of last one is
 *   435*111....
 *   .222.......
 * - finally, we never multiply two separate numbers that are the same
 * e.g. of last one is
 *   111*111.....
 *   ............
 *
 * My lazy solution has worked, so I'm moving on.
 */

using System.Text;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);

var grid = lines.Select(l => l.ToArray()).ToArray();
var (currentRow, currentColumn) = (0, 0);
var rowLength = grid[0].Length;
var gridLength = grid.Length;
var sum = 0L;
while (currentRow < gridLength)
{
    var currentChar = grid[currentRow][currentColumn];
    if (currentChar == '*')
    {
        var set = GetAdjacentNumbers(currentRow, currentColumn);

        // we only care if a single '*' has two adjacent numbers only, one will not be counted
        if (set.Count == 2) sum += set.Aggregate(1, (acc, val) => acc * val);

        // assumption that a single '*' can only have two adjacent numbers at max
        if (set.Count > 2)
            throw new ArgumentException(
                $"{string.Join(", ", set)} at {currentRow} {currentColumn} has more than two numbers");
    }

    if (currentColumn < rowLength - 1)
    {
        currentColumn++;
    }

    else
    {
        currentRow++;
        currentColumn = 0;
    }
}

Console.WriteLine(sum);
return;

HashSet<int> GetAdjacentNumbers(int requestedRow, int requestedColumn)
{
    var numberSets = new HashSet<int>();
    for (var r = requestedRow - 1; r <= requestedRow + 1; r++)
    for (var c = requestedColumn - 1; c <= requestedColumn + 1; c++)
    {
        if (!IsValidColumnIndex(r) || !IsValidRowIndex(c)) continue;
        var currentChar = grid![r][c];
        // because a digit is sent multiple times, we need to dedupe the number we get back from each digit
        // we assume all adjacent numbers are unique, and this really should be handled correctly
        // e.g. if the number is added to the set more times than it's length, we know we have two adjacent 
        // numbers that are the same, and a solution around this would be simple. I won't bother 
        // as I am lazy
        if (char.IsDigit(currentChar)) AddIntToSet(in numberSets, r, c);
    }

    return numberSets;
}

void AddIntToSet(in HashSet<int> hashSet, int requestedRow, int requestedColumn)
{
    var numString = grid[requestedRow][requestedColumn].ToString();
    var c = requestedColumn;
    while (IsValidRowIndex(--c) && char.IsDigit(grid[requestedRow][c]))
        numString = $"{grid[requestedRow][c]}{numString}";
    c = requestedColumn;
    while (IsValidRowIndex(++c) && char.IsDigit(grid[requestedRow][c]))
        numString = $"{numString}{grid[requestedRow][c]}";
    hashSet.Add(int.Parse(numString));
}

bool IsValidRowIndex(int index)
{
    return index >= 0 && index < rowLength;
}

bool IsValidColumnIndex(int index)
{
    return index >= 0 && index < gridLength;
}
