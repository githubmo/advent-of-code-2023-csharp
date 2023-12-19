// See https://aka.ms/new-console-template for more information

using System.Text;

var map = File
    .ReadAllLines("Resources/input.txt", Encoding.UTF8)
    .Select(s => s.ToCharArray()).ToArray();

var allEdgeBeams = new List<Beam>();
for (var i = 0; i < map.Length; i++)
{
    allEdgeBeams.Add(new Beam(Direction.Right, new Point(i, 0)));
    allEdgeBeams.Add(new Beam(Direction.Left, new Point(i, map[0].Length - 1)));
}

for (var j = 0; j < map[0].Length; j++)
{
    allEdgeBeams.Add(new Beam(Direction.Down, new Point(0, j)));
    allEdgeBeams.Add(new Beam(Direction.Up, new Point(map.Length - 1, j)));
}

;

Console.WriteLine(allEdgeBeams.Select(NumOfEnergisedTiles).Max());
return;

int NumOfEnergisedTiles(Beam b)
{
    var beams = new List<Beam> { b };
    var visited = map.Select(a => (char[])a.Clone()).ToArray();
    foreach (var row in visited)
        for (var j = 0; j < map[0].Length; j++)
            row[j] = '.';

    var beamsSet = new HashSet<Beam>();
    while (beams.Any(b => b.Direction != Direction.Stop))
        beams = beams.SelectMany(b =>
        {
            b = NormaliseBeam(b);
            var p = b.Point;
            if (!(p.I < 0 || p.J < 0 || p.I >= map.Length || p.J >= map[0].Length)) visited[p.I][p.J] = '#';
            if (b.Direction == Direction.Stop || beamsSet.Contains(b)) return Array.Empty<Beam>();
            beamsSet.Add(b);
            var current = map[p.I][p.J];
            if (current == '.' || (b.Direction is Direction.Up or Direction.Down && current == '|') ||
                (b.Direction is Direction.Left or Direction.Right && current == '-'))
            {
                var (moveI, moveJ) = NextStep(b.Direction);
                moveI += p.I;
                moveJ += p.J;
                return new[] { b with { Point = new Point(moveI, moveJ) } };
            }

            switch (current)
            {
                case '/':
                {
                    var newDirection = b.Direction switch
                    {
                        Direction.Up => Direction.Right,
                        Direction.Down => Direction.Left,
                        Direction.Left => Direction.Down,
                        Direction.Right => Direction.Up,
                        Direction.Stop => Direction.Stop,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    var (moveI, moveJ) = NextStep(newDirection);
                    moveI += p.I;
                    moveJ += p.J;
                    return new[] { new Beam(newDirection, new Point(moveI, moveJ)) };
                }
                case '\\':
                {
                    var newDirection = b.Direction switch
                    {
                        Direction.Up => Direction.Left,
                        Direction.Down => Direction.Right,
                        Direction.Left => Direction.Up,
                        Direction.Right => Direction.Down,
                        Direction.Stop => Direction.Stop,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    var (moveI, moveJ) = NextStep(newDirection);
                    moveI += p.I;
                    moveJ += p.J;
                    return new[] { new Beam(newDirection, new Point(moveI, moveJ)) };
                }
                case '|' when b.Direction is Direction.Left or Direction.Right:
                    return new[]
                    {
                        new Beam(Direction.Up, b.Point with { I = b.Point.I - 1 }),
                        new Beam(Direction.Down, b.Point with { I = b.Point.I + 1 })
                    };
                case '-' when b.Direction is Direction.Up or Direction.Down:
                    return new[]
                    {
                        new Beam(Direction.Left, b.Point with { J = b.Point.J - 1 }),
                        new Beam(Direction.Right, b.Point with { J = b.Point.J + 1 })
                    };
                default: throw new InvalidProgramException("case not covered");
            }
        }).ToList();
    Console.WriteLine(string.Join("\n", visited.Select(l => string.Join("", l))));
    return visited.Sum(l => l.Count(c => c == '#'));
}

Beam NormaliseBeam(Beam b)
{
    var p = b.Point;
    if (p.I < 0 || p.J < 0 || p.I >= map.Length || p.J >= map[0].Length) return b with { Direction = Direction.Stop };

    return b;
}

(int, int) NextStep(Direction direction)
{
    return direction switch
    {
        Direction.Up => (-1, 0),
        Direction.Right => (0, 1),
        Direction.Down => (1, 0),
        Direction.Left => (0, -1),
        Direction.Stop => (0, 0),
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };
}

internal enum Direction
{
    Up,
    Right,
    Down,
    Left,
    Stop
}

internal record Point(int I, int J);

internal record Beam(Direction Direction, Point Point);
