// See https://aka.ms/new-console-template for more information

using System.Text;

var linesEnumerator = File.ReadLines("Resources/input.txt", Encoding.UTF8).GetEnumerator();
var instructions = new Dictionary<string, InstructionSet>();
var partSets = new List<PartSet>();
using (linesEnumerator)
{
    // gather the InstructionSet
    while (linesEnumerator.MoveNext())
    {
        var current = linesEnumerator.Current;
        if (string.IsNullOrWhiteSpace(current)) break;
        var tokens = current.Split('{', '}');
        var label = tokens[0];
        var rulesTokens = tokens[1].Split(',');
        var order = 0;
        var rules = rulesTokens.TakeWhile(s => s.Contains(':')).Select(s =>
        {
            var type = s[0];
            var c = FromChar(s[1]);
            var valueAndDestination = string.Join("", s.Skip(2)).Split(":");
            var value = int.Parse(valueAndDestination[0]);
            return (type, new Rule(c, value, order++, valueAndDestination[1]));
        }).ToList();

        var instructionSet = new InstructionSet(label, rules, rulesTokens.Last());
        instructions.Add(label, instructionSet);

        // Console.WriteLine(instructionSet);
    }

    // gather inputs
    while (linesEnumerator.MoveNext())
    {
        var current = linesEnumerator.Current;
        var splits = string.Join("", current.Where(c => c is not '{' and not '}')).Split(',').ToList();
        var partSet = new PartSet(0, 0, 0, 0);
        partSet = splits.Aggregate(partSet, (p, s) =>
        {
            var partAndValue = s.Split("=");
            var value = int.Parse(partAndValue[1]);
            return partAndValue[0] switch
            {
                "x" => p with { X = value },
                "m" => p with { M = value },
                "a" => p with { A = value },
                "s" => p with { S = value },
                _ => throw new ArgumentOutOfRangeException(nameof(p), $"{p} is not a supported part")
            };
        });
        partSets.Add(partSet);
        // Console.WriteLine(partSet);
    }
}

var accepted = new List<PartSet>();
foreach (var partSet in partSets)
{
    var destination = "in";
    while (destination is not ("A" or "R"))
    {
        var instruction = instructions[destination];
        var (_, rule) = instruction.Rules.FirstOrDefault(r => r.Item1 switch
        {
            'x' => r.Item2.CanPass(partSet.X),
            'm' => r.Item2.CanPass(partSet.M),
            'a' => r.Item2.CanPass(partSet.A),
            's' => r.Item2.CanPass(partSet.S),
            _ => throw new ArgumentOutOfRangeException()
        }, ('b', new Rule(Comparator.LessThan, 0, 0, instruction.Default)));
        destination = rule.Destination;
    }

    if (destination == "A") accepted.Add(partSet);
}

Console.WriteLine(accepted.Sum(s => s.Sum));

Comparator FromChar(char c)
{
    return c switch
    {
        '>' => Comparator.MoreThan,
        '<' => Comparator.LessThan,
        _ => throw new ArgumentOutOfRangeException(nameof(c), "not supported comparator")
    };
}

internal enum Comparator
{
    LessThan = '<',
    MoreThan = '>'
}

internal record PartSet(int X, int M, int A, int S)
{
    public int Sum => X + M + A + S;
}

internal record Rule(Comparator Comparator, int Value, int Order, string Destination)
{
    public bool CanPass(int v)
    {
        return Comparator switch
        {
            Comparator.LessThan => v < Value,
            Comparator.MoreThan => v > Value,
            _ => throw new ArgumentOutOfRangeException(nameof(Comparator), "Invalid comparator")
        };
    }
}


internal record InstructionSet(string Label, List<(char c, Rule)> Rules, string Default);
