using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayLevel : MonoBehaviour
{
	public TMPro.TextMeshProUGUI LevelText;

    // Start is called before the first frame update
    void Start()
    {
        int LevelNum = BetterSingleton<GameplayLoop>.Instance.Level + 1;
		LevelText.text = "Level " + LevelNum;
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
