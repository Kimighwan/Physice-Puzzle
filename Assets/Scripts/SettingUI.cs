using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    Animator anim;
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }


    public void Close()
    {
        StartCoroutine("CloseDelay");
    }

    IEnumerator CloseDelay()
    {
        anim.SetTrigger("Close");
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
        anim.ResetTrigger("Close");
    }
}
