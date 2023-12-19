// See https://aka.ms/new-console-template for more information

using System.Text;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);

var sum = lines.Sum(line => line.Split(",").Sum(Hash));

Console.WriteLine(sum);
return;

int Hash(string s)
{
    var chars = s.ToCharArray();
    return chars.Select(Convert.ToInt32).Aggregate(0, (total, next) => (total + next) * 17 % 256);
}
