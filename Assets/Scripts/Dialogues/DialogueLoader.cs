using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueLoader : MonoBehaviour
{
    
    public DialogueData dialogueData;
    public GameObject dialogueUI;
    
    private List<DialoguePoint> dialoguePoints;
    private DialogueManager dialogueManager;

    private class DialoguePoint
    {
        public string dialogueName;
        public Vector3 position;
        public bool accessed = false;

        public DialoguePoint(string dialogueName, Vector3 position)
        {
            this.dialogueName = dialogueName;
            this.position = position;
        }
    }
    

    public void Awake()
    {
        dialogueManager = GetComponent<DialogueManager>();
        dialogueUI.gameObject.SetActive(false);

        if (! (PlayerPrefs.GetInt("Dialogue_IntroDialogue_Accessed", 0) == 1))
        {
            StartCoroutine(FirstDialogue());
        }
        
        
        dialoguePoints = new List<DialoguePoint>(); // creamos la lista de puntos de dialogo
        dialoguePoints.Add(new DialoguePoint("IntroDialogue2", new Vector3(5, -2, 0)));
        dialoguePoints.Add(new DialoguePoint("IntroDialogue3", new Vector3(-3, -8, 0)));
        dialoguePoints.Add(new DialoguePoint("ShopIntroDialogue", new Vector3(0, -19, 0)));
        dialoguePoints.Add(new DialoguePoint("LaserUpgradeDialogue", new Vector3(6, -36, 0)));

        StartCoroutine(CheckPlayerPositionPeriodically());
    }
    

    public void LoadDialogue(string fileName)
    {
        EffectsManager.Instance.PlayDialogueStartSound();
        TextAsset jsonFile = Resources.Load<TextAsset>("Dialogues/" + fileName);

        if (jsonFile != null)
        {
            dialogueData = JsonUtility.FromJson<DialogueData>(jsonFile.text); // cargamos el file
            Debug.Log("Loaded dialogue: " + fileName);
        }
        else
        {
            Debug.LogError("Dialogue file not found: " + fileName);
        }
    }
    
    private IEnumerator CheckPlayerPositionPeriodically()
    {
        while (true)
        {
            CheckPosition();

            yield return new WaitForSeconds(0.75f); // Wait before the next check
        }
    }

    private void CheckPosition()
    {
        Vector3 playerpos = GameObject.FindGameObjectWithTag("Player").transform.position;
        
        foreach (var point in dialoguePoints)
        {
            if (PlayerPrefs.GetInt($"Dialogue_{point.dialogueName}_Accessed", 0) == 1) // Comprobamos memoria de escena anterior
            {
                point.accessed = true; // Mark as accessed
            }
            
            if (!point.accessed) // comprobar que no se haya usado
            {
                Vector3 coords = point.position;
                if (Vector3.Distance(playerpos, coords) <= 2.5f) // si está en el rango
                {
                    point.accessed = true; // poner a true para evitar repetir
                    
                    PlayerPrefs.SetInt($"Dialogue_{point.dialogueName}_Accessed", 1); // Save state
                    PlayerPrefs.Save();
                    
                    dialogueUI.SetActive(true); // Hacemos visible
                    LoadDialogue(point.dialogueName); // cargar dialogo
                    dialogueManager.DisplayDialogue(1); // Enseñamos dialogo
                    break;
                }
            }
        }
    }
    
    
    private IEnumerator FirstDialogue()
    {
        yield return new WaitForSeconds(3.5f); // Esperar  segundos a empezar el dialogo
        LoadDialogue("IntroDialogue"); // cargamos el dialogo
        dialogueUI.gameObject.SetActive(true);
        dialogueManager.DisplayDialogue(1); // Start with the first dialogue entry (ID 1)
        
        PlayerPrefs.SetInt("Dialogue_IntroDialogue_Accessed", 1); // Save state
        PlayerPrefs.Save();
    }
    
}