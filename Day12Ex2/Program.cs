// See https://aka.ms/new-console-template for more information

using System.Text;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);

var solutions = new Dictionary<string, long>();

var sum = 0L;
foreach (var line in lines)
{
    var inputAndRanges = line.Split(" ");
    var input = inputAndRanges[0];
    input = RepeatConcat(input, 5, '?');
    var ranges = RepeatConcat(inputAndRanges[1], 5, ',').Split(',').Select(int.Parse).ToList();

    var count = Solve(input, ranges);

    Console.WriteLine($"{input} ::: {string.Join(",", ranges)} ::: {count}");
    sum += count;
    // Console.WriteLine($"{input} ==> {string.Join(" ::: ", GenerateAllPermutations(input))}");
}

Console.WriteLine(sum);

return;

long Solve(string record, List<int> groups)
{
    var key = $"{record}::{string.Join(",", groups)}";
    if (solutions!.TryGetValue(key, out var value)) return value;

    // # Logic that treats the first character as pound-sign "#"
    if (groups.Count == 0) return record.Contains("#") ? 0 : 1;

    if (string.IsNullOrEmpty(record)) return 0;

    var nextChar = record[0];
    var nextGroup = groups[0];

    long Pound()
    {
        var thisGroup = string.Join("", record.Take(nextGroup)).Replace('?', '#');
        if (thisGroup != string.Join("", Enumerable.Repeat('#', nextGroup))) return 0;
        if (record.Length == nextGroup) return groups.Count == 1 ? 1 : 0;
        // record[next_group+1:]
        if (record[nextGroup] is '?' or '.')
            return Solve(string.Join("", record.Skip(nextGroup + 1)), groups.Skip(1).ToList());

        return 0;
    }

    long Dot()
    {
        return Solve(record.Substring(1), groups);
    }

    var result = nextChar switch
    {
        '#' => Pound(),
        '.' => Dot(),
        '?' => Dot() + Pound(),
        _ => throw new ArgumentException($"{nextChar} should not be part of input")
    };

    // Console.WriteLine($"{record} {string.Join(", ", groups)} ==> {result}");
    solutions[key] = result;
    return result;
}

string RepeatConcat(string s, int n, char c)
{
    return string.Join(c, Enumerable.Repeat(s, n));
}
