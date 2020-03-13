using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Methods
{
    IPE,
    PI,
    MCES,
    VI
}

public class TestGridWorld : MonoBehaviour
{
    public GridLayoutGroup gridLayout;
    public int width = 10;
    public int height = 10;
    public GameObject prefabCase;
    public Methods methods;

    // Start is called before the first frame update
    void Start()
    {
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = width;

        GridWorld gridWorld = new GridWorld(width, height);
        float[,] Pi;
        switch (methods) {
            case Methods.IPE:
                Pi = new float[gridWorld.S.Length, gridWorld.A.Length];
                for (int i = 0; i < gridWorld.S.Length; ++i)
                {
                    for (int y = 0; y < gridWorld.A.Length; ++y)
                    {
                        Pi[i, y] = 0.0f;
                    }
                }

                for (int i = 0; i < gridWorld.S.Length; ++i)
                {
                    Pi[i, 1] = 1.0f;
                }
                float[] V = Algorithms.iterative_policy_evaluation(gridWorld.S, gridWorld.A, gridWorld.T, gridWorld.P, gridWorld.R, Pi);
                foreach (float v in V)
                {
                    GameObject caseGO = GameObject.Instantiate(prefabCase, gridLayout.transform);
                    caseGO.GetComponentInChildren<Text>().text = "V=" + v;
                }
                break;
            case Methods.PI:
                PairVPi VPi = Algorithms.policy_iteration(gridWorld.S, gridWorld.A, gridWorld.T, gridWorld.P, gridWorld.R);
                foreach (float v in VPi.V)
                {
                    GameObject caseGO = GameObject.Instantiate(prefabCase, gridLayout.transform);
                    caseGO.GetComponentInChildren<Text>().text = "V="+v;
                }
                break;
            case Methods.MCES:
                float[,] Q = Algorithms.monte_carlo_control_with_exploring_starts(
                    gridWorld.S, gridWorld.A, gridWorld.T, gridWorld.step, gridWorld.step_until_the_end_of_episode_and_return_transitions, out Pi, 0.99f, 5000
                );

                for (int s = 0; s < gridWorld.S.Length; ++s)
                {
                    GameObject caseGO = GameObject.Instantiate(prefabCase, gridLayout.transform);
                    caseGO.GetComponentInChildren<Text>().text = "";
                    for (int a = 0; a < gridWorld.A.Length; ++a)
                    {
                        caseGO.GetComponentInChildren<Text>().text += "S:" + s + " A:" + a + " Q:" + Q[s, a] + "\n";
                    }
                }
                break;
            case Methods.VI:
                Pi = new float[gridWorld.S.Length, gridWorld.A.Length];
                for (int i = 0; i < gridWorld.S.Length; ++i)
                {
                    for (int y = 0; y < gridWorld.A.Length; ++y)
                    {
                        Pi[i, y] = 0.0f;
                    }
                }

                for (int i = 0; i < gridWorld.S.Length; ++i)
                {
                    Pi[i, 1] = 1.0f;
                }
                V = Algorithms.value_iteration(gridWorld.S, gridWorld.A, gridWorld.T, gridWorld.P, gridWorld.R, Pi);
                foreach (float v in V)
                {
                    GameObject caseGO = GameObject.Instantiate(prefabCase, gridLayout.transform);
                    caseGO.GetComponentInChildren<Text>().text = "V=" + v;
                }
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
