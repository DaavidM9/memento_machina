using System.Collections;
using UnityEngine;
public static class AnimationUtils {
    public static IEnumerator WaitForAnimationEnd(string animationName, Animator animator)
    {
        AnimatorStateInfo stateInfo;

        do
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        } while (!stateInfo.IsName(animationName));

        while (stateInfo.normalizedTime < 1.0f)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }
    }
}