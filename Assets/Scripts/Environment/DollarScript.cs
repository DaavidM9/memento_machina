using Unity.VisualScripting;
using UnityEngine;

public class DollarScript : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.CompareTag("Player")) {
            Destroy(gameObject);
            Money.Instance.GrabDollar();
            EffectsManager.Instance.PlayCoinSound();
        }
    }
}
