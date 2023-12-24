// See https://aka.ms/new-console-template for more information

using System.Text;

var lines = File.ReadAllText("Resources/input.txt", Encoding.UTF8);

Console.WriteLine(Solve(lines).Sum());

IEnumerable<int> Solve(string input)
{
    var bricks = Fall(ParseBricks(input));
    var supports = GetSupports(bricks);

    foreach (var desintegratedBrick in bricks)
    {
        var q = new Queue<Brick>();
        q.Enqueue(desintegratedBrick);

        var falling = new HashSet<Brick>();
        while (q.TryDequeue(out var brick))
        {
            falling.Add(brick);

            var bricksStartFalling =
                from brickT in supports.bricksAbove[brick]
                where supports.bricksBelow[brickT].IsSubsetOf(falling)
                select brickT;

            foreach (var brickT in bricksStartFalling) q.Enqueue(brickT);
        }

        yield return falling.Count - 1; // -1: desintegratedBrick doesn't count 
    }
}

// applies 'gravity' to the bricks.
Brick[] Fall(Brick[] bricks)
{
    // sort them in Z first so that we can work in bottom to top order
    bricks = bricks.OrderBy(brick => brick.Bottom).ToArray();

    for (var i = 0; i < bricks.Length; i++)
    {
        var newBottom = 1;
        for (var j = 0; j < i; j++)
            if (IntersectsXY(bricks[i], bricks[j]))
                newBottom = Math.Max(newBottom, bricks[j].Top + 1);
        var fall = bricks[i].Bottom - newBottom;
        bricks[i] = bricks[i] with
        {
            z = new Range(bricks[i].Bottom - fall, bricks[i].Top - fall)
        };
    }

    return bricks;
}

// calculate upper and lower neighbours for each brick
Supports GetSupports(Brick[] bricks)
{
    var bricksAbove = bricks.ToDictionary(b => b, _ => new HashSet<Brick>());
    var bricksBelow = bricks.ToDictionary(b => b, _ => new HashSet<Brick>());
    for (var i = 0; i < bricks.Length; i++)
    for (var j = i + 1; j < bricks.Length; j++)
    {
        var zNeighbours = bricks[j].Bottom == 1 + bricks[i].Top;
        if (zNeighbours && IntersectsXY(bricks[i], bricks[j]))
        {
            bricksBelow[bricks[j]].Add(bricks[i]);
            bricksAbove[bricks[i]].Add(bricks[j]);
        }
    }

    return new Supports(bricksAbove, bricksBelow);
}

bool IntersectsXY(Brick brickA, Brick brickB)
{
    return Intersects(brickA.x, brickB.x) && Intersects(brickA.y, brickB.y);
}

// see https://stackoverflow.com/a/3269471
bool Intersects(Range r1, Range r2)
{
    return r1.begin <= r2.end && r2.begin <= r1.end;
}

Brick[] ParseBricks(string input)
{
    return (
        from line in input.Split('\n')
        let numbers = line.Split(',', '~').Select(int.Parse).ToArray()
        select new Brick(
            new Range(numbers[0], numbers[3]),
            new Range(numbers[1], numbers[4]),
            new Range(numbers[2], numbers[5])
        )
    ).ToArray();
}

internal record Range(int begin, int end);

internal record Brick(Range x, Range y, Range z)
{
    public int Top => z.end;
    public int Bottom => z.begin;
}

internal record Supports(
    Dictionary<Brick, HashSet<Brick>> bricksAbove,
    Dictionary<Brick, HashSet<Brick>> bricksBelow
);
