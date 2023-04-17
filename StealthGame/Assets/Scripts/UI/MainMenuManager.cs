using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private List<GameObject> gamemodePanels;
    [SerializeField] private GameObject gamemodeButtons;
    private GameObject currentPanel;
    private int currentGamemodeIndex = 0;

    private void Start()
    {
        currentPanel = mainMenuPanel;
    }

    public void BackToMainMenu()
    {
        mainMenuPanel.SetActive(true);
        currentPanel.SetActive(false);
        gamemodeButtons.SetActive(false);
    }
    
    // Start is called before the first frame update
    public void ToGamemodeSelection()
    {
        mainMenuPanel.SetActive(false);
        gamemodePanels[currentGamemodeIndex].SetActive(true);
        currentPanel = gamemodePanels[currentGamemodeIndex];
        gamemodeButtons.SetActive(true);
    }

    // Update is called once per frame
    public void SwipeRight()
    {
        Debug.Log("click");
        gamemodePanels[currentGamemodeIndex].SetActive(false);
        if (currentGamemodeIndex + 1 >= gamemodePanels.Count)
            currentGamemodeIndex = -1;
        gamemodePanels[currentGamemodeIndex + 1].SetActive(true);
        currentGamemodeIndex++;
        currentPanel = gamemodePanels[currentGamemodeIndex];
    }

    public void SwipeLeft()
    {
        gamemodePanels[currentGamemodeIndex].SetActive(false);
        if (currentGamemodeIndex - 1 < 0)
            currentGamemodeIndex = gamemodePanels.Count;
        gamemodePanels[currentGamemodeIndex - 1].SetActive(true);
        currentGamemodeIndex--;
        currentPanel = gamemodePanels[currentGamemodeIndex];
    }
}
