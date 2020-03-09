using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineWorld
{
    public int[] S { get; private set; }
    public int[] A { get; private set; }
    public int[] T { get; private set; }
    public float[,,] P { get; private set; }
    public float[,,] R { get; private set; }

    public LineWorld()
    {
        S = new int[] { 0, 1, 2, 3, 4 };
        A = new int[] { 0, 1 };
        T = new int[] { 0, 4 };
        P = new float[S.Length, A.Length, S.Length];
        R = new float[S.Length, A.Length, S.Length];

        for(int i = 0; i < S.Length; i++)
        {
            for(int j = 0; j < A.Length; j++)
            {
                for(int k = 0; k < S.Length; k++)
                {
                    P[i, j, k] = 0;
                    R[i, j, k] = 0;
                }
            }
        }


        for(int i = 1; i < S.Length-1; i++)
        {
            P[S[i], 0, S[i - 1]] = 1.0f;
            P[S[i], 0, S[i + 1]] = 1.0f;
        }

        R[1, 0, 0] = -1.0f;
        R[3, 1, 4] = 1.0f;
    }
}
