// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);

var labelRegex = new Regex(@"(\w+)([=|-])(\d*)");
var boxes = Enumerable.Range(0, 256).Select(i => (i, new Box(i, []))).ToDictionary();
foreach (var lens in lines.SelectMany(l => l.Split(",")))
{
    var groups = labelRegex.Match(lens).Groups;
    int.TryParse(groups[3].Value, out var lensStrength);
    var ll = new LabelledLens(groups[1].Value, FromString(groups[2].Value), lensStrength);
    boxes[Hash(ll.Label)].AddOrRemoveLens(ll);
}

var sum = boxes.Values.Sum(b => b.Sum);

Console.WriteLine(sum);
return;

int Hash(string s)
{
    var chars = s.ToCharArray();
    return chars.Select(Convert.ToInt32).Aggregate(0, (total, next) => (total + next) * 17 % 256);
}

Operation FromString(string s)
{
    return s.Trim() == "=" ? Operation.Add : Operation.Remove;
}

internal enum Operation
{
    Add = '=',
    Remove = '-'
}

internal record LabelledLens(string Label, Operation Operation, int Lens = 0);

internal record Box(int Number, List<LabelledLens> Lenses)
{
    public int Sum =>
        Lenses.Select(
            (l, i) => (Number + 1) * (i + 1) * l.Lens).Sum();

    public void AddOrRemoveLens(LabelledLens lens)
    {
        var index = Lenses.FindIndex(l => l.Label == lens.Label);
        if (lens.Operation == Operation.Remove) Lenses.RemoveAll(l => l.Label == lens.Label);
        else if (index >= 0) Lenses[index] = lens;
        else Lenses.Add(lens);
    }
}
