// See https://aka.ms/new-console-template for more information

using System.Text;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);

var sum = 0L;
foreach (var line in lines)
{
    var numbs = line.Split(" ").Select(long.Parse).ToList();
    var number = NextInSeq(numbs);
    sum += number;
}

Console.WriteLine(sum);

long NextInSeq(List<long> list)
{
    var diffs = list.Zip(list.Skip(1)).Select(pair => pair.Second - pair.First).ToList();
    // Console.WriteLine(string.Join(" ::: ", diffs));
    return diffs.All(l => l == 0) ? list.Last() : list.Last() + NextInSeq(diffs);
}
