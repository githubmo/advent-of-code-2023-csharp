// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);

var enumerator = lines.GetEnumerator();

// used to grab the mapping
var mapRegex = new Regex(@"(\w+)-to-(\w+)");

// This is a single pass algorithm optimised for speed and is not a godo example of solving for real-world prod systems.
// Assumptions here based on input:
//   - each x-to-y mappings come one after the other (in a real world prod solution, I would never assume this)
//   - mappings within a single x-to-y map don't overlap (if they did, we would have ambiguity)
// Therefore, by keeping track of what I have mapped from the beginning, I can update mappings as I go section by section,
// line by line, without the need keep track of the lookup table. This means I do it with a single pass
// 
// I'm also using the enumerator here rather than `for (line in lines)` to avoid checking for the seed line in every 
// iteration of the loop, further optimising for efficiency. That is where I draw the line for the efficient implementation,
// as beyond that, I try and make my code as readable as possible even if it means I lose a little efficiency here and there.
// The main thing I achieve is O(n) time and O(1) space and beyond that everything else is trivial.
using (enumerator)
{
    enumerator.MoveNext();
    var seedsLine = enumerator.Current;
    var sourceSteps = seedsLine
        .Replace("seeds: ", "")
        .Trim()
        .Split(" ")
        .Select(long.Parse).ToHashSet();
    var destinationSteps = new HashSet<long>();

    while (enumerator.MoveNext())
    {
        // get the current line
        var line = enumerator.Current;

        // if the line is a new set of mappings, 
        if (mapRegex.IsMatch(line))
        {
            destinationSteps.UnionWith(sourceSteps);
            sourceSteps = destinationSteps;
            destinationSteps = [];
            continue;
        }

        // if the line is empty, or we don't have any more source steps to map, continue to next iteration
        if (string.IsNullOrWhiteSpace(line) || sourceSteps.Count == 0) continue;

        // Now that we dealt with the edge cases, the real meat of the solution starts here
        var numbers = numberMappings(line);

        // extract to what we want
        var (destStart, sourceStart, rangeLength) = (numbers[0], numbers[1], numbers[2]);


        foreach (var source in sourceSteps.Where(source => source >= sourceStart && source < sourceStart + rangeLength))
        {
            destinationSteps.Add(source - sourceStart + destStart);
            sourceSteps.Remove(source);
        }
    }

    Console.WriteLine(destinationSteps.Concat(sourceSteps).Min());
}

// Console.WriteLine(sum);
return;

// extract numbers
List<long> numberMappings(string line)
{
    return line.Trim().Split(" ").Select(long.Parse).ToList();
}
