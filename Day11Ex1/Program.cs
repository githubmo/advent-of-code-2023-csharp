// See https://aka.ms/new-console-template for more information

using System.Text;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);
var newLines = new List<LinkedList<char>>();
var sum = 0L;
foreach (var line in lines)
{
    if (line.ToCharArray().All(c => c == '.')) newLines.Add(new LinkedList<char>(line.ToCharArray()));
    newLines.Add(new LinkedList<char>(line.ToCharArray()));
}

var heads = newLines.Select(l => l.First).ToList();
while (true)
    if (heads.All(n => n.Value == '.'))
        heads = heads.Select((n, index) =>
        {
            newLines[index].AddBefore(n, new LinkedListNode<char>('.'));
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
        coords.Add(new Coord(i, j));

while (coords.Count > 0)
{
    var coord = coords.First();
    coords.RemoveAt(0);
    sum = coords.Aggregate(sum, (current, c) => current + coord.Distance(c));
}

Console.WriteLine(string.Join("\n", newLines.Select(l => string.Join("", l))));
Console.WriteLine(sum);

internal record Coord(int I, int J)
{
    public int Distance(Coord other)
    {
        return Math.Abs(other.I - I) + Math.Abs(other.J - J);
    }
}
