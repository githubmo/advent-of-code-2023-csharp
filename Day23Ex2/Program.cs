// See https://aka.ms/new-console-template for more information

using System.Text;

// this solution is terrible, takes ages, never finishes but if I break point after leaving it 
// running for a long time and look at max, i get the right answer
// i should do better

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);
var map = lines.Select(l => l.ToCharArray()).ToArray();
var (rows, columns) = (map.Length, map[0].Length);

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
        if (c != '#')
        {
            var np = point with { J = point.J + 1 };
            if (IsValid(np)) neighbours.Add(np);

            np = point with { I = point.I + 1 };
            if (IsValid(np)) neighbours.Add(np);

            np = point with { J = point.J - 1 };
            if (IsValid(np)) neighbours.Add(np);

            np = point with { I = point.I - 1 };
            if (IsValid(np)) neighbours.Add(np);
        }

        tiles.Add(point, new Tile(point, neighbours));
    }
}

// Using a DFS to find all the paths
var paths = new List<List<Point>>();
paths.Add(new List<Point> { new(0, 1) });
var lastPoint = new Point(rows - 1, columns - 2); // last point always on last row and second to last column
var max = 0;
while (!paths.TrueForAll(l => l.Contains(lastPoint)))
    paths = paths.SelectMany(path =>
    {
        // if path contains last point, remove it to save time
        if (path.Last() == lastPoint)
        {
            max = Math.Max(max, path.Count);
            return [];
        }

        // create a new path for every point that is connecting to the last point in the path
        tiles.TryGetValue(path.Last(), out var tile);
        var validConnected = tile!.Connected.Where(c => !path.Contains(c)).ToList();
        switch (validConnected.Count)
        {
            case 0:
                return []; // if it cannot connect, it's not a valid path
            case > 1:
                return validConnected.Select(p => path.Concat([p])).ToList();
        }

        var ps = new List<Point>();
        while (validConnected.Count == 1)
        {
            ps.AddRange(validConnected);
            tiles.TryGetValue(ps.Last(), out tile);
            validConnected = tile!.Connected.Where(c => !path.Contains(c) && !ps.Contains(c)).ToList();
        }

        return [path.Concat(ps)];
    }).Select(p => p.ToList()).ToList();

// Console.WriteLine(string.Join("\n", tiles.Select(t => $"{t.Key} >> {string.Join(" :: ", t.Value.Connected)}")));

// Finding the one that 
max = paths.Count == 0 ? max - 1 : Math.Max(max, paths.Max(p => p.Count)) - 1;
Console.WriteLine(max); // -1 because the first tile you don't step to it

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
