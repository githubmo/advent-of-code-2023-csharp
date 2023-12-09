// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);
using var enumerator = lines.GetEnumerator();
enumerator.MoveNext();

var labelRegex = new Regex(@"(\w{3}) = \((\w{3}), (\w{3})");
var directions = enumerator.Current.ToCharArray();
enumerator.MoveNext();

var map = new Dictionary<string, LeftRight>();
while (enumerator.MoveNext())
{
    var groups = labelRegex.Match(enumerator.Current).Groups;
    var (key, left, right) = (groups[1].Value, groups[2].Value, groups[3].Value);
    map[key] = new LeftRight(left, right);
}


//13207
var starts = map.Keys.Where(k => k.Last() == 'A').ToList();
var finals = new List<long>();
foreach (var start in starts)
{
    var steps = 0L;
    var current = start;
    while (current.Last() != 'Z')
    {
        // LRLRLLRLRLRRLRLRLRRLRLRLLRRLRRLRLRLRLLR
        var direction = directions[steps % directions.Length];
        var options = map[current];
        current = direction == 'R' ? options.Right : options.Left;
        steps++;
    }

    finals.Add(steps);
}


Console.WriteLine(finals.Aggregate((x, y) => Lcm(x, y)));
return;

long Lcm(long a, long b)
{
    if (a == 0 || b == 0) return 0;
    var higher = Math.Max(a, b);
    var lower = Math.Min(a, b);
    var lcm = higher;
    while (lcm % lower != 0) lcm += higher;
    return lcm;
}

internal record LeftRight(string Left, string Right);
