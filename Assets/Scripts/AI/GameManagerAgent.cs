using TMPro;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManagerAgent : MonoBehaviour
{
    public static GameManagerAgent Instance { get; private set; }

    public float initialGameSpeed = 5f;
    public float gameSpeedIncrease = 0.1f;
    public float gameSpeed { get; private set; }

    public PlayerAgent player;
    public Spawner spawner;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hiscoreText;

    private float score;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        score = 0f;
        gameSpeed = initialGameSpeed;
        spawner.gameObject.SetActive(true);

        destroyObstacles();
        UpdateHiscore();
    }

    private void destroyObstacles()
    {
        ObstacleAgent[] obstacles = FindObjectsOfType<ObstacleAgent>();

        foreach (var obstacle in obstacles) {
            Destroy(obstacle.gameObject);
        }
    }

    public void GameOver()
    {
        gameSpeed = 0f;
        spawner.gameObject.SetActive(false);

        player.EndEpisode();
        UpdateHiscore();
        NewGame();
    }

    private void Update()
    {
        gameSpeed += gameSpeedIncrease * Time.deltaTime;
        score += gameSpeed * Time.deltaTime;
        scoreText.text = Mathf.FloorToInt(score).ToString("D5");
    }

    private void UpdateHiscore()
    {
        float hiscore = PlayerPrefs.GetFloat("hiscore_ai", 0);

        if (score > hiscore)
        {
            hiscore = score;
            PlayerPrefs.SetFloat("hiscore_ai", hiscore);
        }

        hiscoreText.text = Mathf.FloorToInt(hiscore).ToString("D5");
    }
}
