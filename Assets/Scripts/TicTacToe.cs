using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TicTacToe : MonoBehaviour
{
    public Button[] GridButtons;
    int[] S;
    int[] A = { 0,1,2,3,4,5,6,7,8 };
    int[] T = { };
    float[,] Pi;
    bool hasToPlay = false;
    bool playerTurn = false;
    bool isPlaying = false;
    float[,] Q;

    public int iteration = 5000;

    bool isTermined = false;

    // Start is called before the first frame update
    void Start()
    {
        S = new int[Mathf.RoundToInt(Mathf.Pow(3,9))];
        Q = new float[Mathf.RoundToInt(Mathf.Pow(3,9)), A.Length];
        for (int i=0; i<S.Length; ++i)
        {
            S[i] = i;
        }

        /*Debug.Log("L'action value optimale :");
        for (int s = 0; s < S.Length; ++s)
        {
            for (int a = 0; a < A.Length; ++a)
            {
                Debug.Log("S:" + s + " A:" + a + " Q:" + Q[s, a]);
            }
        }
        Debug.Log("La policy \"optimale\" :");
        for (int s = 0; s < S.Length; ++s)
        {
            for (int a = 0; a < A.Length; ++a)
            {
                Debug.Log("S:" + s + " A:" + a + " Pi:" + Pi[s, a]);
            }
        }*/
    }

    public void MonteCarlo()
    {
        Reset();
        Q = Algorithms.monte_carlo_control_with_exploring_starts(
            S, A, T, step, step_until_the_end_of_episode_and_return_transitions, out Pi, 0.99f, iteration
        );
    }

    // Update is called once per frame
    void Update()
    {
        if (hasToPlay)
        {
            hasToPlay = false;
            float max = float.MinValue;
            int action = 0;
            int s = GridToState();
            for (int i=0; i<A.Length; ++i)
            {
                if(Q[s, i] > max)
                {
                    max = Q[s, i];
                    action = i;
                }
            }

            if (GridButtons[action].GetComponentInChildren<Text>().text == "0")
                SetButtonState(action, 1);
            if(!isTerminal()) playerTurn = true;
        }
    }
    public void Play()
    {
        Reset();
        isPlaying = true;
        playerTurn = true;
    }

    public void SetButtonState(int numButton, int state)
    {
        GridButtons[numButton].GetComponentInChildren<Text>().text = state.ToString();
    }

    public int GridToState()
    {
        int state = 0;
        for(int i=0; i<GridButtons.Length; ++i)
        {
            int num = 0;
            switch(GridButtons[i].GetComponentInChildren<Text>().text)
            {
                case "0":
                    num = 0;
                    break;
                case "1":
                    num = 1;
                    break;
                case "2":
                    num = 2;
                    break;
            }
            state += num*Mathf.RoundToInt(Mathf.Pow(3, i));
        }
        return state;
    }

    public PairSR step(int s, int a)
    {
        float r = 0;
        if (GridButtons[a].GetComponentInChildren<Text>().text == "0")
        {
            SetButtonState(a, 1);
            isTermined = isTerminal();
            r = isTermined ? 1 : 0;
        }
        else
        {
            r = -0.5f;
        }

        if (!isTermined)
        {
            //Random Play
            List<int> possible = new List<int>();
            for (int i = 0; i < GridButtons.Length; ++i)
            {
                if (GridButtons[i].GetComponentInChildren<Text>().text == "0")
                    possible.Add(i);
            }
            if(possible.Count > 0)
                SetButtonState(possible[Random.Range(0, possible.Count)], 2);

            isTermined = isTerminal();
            if(isTermined)
                r = isTermined ? -1 : 0;
        }

        int s_p = GridToState();
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
        while (!isTermined/*Algorithms.arrayContain(T, s)*/ && s_list.Count < S.Length * 10)
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
        Reset();
        PairSARSP pairSARSP;
        pairSARSP.S = s_list;
        pairSARSP.A = a_list;
        pairSARSP.R = r_list;
        pairSARSP.SP = s_p_list;
        return pairSARSP;
    }

    private void Reset()
    {
        isTermined = false;
        for(int i=0; i<GridButtons.Length; ++i)
        {
            GridButtons[i].GetComponentInChildren<Text>().text = "0";
        }
    }

    bool isTerminal()
    {
        int a,b,c;
        int.TryParse(GridButtons[0].GetComponentInChildren<Text>().text, out a);
        if (a != 0)
        {
            int.TryParse(GridButtons[1].GetComponentInChildren<Text>().text, out b);
            int.TryParse(GridButtons[2].GetComponentInChildren<Text>().text, out c);
            if (a == b && a == c) return true;
            int.TryParse(GridButtons[3].GetComponentInChildren<Text>().text, out b);
            int.TryParse(GridButtons[6].GetComponentInChildren<Text>().text, out c);
            if (a == b && a == c) return true;
            int.TryParse(GridButtons[4].GetComponentInChildren<Text>().text, out b);
            int.TryParse(GridButtons[8].GetComponentInChildren<Text>().text, out c);
            if (a == b && a == c) return true;
        }

        int.TryParse(GridButtons[1].GetComponentInChildren<Text>().text, out a);
        if (a != 0)
        {
            int.TryParse(GridButtons[4].GetComponentInChildren<Text>().text, out b);
            int.TryParse(GridButtons[7].GetComponentInChildren<Text>().text, out c);
            if (a == b && a == c) return true;
        }

        int.TryParse(GridButtons[2].GetComponentInChildren<Text>().text, out a);
        if (a != 0)
        {
            int.TryParse(GridButtons[4].GetComponentInChildren<Text>().text, out b);
            int.TryParse(GridButtons[6].GetComponentInChildren<Text>().text, out c);
            if (a == b && a == c) return true;
        }

        int.TryParse(GridButtons[3].GetComponentInChildren<Text>().text, out a);
            if (a != 0)
            {
                int.TryParse(GridButtons[4].GetComponentInChildren<Text>().text, out b);
                int.TryParse(GridButtons[5].GetComponentInChildren<Text>().text, out c);
                if (a == b && a == c) return true;
            }

        int.TryParse(GridButtons[2].GetComponentInChildren<Text>().text, out a);
        if (a != 0)
        {
            int.TryParse(GridButtons[5].GetComponentInChildren<Text>().text, out b);
            int.TryParse(GridButtons[7].GetComponentInChildren<Text>().text, out c);
            if (a == b && a == c) return true;
        }
        return false;
    }

    public void UseButton(int button)
    {
        if (!playerTurn) return;
        SetButtonState(button, 2);
        if(!isTerminal()) hasToPlay=true;
        playerTurn = false;
    }
}
