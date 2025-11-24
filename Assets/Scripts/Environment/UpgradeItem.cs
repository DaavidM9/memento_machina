using Unity.VisualScripting;
using UnityEngine;


public class UpgradeItem : MonoBehaviour
{
    
    void Start()
    {
        if (PlayerPrefs.GetInt("PickedUp_UpgradeItem1", 0) == 1) // Default is 0 if key doesn't exist
        {
            Destroy(gameObject); // Object was already picked up
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.CompareTag("Player"))
        {
            Money.Instance.GrabDollar(30); // dar dinero
            
            // Evitar que vuelva a aparecer una vez cogido
            PlayerPrefs.SetInt("PickedUp_UpgradeItem1", 1);
            PlayerPrefs.Save(); // Save the preference
            
            Destroy(gameObject);
            EffectsManager.Instance.PlayCoinSound(); // Por poner un sonido
        }
    }
}

