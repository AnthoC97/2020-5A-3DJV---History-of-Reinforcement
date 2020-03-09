using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Algorithms
{
    public static float[,] create_random_uniform_policy(int state_size, int action_size)
    {
        float[,] temp = new float[state_size, action_size];
        for (int i =0; i<state_size; ++i)
        {
            for (int y = 0; y < action_size; ++y)
            {
                temp[i, y] = 1.0f/action_size;
            }
        }
        return temp;
    }

    public static void setArrayAtArray<T>(ref T[] Array, ref int[] KeyArray, T value)
    {
        foreach(int key in KeyArray)
        {
            Array[key] = value;
        }
    }

    public static float[] iterative_policy_evaluation(
       int[] S,
       int[] A,
       int[] T,
       float[,,] P,
       float[,,] R,
       float[,] Pi,
       float gamma = 1.0f,
       float theta = 0.000001f)
    {
        Debug.Assert(0.0 <= gamma && gamma <= 1.0);
        Debug.Assert(theta > 0);

        float[] V = new float[S.Length];
        setArrayAtArray<float>(ref V, ref T, 0.0f);
        while (true) {
            float delta = 0;
            foreach (int s in S) {
                float temp_v = V[s];
                float temp_sum = 0.0f;
                foreach (int a in A) {
                    foreach (int s_p in S) {
                        temp_sum += Pi[s, a] * P[s, a, s_p] * (
                                R[s, a, s_p] + gamma * V[s_p]
                        );
                    }
                }
                V[s] = temp_sum;
                delta = Mathf.Max(delta, Mathf.Abs(V[s] - temp_v));
            }
            if (delta < theta)
                break;
        }
        return V;
    }
}
