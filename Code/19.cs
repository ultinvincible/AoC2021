﻿using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace Advent_of_Code
{
    class Scanner_Beacon : AoCDay
    {
        public Scanner_Beacon() : base(19) { }
        Func<float, float, float, Vector3>[] Rotations = new Func<float, float, float, Vector3>[]
        {
            (x,y,z)=>(new(x,y,z)),
            (x,y,z)=>(new(-x,-y,z)),
            (x,y,z)=>(new(x,-y,-z)),
            (x,y,z)=>(new(-x,y,-z)),
            (x,y,z)=>(new(y,z,x)),
            (x,y,z)=>(new(-y,-z,x)),
            (x,y,z)=>(new(y,-z,-x)),
            (x,y,z)=>(new(-y,z,-x)),
            (x,y,z)=>(new(z,x,y)),
            (x,y,z)=>(new(-z,-x,y)),
            (x,y,z)=>(new(z,-x,-y)),
            (x,y,z)=>(new(-z,x,-y)),
            (x,y,z)=>(new(-x,z,y)),
            (x,y,z)=>(new(x,-z,y)),
            (x,y,z)=>(new(x,z,-y)),
            (x,y,z)=>(new(-x,-z,-y)),
            (x,y,z)=>(new(-z,y,x)),
            (x,y,z)=>(new(z,-y,x)),
            (x,y,z)=>(new(z,y,-x)),
            (x,y,z)=>(new(-z,-y,-x)),
            (x,y,z)=>(new(-y,x,z)),
            (x,y,z)=>(new(y,-x,z)),
            (x,y,z)=>(new(y,x,-z)),
            (x,y,z)=>(new(-y,-x,-z)),
        };
        Vector3 Rotate(Vector3 pos, int i)
            => Rotations[i](pos.X, pos.Y, pos.Z);
        public override void Run()
        {
            List<List<Vector3>> scanner = new() { new() };
            for (int i = 1; i < input.Length; i++)
            {
                if (input[i] == "")
                {
                    scanner.Add(new());
                    i++;
                }
                else
                {
                    int[] split = Array.ConvertAll(input[i].Split(','), s => int.Parse(s));
                    scanner[^1].Add(new(split[0], split[1], split[2]));
                }
            }

            Vector3[] position_scanner = new Vector3[scanner.Count];
            HashSet<Vector3> beacons = scanner[0].ToHashSet();
            Queue<int> mapped = new();
            mapped.Enqueue(0);
            while (mapped.Count != 0)
            {
                int s1 = mapped.Dequeue();
                for (int s2 = 1; s2 < scanner.Count; s2++) // unmapped
                    if (position_scanner[s2] == Vector3.Zero && s1 != s2)
                    {
                        bool overlap = false;
                        for (int r = 0; r < 24 && !overlap; r++)
                        {
                            Dictionary<Vector3, int> matches = new();
                            for (int b1 = 0; b1 < scanner[s1].Count && !overlap; b1++)
                                for (int b2 = 0; b2 < scanner[s2].Count && !overlap; b2++)
                                {
                                    Vector3 s1_to_s2 = scanner[s1][b1] - Rotate(scanner[s2][b2], r);
                                    if (!matches.ContainsKey(s1_to_s2))
                                        matches.Add(s1_to_s2, 1);
                                    else if (++matches[s1_to_s2] == 12)
                                    {
                                        mapped.Enqueue(s2);
                                        position_scanner[s2] = s1_to_s2 + position_scanner[s1];
                                        overlap = true;
                                        for (int bea = 0; bea < scanner[s2].Count; bea++)
                                            scanner[s2][bea] = Rotate(scanner[s2][bea], r);
                                    }
                                }
                        }
                        if (overlap)
                            foreach (Vector3 beacon in scanner[s2])
                                beacons.Add(position_scanner[s2] + beacon);
                    }
            }
            Console.WriteLine(beacons.Count);

            float max = 0;
            foreach (Vector3 s1 in position_scanner)
                foreach (Vector3 s2 in position_scanner)
                    if (s1 != s2)
                        max = Manhattan_Distance(max, s1, s2);
            Console.WriteLine(max);
        }

        private static float Manhattan_Distance(float max, Vector3 s1, Vector3 s2)
        {
            max = Math.Max(max, Math.Abs(s1.X - s2.X) +
                Math.Abs(s1.Y - s2.Y) + Math.Abs(s1.Z - s2.Z));
            return max;
        }
    }
}
