// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);

var instructionRegex = new Regex(@"(\w)\s+(\d+)\s+\(\#(\w+)\)");


var sum = 0;
var instructionsList = new List<Instruction>();
foreach (var line in lines)
{
    var groups = instructionRegex.Match(line).Groups;
    var instruction = new Instruction(FromChar(groups[1].Value[0]), int.Parse(groups[2].Value), groups[3].Value);
    // Console.WriteLine(instruction);
    instructionsList.Add(instruction);
}

var points = new List<(long i, long j)>();
points.Add((1, 1));

foreach (var newPoint in from instruction in instructionsList
         let lastPoint = points.Last()
         let steps = instruction.HexSteps
         let direction = instruction.HexDirection
         select direction switch
         {
             Direction.R => (lastPoint.i, lastPoint.j + steps),
             Direction.L => (lastPoint.i, lastPoint.j - steps),
             Direction.D => (lastPoint.i + steps, lastPoint.j),
             Direction.U => (lastPoint.i - steps, lastPoint.j),
             _ => throw new ArgumentOutOfRangeException(nameof(direction), "unsupported direction")
         })
    points.Add(newPoint);

var pointsInside = CalculateArea();
var edges = instructionsList.Select(i => i.HexSteps).Sum();


Console.WriteLine(edges / 2 + pointsInside - 1 + 2);
return;


long CalculateArea()
{
    double area = 0;
    var j = points.Count() - 1;

    for (var i = 0; i < points.Count(); i++)
    {
        area += (points[j].Item1 + points[i].Item1) * (points[j].Item2 - points[i].Item2);
        j = i;
    }

    return (long)Math.Abs(area / 2);
}

Direction FromChar(char c)
{
    Enum.TryParse(c.ToString(), false, out Direction d);
    return d;
}

internal enum Direction
{
    R = 'R',
    U = 'U',
    L = 'L',
    D = 'D'
}

internal record Instruction(Direction Direction, long Steps, string Colour)
{
    public Direction HexDirection => Colour.Last() switch
    {
        '0' => Direction.R,
        '1' => Direction.D,
        '2' => Direction.L,
        '3' => Direction.U,
        _ => throw new ArgumentException($"{Colour} does not have a valid direction in its last digit")
    };

    public long HexSteps => Convert.ToInt32(Colour.Substring(0, 5), 16);
}
