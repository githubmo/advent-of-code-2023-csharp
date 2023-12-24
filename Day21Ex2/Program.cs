// See https://aka.ms/new-console-template for more information

using System.Text;

var lines = File.ReadAllLines("Resources/input.txt", Encoding.UTF8).ToList();
var map = lines.Select(l => l.ToCharArray()).ToArray();

var (rows, columns) = (map.Length, map[0].Length);
var (step, steps) = (0, 26501365);
var possibleSteps = new[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
var nextSteps = new HashSet<(int, int)>();
var stepsAtEachCycle = new long[steps];
for (var i = 0; i < rows; i++)
for (var j = 0; j < columns; j++)
    if (map[i][j] == 'S')
    {
        nextSteps.Add((i, j));
        break;
    }

while (step < 350)
{
    var currentSteps = nextSteps;
    nextSteps = new HashSet<(int, int)>();
    foreach (var s in currentSteps)
    foreach (var possibleStep in possibleSteps)
    {
        var (x, y) = (s.Item1 + possibleStep.Item1, s.Item2 + possibleStep.Item2);
        var (xx, yy) = ((x % rows + rows) % rows, (y % columns + columns) % columns);
        if (map[xx][yy] is '.' or 'S') nextSteps.Add((x, y));
    }

    stepsAtEachCycle[step] = nextSteps.Count;

    if (step % 50 == 0) Console.WriteLine(step);
    // Console.WriteLine(MapToString());
    step++;
}

Console.WriteLine(stepsAtEachCycle[step - 1]);

var bb = 65;
var dd = 131;


// three points on the quadratic equation graph
var c = stepsAtEachCycle[bb - 1];
var two = stepsAtEachCycle[bb + dd - 1];
var three = stepsAtEachCycle[bb + dd * 2 - 1];

// getting a,b,c in a**2x + bx + c = 0
var a = (three - 2 * two + c) / 2;
var b = two - c - a;
var n = (steps - bb) / dd;

var result = a * Math.Pow(n, 2) + b * n + c;

Console.WriteLine(result);
return;

bool IsValid(int ii, int jj)
{
    return ii >= 0 && ii < rows && jj >= 0 && jj < columns;
}
