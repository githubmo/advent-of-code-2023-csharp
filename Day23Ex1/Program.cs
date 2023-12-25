// See https://aka.ms/new-console-template for more information

using System.Text;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);
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
var paths = new List<List<Point>>();
paths.Add(new List<Point> { new(0, 1) });
var lastPoint = new Point(rows - 1, columns - 2); // last point always on last row and second to last column
while (!paths.TrueForAll(l => l.Contains(lastPoint)))
    paths = paths.SelectMany(path =>
    {
        // if path contains last point, just return it without a new path
        if (path.Contains(lastPoint)) return [path];

        // create a new path for every point that is connecting to the last point in the path
        tiles.TryGetValue(path.Last(), out var tile);
        var validConnected = tile!.Connected.Where(c => !path.Contains(c)).ToList();
        if (validConnected.Count == 0) return []; // if it cannot connect, it's not a valid path
        return validConnected.Select(p => path.Concat([p])).ToList();
    }).Select(p => p.ToList()).ToList();

// Console.WriteLine(string.Join("\n", tiles.Select(t => $"{t.Key} >> {string.Join(" :: ", t.Value.Connected)}")));

// Finding the one that 
Console.WriteLine(paths.Max(p => p.Count - 1)); // -1 because the first tile you don't step to it

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
