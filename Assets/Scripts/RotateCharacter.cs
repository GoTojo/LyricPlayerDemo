using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCharacter : MonoBehaviour
{
    // Start is called before the first frame update
    private bool fMidiTrigger = false;
    void Start()
    {
        MidiWatcher midiWatcher = MidiWatcher.Instance;
		midiWatcher.onMeasureIn += MeasureIn;
    }

    // Update is called once per frame
    void Update()
    {
        Transform transform = this.gameObject.transform;
        if (fMidiTrigger) {
            transform.rotation = Quaternion.Euler( 0f, 180f, 0f);
            fMidiTrigger = false;
        } else if (transform.eulerAngles.y <= 120 || transform.eulerAngles.y >= 180) {
            transform.Rotate( 0f, 5f, 0f);
        }
    }

	public void MeasureIn(int measure, int measureInterval, uint currentMsec)
	{
        fMidiTrigger = true;
	}
}
