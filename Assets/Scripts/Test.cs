﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private float[,] Pi; // state, action
    private LineWorld lineworld;
    private GridWorld gridworld;
    private float[] V; // value per state

    // Start is called before the first frame update
    void Start()
    {
        lineworld = new LineWorld(); 
        Pi = Algorithms.create_random_uniform_policy(lineworld.S.Length, lineworld.A.Length); // 
        V = Algorithms.iterative_policy_evaluation(lineworld.S, lineworld.A, lineworld.T, lineworld.P, lineworld.R, Pi);

        UIDisplayator.AddVF("Value Function de la stratégie \"random uniform\"", ref V);

        ////////////////////////////////////////////////////////////////////////////////////////
        
        for (int i = 0; i < lineworld.S.Length; ++i)
        {
            for (int y = 0; y < lineworld.A.Length; ++y)
            {
                Pi[i, y] = 0.0f;
            }
        }
    
        for (int i = 0; i < lineworld.S.Length; ++i)
        {
            Pi[i, 1] = 1.0f;

        }
        V = Algorithms.iterative_policy_evaluation(lineworld.S, lineworld.A, lineworld.T, lineworld.P, lineworld.R, Pi);

        UIDisplayator.AddVF("Value Function de la stratégie \"tout a droite\"", ref V);

        PairVPi VPi = Algorithms.policy_iteration(lineworld.S, lineworld.A, lineworld.T, lineworld.P, lineworld.R);

        UIDisplayator.AddVF("La value function optimale", ref VPi.V);
        Debug.Log("La policy optimale : ");
        for (int s = 0; s < lineworld.S.Length; ++s)
        {
            for (int a = 0; a < lineworld.A.Length; ++a)
            {
                Debug.Log("S="+s+" A="+a+" ="+VPi.Pi[s,a]);
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////

        gridworld = new GridWorld();
        Pi = Algorithms.create_random_uniform_policy(gridworld.S.Length, gridworld.A.Length); // 
        V = Algorithms.iterative_policy_evaluation(gridworld.S, gridworld.A, gridworld.T, gridworld.P, gridworld.R, Pi);

        UIDisplayator.AddVF("Value Function de la stratégie \"random uniform\"", ref V);

        ////////////////////////////////////////////////////////////////////////////////////////

        for (int i = 0; i < gridworld.S.Length; ++i)
        {
            for (int y = 0; y < gridworld.A.Length; ++y)
            {
                Pi[i, y] = 0.0f;
            }
        }

        for (int i = 0; i < gridworld.S.Length; ++i)
        {
            Pi[i, 1] = 1.0f;

        }
        V = Algorithms.iterative_policy_evaluation(gridworld.S, gridworld.A, gridworld.T, gridworld.P, gridworld.R, Pi);

        UIDisplayator.AddVF("Value Function de la stratégie \"tout a droite\"", ref V);

        VPi = Algorithms.policy_iteration(gridworld.S, gridworld.A, gridworld.T, gridworld.P, gridworld.R);

        UIDisplayator.AddVF("La value function optimale", ref VPi.V);
        Debug.Log("La policy optimale : ");
        for (int s = 0; s < gridworld.S.Length; ++s)
        {
            for (int a = 0; a < gridworld.A.Length; ++a)
            {
                Debug.Log("S=" + s + " A=" + a + " =" + VPi.Pi[s, a]);
            }
        }

        //////////////////////////////////////////////////

        float[,] Q = Algorithms.monte_carlo_control_with_exploring_starts(
            lineworld.S, lineworld.A, lineworld.T, lineworld.step, lineworld.step_until_the_end_of_episode_and_return_transitions, out Pi, 0.99f, 5000
        );

        Debug.Log("L'action value optimale :");
        for (int s = 0; s < lineworld.S.Length; ++s)
        {
            for (int a = 0; a < lineworld.A.Length; ++a)
            {
                Debug.Log("S:"+s+" A:"+a+" Q:"+Q[s,a]);
            }
        }
        Debug.Log("La policy \"optimale\" :");
        for (int s = 0; s < lineworld.S.Length; ++s)
        {
            for (int a = 0; a < lineworld.A.Length; ++a)
            {
                Debug.Log("S:" + s + " A:" + a + " Pi:" + Pi[s, a]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
