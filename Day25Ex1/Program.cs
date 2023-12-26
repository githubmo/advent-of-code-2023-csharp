// See https://aka.ms/new-console-template for more information

using System.Text;
using MoreLinq;
using MoreLinq.Extensions;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);

var sum = 0;
var wires = new Dictionary<string, HashSet<string>>();
var individualConnections = new HashSet<(string, string)>();

// for debugging and visualisation
var dotFile = new StringBuilder();
dotFile.AppendLine("graph {");

foreach (var line in lines)
{
    var parts = line.Split(": ");
    var wire = parts[0].Trim();
    var connections = parts[1].Trim().Split(" ").ToList();

    // wires are bidirectional, if we have x <-> y, we want to add an entry for x and y
    if (!wires.ContainsKey(wire)) wires[wire] = [];

    foreach (var connection in connections)
    {
        // Generate content for a GraphViz graph.
        dotFile.AppendLine($"    {wire} -- {connection}");

        if (!wires.ContainsKey(connection)) wires[connection] = [];

        wires[wire].Add(connection);
        wires[connection].Add(wire);

        if (!individualConnections.Contains((wire, connection)) || !individualConnections.Contains((connection, wire)))
            individualConnections.Add((wire, connection));
    }
}

dotFile.AppendLine("}");
File.WriteAllText("graph.dot", dotFile.ToString());

var allNodes = wires.Keys.ToList();

// I am going for a brute force approach because I am a dumb ass
// the idea is I'm going to pick two random nodes, find the path that connects them
// then count the number of times I see each node in each path I do this over a number of times.
// The three with the highest number of visits are almost always the ones that are the "bridges" 
// between the two separated sub-graphs
var counts =
    ToDictionaryExtension.ToDictionary(individualConnections.Select(s => (s, 0L)));

MoreEnumerable.ForEach(Enumerable.Range(1, 100), _ =>
{
    var randomSubset = MoreEnumerable.RandomSubset(allNodes, 2).ToList();
    Console.WriteLine(string.Join("<>", randomSubset));
    var path = GetPath(randomSubset.First(), randomSubset.Last());
    var allPairs = path.Zip(path.Skip(1)).ToList();
    allPairs.ForEach(n =>
    {
        if (!counts.ContainsKey(n)) n = (n.Second, n.First);
        counts[n]++;
    });
});
// var wireEnds = wires.Keys.ToList().Sub(3).ToList();
// Console.WriteLine(string.Join("\n", individualConnections));
// Console.WriteLine(individualConnections.Count);
// Console.WriteLine(string.Join("\n", wireEnds.Select(l => string.Join("<>", l))));
var orderedCounts = counts.ToList().OrderBy(kv => kv.Value).ToList();
orderedCounts.ForEach(kv => Console.WriteLine($"{kv.Key}<<>>{kv.Value}"));

// reverse and take the 3 highest visits
orderedCounts.Reverse();
var banned = orderedCounts.Take(3).ToList();
var bannedPairs = banned.Select(kv => kv.Key).ToList();
var f = bannedPairs.First();
var count1 = AllConnected(f.Item1, bannedPairs).Count;
var count2 = AllConnected(f.Item2, bannedPairs).Count;
Console.WriteLine(count1);
Console.WriteLine(count2);
Console.WriteLine(count1 * count2);

return;


List<string> GetPath(string first, string second)
{
    if (first == second) return [first];

    var paths = new List<List<string>>();
    MoreEnumerable.ForEach(wires[first], n => paths.Add([n]));
    while (paths.All(p => p.Last() != second))
        paths = paths.SelectMany(path =>
        {
            var last = path.Last();
            var nexts = wires[last].Where(n => !path.Contains(n));
            return nexts.Select(n => path.Concat([n]).ToList());
        }).ToList();

    return paths.First(p => p.Last() == second);
}

HashSet<string> AllConnected(string s, List<(string, string)> banned)
{
    var queue = new Queue<string>();
    var result = new HashSet<string>();
    queue.Enqueue(s);
    while (queue.Count > 0)
    {
        var current = queue.Dequeue();
        var nexts = wires[current];
        foreach (var next in nexts.Where(next =>
                     !(banned.Contains((current, next)) || banned.Contains((next, current))) && !result.Contains(next)))
        {
            queue.Enqueue(next);
            result.Add(next);
        }
    }

    return result;
}
