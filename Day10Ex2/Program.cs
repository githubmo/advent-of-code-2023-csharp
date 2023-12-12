// See https://aka.ms/new-console-template for more information

using System.Text;

var mappings = new Dictionary<char, Connection>
{
    { '|', new Connection(new Coord(0, 1), new Coord(0, -1)) },
    { '-', new Connection(new Coord(1, 0), new Coord(-1, 0)) },
    { 'L', new Connection(new Coord(0, -1), new Coord(1, 0)) },
    { 'J', new Connection(new Coord(0, -1), new Coord(-1, 0)) },
    { '7', new Connection(new Coord(0, 1), new Coord(-1, 0)) },
    { 'F', new Connection(new Coord(0, 1), new Coord(1, 0)) }
};
var file = "Resources/input.txt";
var map = File.ReadAllLines(file, Encoding.UTF8).Select(s => s.ToCharArray()).ToArray();
var copy = File.ReadAllLines(file, Encoding.UTF8).Select(s => s.ToCharArray()).ToArray();
var width = map[0].Length;
var height = map.Length;


Coord sCoord = null!;
for (var i = 0; i < height; i++)
for (var j = 0; j < width; j++)
    if (map[i][j] == 'S')
    {
        sCoord = new Coord(j, i);
        map[i][j] = '$';
        break;
    }


var current = new Coord(sCoord!.X, sCoord!.Y);
while (true)
{
    var currentC = map[current.Y][current.X];
    map[current.Y][current.X] = '$';

    var surroundings = mappings.GetValueOrDefault(currentC)?.ToArray.Select(c => current.Add(c)) ?? new[]
    {
        current with { X = current.X - 1 },
        current with { X = current.X + 1 },
        current with { Y = current.Y - 1 },
        current with { Y = current.Y + 1 }
    };

    var validCoords = surroundings.Where(IsValid).ToList();
    // Console.WriteLine(string.Join(", ", bools));
    current = validCoords.FirstOrDefault(c => IsConnected(current, map[c.Y][c.X], c));
    if (current == null) break;
}

foreach (var chars in map) Console.WriteLine(string.Join("", chars));

Console.Write("Part1: ");
Console.WriteLine(map.SelectMany(c => c).Count(c => c == '$') / 2);

// part 2 
for (var i = 0; i < height; i++)
for (var j = 0; j < width; j++)
{
    if (map[i][j] == '$') continue;
    var left = 0;
    if (j > 0)
        for (var jj = 0; jj < j; jj++)
            // |, J, L, or S
            if (copy[i][jj] is '|' or 'J' or 'L' && map[i][jj] == '$')
                left++;

    if (left % 2 == 1)
        map[i][j] = 'I';
}

foreach (var chars in map) Console.WriteLine(string.Join("", chars));

Console.Write("Part2: ");
Console.WriteLine(map.SelectMany(c => c).Count(c => c == 'I'));
// The pipes are arranged in a two-dimensional grid of tiles:
// 
// | is a vertical pipe connecting north and south.
// - is a horizontal pipe connecting east and west.
// L is a 90-degree bend connecting north and east.
// J is a 90-degree bend connecting north and west.
// 7 is a 90-degree bend connecting south and west.
// F is a 90-degree bend connecting south and east.
// . is ground; there is no pipe in this tile.
// S is the starting position of the animal; there is a pipe on this tile, but your sketch doesn't show what shape the pipe has.

bool IsConnected(Coord currentLocation, char c, Coord to)
{
    if (c is '.' or 'S' or '$') return false;
    var connection = mappings![c];
    var isConnected = to.Add(connection.From) == currentLocation || to.Add(connection.To) == currentLocation;
    return isConnected;
}

bool IsValid(Coord c)
{
    return c is { X: >= 0, Y: >= 0 } && c.X < width && c.Y < height;
}


internal record Coord(int X, int Y)
{
    public Coord Add(Coord c)
    {
        return new Coord(X + c.X, Y + c.Y);
    }
}

internal record Connection(Coord From, Coord To)
{
    public Coord[] ToArray
    {
        get { return new[] { From, To }; }
    }
}
