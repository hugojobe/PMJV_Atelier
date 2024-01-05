using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    private Rythm manager => GetComponentInParent<Rythm>();

    public GameObject noteObject;
    public Vector2 noteSpawnpoint;

    public bool canSpawn;
    public bool spawnFinished;

    public void StartSequence() {
        int repetitions = (int)manager.selectedPattern[0];
        List<float> delays = manager.selectedPattern.Skip(1).ToList();

        canSpawn = true;

        StartCoroutine(NotesSequencer(repetitions, delays));
    }

    private IEnumerator NotesSequencer(int count, List<float> delays) {
        for (int i = 0; i < count; i++) {
            for(int j = 0; j < delays.Count; j++) {
                yield return new WaitForSecondsRealtime(delays[j]);

                if(!canSpawn)
                    yield break;

                SpawnNote();
            }
            yield return null;
        }

        spawnFinished = true;
    }

    private void SpawnNote() {
        Note note = Instantiate(noteObject, transform).GetComponent<Note>();
        note.transform.localPosition = noteSpawnpoint;
    }
}
