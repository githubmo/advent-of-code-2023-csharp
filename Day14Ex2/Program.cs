// See https://aka.ms/new-console-template for more information

using System.Text;

var map = File.ReadLines("Resources/input.txt", Encoding.UTF8).Select(s => s.ToArray()).ToArray();
var mapLength = map.Length;

// CycleNorth();
// Console.WriteLine();
// foreach (var line in map) Console.WriteLine(string.Join("", line));
// CycleWest();
// Console.WriteLine();
// foreach (var line in map) Console.WriteLine(string.Join("", line));
// CycleSouth();
// Console.WriteLine();
// foreach (var line in map) Console.WriteLine(string.Join("", line));
// CycleEast();
// Console.WriteLine();

var seen = new Dictionary<string, int>();

var limit = 1000000000;
var loopCycle = 0;
var cycle = 0;

for (var i = 0; i < limit; i++)
{
    FullCycle();
    var result = string.Join("", map.Select(line => string.Join("", line)));
    if (seen.ContainsKey(result))
    {
        cycle = i + 1;
        loopCycle = i + 1 - seen[result];
        break;
    }

    seen[result] = i + 1;
}

var remains = (limit - cycle) % loopCycle;
if (remains > 0)
    for (var i = 0; i < remains; i++)
        FullCycle();

foreach (var line in map) Console.WriteLine(string.Join("", line));

var sum = 0;
var index = 0;
foreach (var line in map)
{
    var rounds = line.Count(c => c == 'O');
    sum += rounds * (mapLength - index);
    index++;
}

Console.WriteLine(sum);

void FullCycle()
{
    CycleNorth();
    CycleWest();
    CycleSouth();
    CycleEast();
    // foreach (var line in map) Console.WriteLine(string.Join("", line));
    // Console.WriteLine();
}

void CycleNorth()
{
    for (var i = 1; i < map.Length; i++)
    for (var j = 0; j < map[0].Length; j++)
    {
        if (map[i][j] != 'O') continue;
        var ii = i;
        while (ii > 0 && map[ii - 1][j] == '.') ii--;
        map[i][j] = '.';
        map[ii][j] = 'O';
    }
}

void CycleEast()
{
    for (var j = map[0].Length - 2; j >= 0; j--)
    for (var i = 0; i < map.Length; i++)
    {
        if (map[i][j] != 'O') continue;
        var jj = j;
        while (jj < map[0].Length - 1 && map[i][jj + 1] == '.') jj++;
        map[i][j] = '.';
        map[i][jj] = 'O';
    }
}

void CycleSouth()
{
    for (var i = map.Length - 2; i >= 0; i--) // Starts from second last row
    for (var j = 0; j < map[0].Length; j++)
    {
        if (map[i][j] != 'O') continue;
        var ii = i;

        // As the direction is south, iterate while the next cell exists and is empty.
        while (ii < map.Length - 1 && map[ii + 1][j] == '.') ii++;

        // Move 'O' to the new position and clear the old.
        map[i][j] = '.';
        map[ii][j] = 'O';
    }
}

void CycleWest()
{
    for (var i = 0; i < map.Length; i++) // loop through each row
    for (var j = 0; j < map[0].Length; j++)
    {
        if (map[i][j] != 'O') continue; // Continue if we are not dealing with 'O'

        var jj = j;
        while (jj > 0 && map[i][jj - 1] == '.') jj--;
        map[i][j] = '.'; // clear the old position
        map[i][jj] = 'O'; // move 'O' to the new position
    }
}

void Transpose()
{
    var transposedMap = new char[map[0].Length][];
    for (var i = 0; i < map[0].Length; i++)
    {
        transposedMap[i] = new char[map.Length];
        for (var j = 0; j < map.Length; j++) transposedMap[i][j] = map[j][i];
    }

    map = transposedMap;
}
