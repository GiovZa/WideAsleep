using UnityEngine;
using UnityEngine.UI;

public class NoteManager : MonoBehaviour
{
    public static NoteManager Instance { get; private set; }

    public int notesCollected = 0;
    public int requiredNotes = 5; // Adjust as needed
    public Sprite paperNote;
    public Sprite emptyPaperNote;
    public Image[] notes;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        UpdateNotesHUD();
    }

    public void CollectNote()
    {
        notesCollected++;
        Debug.Log("[Piano] Notes collected: " + notesCollected);

        if (notesCollected == requiredNotes)
        {
            Debug.Log("[Piano] Collected all notes!");
        }

        UpdateNotesHUD();
    }

    public bool HasEnoughNotes()
    {
        return notesCollected >= requiredNotes;
    }

    public int GetNoteCount()
    {
        return notesCollected;
    }

    void UpdateNotesHUD()
    {
        for (int i = 0; i < notes.Length; i++)
        {
            if (i < notesCollected)
                notes[i].sprite = paperNote;
            else
                notes[i].sprite = emptyPaperNote;
            
            if (i < requiredNotes)
                notes[i].enabled = true;
            else
                notes[i].enabled = false;
        }
    }
}