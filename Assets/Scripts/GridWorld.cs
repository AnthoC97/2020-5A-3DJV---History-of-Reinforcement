using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridWorld
{

    public int[] S { get; private set; } // emsemble des états
    public int[] A { get; private set; } // emsemble des actions
    public int[] T { get; private set; }
    public float[,,] P { get; private set; }
    public float[,,] R { get; private set; } // ensemble des récompenses immédiates

    public int width = 10;
    public int height = 10;
    public int numState = 0;

    public GridWorld()
    {
        Initialize();
    }

    public GridWorld(int width, int height)
    {
        this.width = width;
        this.height = height;
        Initialize();
    }

    // Start is called before the first frame update
    private void Initialize()
    {
        numState = width * height;

        S = new int[numState];
        A = new[] { 0, 1, 2, 3 };
        T = new[] { width - 1, numState - 1 };
        P = new float[S.Length, A.Length, S.Length]; // Probabilité de passer dans un état donner a partir d'un état et d'une action
        R = new float[S.Length, A.Length, S.Length]; // Récompense a partir d'un état et d'une action 

        for(int s=0; s<S.Length; ++s)
        {
            S[s] = s;
        }

        foreach (int s in S)
        {
            if (s % width == 0)
            {
                P[s, 0, s] = 1.0f;
            }
            else
            {
                P[s, 0, s - 1] = 1.0f;
            }

            if ((s + 1) % width == 0)
            {
                P[s, 1, s] = 1.0f;
            }
            else
            {
                P[s, 1, s + 1] = 1.0f;
            }

            if (s < width)
            {
                P[s, 2, s] = 1.0f;
            }
            else
            {
                P[s, 2, s - width] = 1.0f;
            }

            if (s >= (numState - width))
            {
                P[s, 3, s] = 1.0f;
            }
            else
            {
                P[s, 3, s + width] = 1.0f;
            }
        }

        for (int i = 0; i < A.Length; i++)
        {
            for (int j = 0; j < S.Length; j++)
            {
                P[width - 1, i, j] = 0.0f;
                P[numState - 1, i, j] = 0.0f;
            }
        }

        for (int i = 0; i < S.Length; i++)
        {
            for (int j = 0; j < A.Length; j++)
            {
                R[i, j, width - 1] = -5.0f;
                R[i, j, numState - 1] = 1.0f;
            }
        }
    }

    public PairSR step(int s, int a)
    {
        int s_p = 0;
        float rand = Random.value;
        float temp = 0;
        for (int i = 0; i < S.Length; ++i)
        {
            temp += P[s, a, i];
            if (rand <= temp)
            {
                s_p = i;
                break;
            }
        }
        float r = R[s, a, s_p];
        PairSR pairSR;
        pairSR.S = s_p;
        pairSR.R = r;
        return pairSR;
    }

    public PairSARSP step_until_the_end_of_episode_and_return_transitions(int s, float[,] Pi)
    {
        List<int> s_list = new List<int>();
        List<int> a_list = new List<int>();
        List<float> r_list = new List<float>();
        List<int> s_p_list = new List<int>();
        while (!Algorithms.arrayContain(T, s) && s_list.Count < S.Length * 10)
        {
            int a = 0;
            float rand = Random.value;
            float temp = 0;
            for (int i = 0; i < A.Length; ++i)
            {
                temp += Pi[s, i];
                if (rand <= temp)
                {
                    a = i;
                    break;
                }
            }
            PairSR pairSR = step(s, a);
            s_list.Add(s);
            a_list.Add(a);
            r_list.Add(pairSR.R);
            s_p_list.Add(pairSR.S);
            s = pairSR.S;
        }
        PairSARSP pairSARSP;
        pairSARSP.S = s_list;
        pairSARSP.A = a_list;
        pairSARSP.R = r_list;
        pairSARSP.SP = s_p_list;
        return pairSARSP;
    }
}
