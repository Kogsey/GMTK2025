using System.Collections.Generic;
using UnityEngine;

public class TimedTextBoxManager : MonoBehaviour
{
	public GameObject TextPanel;
	public TMPro.TextMeshProUGUI MessageText;

	public float displayDuration = 6f; // Time in seconds before text disappears

	public static List<string> MessageOptionsList = new()
	{
		"This will count as leave without pay",
		"Even if you did escape, I'd just make you work from home!",
		"My my - trying to quit? I'm afraid I just can't allow it",
		"This behaviour will reflect on your bonus - oh wait that's right, you don't get one ahaha",
		"Oh no - I'm afraid you've failed to submit the appropriate paperwork for this little excursion",
		"Oh so close - but you'll go no further. You don't want to be late for work now do you?",
		"Do you know how many codes you're violating? Oh the paperwork I'm going to have to fill out"
	};

	// Start is called before the first frame update
	private void Start()
	{
		// Ensure the panel is active at the start
		if (TextPanel != null)
		{
			TextPanel.SetActive(true);
		}
		// Ensure the text is active at the start
		if (MessageText != null)
		{
			MessageText.gameObject.SetActive(true);
		}

		// Select message from list at random
		string randomMessage = MessageOptionsList.PickRandom();
		MessageText.text = randomMessage;
		StartCoroutine(HideTextAfterDelay(displayDuration));
	}

	IEnumerator<WaitForSeconds> HideTextAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay); // Wait for the specified duration

		if (TextPanel != null)
		{
			TextPanel.SetActive(false); // Deactivate the Panel GameObject
		}
		if (MessageText != null)
		{
			MessageText.gameObject.SetActive(false); // Deactivate the Text GameObject
		}
	}
}