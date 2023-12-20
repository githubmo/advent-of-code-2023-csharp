using System.Diagnostics;
using System.Text;

var stopwatch = Stopwatch.StartNew();

var map = File.ReadAllLines("Resources/input.txt").Select(a => a.Select(b => b - '0').ToList()).ToList();

long part1 = 0;
long part2 = 0;

var visited = new bool[map[0].Count, map.Count, 4, 4];
var directions = new (int x, int y)[] { (1, 0), (0, 1), (-1, 0), (0, -1) };
var work = new PriorityQueue<(int x, int y, int streak, int direction), long>();
work.Enqueue((0, 0, 0, 0), 0);
work.Enqueue((0, 0, 0, 1), 0);
var mindist = new Dictionary<(int x, int y, int streak, int direction), long>();
mindist[(0, 0, 0, 0)] = 0;
mindist[(0, 0, 0, 1)] = 0;
var found = false;
while (work.Count > 0 && !found)
{
    var cur = work.Dequeue();
    visited[cur.x, cur.y, cur.direction, cur.streak] = true;
    //add new work to the queue
    if (cur.streak < 2)
    {
        var dir = cur.direction;
        var urge = cur.streak + 1;
        TryToAddWork(map, directions, work, mindist, cur, dir, urge, visited);
    }

    if (cur.direction == 0 || cur.direction == 2)
    {
        TryToAddWork(map, directions, work, mindist, cur, 1, 0, visited);
        TryToAddWork(map, directions, work, mindist, cur, 3, 0, visited);
    }
    else if (cur.direction == 1 || cur.direction == 3)
    {
        TryToAddWork(map, directions, work, mindist, cur, 0, 0, visited);
        TryToAddWork(map, directions, work, mindist, cur, 2, 0, visited);
    }

    if (cur.x == map[0].Count - 1 && cur.y == map.Count - 1)
    {
        found = true;
        part1 = mindist[cur];
    }
}

work = new PriorityQueue<(int x, int y, int streak, int direction), long>();
work.Enqueue((0, 0, 0, 0), 0);
work.Enqueue((0, 0, 0, 1), 0);
mindist = new Dictionary<(int x, int y, int streak, int direction), long>();
mindist[(0, 0, 0, 0)] = 0;
mindist[(0, 0, 0, 1)] = 0;
found = false;
visited = new bool[map[0].Count, map.Count, 4, 10];
while (work.Count > 0 && !found)
{
    var cur = work.Dequeue();
    visited[cur.x, cur.y, cur.direction, cur.streak] = true;

    //add new work to the queue
    if (cur.streak < 9)
    {
        var dir = cur.direction;
        var urge = cur.streak + 1;
        TryToAddWork(map, directions, work, mindist, cur, dir, urge, visited);
    }

    if (cur.streak >= 3)
    {
        if (cur.direction == 0 || cur.direction == 2)
        {
            TryToAddWork(map, directions, work, mindist, cur, 1, 0, visited);
            TryToAddWork(map, directions, work, mindist, cur, 3, 0, visited);
        }
        else if (cur.direction == 1 || cur.direction == 3)
        {
            TryToAddWork(map, directions, work, mindist, cur, 0, 0, visited);
            TryToAddWork(map, directions, work, mindist, cur, 2, 0, visited);
        }

        if (cur.x == map[0].Count - 1 && cur.y == map.Count - 1)
        {
            found = true;
            part2 = mindist[cur];
        }
    }
}

stopwatch.Stop();
Console.WriteLine(part1);
Console.WriteLine(part2);
Console.WriteLine("Time in miliseconds: " + stopwatch.ElapsedMilliseconds);

void TryToAddWork(List<List<int>> S, (int x, int y)[] directions,
    PriorityQueue<(int x, int y, int streak, int direction), long> work,
    Dictionary<(int x, int y, int streak, int direction), long> mindist,
    (int x, int y, int streak, int direction) cur, int dir, int urge,
    bool[,,,] visited)
{
    (int x, int y, int streak, int direction)
        newpos = (cur.x + directions[dir].x, cur.y + directions[dir].y, urge, dir);
    if (newpos.x >= 0 && newpos.y >= 0 && newpos.x < S[0].Count && newpos.y < S.Count &&
        !visited[newpos.x, newpos.y, newpos.direction, newpos.streak])
    {
        var newval = mindist[cur] + S[newpos.y][newpos.x];
        if (!mindist.ContainsKey(newpos))
        {
            mindist[newpos] = newval;
            work.Enqueue(newpos, newval);
        }
        else if (mindist[newpos] > newval)
        {
            mindist[newpos] = newval;
            work.Enqueue(newpos, newval);
        }
    }
}

// var rowCount = map.Length;
// var columnCount = map[0].Length;
//
// // Create a DP table same as given map size with initially maximum possible value
// var dp = Enumerable.Repeat(0, rowCount)
//     .Select(x => Enumerable.Repeat(int.MaxValue, columnCount).ToArray())
//     .ToArray();
//
// dp[0][0] = map[0][0];
//
// // Minimum neighbouring cell's path sum
// // int[] dx = { 0, 1, 0, -1 }; // possible directions.
// // int[] dy = { 1, 0, -1, 0 };
//
// // Dynamic Programming state transition equations
// // for (var i = 0; i < rowCount; i++)
// // for (var j = 0; j < columnCount; j++)
// // for (var k = 0; k < 4; k++)
// // {
// //     var step = 0;
// //     while (step < 3) // can't go in the same direction more than 3 cells
// //     {
// //         var ni = i + dx[k] * (step + 1);
// //         var nj = j + dy[k] * (step + 1);
// //
// //         if (ni >= 0 && ni < rowCount && nj >= 0 && nj < columnCount)
// //         {
// //             Console.WriteLine($"{ni} {nj}");
// //             dp[ni][nj] = Math.Min(dp[ni][nj], dp[i][j] + map[ni][nj]);
// //             step++;
// //         }
// //         else
// //         {
// //             break;
// //         }
// //     }
// // }
//
// var steps = new Queue<Step>();
// var visited = new HashSet<Step>();
// steps.Enqueue(new Step(0, 0, 1, Direction.Right, in map));
// steps.Enqueue(new Step(0, 0, 1, Direction.Down, in map));
//
// while (steps.Count > 0)
// {
//     var step = steps.Dequeue();
//     visited.Add(step);
//     var nextSteps = step.Next().Where(s => !visited.Contains(s)).ToList();
//     foreach (var nextStep in nextSteps)
//     {
//         var (i, j) = (nextStep.I, nextStep.J);
//         dp[i][j] = Math.Min(dp[i][j], dp[step.I][step.J] + map[i][j]);
//         steps.Enqueue(nextStep);
//     }
// }
//
// // Shortest path distance from (0,0) to (rowCount - 1, columnCount - 1)
// var shortestPathDistance = dp[rowCount - 1][columnCount - 1];
// Console.WriteLine(shortestPathDistance);
//
// internal enum Direction
// {
//     Up,
//     Right,
//     Down,
//     Left,
//     Stop
// }
//
// internal record Step(int I, int J, int StepNumber, Direction Direction, in int[][] Map)
// {
//     public List<Step> Next()
//     {
//         var directions = new List<Direction> { Direction };
//         switch (Direction)
//         {
//             case Direction.Up or Direction.Down:
//                 directions.Add(Direction.Right);
//                 directions.Add(Direction.Left);
//                 break;
//             case Direction.Left or Direction.Right:
//                 directions.Add(Direction.Up);
//                 directions.Add(Direction.Down);
//                 break;
//         }
//
//         var steps = new List<Step>();
//         foreach (var direction in directions)
//         {
//             var (newI, newJ) = NextStep(direction);
//             newI += I;
//             newJ += J;
//             var newStep = direction == Direction ? StepNumber + 1 : 1;
//             if (IsValid(newI, newJ) && newStep < 3)
//                 steps.Add(new Step(newI, newJ, newStep, Direction, Map));
//         }
//
//         return steps;
//     }
//
//     private (int, int) NextStep(Direction direction)
//     {
//         return direction switch
//         {
//             Direction.Up => (-1, 0),
//             Direction.Right => (0, 1),
//             Direction.Down => (1, 0),
//             Direction.Left => (0, -1),
//             Direction.Stop => (0, 0),
//             _ => throw new ArgumentOutOfRangeException(nameof(Direction), Direction, null)
//         };
//     }
//
//     private bool IsValid(int i, int j)
//     {
//         return i >= 0 && j >= 0 && i < Map.Length && j < Map[0].Length;
//     }
// }
