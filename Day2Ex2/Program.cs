// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);

// this could be a compile time regex, but I chose not to for readability
var gameNumRegex = new Regex(@"Game\s+(\d+):");
var powers = (from line in lines
    select gameNumRegex.Replace(line, "")
    into cubeSetsSubstring
    select cubeSetsSubstring.Trim().Split(";").SelectMany(s => s.Split(", "))
    into cubeSets
    select cubeSets.Select(Cubes.FromString).ToList()
    into cubes
    let maxRed = MaxColour(cubes, Colour.Red)
    let maxBlue = MaxColour(cubes, Colour.Blue)
    let maxGreen = MaxColour(cubes, Colour.Green)
    select maxRed * maxBlue * maxGreen).Aggregate(0L, (current, power) => current + power);

Console.WriteLine(powers);
return;

int MaxColour(List<Cubes> cubesList, Colour colour)
{
    var filtered = cubesList.Where(c => c.Colour == colour).ToList();
    return filtered.Count != 0 ? filtered.Max(c => c.NumOfCubes) : 1;
}

internal enum Colour
{
    Red = 12,
    Green = 13,
    Blue = 14
}

internal record Cubes(int NumOfCubes, Colour Colour)
{
    private static readonly Regex regex = new(@"(\d+)\s+(\w+)");

    public static Cubes FromString(string s)
    {
        var match = regex.Match(s);
        var colour = match.Groups[2].Value switch
        {
            "red" => Colour.Red,
            "green" => Colour.Green,
            "blue" => Colour.Blue
        };
        return new Cubes(int.Parse(match.Groups[1].Value), colour);
    }
}
