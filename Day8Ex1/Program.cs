// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);
var enumerator = lines.GetEnumerator();
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


var steps = 0L;

//13207
var current = "AAA";
while (current != "ZZZ")
{
    // LRLRLLRLRLRRLRLRLRRLRLRLLRRLRRLRLRLRLLR
    var direction = directions[steps % directions.Length];
    var options = map[current];
    current = direction == 'R' ? options.Right : options.Left;
    Console.WriteLine(current);
    steps++;
}

Console.WriteLine(steps);

internal record LeftRight(string Left, string Right);
