// See https://aka.ms/new-console-template for more information

using System.Text;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);
var sum = 0L;
var newLines = lines.Select(line => line.ToCharArray().All(c => c == '.')
        ? new LinkedList<char>(line.ToCharArray().Select(unused => 'm'))
        : new LinkedList<char>(line.ToCharArray()))
    .ToList();

var heads = newLines.Select(l => l.First).ToList();
while (true)
    if (heads.All(n => n.Value is '.' or 'm'))
        heads = heads.Select((n, index) =>
        {
            // newLines[index].AddBefore(n, new LinkedListNode<char>('.'));
            n.Value = n.Value == '.' ? 'm' : 'M';
            return n.Next;
        }).ToList();
    else if (heads.All(n => n.Next != null))
        heads = heads.Select((n, index) => n!.Next).ToList();
    else break;

var newLinesArray = newLines.Select(l => l.ToArray()).ToArray();
var coords = new List<Coord>();
for (var i = 0; i < newLinesArray.Length; i++)
for (var j = 0; j < newLinesArray[0].Length; j++)
    if (newLinesArray[i][j] == '#')
        coords.Add(new Coord(i, j, newLinesArray));

while (coords.Count > 0)
{
    var coord = coords.First();
    coords.RemoveAt(0);
    sum = coords.Aggregate(sum, (current, c) => current + coord.Distance(c));
}

Console.WriteLine(string.Join("\n", newLines.Select(l => string.Join("", l))));
Console.WriteLine(sum);

internal record Coord(int I, int J, char[][] newLinesArray)
{
    private long Multiple => 1000000L;
    private long MultSquared => Multiple * Multiple;

    public long Distance(Coord other)
    {
        var (minI, maxI) = (Math.Min(I, other.I), Math.Max(I, other.I));
        var (minJ, maxJ) = (Math.Min(J, other.J), Math.Max(J, other.J));
        var (distance1, distance2) = (0L, 0L);
        if (minI != maxI)
            distance1 += Enumerable.Range(minI, maxI - minI).Sum(n =>
            {
                var c = newLinesArray[n][minJ];
                return c switch
                {
                    'M' => MultSquared,
                    'm' => Multiple,
                    _ => 1
                };
            });
        if (minJ != maxJ)
            distance1 += Enumerable.Range(minJ, maxJ - minJ).Sum(n =>
            {
                var c = newLinesArray[maxI][n];
                return c switch
                {
                    'M' => MultSquared,
                    'm' => Multiple,
                    _ => 1
                };
            });

        if (minI != maxI)
            distance2 += Enumerable.Range(minI, maxI - minI).Sum(n =>
            {
                var c = newLinesArray[n][maxJ];
                return c switch
                {
                    'M' => MultSquared,
                    'm' => Multiple,
                    _ => 1
                };
            });
        if (minJ != maxJ)
            distance2 += Enumerable.Range(minJ, maxJ - minJ).Sum(n =>
            {
                var c = newLinesArray[minI][n];
                return c switch
                {
                    'M' => MultSquared,
                    'm' => Multiple,
                    _ => 1
                };
            });

        return Math.Min(distance1, distance2);
    }
}
