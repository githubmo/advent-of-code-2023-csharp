// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);
var scratchCards = new Dictionary<int, int> { { 1, 1 } };
var cardRegex = new Regex(@"Card\s+(\d+):"); // not making it compile time regex for readability
foreach (var line in lines)
{
    var cardNum = int.Parse(cardRegex.Match(line).Groups[1].Value);
    scratchCards.TryAdd(cardNum, 1); // if we don't count for the original already, then we have to add it
    var l = cardRegex.Replace(line, "").Split("|");
    var (winningSet, mySet) = (StringToInts(l[0]), StringToInts(l[1]));
    var intersects = winningSet.Intersect(mySet).Count();
    var scratches = scratchCards[cardNum];
    AddScratchCards(cardNum + 1, cardNum + intersects, scratches);
}

Console.WriteLine(scratchCards.Values.Sum());
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

void AddScratchCards(int currentIndex, int endIndex, int count)
{
    for (var i = currentIndex; i <= endIndex; i++)
    {
        var instances = scratchCards!.GetValueOrDefault(i);
        instances = instances == 0
            ? 1
            : instances; // we account for the original if we don't have it yet in our dictionary
        var newCount = instances + count;
        scratchCards[i] = newCount;
    }
}
