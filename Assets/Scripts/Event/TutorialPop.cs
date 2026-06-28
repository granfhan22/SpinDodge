using System.Collections;
using UnityEngine;

public class TutorialPop : MonoBehaviour
{
    public float showTime = 3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(TutorialShow());
    }

    // show Tutorial in amount of show time. @Vinky-cdoing
    IEnumerator TutorialShow()
    {
        yield return new WaitForSeconds(showTime);
        gameObject.SetActive(false);
    }
    
}
