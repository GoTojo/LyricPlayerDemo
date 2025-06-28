using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;
public class UIPanelControl : MonoBehaviour {
	public Image uiPanel;
	public TMPro.TMP_Text text;
	private float lifetime = 2;
	private float lefttime = 0;
	// Start is called before the first frame update
	void Start() {
	}

	// Update is called once per frame
	void Update() {
		if (lefttime >= 0) {
			lefttime -= Time.deltaTime;
			if (lefttime <= 0) {
				Hide();
			}
		}
	}

	public void Show(string text, float lifetime) {
		this.text.text = text;
		uiPanel.gameObject.SetActive(true);
		lefttime = lifetime;
	}

	public void Hide() {
		uiPanel.gameObject.SetActive(false);
	}
}
