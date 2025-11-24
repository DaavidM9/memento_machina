using TMPro; // Import the TextMeshPro namespace
using UnityEngine;
using System.Collections;
using System.Linq;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text characterNameText; // Use TMP_Text instead of Text
    public TMP_Text dialogueText; // Use TMP_Text instead of Text
    public GameObject dialogueUI;
    
    private float textRevealSpeed = 0.03f;
    private DialogueLoader dialogueLoader;
    private DialogueEntry currentDialogue;
    private bool isTextRevealing = false;
    private bool isTextFullyRevealed = false;
    private Coroutine revealCoroutine;

    private void Start()
    {
        dialogueLoader = GetComponent<DialogueLoader>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && gameObject.activeSelf)
        {
            if (isTextRevealing)
            {
                textRevealSpeed = 0.001f; // Speed up text reveal
            }
            else if (isTextFullyRevealed && currentDialogue.nextId != -1)
            {
                DisplayDialogue(currentDialogue.nextId); // Move to the next dialogue
            }
            else if (currentDialogue.nextId == -1)
            {
                dialogueUI.SetActive(false); // Esconder si se ha acabado
            }
        }
    }

    public void DisplayDialogue(int dialogueId)
    {
        currentDialogue = dialogueLoader.dialogueData.dialogues.FirstOrDefault(d => d.id == dialogueId);

        if (currentDialogue != null)
        {
            characterNameText.text = currentDialogue.characterName;
            isTextFullyRevealed = false;

            // Start the text reveal coroutine
            if (revealCoroutine != null)
            {
                StopCoroutine(revealCoroutine);
            }
            revealCoroutine = StartCoroutine(RevealText(currentDialogue.text));
        }
        else
        {
            Debug.Log("Dialogue ID not found!");
        }
    }

    private IEnumerator RevealText(string text)
    {
        isTextRevealing = true;
        dialogueText.text = ""; // Clear existing text

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter; // Append the next letter
            yield return new WaitForSeconds(textRevealSpeed);
        }

        isTextRevealing = false;
        isTextFullyRevealed = true;
        textRevealSpeed = 0.05f; // Reset speed for the next dialogue
    }


}