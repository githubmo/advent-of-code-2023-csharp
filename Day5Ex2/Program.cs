// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

const string inputResource = "Resources/example.txt";
var lines = File.ReadLines(inputResource, Encoding.UTF8);

// used to grab the mapping
var mapRegex = new Regex(@"(\w+)-to-(\w+)");

var mappingsToRanges = new Dictionary<string, List<MappingRange>>();
var currentMappingIndex = "";
foreach (var line in lines)
{
    if (string.IsNullOrWhiteSpace(line) || line.Contains("seeds: ")) continue;
    if (mapRegex.IsMatch(line))
    {
        currentMappingIndex = mapRegex.Match(line).Value;
        mappingsToRanges.Add(currentMappingIndex, []);
        continue;
    }

    var numbers = NumberMappings(line);
    var (destStart, sourceStart, rangeLength) = (numbers[0], numbers[1], numbers[2]);
    var rangeSet = new MappingRange(destStart, new SourceRange(sourceStart, rangeLength));
    mappingsToRanges[currentMappingIndex].Add(rangeSet);
}

var enumerator = File.ReadLines(inputResource, Encoding.UTF8).GetEnumerator();

using (enumerator)
{
    enumerator.MoveNext();
    var seedsLine = enumerator.Current;
    var ss = seedsLine
        .Replace("seeds: ", "")
        .Trim()
        .Split(" ")
        .Select((value, index) => new { value, index })
        .GroupBy(x => x.index / 2, x => long.Parse(x.value));
    var sourceSteps = new List<Range>();
    foreach (var group in ss)
    {
        var from = group.First();
        var last = group.Last();
        for (var l = from; l < from + last; l++) sourceSteps.Add(new Range(from, from + last - 1));
    }

    var destinationSteps = sourceSteps;

    while (enumerator.MoveNext())
    {
        // get the current line
        var line = enumerator.Current;

        // if the line is a new set of mappings, 
        if (mapRegex.IsMatch(line))
        {
            currentMappingIndex = mapRegex.Match(line).Value;
            sourceSteps = destinationSteps;
            var currentMappings = mappingsToRanges[currentMappingIndex];
            destinationSteps = sourceSteps.SelectMany(range =>
            {
                // foreach (var currentMapping in currentMappings.Where(currentMapping =>
                //              currentMapping.SourceRange.IsInRange(range)))
                //     return range - currentMapping.SourceRange.From + currentMapping.DestFrom;

                var orderedMappings = currentMappings
                    .Where(m => !(range.From < m.SourceRange.From && range.To < m.SourceRange.From))
                    .Where(m => !(range.From > m.SourceRange.To))
                    .OrderBy(m => m.SourceRange.From)
                    .ToList();

                if (orderedMappings.Count == 0) return new List<Range> { range };
                var ranges = new List<Range>();
                var minSource = orderedMappings.Select(m => m.SourceRange.From).Min();
                if (range.From < minSource) ranges.Add(new Range(range.From, minSource - 1));
                var

                orderedMappings.SelectMany(m =>
                {
                    var sourceRange = m.SourceRange;
                    if (range.To < sourceRange.From || range.From > sourceRange.To) return new List<Range> { range };
                    if (sourceRange.From > range.From) ranges.Add(new Range(range.From, sourceRange.From - 1));
                })


                return range;
            }).ToList();
            Console.WriteLine("blah");
        }
    }

    Console.WriteLine(destinationSteps.Min());
}

// Console.WriteLine(sum);
return;

// extract numbers

List<long> NumberMappings(string line)
{
    return line.Trim().Split(" ").Select(long.Parse).ToList();
}

internal sealed record SourceRange(long From, long RangeLength)
{
    public long To => From + RangeLength - 1;

    public bool IsInRange(long input)
    {
        return input >= From && input <= To;
    }
}

internal sealed record MappingRange(long DestFrom, SourceRange SourceRange);

internal sealed record Range(long From, long To);
