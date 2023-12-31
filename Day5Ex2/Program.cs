﻿// See https://aka.ms/new-console-template for more information

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

var stopWatch = new Stopwatch();
stopWatch.Start();
const string inputResource = "Resources/input.txt";
var lines = File.ReadLines(inputResource, Encoding.UTF8);

// used to grab the mapping
var mapRegex = new Regex(@"(\w+)-to-(\w+)");

var mappingsToRanges = new List<List<MappingRange>>();
var currentMappingIndex = "";
var sourceSteps = new List<Range>();
foreach (var line in lines)
{
    if (string.IsNullOrWhiteSpace(line) || line.Contains("seeds: "))
    {
        if (line.Contains("seeds: "))
        {
            var ss = line
                .Replace("seeds: ", "")
                .Trim()
                .Split(" ")
                .Select((value, index) => new { value, index })
                .GroupBy(x => x.index / 2, x => long.Parse(x.value));
            foreach (var group in ss)
            {
                var from = group.First();
                var last = group.Last();
                sourceSteps.Add(new Range(from, from + last - 1));
            }
        }

        continue;
    }

    if (mapRegex.IsMatch(line))
    {
        mappingsToRanges.Add([]);
        continue;
    }

    var numbers = NumberMappings(line);
    var (destStart, sourceStart, rangeLength) = (numbers[0], numbers[1], numbers[2]);
    var rangeSet = new MappingRange(destStart, new SourceRange(sourceStart, rangeLength));
    mappingsToRanges.Last().Add(rangeSet);
}

// var min = long.MaxValue;

var cb = new ConcurrentBag<long>();
Parallel.ForEach(sourceSteps, range =>
{
    Parallel.ForEach(FromTo(range.From, range.To)
        .GroupBy(x => x / 100000000)
        .Select(x => x.Select(y => y)), longs =>
    {
        var localMin = long.MaxValue;

        foreach (var l in longs)
        {
            var currentL = l;
            foreach (var ms in mappingsToRanges)
            {
                var captured = ms.Select(m => m.Apply(currentL)).Where(ll => ll != null);
                var firstOrDefault = captured.FirstOrDefault();
                currentL = firstOrDefault ?? currentL;
            }

            localMin = Math.Min(currentL, localMin);
        }

        cb.Add(localMin);
    });
});
//72263011
Console.WriteLine(cb.Min());
// Console.WriteLine(sum);
stopWatch.Stop();
Console.WriteLine(stopWatch.Elapsed);
return;

List<long> NumberMappings(string line)
{
    return line.Trim().Split(" ").Select(long.Parse).ToList();
}

IEnumerable<long> FromTo(long from, long to)
{
    for (var l = from; l < to; l++) yield return l;
}

internal sealed record SourceRange(long From, long RangeLength)
{
    public long To => From + RangeLength - 1;
}

internal sealed record MappingRange(long DestFrom, SourceRange SourceRange)
{
    public readonly long Diff = DestFrom - SourceRange.From;

    public bool ContainsRange(Range r)
    {
        var result = !(r.To < SourceRange.From || r.From > SourceRange.To);
        return result;
    }

    public long? Apply(long l)
    {
        return l >= SourceRange.From && l <= SourceRange.To ? l + Diff : null;
    }
}

internal sealed record Range(long From, long To);
