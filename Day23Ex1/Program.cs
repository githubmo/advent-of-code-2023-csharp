// See https://aka.ms/new-console-template for more information

using System.Text;

var lines = File.ReadLines("Resources/example.txt", Encoding.UTF8);
var map = lines.Select(l => l.ToCharArray()).ToArray();
var (rows, columns) = (map.Length, map[0].Length);

var sum = 0;
// my collection that represents my directed acyclical graph (DAG)
var tiles = new Dictionary<Point, Tile>();

// generating my DAG
for (var i = 0; i < rows; i++)
for (var j = 0; j < columns; j++)
{
    var c = map[i][j];
    if (c != '#')
    {
        var point = new Point(i, j);
        var neighbours = new List<Point>();
        if (c is '.' or '>')
        {
            var np = point with { J = point.J + 1 };
            if (IsValid(np)) neighbours.Add(np);
        }

        if (c is '.' or 'v')
        {
            var np = point with { I = point.I + 1 };
            if (IsValid(np)) neighbours.Add(np);
        }

        if (c is '.' or '<')
        {
            var np = point with { J = point.J - 1 };
            if (IsValid(np)) neighbours.Add(np);
        }

        if (c is '.' or '^')
        {
            var np = point with { I = point.I - 1 };
            if (IsValid(np)) neighbours.Add(np);
        }

        tiles.Add(point, new Tile(point, neighbours));
    }
}

// Using a DFS to find all the paths

// Finding the one that 

Console.WriteLine(string.Join("\n", tiles.Select(t => $"{t.Key} >> {string.Join(" :: ", t.Value.Connected)}")));
Console.WriteLine(sum);

// a tile is a valid stepping tile if it is within the map and is not a rock '#'
bool IsValid(Point p)
{
    return p is { I: >= 0, J: >= 0 } && p.I < rows && p.J < columns && map[p.I][p.J] != '#';
}

// 2D point, naming I, J as using X,Y is confusing since J is on the X plane and I is on the Y plane
internal record Point(int I, int J);

// a tile is point and connects to adjacent valid point
// we don't care it's direction since we account for that when created `Connected`
internal record Tile(Point Location, List<Point> Connected);
