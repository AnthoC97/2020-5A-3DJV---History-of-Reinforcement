using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PairVPi
{
    public float[] V;
    public float[,] Pi;
};

public class Algorithms
{
    // Retourne pour chaque état un meme pourcentage de chance pour chaque action
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

    public static int getIndexOfMaxInArray(ref float[,] A, int s, int action_size)
    {
        int maxIndex = 0;
        float max = float.MinValue;
        for (int a = 0; a < action_size; ++a)
        {
            if (A[s, a] > max)
            {
                max = A[s, a];
                maxIndex = a;
            }
        }
        return maxIndex;
    }

    public static void setPiArrayValueAtState(ref float[,] Pi, int s, float value, int action_size)
    {
        for (int a = 0; a < action_size; ++a)
        {
            Pi[s, a] = value;
        }
    }

    public static float[] iterative_policy_evaluation(
       int[] S,
       int[] A,
       int[] T,
       float[,,] P,
       float[,,] R,
       float[,] Pi,
       float gamma = 0.99f,
       float theta = 0.000001f,
       float[] V = null)
    {
        Debug.Assert(0.0 <= gamma && gamma <= 1.0);
        Debug.Assert(theta > 0);

        if (V == null)
        {
            V = new float[S.Length]; // value function resultats
            for (int iv = 0; iv < V.Length; ++iv)
            {
                V[iv] = Random.value;
            }
            setArrayAtArray<float>(ref V, ref T, 0.0f);
        }
        while (true) {
            float delta = 0;
            foreach (int s in S) { // pour chaque état
                float temp_v = V[s];
                float temp_sum = 0.0f;
                foreach (int a in A) { // pour chaque action
                    foreach (int s_p in S) { // pour chaque état
                        temp_sum += Pi[s, a] * P[s, a, s_p] * (
                                R[s, a, s_p] + gamma * V[s_p]
                        ); // (pourcentage de meilleur action) * (probabilité d'atteindre l'état) * (Reward+ValueFunction)
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

    public static PairVPi policy_iteration(
       int[] S,
       int[] A,
       int[] T,
       float[,,] P,
       float[,,] R,
       float gamma = 0.99f,
       float theta = 0.000001f)
    {
        float[] V = new float[S.Length]; // value function resultats
        for (int iv = 0; iv < V.Length; ++iv)
        {
            V[iv] = Random.value;
        }
        setArrayAtArray<float>(ref V, ref T, 0.0f);
        float[,] Pi = create_random_uniform_policy(S.Length, A.Length);
        while (true) {
            V = iterative_policy_evaluation(S, A, T, P, R, Pi, gamma, theta, V);
            bool policy_stable = true;
            foreach (int s in S) {
                int old_action = getIndexOfMaxInArray(ref Pi, s, A.Length);

                int best_action = 0;
                float best_action_score = float.MinValue;
                foreach (int a in A) {
                    float temp_score = 0.0f;
                    foreach (int s_p in S) {
                        temp_score += P[s, a, s_p] * (R[s, a, s_p] + gamma * V[s_p]);
                    }
                    if (best_action_score <= temp_score) {
                        best_action = a;
                        best_action_score = temp_score;
                    }
                }
                setPiArrayValueAtState(ref Pi, s, 0.0f, A.Length);
                Pi[s, best_action] = 1.0f;
                if (best_action != old_action) {
                    policy_stable = false;
                }
            }
            if(policy_stable) break;
        }
        PairVPi pairVPi;
        pairVPi.V = V;
        pairVPi.Pi = Pi;
        return pairVPi;
    }
}