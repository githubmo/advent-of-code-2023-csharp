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
var stringToNumDict = new Dictionary<string, string>()
{
    { "zero", "0"},
    { "one", "1" },
    { "two", "2" },
    { "three", "3" },
    { "four", "4"},
    { "five", "5" },
    { "six", "6" },
    { "seven", "7" },
    { "eight", "8" },
    { "nine", "9" },
};

var sum = 0;
var regex = new Regex(@"\d|one|two|three|four|five|six|seven|eight|nine");

// Get each line
foreach (var line in lines)
{
    // match each single digit or number in string form and add to nums list
    var nums = new List<string>() { };
    var matchObj = regex.Match(line);
    while (matchObj.Success)
    {
        nums.Add(matchObj.Value);
        matchObj = regex.Match(line, matchObj.Index + 1);
    }
    
    // convert strings to numbers if they are not single digit chars
    var sanitisedNums = nums.Select(n => stringToNumDict.GetValueOrDefault(n) ?? n).ToList();
    
    // concatenate first and last, convert to int
    var num = $"{sanitisedNums.First()}{sanitisedNums.Last()}";
    sum += int.Parse(num);
}

Console.WriteLine(sum);

