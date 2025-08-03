using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class TextBoxManager : MonoBehaviour
{
    public GameObject TextPanel;
    public TMPro.TextMeshProUGUI MessageText; 

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the panel is hidden at the start
        if (TextPanel != null)
        {
            TextPanel.gameObject.SetActive(false); 
        }
        // Ensure the text is hidden at the start
        if (MessageText != null)
        {
            MessageText.gameObject.SetActive(false); 
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.TryGetComponent(out PlayerController _))
			TextPanel.gameObject.SetActive(true); // Make the panel visible
			MessageText.gameObject.SetActive(true); // Make the text visible
	}

    private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.TryGetComponent(out PlayerController _))
			TextPanel.gameObject.SetActive(false);  // Make the panel not visible
			MessageText.gameObject.SetActive(false); // Make the text not visible
	}

}
