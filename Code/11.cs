﻿using System;
using System.Collections.Generic;

namespace Advent_of_Code
{
    class OctoFlashes : AoCDay
    {
        public OctoFlashes() : base(11) { }
        public override void Run()
        {
            int length = 10;
            int[,] octos = GridParse();
            ref int Octo(int[,] octos, (int, int) yx) => ref octos[yx.Item1, yx.Item2];
            List<(int, int)> Neighbors(int y, int x)
            {
                List<(int, int)> result = new();
                for (int neiY = y - 1; neiY <= y + 1; neiY++)
                    for (int neiX = x - 1; neiX <= x + 1; neiX++)
                        if ((neiY, neiX) != (y, x) &&
                            neiY >= 0 && neiY < length &&
                            neiX >= 0 && neiX < length)
                            result.Add((neiY, neiX));
                return result;
            }

            int Flashes_Cycle()
            {
                for (int y = 0; y < length; y++)
                    for (int x = 0; x < length; x++)
                        octos[y, x]++;

                int[,] newOctos = new int[length, length];
                for (int y = 0; y < length; y++)
                    for (int x = 0; x < length; x++)
                        newOctos[y, x] = octos[y, x];

                bool flash;
                List<(int, int)> flashed = new();
                do
                {
                    flash = false;
                    for (int y = 0; y < length; y++)
                        for (int x = 0; x < length; x++)
                            if (octos[y, x] > 9 && !flashed.Contains((y, x)))
                            {
                                var neis = Neighbors(y, x);
                                foreach ((int, int) nei in neis)
                                    Octo(newOctos, nei)++;
                                flashed.Add((y, x));
                                if (!flash)
                                    flash = true;
                            }
                    octos = newOctos;
                } while (flash);
                foreach (var f in flashed)
                    Octo(newOctos, f) = 0;

                //string print = "";
                //foreach (int[] line in octos)
                //{
                //    foreach (int octo in line)
                //    {
                //        print += octo/* + "|"*/;
                //    }
                //    print += "\n";
                //}
                //Console.Clear();
                //Console.WriteLine(print);
                return flashed.Count;
            }

            int result = 0;
            for (int i = 0; ; i++)
            {
                int flashes = Flashes_Cycle();
                if (i < 100)
                    result += flashes;
                if (i == 99)
                    Console.WriteLine(result);
                if (flashes == Math.Pow(length, 2))
                {
                    Console.WriteLine(i + 1);
                    break;
                }
            }
        }
    }
}
