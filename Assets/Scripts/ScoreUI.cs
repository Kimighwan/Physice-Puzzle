using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    private RectTransform trans;
    private Text scoreText;
    private Animator anim;


    void Awake()
    {
        PlayerPrefs.SetInt("CurScore", 0);
        trans = GetComponent<RectTransform>();
        scoreText = GetComponentInChildren<Text>();
        anim = GetComponent<Animator>();
    }

    public void PlayAnim(int score)
    {
        if (score == 0)
            return;

        if (PlayerPrefs.GetInt("CurScore") < int.Parse(scoreText.text))
        {
            anim.SetBool("Do", true);
            Invoke("StopAnim", 0.22f);
        }
    }

    void StopAnim()
    {
        anim.SetBool("Do", false);
        PlayerPrefs.SetInt("CurScore", int.Parse(scoreText.text));
        Debug.Log(PlayerPrefs.GetInt("CurScore"));
    }
}
