using System;
using System.Collections.Generic;
using System.IO;

namespace Advent_of_Code
{
    abstract class AoCDay
    {
        //static public int day;
        protected string input;
        protected string[] inputLines;
        //public AoCDay(int d)
        //{
        //    day = d;
            //string path = year + "/Inputs/" + day.ToString("00") + ".txt";
            //inputLines = File.ReadAllLines(path);
            //inputString = File.ReadAllText(path);
        //}
        public (object Part1, object Part2) Solve(string input)
        {
            this.input = input;
            inputLines = input.Split("\n")[..^1];
            Run(out (object Part1, object Part2) result);
            if (result.Part1 is null) result.Part1 = "Not done.";
            if (result.Part2 is null) result.Part2 = "Not done.";
            return result;
        }
        protected abstract void Run(out (object part1, object part2) answer);

        // Helper functions
        protected static bool debug = false;
        protected static T[,] GridParse<T>(string[] input, Func<char, T> Converter)
        {
            T[,] result = new T[input.Length, input[0].Length];
            for (int y = 0; y < input.Length; y++)
                for (int x = 0; x < input[0].Length; x++)
                    result[y, x] = Converter(input[y][x]);
            return result;
        }
        protected bool[,] GridParse(char cTrue, int fromLine = 0)
            => GridParse(inputLines[fromLine..], c => { if (c == cTrue) return true; return false; });
        protected int[,] GridParse(int fromLine = 0)
            => GridParse(inputLines[fromLine..], c => (int)char.GetNumericValue(c));

        protected static string GridStr<T>(T[,] input, Func<T, string> ToStr)
        {
            string result = "";
            for (int y = 0; y < input.GetLength(0); y++)
            {
                for (int x = 0; x < input.GetLength(1); x++)
                    result += ToStr(input[y, x]);
                result += "\n";
            }
            return result;
        }
        protected static string GridStr<T>(T[,] input, string pad = "")
            => GridStr(input, b => b + pad);
        protected static string CollStr<T>(IEnumerable<T> coll, Func<T, string> ToStr)
        {
            string result = "";
            foreach (T item in coll)
            {
                result += ToStr(item);
            }
            return result;
        }
        protected static string CollStr<T>(IEnumerable<T> coll, string pad = "")
            => CollStr(coll, t => t.ToString() + pad);

        protected static List<(int y, int x)> Neighbors
            (int y, int x, bool diagonal = false, bool self = false)
        {
            List<(int, int)> neighbors = new();
            for (int neiY = y - 1; neiY <= y + 1; neiY++)
                for (int neiX = x - 1; neiX <= x + 1; neiX++)
                    if ((diagonal || neiY == y || neiX == x) &&
                        (self || (neiY, neiX) != (y, x)))
                        neighbors.Add((neiY, neiX));
            return neighbors;
        }
        protected static List<(int y, int x)> Neighbors
            (int y, int x, int boundY, int boundX, bool diagonal = false, bool self = false)
        {
            List<(int y, int x)> neighbors = Neighbors(y, x, self, diagonal);
            neighbors.RemoveAll(nei => OutOfBounds(nei.y, nei.x, boundY, boundX));
            return neighbors;
        }
        protected static bool OutOfBounds(int y, int x, int boundY, int boundX)
            => y < 0 || y >= boundY || x < 0 || x >= boundX;

        /// <summary>
        /// Generic Dijkstra's algorithm, finds path from start to destination
        /// </summary>
        /// <typeparam name="Key">Unique indentifier for a vertex</typeparam>
        /// <param name="start"></param>
        /// <param name="dest"></param>
        /// <param name="PathWeight"></param>
        /// <param name="Nei_key_dist"></param>
        /// <param name="Heuristic"></param>
        /// <returns></returns>
        protected static List<(Key key, int distance, int prev)> A_Star<Key>(
            Key start, Key dest,
            Func<Key, List<(Key, int)>> Nei_key_dist,
            Func<Key, Key, bool> Equal,
            Func<Key, int> Heuristic = null)
        {
            (Key key, int distance, int) current = (start, 0, -1);
            int curIndex = 0;
            List<(Key key, int distance, int)>
                visited = new(), unvisited = new() { current };

            do
            {
                int min = int.MaxValue;
                for (int i = 0; i < unvisited.Count; i++)
                {
                    var (key, distance, prev) = unvisited[i];
                    int unvDistHeur = distance;
                    if (Heuristic != null)
                        unvDistHeur += Heuristic(key);
                    if (min > unvDistHeur)
                    {
                        current = (key, distance, prev);
                        curIndex = i;
                        min = unvDistHeur;
                    }
                }

                List<(Key, int)> neis = Nei_key_dist(current.key);
                foreach (var (nei, pathWeight) in neis)
                {
                    if (visited.FindIndex(v => Equal(v.key, nei)) != -1) // too costly
                        continue;
                    int newDist = current.distance + pathWeight;
                    int find = unvisited.FindIndex(u => Equal(u.key, nei));
                    if (find == -1)
                        unvisited.Add((nei, newDist, visited.Count));
                    else if (newDist < unvisited[find].distance)
                        unvisited[find] = (nei, newDist, visited.Count);
                }

                visited.Add(current);
                unvisited.RemoveAt(curIndex);
                //if (debug)
                //{
                //Console.WriteLine(current.key.ToString()+ "Energy: " + current.distance);
                //Console.WriteLine(new string('-', 16));
                //Console.WriteLine(CollStr(unvisited,
                //    p => p.key.ToString() + "Energy: " + p.distance + "\n"));
                //Console.WriteLine(new string('\u2588', 16));
                //}
            } while (!Equal(current.key, dest));
            //} while (unvisited.Count != 0);
            return visited;
        }
    }
}
