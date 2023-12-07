// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

var lines = File.ReadLines("Resources/input.txt", Encoding.UTF8);

var whiteSpaceRegex = new Regex(@"\s+");
var hands = lines
    .Select(line => whiteSpaceRegex.Split(line.Trim()))
    .Select(handAndWinning =>
        new Hand(handAndWinning[0].ToCharArray(), long.Parse(handAndWinning[1])))
    .ToList();


// 248622819
// 248559379

var sum = hands.Order().Select(
        (hand, index) =>
        {
            var result = hand.Winning * (index + 1);
            Console.WriteLine(
                // $"{string.Join("", hand.Cards)} ::: {index + 1}  :::   {hand.Winning}     >>>    {result}");
                $"{string.Join("", hand.Cards)}");
            return result;
        })
    .Sum();
Console.WriteLine(sum);

// Tests
// Console.WriteLine(new Hand("32T3K".ToCharArray(), 1).Type.ToString()); // one pair
// Console.WriteLine(new Hand("KK677".ToCharArray(), 1).Type.ToString()); // two pair
// Console.WriteLine(new Hand("KTJJT".ToCharArray(), 1).Type.ToString()); // two pair
// Console.WriteLine(new Hand("T55J5".ToCharArray(), 1).Type.ToString()); // three 
// Console.WriteLine(new Hand("QQQJA".ToCharArray(), 1).Type.ToString()); // three
// Console.WriteLine(new Hand("AAQQQ".ToCharArray(), 1).Type.ToString()); // house
// Console.WriteLine(new Hand("AAAAA".ToCharArray(), 1).Type.ToString()); // full
// Console.WriteLine(new Hand("AAAAQ".ToCharArray(), 1).Type.ToString()); // four
// Console.WriteLine(new Hand("23456".ToCharArray(), 1).Type.ToString()); // high
// Console.WriteLine(new Hand("789TJ".ToCharArray(), 1).Type.ToString()); // high
// Console.WriteLine(new Hand("2468T".ToCharArray(), 1).Type.ToString()); // four
// Console.WriteLine(new Hand("AAAAA".ToCharArray(), 1).Type.ToString()); // five
// Console.WriteLine(new Hand("AAAAJ".ToCharArray(), 1).Type.ToString()); // four
// Console.WriteLine(new Hand("AAAJJ".ToCharArray(), 1).Type.ToString()); // full
// Console.WriteLine(new Hand("AAAJT".ToCharArray(), 1).Type.ToString()); // three
// Console.WriteLine(new Hand("AAKJJ".ToCharArray(), 1).Type.ToString()); // two
// Console.WriteLine(new Hand("23456".ToCharArray(), 1).Type.ToString()); // high
// Console.WriteLine(new Hand("TAQJK".ToCharArray(), 1).Type.ToString()); // bad


internal record Hand(char[] Cards, long Winning) : IComparable<Hand>
{
    public static readonly Dictionary<char, int> Weights = new()
    {
        { 'T', 10 },
        { 'J', 11 },
        { 'Q', 12 },
        { 'K', 13 },
        { 'A', 14 }
    };


    public HandType Type
    {
        get
        {
            var grouped =
                Cards.GroupBy(c => c).Select(chars => new { chars.Key, Count = chars.Count() }).ToList();
            var groupCount = grouped.Count;
            if (groupCount == 1)
                return HandType.Five;
            if (grouped.Exists(tuple => tuple.Count == 4))
                return HandType.Four;
            if (grouped.Select(pair => pair.Count).Order().ToList().SequenceEqual(new List<int> { 2, 3 }))
                return HandType.FullHouse;
            if (grouped.Exists(pair => pair.Count == 3))
                return HandType.Three;
            if (grouped.Count(pair => pair.Count == 2) == 2)
                return HandType.TwoPair;
            if (grouped.Exists(pair => pair.Count == 2))
                return HandType.OnePair;
            // var orderedCards = Cards.Select(CardToInt).Order().ToList();
            // if (orderedCards.Last() - orderedCards.First() == 4) return HandType.HighCard;
            if (Cards.Distinct().Count() == 5) return HandType.HighCard;

            return HandType.Bad;
        }
    }

    public int CompareTo(Hand? other)
    {
        if (other == null) throw new ArgumentException("Hand cannot be null");

        var handTypeCompared = Type.CompareTo(other.Type);
        if (handTypeCompared != 0) return handTypeCompared;

        var index = 0;
        while (index < Cards.Length && Cards[index] == other.Cards[index]) index++;

        return CardToInt(Cards[index]).CompareTo(CardToInt(other.Cards[index]));
    }

    public static int CardToInt(char c)
    {
        if (Weights!.TryGetValue(c, out var value)) return value;
        return c - '0';
    }
}

internal enum HandType
{
    Five = 7,
    Four = 6,
    FullHouse = 5,
    Three = 4,
    TwoPair = 3,
    OnePair = 2,
    HighCard = 1,
    Bad = 0
}
