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
        Debug.Log("Notes collected: " + notesCollected);

        if (notesCollected == requiredNotes)
        {
            Debug.Log("Collected all notes!");
        }
    }

    public bool HasEnoughNotes()
    {
        return notesCollected >= requiredNotes;
    }
}