// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);

var dotsRegex = new Regex(@"\.+");

var sum = 0;
foreach (var line in lines)
{
    var inputAndRanges = line.Split(" ");
    var input = inputAndRanges[0];
    var ranges = inputAndRanges[1].Split(',').Select(int.Parse).ToList();
    var results = GenerateAllPermutations(input, ranges);
    var counts =
        results
            .Select(s => dotsRegex.Split(s))
            .Select(s => s.Select(ss => ss.Length).Where(i => i != 0).ToList())
            .Where(ii => ii.SequenceEqual(ranges)).ToList();

    var count = counts.Count;

    Console.WriteLine($"{input} ::: {string.Join(",", ranges)} ::: {count}");
    sum += count;
    // Console.WriteLine($"{input} ==> {string.Join(" ::: ", GenerateAllPermutations(input))}");
}

Console.WriteLine(sum);

HashSet<string> GenerateAllPermutations(string input, List<int> ranges)
{
    var queue = new Queue<string>();
    var output = new HashSet<string>();
    var maxHash = ranges.Sum();

    queue.Enqueue(input);

    while (queue.Count > 0)
    {
        var current = queue.Dequeue();

        if (current.Contains('?'))
        {
            var index = current.IndexOf('?');
            if (index < 0) continue;
            var replacement1 = current.Remove(index, 1).Insert(index, ".");
            var replacement2 = current.Remove(index, 1).Insert(index, "#");

            queue.Enqueue(replacement1);
            queue.Enqueue(replacement2);
        }
        else
        {
            if (current.Count(c => c == '#') == maxHash)
                output.Add(current);
        }
    }

    return output;
}
