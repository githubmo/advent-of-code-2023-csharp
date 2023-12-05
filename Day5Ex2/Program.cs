// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

const string inputResource = "Resources/input.txt";
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
        sourceSteps.Add(new Range(from, from + last - 1));
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
                    .Where(m => m.ContainsRange(range))
                    .OrderBy(m => m.SourceRange.From)
                    .ToList();

                if (orderedMappings.Count == 0)
                    return new List<Range> { range };
                var ranges = new List<Range>();
                var minSource = orderedMappings.Select(m => m.SourceRange.From).Min();
                if (range.From < minSource)
                    ranges.Add(range with { To = minSource - 1 });
                else minSource = range.From;
                var maxSource = orderedMappings.Select(m => m.SourceRange.To).Max();
                if (range.To > maxSource)
                    ranges.Add(range with { From = maxSource + 1 });
                else maxSource = range.To;

                foreach (var m in orderedMappings)
                {
                    if (minSource < m.SourceRange.From)
                    {
                        ranges.Add(new Range(minSource, m.SourceRange.From - 1));
                        minSource = m.SourceRange.From;
                    }

                    var localMax = Math.Min(maxSource, m.SourceRange.To);
                    ranges.Add(new Range(minSource + m.Diff, localMax + m.Diff)); // not sure

                    if (localMax < range.To)
                        minSource = localMax;
                    else break;
                }


                return ranges.Distinct();
            }).ToList();
            Console.WriteLine("blah");
        }
    }

    Console.WriteLine(destinationSteps.Select(r => r.From).Min());
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
}

internal sealed record MappingRange(long DestFrom, SourceRange SourceRange)
{
    public long Diff = DestFrom - SourceRange.From;

    public bool ContainsRange(Range r)
    {
        var result = !(r.To < SourceRange.From || r.From > SourceRange.To);
        return result;
    }
}

internal sealed record Range(long From, long To);
