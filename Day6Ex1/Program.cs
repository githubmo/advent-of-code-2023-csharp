// See https://aka.ms/new-console-template for more information

using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;

// it's only two lines
var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8).ToList();
var numRegex = new Regex(@"[^\d]+");
Console.WriteLine(numRegex);
var times = numRegex.Split(lines[0]).Where(d => !string.IsNullOrWhiteSpace(d)).Select(long.Parse);
var distances = numRegex.Split(lines[1]).Where(d => !string.IsNullOrWhiteSpace(d)).Select(long.Parse);
var timesAndDistancePair = times.Zip(distances); 
var cb = new ConcurrentBag<long>();

Parallel.ForEach(timesAndDistancePair, tuple =>
{
    var time = tuple.First;
    var distance = tuple.Second;
    var numberOfWays = 0L;

    for (var l = 1L; l < time; l++)
    {
        var t = time - l;
        if (t * l > distance) numberOfWays++;
    }

    cb.Add(numberOfWays);
});

Console.WriteLine(cb.Aggregate(1L, (acc, x) => acc * x));
