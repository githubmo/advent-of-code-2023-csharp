// See https://aka.ms/new-console-template for more information

using System.Text;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);

var grid = lines.Select(l => l.ToArray()).ToArray();
var (i, j) = (0, 0);
var rowLength = grid[0].Length;
var gridLength = grid.Length;
var sum = 0L;
while (i < gridLength)
{
    var currentChar = grid[i][j];
    if (currentChar == '*')
    {
        var set = GetAdjacentNumbers(i, j);
        if (set.Count == 2) sum += set.Aggregate(1, (acc, val) => acc * val);

        if (set.Count() > 2)
            throw new ArgumentException($"{string.Join(", ", set)} at {i} {j} has more than two numbers");
    }

    if (j < rowLength - 1)
    {
        j++;
    }
    else
    {
        i++;
        j = 0;
    }
}

Console.WriteLine(sum);
return;

HashSet<int> GetAdjacentNumbers(int n, int m)
{
    var numberSets = new HashSet<int>();
    for (var ii = n - 1; ii <= n + 1; ii++)
    for (var jj = m - 1; jj <= m + 1; jj++)
    {
        if (!IsValidColumnIndex(ii) || !IsValidRowIndex(jj)) continue;
        var currentChar = grid![ii][jj];
        if (char.IsDigit(currentChar)) AddIntToSet(in numberSets, ii, jj);
    }

    return numberSets;
}

void AddIntToSet(in HashSet<int> hashSet, int n, int m)
{
    var numString = grid[n][m].ToString();
    var jj1 = m;
    while (IsValidRowIndex(--jj1) && char.IsDigit(grid[n][jj1])) numString = $"{grid[n][jj1]}{numString}";
    jj1 = m;
    while (IsValidRowIndex(++jj1) && char.IsDigit(grid[n][jj1])) numString = $"{numString}{grid[n][jj1]}";
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
