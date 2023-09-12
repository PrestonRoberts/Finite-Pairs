using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    public void PlayAgain()
    {
        // Start a new game
        gameManager.NewGame();
    }

    public void MainMenu()
    {
        // Load the main menu scene
        SceneManager.LoadScene("MainMenu");
    }
}
