using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineWorld
{
    public int[] S { get; private set; } // emsemble des états
    public int[] A { get; private set; } // emsemble des actions
    public int[] T { get; private set; }
    public float[,,] P { get; private set; }
    public float[,,] R { get; private set; } // ensemble des récompenses immédiates

    public LineWorld()
    {
        S = new int[] { 0, 1, 2, 3, 4 }; // liste de tout les états possibles
        A = new int[] { 0, 1 }; // liste de toute les actions possibles
        T = new int[] { 0, 4 }; // état terminaux
        P = new float[S.Length, A.Length, S.Length]; // Probabilité de passer dans un état donner a partir d'un état et d'une action
        R = new float[S.Length, A.Length, S.Length]; // Récompense a partir d'un état et d'une action

        //Initialise à 0
        for (int i = 0; i < S.Length; i++) // pour chaque état
        {
            for(int j = 0; j < A.Length; j++) // pour chaque action
            {
                for(int k = 0; k < S.Length; k++) // pour chaque état
                {
                    P[i, j, k] = 0;
                    R[i, j, k] = 0;
                }
            }
        }

        for(int i = 1; i < S.Length-1; i++) // du second état a l'avant derniere état
        {
            P[S[i], 0, S[i]-1] = 1.0f; // pour un état donner faire l'action 0 amene a l'état -1
            P[S[i], 1, S[i ]+1] = 1.0f; // pour un état donner faire l'action 1 amene a l'état +1
        }

        R[1, 0, 0] = -1.0f;
        R[3, 1, 4] = 1.0f;
    }
}
