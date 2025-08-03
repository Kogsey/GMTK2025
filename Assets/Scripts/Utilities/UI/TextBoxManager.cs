using System.Collections.Generic;
using UnityEngine;

public class TextBoxManager : MonoBehaviour
{
	public GameObject TextPanel;
	public TMPro.TextMeshProUGUI MessageText;

	public static List<string> MessageOptionsList = new()
	{
		"Well that didn't go so well. Remember to actually dodge next time!",
		"Isn't the idea not to get hit? One might think you actually like getting pulled back to this dump.",
		"Hey! Don't forget your half of the rent is due! Would be nice to get it before you disappear Up Top.",
		"Man you're lucky we work in a weapons factory. No way you'd be able to steal such a nice sword if you worked in the agricultural fields.",
		"Annnd your back - don't tell me you missed your dear old roommate! ... no? ... ah well never mind then.",
		"You almost had it that time... or not. I don't know, I'm not really keeping track here."
	};

	// Start is called before the first frame update
	private void Start()
	{
		// Ensure the panel is hidden at the start
		if (TextPanel != null)
		{
			TextPanel.SetActive(false);
		}
		// Ensure the text is hidden at the start
		if (MessageText != null)
		{
			MessageText.gameObject.SetActive(false);
		}

		if (BetterSingleton<GameplayLoop>.Instance.DeathCounter == 0)
		{
			MessageText.text = "Don't forget - left click to attack and right click to attack *hard*";
		}
		else
		{
			// Select message from list at random
			string randomMessage = MessageOptionsList.PickRandom();
			MessageText.text = randomMessage;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.TryGetComponent(out PlayerController _))
		{
			TextPanel.gameObject.SetActive(true); // Make the panel visible
			MessageText.gameObject.SetActive(true); // Make the text visible
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.TryGetComponent(out PlayerController _))
		{
			TextPanel.SetActive(false);  // Make the panel not visible
			MessageText.gameObject.SetActive(false); // Make the text not visible
		}
	}
}