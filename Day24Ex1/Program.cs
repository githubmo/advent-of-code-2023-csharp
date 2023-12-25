// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);

// var lowerLimit = 7.0;
// var upperLimit = 27.0;
var lowerLimit = 200000000000000.0;
var upperLimit = 400000000000000.0;

var sum = 0;

var lineRegex = new Regex(@"(\d+, \d+, \d+) @ (.+, .+, .+)");
var hails =
    (from line in lines
        select lineRegex.Match(line).Groups
        into groups
        let point = Point3D.From(groups[1].Value)
        let velocity = Velocity.From(groups[2].Value)
        select new Hail(point, velocity)
    ).ToList();

while (hails.Count > 1)
{
    var current = hails.First();
    hails.RemoveAt(0);

    foreach (var hail in hails)
    {
        // check if parallel
        if (current.Velocity.isSame(hail.Velocity))
            // Console.WriteLine($"Both are the parallel\n{current}\n{hail}\n");
            continue;

        var p1 = current.Point;
        var v1 = current.Velocity;
        var p2 = hail.Point;
        var v2 = hail.Velocity;

        // elimination method
        var s = (p1.Y - p2.Y + (p2.X - p1.X) * (v1.Y / v1.X)) / (v2.Y - v1.Y * (v2.X / v1.X));
        var px = p2.X + s * v2.X;
        var py = p2.Y + s * v2.Y;
        var t = (px - p1.X) / v1.X;

        // Console.WriteLine(
        //     $"Collision of the following\n{current}\n{hail}\nWas at {px},{py} using multiplier {s} and {t}\n");

        // if s and t are positive, it means they intersect in the future
        if (s > 0.0 && t > 0.0 && px >= lowerLimit && px <= upperLimit && py >= lowerLimit && py <= upperLimit)
            sum += 1;
    }
}

Console.WriteLine(string.Join("\n", hails));
Console.WriteLine(sum);

internal record Hail(Point3D Point, Velocity Velocity)
{
    public Point3D Combine()
    {
        return Point.Add(Velocity);
    }
}

internal record Point3D(double X, double Y, double Z)
{
    public static Point3D From(string s)
    {
        var array = s.Split(", ").Select(ss => ss.Trim()).Select(double.Parse).ToArray();
        return new Point3D(array[0], array[1], array[2]);
    }

    public Point3D Add(Velocity v)
    {
        return new Point3D(X + v.X, Y + v.Y, Z + v.Z);
    }
}

internal record Velocity(double X, double Y, double Z)
{
    public static Velocity From(string s)
    {
        var array = s.Split(", ").Select(ss => ss.Trim()).Select(double.Parse).ToArray();
        return new Velocity(array[0], array[1], array[2]);
    }

    public bool isSame(Velocity other)
    {
        var divs = new[] { X / other.X, Y / other.Y, Z / other.Z };
        return divs.Distinct().Count() == 1; // they are all the same
    }
}
