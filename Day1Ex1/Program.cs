// See https://aka.ms/new-console-template for more information

using System.Text;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);

var sum = 0;
foreach (var line in lines)
{
    var chars = line.ToList();
    var numbers = chars.Where(char.IsDigit).Select(c => int.Parse(c.ToString())).ToList();
    var number = numbers.First() * 10 + numbers.Last();
    sum += number;
}

Console.WriteLine(sum);
