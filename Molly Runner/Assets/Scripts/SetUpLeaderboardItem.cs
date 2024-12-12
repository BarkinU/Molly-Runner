using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetUpLeaderboardItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _username;
    [SerializeField] private TMP_Text score;

    public void SetUp(string username, int _score)
    {
        _username.text = username;
        score.text = _score.ToString();
    }

}
