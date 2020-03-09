using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private float[,] Pi;
    private LineWorld lineworld;
    private float[] V;

    // Start is called before the first frame update
    void Start()
    {
        lineworld = new LineWorld();
        Pi = Algorithms.create_random_uniform_policy(lineworld.S.Length, lineworld.A.Length);
        V = Algorithms.iterative_policy_evaluation(lineworld.S, lineworld.A, lineworld.T, lineworld.P, lineworld.R, Pi);

        Debug.Log("Value Function de la stratégie \"random uniform\" ");
        foreach (float v in V)
        {
            Debug.Log(v);
        }

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
        Debug.Log("Value Function de la stratégie \"tout a droite\" ");
        foreach (float v in V)
        {
            Debug.Log(v);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
