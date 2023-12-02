// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);
// test
// var lines = Regex.Split("""
//             two1nine
//             eightwothree
//             abcone2threexyz
//             xtwone3four
//             4nineeightseven2
//             zoneight234
//             7pqrstsixteen
//             """.Trim(), @"\s+");
var stringToNumDict = new Dictionary<string, int>()
{
    { "zero", 0},
    { "one", 1 },
    { "two", 2 },
    { "three", 3 },
    { "four", 4},
    { "five", 5 },
    { "six", 6 },
    { "seven", 7 },
    { "eight", 8 },
    { "nine", 9 },
};

var sum = 0;
var regex = new Regex(@"\d|one|two|three|four|five|six|seven|eight|nine");
foreach (var line in lines)
{
    var nums = new List<string>() { };
    var matchObj = regex.Match(line);
    while (matchObj.Success)
    {
        nums.Add(matchObj.Value);
        matchObj = regex.Match(line, matchObj.Index + 1);
    }
    var digits = 
        nums
            .Where(n => n.Length == 1 && char.IsDigit(n.First()) || stringToNumDict.Keys.Any(n.Contains))
            .Select(n =>
            {
                int i;
                var match = stringToNumDict.Keys.FirstOrDefault(k => n.Contains(k));
                if (string.IsNullOrEmpty(match))
                {
                    i = int.Parse(n);
                } else
                {
                    stringToNumDict.TryGetValue(match, out i);
                } 
                return i;
            })
            .ToList();
    var number = digits.First() * 10 + digits.Last();
    sum += number;
}

Console.WriteLine(sum);

