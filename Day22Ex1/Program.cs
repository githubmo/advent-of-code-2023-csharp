// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);

var brickRegex = new Regex(@"(\d+,\d+,\d+)~(\d+,\d+,\d+)");
var bricks = lines
    .Select(line => brickRegex.Match(line).Groups)
    .Select(gs => new EdgeToEdge(Point3D.FromString(gs[1].Value), Point3D.FromString(gs[2].Value)))
    .ToList();

var orderedBricks = bricks.OrderBy(b => b.LowestZ()).ToList();
var shiftedBricks = new List<EdgeToEdge>();
foreach (var edgeToEdge in orderedBricks)
    // it's already at the bottom 
    if (edgeToEdge.LowestZ() == 1)
    {
        shiftedBricks.Add(edgeToEdge);
    }
    else
    {
        var belowXandYs = edgeToEdge.AllXandYs();
        // find all the cubes directly below it, find the highest point on z, and then shift to one above it
        var directlyBelow = shiftedBricks
            .SelectMany(p => p.AllCubes())
            .Where(p => belowXandYs.Contains(new Point2D(p.X, p.Y)))
            .ToArray();

        var shiftBy = directlyBelow.Length == 0
            ? edgeToEdge.LowestZ() - 1
            : edgeToEdge.LowestZ() - directlyBelow.Select(p => p.Z).Max() - 1;
        shiftedBricks.Add(edgeToEdge.ShiftDown(shiftBy));
    }

// var supported = shiftedBricks.Select(b => (b.Label, 0)).ToDictionary();
var essential = new HashSet<string>();
shiftedBricks.ForEach(b =>
{
    // we are looking for cubes that are directly below us 
    var lowCubes = b.AllXandYs().Select(p => new Point3D(p.X, p.Y, b.LowestZ() - 1)).ToList();
    // find the bricks that support us
    var supporters = shiftedBricks.Where(bb => bb.AllCubes().Intersect(lowCubes).Any()).ToList();
    if (supporters.Count() == 1) essential.Add(supporters.First().Label);
});

Console.WriteLine(shiftedBricks.Count - essential.Count);

internal record Point2D(int X, int Y);

internal record Point3D(int X, int Y, int Z)
{
    public static Point3D FromString(string s)
    {
        var r = s.Split(",").Select(s => int.Parse(s.Trim())).ToArray();
        return new Point3D(r[0], r[1], r[2]);
    }
}

internal record EdgeToEdge(Point3D P1, Point3D P2)
{
    private readonly HashSet<Point3D> _set = [];

    public string Label => $"{P1}{P2}";

    public HashSet<Point3D> AllCubes()
    {
        if (_set.Count != 0) return _set;

        _set.Add(P1);
        _set.Add(P2);
        for (var x = Math.Min(P1.X, P2.X); x <= Math.Max(P1.X, P2.X); x++)
        for (var y = Math.Min(P1.Y, P2.Y); y <= Math.Max(P1.Y, P2.Y); y++)
        for (var z = Math.Min(P1.Z, P2.Z); z <= Math.Max(P1.Z, P2.Z); z++)
            _set.Add(new Point3D(x, y, z));
        return _set;
    }

    public HashSet<Point2D> AllXandYs()
    {
        return _set.Select(p => new Point2D(p.X, p.Y)).ToHashSet();
    }

    public int LowestZ()
    {
        return AllCubes().ToList().Select(s => s.Z).Min();
    }

    public int HighestZ()
    {
        return AllCubes().Select(s => s.Z).Max();
    }

    public EdgeToEdge ShiftDown(int s)
    {
        return new EdgeToEdge(P1 with { Z = P1.Z - s }, P2 with { Z = P2.Z - s });
    }
}
