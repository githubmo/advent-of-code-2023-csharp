// See https://aka.ms/new-console-template for more information

using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;

// it's only two lines
var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8).ToList();
var numRegex = new Regex(@"[^\d]+");
Console.WriteLine(numRegex);
var time = long.Parse(string.Join("", numRegex.Split(lines[0]).Where(d => !string.IsNullOrWhiteSpace(d))));
var distance = long.Parse(string.Join("", numRegex.Split(lines[1]).Where(d => !string.IsNullOrWhiteSpace(d))));
var cb = new ConcurrentBag<long>();

var (min, max) = (0L, 0L);
for (var l = 1L; l < time; l++)
{
    var t = time - l;
    if (t * l > distance)
    {
        min = l;
        break;
    }
}

for (var l = time; l > 1; l--)
{
    var t = time - l;
    if (t * l > distance)
    {
        max = l;
        break;
    }
}

Console.WriteLine(max - min + 1); // +1 to include the 0th element
