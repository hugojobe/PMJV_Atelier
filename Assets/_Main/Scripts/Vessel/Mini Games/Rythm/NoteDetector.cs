using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteDetector : MonoBehaviour
{
    private Rythm manager => GetComponentInParent<Rythm>();

    private bool buttonClicked;
    private bool hasNoteInDetector;
    private Note noteInDetector;

    public Button noteButton;

    public bool noteRemaining;

    private void Update() {
        if(buttonClicked)
            OnButtonClick();

        noteRemaining = FindObjectOfType<Note>() != null;
    }

    public void OnButtonClick() {
        buttonClicked = false;

        if(hasNoteInDetector) {
            DestroyNote(noteInDetector);
        } else {
            manager.Lose(0);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Note")){
            Note noteEntered = other.GetComponent<Note>();
            if(hasNoteInDetector && noteEntered != noteInDetector) {
                manager.Lose(1);
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if(other.CompareTag("Note"))
            noteInDetector = other.GetComponent<Note>();
            hasNoteInDetector = true;
    }

    public void DestroyNote(Note note) {
        hasNoteInDetector = false;
        Destroy(note.gameObject);
    }
}
