using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoardManager : MonoBehaviour
{
    public PlayFabManager playFabManager;


    private void Start()
    {
        if (playFabManager != null)
        {
            playFabManager.GetLeaderBoard();
        }
    }
}
