using System.Text;

var map = File
    .ReadAllLines("Resources/example.txt", Encoding.UTF8)
    .Select(l =>
        l.ToCharArray().Select(c => int.Parse(c.ToString())).ToArray()).ToArray();

var rowCount = map.Length;
var columnCount = map[0].Length;

// Create a 2D array to store the minimum sum path to each cell.
var minimumSums = new int[rowCount, columnCount];

// The minimum sum path to the starting cell is the value of the cell.
minimumSums[0, 0] = map[0][0];

// Initialize the first row.
for (var i = 1; i < columnCount; i++) minimumSums[0, i] = minimumSums[0, i - 1] + map[0][i];

// Initialize the first column.
for (var i = 1; i < rowCount; i++) minimumSums[i, 0] = minimumSums[i - 1, 0] + map[i][0];

// Calculate minimum sum paths for the remaining cells.
for (var i = 1; i < rowCount; i++)
for (var j = 1; j < columnCount; j++)
    // The minimum sum path to a cell is the value of the cell plus
    // the smaller of the minimum sum paths to the cell above and to the left.
    minimumSums[i, j] = map[i][j] + Math.Min(minimumSums[i - 1, j], minimumSums[i, j - 1]);

var sum = minimumSums[rowCount - 1, columnCount - 1];

Console.WriteLine(sum);
