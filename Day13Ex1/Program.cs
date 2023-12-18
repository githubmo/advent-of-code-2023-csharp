// See https://aka.ms/new-console-template for more information

using System.Text;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);

var maps = new List<List<string>>();
var currentMap = new List<string>();
foreach (var line in lines)
    if (string.IsNullOrEmpty(line))
    {
        maps.Add(currentMap);
        currentMap = [];
    }
    else
    {
        currentMap.Add(line);
    }

maps.Add(currentMap);

var middles = maps.Select(FindMiddle);
Console.WriteLine(string.Join("\n", middles));

var sum = middles.Sum(m => m.Value);
Console.WriteLine(sum);

Middle FindMiddle(List<string> map)
{
    var column = 0;
    for (var i = 1; i < map.Count; i++)
    {
        var count = Math.Min(i, map.Count - i);
        var first = map.Slice(i - count, count);
        var second = map.Slice(i, count);
        second.Reverse();
        if (!first.SequenceEqual(second)) continue;
        column = i;
        break;
    }

    var row = 0;
    var inverseMap =
        Enumerable
            .Range(0, map[0].Length)
            .Select(i => string.Join("", map.Select(s => s[i])))
            .ToList();
    for (var i = 1; i < inverseMap.Count; i++)
    {
        var count = Math.Min(i, inverseMap.Count - i);
        var first = inverseMap.Slice(i - count, count);
        var second = inverseMap.Slice(i, count);
        second.Reverse();
        if (!first.SequenceEqual(second)) continue;
        row = i;
        break;
    }

    return new Middle(row, column);
}

internal record Middle(int Column, int Row)
{
    public int Value => 100 * Row + Column;
}
