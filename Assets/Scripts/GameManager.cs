using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Refrence to game objects
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject startButton;
    [SerializeField] private TMP_Text livesText;

    // Shared UI
    [SerializeField] private GameObject UI;
    [SerializeField] private GameObject rules;
    [SerializeField] private GameObject gameEndOptions;

    // Game Over UI
    [SerializeField] private GameObject gameOverScreen;

    // Game Won UI
    [SerializeField] private GameObject gameWonScreen;

    // Canvas Transform
    RectTransform canvasTransform;

    // Game variables
    private int currentWidth;
    private int maxValue;
    private bool isPlaying = false;
    private Tile[] allTiles;
    private Tile currentTile;
    private Tile clickedTile;
    private int lives;
    private int tileCount;
    private bool isChecking;

    // Random
    private readonly System.Random random = new System.Random();

    void Start()
    {
        // Hide UI
        UI.SetActive(false);

        // Show Rules
        rules.SetActive(true);

        // Get canvas transform
        canvasTransform = gameCanvas.GetComponent<RectTransform>();
    }

    public void NewGame()
    {
        // Hide Rules
        rules.SetActive(false);

        // Show UI
        UI.SetActive(true);

        // Hide the game over screen
        gameOverScreen.SetActive(false);

        // Hide the game won screen
        gameWonScreen.SetActive(false);

        // Hide the game end options
        gameEndOptions.SetActive(false);

        // Set current width to 2
        currentWidth = 2;

        // Set currentTile to null
        currentTile = null;

        // Set lives to 3
        lives = 3;

        // Update lives text
        livesText.text = $"Lives: {lives}";

        // Set isChecking to false
        isChecking = false;

        NewRound();
    }

    private void NewRound ()
    {
        // Calculate max value
        maxValue = (currentWidth * currentWidth) / 2;

        // Set canvas width and height to currentSize * 100
        int size = currentWidth * 100;
        canvasTransform.sizeDelta = new Vector2(size, size);

        // Show the start button and set its text to "Start"
        startButton.gameObject.SetActive(true);
        startButton.GetComponentInChildren<TMP_Text>().text = "Start";

        GenerateTiles();
    }

    private void NextRound ()
    {
        // Increase current width by 2
        currentWidth += 2;

        // If current width is greater than 10, the player has won
        if (currentWidth > 10)
        {
            // Set isPlaying to false
            isPlaying = false;

            // Show the game won screen
            gameWonScreen.SetActive(true);

            // Show the game end options
            gameEndOptions.SetActive(true);

            // Return
            return;
        }       

        // Start the next round
        NewRound();
    }

    // Creates the tiles and places them randomly
    private void GenerateTiles()
    {
        // Destroy all of the tiles
        if (allTiles != null)
        {
            foreach (Tile tile in allTiles)
            {
                Destroy(tile.gameObject);
            }
        }

        allTiles = new Tile[maxValue * 2];

        // Create an array of numbers from 1 to current size with each number showing up twice
        int[] tileNumbers = Enumerable.Range(1, maxValue).SelectMany(n => new[] { n, n }).ToArray();

        // Randomly shuffle the array
        ShuffleArray(tileNumbers);

        // Create all of the tiles setting its value to a number in the array and making it a child of the canvas
        for (int i = 0; i < tileNumbers.Length; i++)
        {
            // Create a new tile
            GameObject newTileObject = Instantiate(tilePrefab, canvasTransform);

            // Set the tile's value
            Tile newTile = newTileObject.GetComponent<Tile>();
            newTile.SetValue(tileNumbers[i]);

            // Set the Tile's onclick function to TileClick
            Button tileButton = newTileObject.GetComponent<Button>();
            tileButton.onClick.AddListener(TileClick);

            // Add the tile to the allTiles array
            allTiles[i] = newTile;
        }

        // Set tile count to the length of the allTiles array
        tileCount = allTiles.Length;
    }

    // Start the game
    public void StartGame()
    {
        // Set isPlaying to true
        isPlaying = true;

        // Hide the start button
        startButton.gameObject.SetActive(false);

        // Hide all of the tiles
        foreach (Tile tile in allTiles)
        {
            tile.HideValue();
        }
    }

    // Handle tile clicks
    public void TileClick()
    {
        // If the game is not playing or is checking return
        if (!isPlaying || isChecking) return;

        // Get the tile that was clicked
        clickedTile = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Tile>();

        // If the current tile is null set the current tile to this tile, show the value, and return
        if (currentTile == null)
        {
            currentTile = clickedTile;
            currentTile.ShowValue();
            return;
        }

        // Show clicked tile
        clickedTile.ShowValue();

        // Set isChecking to true
        isChecking = true;

        // Check Tiles
        Invoke(nameof(CheckTiles), 0.2f);
    }

    private void CheckTiles()
    {
        // If the tile's value is the current tile value
        if (clickedTile.GetValue() == currentTile.GetValue())
        {
            // Hide both tiles
            clickedTile.HideTile();
            currentTile.HideTile();

            // Set current tile to null
            currentTile = null;

            // Subtract 2 from tile count
            tileCount -= 2;

            // Check if the round is over
            CheckRoundOver();

            // Set isChecking to false
            isChecking = false;

            return;
        }

        // Decrease lives by 1
        lives--;

        // Update lives text
        livesText.text = $"Lives: {lives}";

        bool isGameOver = CheckGameOver();

        if (!isGameOver)
        {
            isChecking = false;

            // If the tile's value is not the current tile value, show all of the tiles
            foreach (Tile tile in allTiles)
            {
                tile.ShowValue();
            }

            // Show the start button and set it's text to "continue"
            startButton.SetActive(true);
            startButton.GetComponentInChildren<TMP_Text>().text = "Continue";

            // Set isPlaying to false
            isPlaying = false;

            // Set current tile to null
            currentTile = null;
        }
    }

    private void CheckRoundOver()
    {
        // If there are no more tiles, start the next round
        if (tileCount == 0)
        {
            NextRound();
        }
    }

    private bool CheckGameOver()
    {
         // If there are no more lives, show the game over screen
        if (lives == 0)
        {
            // Set isPlaying to false
            isPlaying = false;

            // Show the game over screen
            gameOverScreen.SetActive(true);

            // Show game end options
            gameEndOptions.SetActive(true);

            return true;
        }

        return false;
    }

    // Fisher-Yates shuffle algorithm
    private void ShuffleArray(int[] array)
    {
        int n = array.Length;
        for (int i = n - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
}
