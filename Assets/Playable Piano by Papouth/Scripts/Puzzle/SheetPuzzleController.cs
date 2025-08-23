using System.Collections.Generic;
using UnityEngine;

public class SheetPuzzleController : MonoBehaviour
{
    [SerializeField] private List<DraggablePiece> pieces;

    void Start()
    {
        UpdatePiecesDisplay();
    }

    void OnEnable()
    {
        UpdatePiecesDisplay();
    }

    private void UpdatePiecesDisplay()
    {
        int notes = NoteManager.Instance.GetNoteCount();
        for (int i = 0; i < pieces.Count; i++)
        {
            // Show pieces based on the number of notes collected.
            if (pieces[i] != null)
            {
                pieces[i].gameObject.SetActive(i < notes);
            }
        }
    }
}
