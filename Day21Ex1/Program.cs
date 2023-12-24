// See https://aka.ms/new-console-template for more information

using System.Text;

var lines = File.ReadAllLines("Resources/input.txt", Encoding.UTF8).ToList();
var map = lines.Select(l => l.ToCharArray()).ToArray();
var visited = lines.Select(l => l.ToCharArray()).ToArray();

var (rows, columns) = (map.Length, map[0].Length);
var (step, steps) = (0, 64);
var possibleSteps = new[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
var nextSteps = new Queue<(int, int)>();
for (var i = 0; i < rows; i++)
for (var j = 0; j < columns; j++)
    if (map[i][j] == 'S')
    {
        nextSteps.Enqueue((i, j));
        break;
    }

while (step < steps)
{
    var currentSteps = nextSteps;
    nextSteps = new Queue<(int, int)>();
    while (currentSteps.Count > 0)
    {
        var s = currentSteps.Dequeue();
        foreach (var possibleStep in possibleSteps)
        {
            var (x, y) = (s.Item1 + possibleStep.Item1, s.Item2 + possibleStep.Item2);
            if (IsValid(x, y) && map[x][y] is '.' or 'S')
            {
                visited[x][y] = 'O';
                if (!nextSteps.Contains((x, y))) nextSteps.Enqueue((x, y));
            }
        }
    }

    // Console.WriteLine(MapToString());
    step++;
}

Console.WriteLine(nextSteps.Distinct().Count());

bool IsValid(int ii, int jj)
{
    return ii >= 0 && ii < rows && jj >= 0 && jj < columns;
}

string MapToString()
{
    return string.Join("\n", visited.Select(l => string.Join("", l)));
}
