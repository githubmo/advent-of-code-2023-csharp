// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);

var gameNumRegex = new Regex(@"Game\s+(\d+):");
var viableGames = (from line in lines
    let gameNum = int.Parse(gameNumRegex.Match(line).Groups[1].Value)
    let cubeSetsSubstring = gameNumRegex.Replace(line, "")
    let cubeSets = cubeSetsSubstring.Trim().Split(";").SelectMany(s => s.Split(", "))
    let isValid = cubeSets.All(s =>
    {
        var cube = Cubes.FromString(s);
        return cube.NumOfCubes <= (int)cube.Colour;
    })
    where isValid
    select gameNum).ToList();

Console.WriteLine(viableGames.Sum());

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
        var numOfCubes = int.Parse(match.Groups[1].Value);
        var colour = match.Groups[2].Value switch
        {
            "red" => Colour.Red,
            "green" => Colour.Green,
            "blue" => Colour.Blue
        };
        return new Cubes(int.Parse(match.Groups[1].Value), colour);
    }
}
