using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI centerScreenMessage;

    [SerializeField] private Image blackScreen;
    [SerializeField] private float cutToBlackTime = 2.0f;


    private void Start()
    {
        centerScreenMessage.gameObject.SetActive(false);
        blackScreen.gameObject.SetActive(false);
    }

    public void SetPreGameUI()
    {
        centerScreenMessage.gameObject.SetActive(true);
        centerScreenMessage.text = "Press Space To Start";
    }

    public void OnGameStart(string message)
    {
        StartCoroutine(CutToBlackCo(message));
    }

    private IEnumerator CutToBlackCo(string message)
    {
        centerScreenMessage.gameObject.SetActive(false);
        blackScreen.gameObject.SetActive(true);

        yield return new WaitForSeconds(cutToBlackTime * 0.2f);

        centerScreenMessage.gameObject.SetActive(true);
        centerScreenMessage.text = message;

        yield return new WaitForSeconds(cutToBlackTime * 0.8f);

        centerScreenMessage.gameObject.SetActive(false);
        blackScreen.gameObject.SetActive(false);
    }

    public void OnGameEnd(string message)
    {
        centerScreenMessage.text = message;
        centerScreenMessage.gameObject.SetActive(true);
    }
}
