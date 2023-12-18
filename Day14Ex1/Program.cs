// See https://aka.ms/new-console-template for more information

using System.Text;

var map = File.ReadLines("Resources/input.txt", Encoding.UTF8).Select(s => s.ToArray()).ToArray();
var mapLength = map.Length;

CycleNorth();
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
