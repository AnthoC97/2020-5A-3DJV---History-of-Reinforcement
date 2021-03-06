﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PairVPi
{
    public float[] V;
    public float[,] Pi;
};

public struct PairSARSP
{
    public List<int> S;
    public List<int> A;
    public List<float> R;
    public List<int> SP;
};

public struct PairSR
{
    public int S;
    public float R;
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

    public static bool arrayContain(int[] Array, int val)
    {
        foreach(int v in Array)
            if (v == val) return true;
        return false;
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

    public static float[,] monte_carlo_control_with_exploring_starts(
       int[] S,
       int[] A,
       int[] T,
       System.Func<int, int, PairSR> step_func,
       System.Func<int, float[,], PairSARSP> step_until_the_end_and_return_transitions_func,
       out float[,] Pi,
       float gamma = 0.99f,
       int nb_iter = 1000
    ) {
        Pi = create_random_uniform_policy(S.Length, A.Length);
        float[,] Q = new float[S.Length, A.Length];
        for (int s = 0; s < S.Length; ++s) {
            for (int a = 0; a < A.Length; ++a)
            {
                Q[s,a] = Random.value;
            }
        }
        foreach(int t in T)
        {
            for (int a = 0; a < A.Length; ++a)
                Q[t, a] = 0.0f;
        }
        float[,] ReturnsSum = new float[S.Length, A.Length];
        float[,] ReturnsCount = new float[S.Length, A.Length];

        for (int _ = 0; _ < nb_iter; ++_) {
            int s0 = S[Random.Range(0, S.Length)];

            if (arrayContain(T, s0))
                continue;

            int a0 = A[Random.Range(0, A.Length)];

            PairSR pairSR = step_func.Invoke(s0, a0);
            PairSARSP pairSARSP = step_until_the_end_and_return_transitions_func.Invoke(pairSR.S, Pi);
            float G = 0;
            pairSARSP.S.Insert(0, s0);
            pairSARSP.A.Insert(0, a0);
            pairSARSP.R.Insert(0, pairSR.R);
            for (int t = pairSARSP.S.Count - 1; t > 0; --t) {
                G = pairSARSP.R[t] + gamma * G;
                int st = pairSARSP.S[t];
                int at = pairSARSP.A[t];
                bool stIn = false;
                for (int i = 0; i <= t; ++i)
                {
                    if (pairSARSP.S[t] == pairSARSP.S[i])
                    {
                        stIn = true;
                        break;
                    }
                }
                bool atIn = false;
                for (int i = 0; i <= t; ++i)
                {
                    if (at == pairSARSP.A[i])
                    {
                        atIn = true;
                        break;
                    }
                }
                if (stIn && atIn)
                    continue;
                ReturnsSum[st, at] += G;
                ReturnsCount[st, at] += 1;
                Q[st, at] = ReturnsSum[st, at] / ReturnsCount[st, at];
                setPiArrayValueAtState(ref Pi, st, 0.0f, A.Length);
                float max = float.MinValue;
                int argmax = 0;
                for (int a = 0; a < A.Length; ++a)
                {
                    if (Q[st, a] > max)
                    {
                        max = Q[st, a];
                        argmax = a;
                    }
                }
                Pi[st, argmax] = 1.0f;
            }
        }
        return Q;
    }

    public static float[] value_iteration(
       int[] S,
       int[] A,
       int[] T,
       float[,,] P,
       float[,,] R,
       float[,] Pi,
       float gamma = 0.80f,
       float theta = 0.000001f
        )
    {
        float delta = 0f;
        float av = 0f;
        float[] V = new float[S.Length];
        for (int i = 0; i < V.Length; i++)
        {
            V[i] = Random.value;
        }
        foreach (int t in T)
        {
            V[t] = 0;
        }
        while (delta < theta)
        {
            foreach (int s in S)
            {
                float v = V[s];
                float[] action_val = new float[A.Length];
                foreach (int a in A)
                {
                    foreach (int s2 in S)
                    {
                        av = Mathf.Max(P[s, a, s2] * (R[s, a, s2] + gamma * V[s2]), av);
                        Debug.Log("av : " + av);
                        action_val[a] = av;
                    }
                }
                float maxA = 0;
                foreach (float a_val in action_val)
                {
                    //Debug.Log("a_val : " + a_val);
                    if (maxA < a_val)
                    {
                        maxA = a_val;
                    }
                }
                Debug.Log("MaxA : " + maxA);
                V[s] = maxA;
                float tmp = Mathf.Abs(v - V[s]);
                if (delta < tmp)
                    delta = tmp;
            }
        }

        int state = 0;
        float tpm = 0f;
        av = 0f;
        int bast_a = 0;
        foreach (int s in S)
        {
            float v = V[s];
            float[] action_val = new float[A.Length];
            foreach (int a in A)
            {
                foreach (int s2 in S)
                {
                    tpm = Mathf.Max(P[s, a, s2] * (R[s, a, s2] + gamma * V[s2]), av);
                    if (av < tpm)
                        bast_a = a;
                    action_val[a] = av;
                }
            }
            float maxA = 0;
            foreach (float a_val in action_val)
            {
                //Debug.Log("a_val : " + a_val);
                if (maxA < a_val)
                {
                    maxA = a_val;
                }
            }
            Debug.Log("MaxA : " + maxA);
            V[s] = maxA;
            float tmp = Mathf.Abs(v - V[s]);
            if (delta < tmp)
                delta = tmp;
        }

        return V;
    }
}