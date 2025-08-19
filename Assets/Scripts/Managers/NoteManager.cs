using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public static NoteManager Instance { get; private set; }

    public int notesCollected = 0;
    public int requiredNotes = 5; // Adjust as needed

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void CollectNote()
    {
        notesCollected++;
        Debug.Log("[Piano] Notes collected: " + notesCollected);

        if (notesCollected == requiredNotes)
        {
            Debug.Log("[Piano] Collected all notes!");
        }

        UIManager.Instance.OnNoteCollected();
    }

    public bool HasEnoughNotes()
    {
        return notesCollected >= requiredNotes;
    }

    public int GetNoteCount()
    {
        return notesCollected;
    }
}