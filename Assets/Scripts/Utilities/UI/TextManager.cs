using UnityEngine;

public class TextManager : MonoBehaviour
{
	public TMPro.TextMeshProUGUI InstructionText;

	// Start is called before the first frame update
	private void Start()
	{
		string Jump = Settings.CurrentSettings.Jump.ToString();
		string Dodge = Settings.CurrentSettings.Dodge.ToString();
		InstructionText.text = "[" + Jump + "] to Jump          [" + Dodge + "] to Dodge";
	}
}