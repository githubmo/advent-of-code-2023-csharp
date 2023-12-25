// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);

var (lows, highs) = (0L, 0L);
var steps = new Queue<(string label, string from, Pulse pulse)>();
var buttons = new Dictionary<string, Button>();
var broadcaster = new Broadcaster([]);
var buttonRegex = new Regex(@"(%|&)(\w+) -> (.+)");
foreach (var line in lines)
    if (line.Contains("broadcaster"))
    {
        broadcaster =
            new Broadcaster(
                line
                    .Replace("broadcaster -> ", "")
                    .Split(", ")
                    .Select(s => s.Trim())
                    .ToList());
    }
    else
    {
        var groups = buttonRegex.Match(line).Groups;
        if (groups[1].Value == "%")
            buttons.Add(groups[2].Value,
                new FlipFlip(groups[2].Value, groups[3].Value.Split(",").Select(s => s.Trim()).ToList()));
        else
            buttons.Add(groups[2].Value,
                new Conjunction(groups[2].Value, groups[3].Value.Split(",").Select(s => s.Trim()).ToList()));
        ;
    }

foreach (var button in buttons.Values)
    switch (button)
    {
        case Conjunction conjunction:
            var inputs = buttons.Where(kv => kv.Value.NextLabels.Contains(button.Label)).Select(kv => kv.Value);
            foreach (var input in inputs) conjunction.AddInput(input.Label);
            break;
    }

var i = 1;
var labelToButtons = new Dictionary<string, long>();
while (labelToButtons.Count < 4)
{
    foreach (var s in broadcaster.Nexts.Select(l => (l, Pulse.Low))) steps.Enqueue((s.l, "broadcaster", Pulse.Low));
    lows += broadcaster.Nexts.Count + 1;

    while (steps.Count > 0)
    {
        var (label, from, pulse) = steps.Dequeue();
        if (buttons.TryGetValue(label, out var button))
        {
            if (button is Conjunction cc)
            {
                if (cc.NextLabels.Contains("rx"))
                    if (pulse == Pulse.High && !labelToButtons.ContainsKey(from))
                        labelToButtons[from] = i;
                if (labelToButtons.Count == 4)
                {
                    Console.WriteLine(GetLowestCommonMultiple(labelToButtons.Values));
                    Console.WriteLine(i);
                    break;
                }
            }

            var nextSteps = button.Next(pulse, from);
            foreach (var t in nextSteps)
            {
                switch (t.pulse)
                {
                    case Pulse.High:
                        highs++;
                        break;
                    case Pulse.Low:
                        lows++;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                steps.Enqueue((t.label, button.Label, t.pulse));
            }
        }
    }

    i++;
}

long GetLowestCommonMultiple(IEnumerable<long> buttonPresses)
{
    var numbers = buttonPresses.ToList();
    var lcm = numbers[0];
    for (var i = 1; i < numbers.Count; i++) lcm = Lcm(lcm, numbers[i]);

    return lcm;
}

long Gcd(long a, long b)
{
    while (b != 0)
    {
        var t = b;
        b = a % b;
        a = t;
    }

    return a;
}

long Lcm(long a, long b)
{
    return a * (b / Gcd(a, b));
}

internal enum Pulse
{
    Low,
    High
}

internal interface Button
{
    string Label { get; init; }

    List<string> NextLabels { get; init; }

    List<(string label, Pulse pulse)> Next(Pulse pulse, string From);

    bool IsDefault();
}

internal enum State
{
    On,
    Off
}

internal record FlipFlip(string Label, List<string> NextLabels) : Button
{
    private State State { get; set; } = State.Off;

    public bool IsDefault()
    {
        return State == State.Off;
    }

    public List<(string label, Pulse pulse)> Next(Pulse pulse, string _)
    {
        if (pulse == Pulse.High) return [];
        switch (State)
        {
            case State.Off:
                State = State.On;

                return NextLabels.Select(l => (l, Pulse.High)).ToList();
            case State.On:
                State = State.Off;
                return NextLabels.Select(l => (l, Pulse.Low)).ToList();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

internal record Conjunction(string Label, List<string> NextLabels, Pulse SavedPulse = Pulse.Low) : Button
{
    private readonly Dictionary<string, long> _counts = [];
    private readonly Dictionary<string, long> _firstHighs = [];
    private readonly Dictionary<string, Pulse> _inputs = [];

    public bool IsDefault()
    {
        return _inputs.Values.All(p => p == Pulse.Low);
    }

    public List<(string label, Pulse pulse)> Next(Pulse pulse, string From)
    {
        _inputs[From] = pulse;
        _counts[From] += 1;
        if (pulse == Pulse.High && _firstHighs[From] == 0) _firstHighs[From] = _counts[From];
        return _inputs.Values.All(p => p == Pulse.High)
            ? NextLabels.Select(l => (l, Pulse.Low)).ToList()
            : NextLabels.Select(l => (l, Pulse.High)).ToList();
    }

    public bool FoundAllHighs()
    {
        return _firstHighs.Values.All(l => l > 0);
    }

    public void AddInput(string label)
    {
        _inputs[label] = Pulse.Low;
        _counts[label] = 0L;
        _firstHighs[label] = 0L;
    }
}

internal record Broadcaster(List<string> Nexts);
