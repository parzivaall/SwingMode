using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject gameOverPanel;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI scoreText;
    public AudioSource musicSource;
    public Button restartButton; 
    private float score;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f;
        healthText.text = "HP: 5";
        scoreText.text = "Score: 0";
        musicSource.Play();
        restartButton.onClick.AddListener(RestartGame);
    }
    
    void Update()
    {
        score += Time.deltaTime;
        scoreText.text = "Score: " + (int)score;
    }
    
    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        musicSource.Stop();
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UpdateHealth(int health)
    {
        healthText.text = "HP: " + health;
    }
}

