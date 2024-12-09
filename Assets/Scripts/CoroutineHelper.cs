using System;
using System.Collections;
using UnityEngine;

public class CoroutineHelper : MonoBehaviour
{
    public IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    public void PauseAndContinue(Action actionAfterPause, float delay)
    {
        StartCoroutine(PauseCoroutine(actionAfterPause, delay));
    }

    private IEnumerator PauseCoroutine(Action actionAfterPause, float delay)
    {
        yield return new WaitForSeconds(delay);
        actionAfterPause?.Invoke();
    }
}
