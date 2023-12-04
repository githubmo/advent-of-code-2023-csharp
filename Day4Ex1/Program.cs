// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);

var sum = 0L;
var cardRegex = new Regex(@"Card\s+\d+:");
foreach (var line in lines)
{
    var l = cardRegex.Replace(line, "").Split("|");
    var (winningSet, mySet) = (StringToInts(l[0]), StringToInts(l[1]));
    var intersects = winningSet.Intersect(mySet).Count();
    var winning = intersects == 0 ? 0 : Convert.ToInt32(Math.Pow(2, intersects - 1));
    sum += winning;
}

Console.WriteLine(sum);
return;

List<int> StringToInts(string input)
{
    return input
        .Trim()
        .Split(" ")
        .Where(s => !string.IsNullOrWhiteSpace(s))
        .Select(int.Parse)
        .ToList();
}
