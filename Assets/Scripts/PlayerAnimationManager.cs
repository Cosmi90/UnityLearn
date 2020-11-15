using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    private bool isPlayingAnimation;
    private string animBaseLayer;
    private static IEnumerator animationCoroutine;
    private Animator animator;
    Dictionary<string, int> allAnimationStates = new Dictionary<string, int>();

    private void Start()
    {
        animator = transform.GetComponent<Animator>();
        GetAllAnimationStates();
    }

    private IEnumerator PlayAndWaitForAnim(string stateName, bool playAnimationWhileKeyIsPressed)
    {
        int animHash = allAnimationStates[stateName];

        //targetAnim.Play(stateName); // not working with this method
        animator.CrossFadeInFixedTime(stateName, 0);

        //Wait until we enter the current state
        while (animator.GetCurrentAnimatorStateInfo(0).fullPathHash != animHash)
        {
            yield return null;
        }

        float counter = 0;
        float waitTime = animator.GetCurrentAnimatorStateInfo(0).length;

        //Now, Wait until the current state is done playing
        while (counter <= (waitTime))
        {
            counter += Time.deltaTime;
            yield return null;
        }

        while (playAnimationWhileKeyIsPressed)
        {
            yield return null;
        }

        //Done playing. Do something below!
        isPlayingAnimation = false;
    }

    public void PlayAnimation(string animationName, bool requireFullPlay, bool interruptCurrentAnimation = false, bool selfIntrerrupt = false, bool playAnimationWhileKeyIsPressed = false)
    {
        if (IsPlayingSameAnimation(animationName) && !selfIntrerrupt)
        {
            return;
        }

        if (requireFullPlay)
        {
            isPlayingAnimation = true;

            if (interruptCurrentAnimation && animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            animationCoroutine = PlayAndWaitForAnim(animationName, playAnimationWhileKeyIsPressed);
            StartCoroutine(animationCoroutine);
        }
        else if (!isPlayingAnimation)
            animator.Play(animBaseLayer + animationName, 0);
    }

    public void PlayAnimation(PlayAnimationInput input)
    {
        if (IsPlayingSameAnimation(input.AnimationName) && !input.SelfIntrerrupt)
        {
            return;
        }

        if (input.RequireFullPlay)
        {
            isPlayingAnimation = true;

            if (input.InterruptCurrentAnimation && animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            animationCoroutine = PlayAndWaitForAnim(input.AnimationName, input.PlayAnimationWhileKeyIsPressed);
            StartCoroutine(animationCoroutine);
        }
        else if (!isPlayingAnimation)
            animator.Play(animBaseLayer + input.AnimationName, 0);
    }

    public void StopAnimation(string animationName)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == allAnimationStates[animationName])
        {
            StopCoroutine(animationCoroutine);
        }
        else
        {
            Debug.LogError("Animation " + animationName + " is not currently playing!");
            return;
        }

        isPlayingAnimation = false;
    }

    private bool IsPlayingSameAnimation(string animationName)
    {
        int animHash = allAnimationStates[animationName];

        return animator.GetCurrentAnimatorStateInfo(0).fullPathHash == animHash;
    }

    private void GetAllAnimationStates()
    {
        if (animator.layerCount > 1)
            Debug.LogError("Animator has multiple layers! The PlayerAnimationManager script works with only one layer");
        else
            animBaseLayer = animator.GetLayerName(0) + ".";

        foreach (AnimationClip ac in animator.runtimeAnimatorController.animationClips)
        {
            allAnimationStates[ac.name] = Animator.StringToHash(animBaseLayer + ac.name);
        }
    }
}

public class PlayAnimationInput
{
    public string AnimationName { get; set; }
    public bool RequireFullPlay { get; set; }
    public bool InterruptCurrentAnimation { get; set; }
    public bool SelfIntrerrupt { get; set; }
    public bool PlayAnimationWhileKeyIsPressed { get; set; }
}