using UnityEngine;

public class SlashBridge : MonoBehaviour
{
    public void FinishSlash() {
        PlayerScript.Instance.OnFinishAttackAnimation();
    }
}
