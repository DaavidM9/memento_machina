using UnityEngine;

public class MemoryManager : MonoBehaviour
{
    
    public static MemoryManager Instance;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ClearMemory();
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private void ClearMemory()
    {
        // Dialogos
        PlayerPrefs.DeleteKey("Dialogue_IntroDialogue_Accessed");
        PlayerPrefs.DeleteKey("Dialogue_IntroDialogue2_Accessed");
        PlayerPrefs.DeleteKey("Dialogue_IntroDialogue3_Accessed");
        PlayerPrefs.DeleteKey("Dialogue_ShopIntroDialogue_Accessed");
        PlayerPrefs.DeleteKey("Dialogue_LaserUpgradeDialogue_Accessed");
        // Upgrade
        PlayerPrefs.DeleteKey("PickedUp_UpgradeItem1");
    }

}
