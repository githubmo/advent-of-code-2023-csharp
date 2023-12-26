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

const int range = 900;

Console.WriteLine(Solve());
return;

// brute force approach on the first for hails is enough to get the line for all of them 
double Solve()
{
    foreach (var x in Enumerable.Range(-range, range))
    foreach (var y in Enumerable.Range(-range, range))
    {
        var intersect1 = TryIntersectPos(hails[1], hails[0], (x, y));
        var intersect2 = TryIntersectPos(hails[2], hails[0], (x, y));
        var intersect3 = TryIntersectPos(hails[3], hails[0], (x, y));

        if (!intersect1.intersects || !CloseEnough(intersect1.coord, intersect2.coord) ||
            !CloseEnough(intersect1.coord, intersect3.coord)) continue;

        foreach (var z in Enumerable.Range(-range, range))
        {
            var intersectZ = hails[1].Point.Z + intersect1.t * (hails[1].Velocity.Z + z);
            var intersectZ2 = hails[2].Point.Z + intersect2.t * (hails[2].Velocity.Z + z);
            var intersectZ3 = hails[3].Point.Z + intersect3.t * (hails[3].Velocity.Z + z);

            if (!CloseEnoughProduct(intersectZ, intersectZ2) || !CloseEnoughProduct(intersectZ, intersectZ3)) continue;

            Console.WriteLine($"{intersect1.coord.X} {intersect1.coord.Y} {intersectZ}");
            return intersect1.coord.X + intersect1.coord.Y + intersectZ;
        }
    }

    return -1;
}

bool CloseEnough((double x1, double y1) coord1, (double x2, double y2) coord2)
{
    return Math.Abs(coord1.x1 - coord2.x2) + Math.Abs(coord1.y1 - coord2.y2) < 0.001;
}

bool CloseEnoughProduct(double x1, double x2)
{
    return Math.Abs(x1 - x2) < 0.001;
}

// https://math.stackexchange.com/a/3176648
(bool intersects, (double X, double Y) coord, double t) TryIntersectPos(Hail one, Hail two,
    (int x, int y) offset)
{
    var a = (Pos: (one.Point.X, one.Point.Y), Vel: (X: one.Velocity.X + offset.x, Y: one.Velocity.Y + offset.y));
    var c = (Pos: (two.Point.X, two.Point.Y), Vel: (X: two.Velocity.X + offset.x, Y: two.Velocity.Y + offset.y));

    //Determinant
    var D = a.Vel.X * -1 * c.Vel.Y - a.Vel.Y * -1 * c.Vel.X;

    if (D == 0) return (false, (-1, -1), -1);

    var Qx = -1 * c.Vel.Y * (c.Pos.X - a.Pos.X) - -1 * c.Vel.X * (c.Pos.Y - a.Pos.Y);
    var Qy = a.Vel.X * (c.Pos.Y - a.Pos.Y) - a.Vel.Y * (c.Pos.X - a.Pos.X);

    var t = Qx / D;
    var s = Qy / D;

    var Px = a.Pos.X + t * a.Vel.X;
    var Py = a.Pos.Y + t * a.Vel.Y;

    // Returns the intersection point, as well as the timestamp at which "one" will reach it with the given velocity.
    return (true, (Px, Py), t);
}


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

    public bool IsSame(Velocity other)
    {
        var divs = new[] { X / other.X, Y / other.Y, Z / other.Z };
        return divs.Distinct().Count() == 1; // they are all the same
    }
}
