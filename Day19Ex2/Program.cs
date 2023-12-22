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

// px{a<2006:qkq,m>2090:A,rfg}
// pv{a>1716:R,A}
// lnx{m>1548:A,A}
// rfg{s<537:gd,x>2440:R,A}
// qs{s>3448:A,lnx}
// qkq{x<1416:A,crn}
// crn{x>2662:A,R}
// in{s<1351:px,qqz}
// qqz{s>2770:qs,m<1801:hdj,R}
// gd{a>3333:R,R}
// hdj{m>838:A,pv}

var boundaries = new List<Boundaries>();

var problemQueue = new Queue<(string label, Boundaries)>();
problemQueue.Enqueue(("in", new Boundaries(
    new Dictionary<char, Boundary>
    {
        { 'x', new Boundary(1, 4000) },
        { 'm', new Boundary(1, 4000) },
        { 'a', new Boundary(1, 4000) },
        { 's', new Boundary(1, 4000) }
    }
)));

while (problemQueue.Count > 0)
{
    var problem = problemQueue.Dequeue();
    var (l, b) = (problem.label, problem.Item2);
    var instruction = instructions[l];
    var rules = instruction.Rules.Where(r => r.c != 'R');
    var prevRules = new List<(char c, Rule r)>();
    foreach (var r in rules)
    {
        var rule = r.Item2;
        if (rule.Destination == "R") continue;
        var newBoundaries = rule.Comparator switch
        {
            // less than 101 means upper boundary of 100
            Comparator.LessThan =>
                b with
                {
                    boundaries = b.boundaries.Select(keyValue =>
                    {
                        if (keyValue.Key == r.c)
                            return (r.c,
                                keyValue.Value with { Upper = Math.Min(keyValue.Value.Upper, rule.Value - 1) });
                        return (keyValue.Key, keyValue.Value);
                    }).ToDictionary()
                },
            // more than 99 means lower boundary is 100
            Comparator.MoreThan =>
                b with
                {
                    boundaries = b.boundaries.Select(keyValue =>
                    {
                        if (keyValue.Key == r.c)
                            return (r.c,
                                keyValue.Value with { Lower = Math.Max(keyValue.Value.Lower, rule.Value + 1) });
                        return (keyValue.Key, keyValue.Value);
                    }).ToDictionary()
                },
            _ => throw new ArgumentOutOfRangeException()
        };
        foreach (var rr in prevRules)
        {
            var invalidRule = rr.r;
            newBoundaries = invalidRule.Comparator switch
            {
                Comparator.LessThan =>
                    newBoundaries with
                    {
                        boundaries = newBoundaries.boundaries.Select(keyValue =>
                        {
                            if (keyValue.Key == rr.c)
                                return (rr.c,
                                    keyValue.Value with { Lower = Math.Max(keyValue.Value.Lower, invalidRule.Value) });
                            return (keyValue.Key, keyValue.Value);
                        }).ToDictionary()
                    },
                Comparator.MoreThan =>
                    newBoundaries with
                    {
                        boundaries = newBoundaries.boundaries.Select(keyValue =>
                        {
                            if (keyValue.Key == rr.c)
                                return (rr.c,
                                    keyValue.Value with { Upper = Math.Min(keyValue.Value.Upper, invalidRule.Value) });
                            return (keyValue.Key, keyValue.Value);
                        }).ToDictionary()
                    },
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        prevRules.Add(r);
        if (rule.Destination == "A")
        {
            Console.WriteLine(l);
            Console.WriteLine(string.Join("::::", newBoundaries.boundaries));
            boundaries.Add(newBoundaries);
        }
        else
        {
            problemQueue.Enqueue((rule.Destination, newBoundaries));
        }
    }

    var boundariesForDefault = b;
    foreach (var instructionRule in instruction.Rules)
    {
        var rule = instructionRule.Item2;
        var c = instructionRule.c;

        boundariesForDefault = rule.Comparator switch
        {
            // less than 101 means upper boundary of 100
            Comparator.LessThan =>
                boundariesForDefault with
                {
                    boundaries = boundariesForDefault.boundaries.Select(keyValue =>
                    {
                        if (keyValue.Key == c)
                            return (c,
                                keyValue.Value with { Lower = Math.Max(keyValue.Value.Lower, rule.Value) });
                        return (keyValue.Key, keyValue.Value);
                    }).ToDictionary()
                },
            // more than 99 means lower boundary is 100
            Comparator.MoreThan =>
                boundariesForDefault with
                {
                    boundaries = boundariesForDefault.boundaries.Select(keyValue =>
                    {
                        if (keyValue.Key == c)
                            return (c,
                                keyValue.Value with { Upper = Math.Min(keyValue.Value.Upper, rule.Value) });
                        return (keyValue.Key, keyValue.Value);
                    }).ToDictionary()
                },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    if (instruction.Default == "A")
    {
        Console.WriteLine(l);
        Console.WriteLine(string.Join("::::", boundariesForDefault.boundaries));
        boundaries.Add(boundariesForDefault);
    }
    else if (instruction.Default != "R")
    {
        problemQueue.Enqueue((instruction.Default, boundariesForDefault));
    }
}


// foreach (var instruction in instructions.Values)
// {
//     var rules = instruction.Rules.Select(r =>
//     {
//         var rule = r.Item2;
//         if (rule.Destination != "R") return r;
//         return rule.Comparator == Comparator.LessThan
//             ?
//             // less than 101 rejected => more than 100 accepted 
//             (r.c, rule with { Comparator = Comparator.MoreThan, Value = rule.Value - 1 })
//             :
//             // more than 99 rejected => less than 100 accepted
//             (r.c, rule with { Comparator = Comparator.LessThan, Value = rule.Value + 1 });
//     });
//     foreach ((char c, Rule rule) tuple in rules)
//     {
//         var rule = tuple.rule;
//         switch (rule.Comparator)
//         {
//             // less than 101 means upper boundary of 100
//             case Comparator.LessThan:
//                 boundaries[tuple.c] = boundaries[tuple.c] with
//                 {
//                     Upper = Math.Min(boundaries[tuple.c].Upper, rule.Value - 1)
//                 };
//                 break;
//             // more than 99 means lower boundary is 100
//             case Comparator.MoreThan:
//                 boundaries[tuple.c] = boundaries[tuple.c] with
//                 {
//                     Lower = Math.Max(boundaries[tuple.c].Lower, rule.Value + 1)
//                 };
//                 break;
//         }
//     }
// }

Console.WriteLine(boundaries.Sum(b => b.Sum));

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

internal record Boundary(long Lower, long Upper);

internal record Boundaries(Dictionary<char, Boundary> boundaries)
{
    public long Sum => boundaries.Values.Aggregate(1L, (acc, b) => acc * (b.Upper - b.Lower + 1));
}
