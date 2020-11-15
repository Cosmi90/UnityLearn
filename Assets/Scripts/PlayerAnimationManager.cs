using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    private Animator animator;
    private bool isPlayingAnimation;
    private static IEnumerator animationCoroutine;

    const string animBaseLayer = "Base.";
    int idleAnimHash = Animator.StringToHash(animBaseLayer + "Alucard_Idle_1");
    int turnAnimHash = Animator.StringToHash(animBaseLayer + "Alucard_Turn");


    private void Awake()
    {
        animator = transform.GetComponent<Animator>();
    }

    private IEnumerator PlayAndWaitForAnim(Animator targetAnim, string stateName)
    {
        int animHash = 0;
        if (stateName == "Alucard_Idle_1")
            animHash = idleAnimHash;
        else if (stateName == "Alucard_Turn")
            animHash = turnAnimHash;

        //targetAnim.Play(stateName);
        targetAnim.CrossFadeInFixedTime(stateName, 0);

        //Wait until we enter the current state
        while (targetAnim.GetCurrentAnimatorStateInfo(0).fullPathHash != animHash)
        {
            yield return null;
        }

        float counter = 0;
        float waitTime = targetAnim.GetCurrentAnimatorStateInfo(0).length;

        //Now, Wait until the current state is done playing
        while (counter <= (waitTime))
        {
            counter += Time.deltaTime;
            yield return null;
        }

        //Done playing. Do something below!
        Debug.Log("Done Playing");
        isPlayingAnimation = false;
    }

    //private IEnumerator PlayCoroutine(string animationName, float toWait)
    //{
    //    int animHash = 0;
    //    if (animationName == "Alucard_Idle_1")
    //        animHash = idleAnimHash;
    //    else if (animationName == "Alucard_Turn")
    //        animHash = turnAnimHash;

    //    if (animHash != animator.GetCurrentAnimatorStateInfo(0).fullPathHash)
    //    {
    //        Debug.Log("sas");
    //        isPlayingAnimation = true;
    //        animator.Play(animationName);

    //        yield return new WaitForSeconds(toWait);
    //    }
    //    else
    //    {
    //        yield return null;
    //    }
    //}

    //public void Play(string animationName, float length)
    //{
    //    StartCoroutine(PlayCoroutine(animationName, length));
    //}

    public void PlayAnimation(string animationName, bool requireFullPlay, bool interruptCurrentAnimation = false)
    {
        if (requireFullPlay)
        {
            isPlayingAnimation = true;
            //animator.Play("Base." + animationName, 0, 0);

            if (interruptCurrentAnimation && animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            animationCoroutine = PlayAndWaitForAnim(animator, animationName);
            StartCoroutine(animationCoroutine);
        }
        else
        {
            if(!isPlayingAnimation)
                animator.Play("Base." + animationName, 0);
        }
        Debug.Log($"Playing animation: {animationName}");
    }
}