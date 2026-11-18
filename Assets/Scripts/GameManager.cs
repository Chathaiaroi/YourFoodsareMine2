using UnityEngine;
using UnityEngine.SceneManagement;

// Singleton: มีตัวคุมเกมตัวเดียวต่อฉาก เรียกใช้ได้ทุกที่ด้วย GameManager.Instance
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private Level level;

    [Header("UI Panels (ไม่ใส่ก็ได้)")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject levelCompletePanel;

    [Header("Next Level")]
    [SerializeField] private string nextSceneName;   // เว้นว่าง = ด่านสุดท้าย

    private bool isGameOver;
    private bool isLevelComplete;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;
        SetPanel(gameOverPanel, false);
        SetPanel(levelCompletePanel, false);

        // Observer: GameManager สมัครรับ event จาก Cat และ Level
        if (player != null)
        {
            player.OnCaught += GameOver;
        }
        else
        {
            Debug.LogWarning("GameManager: ยังไม่ได้ใส่ Cat");
        }

        if (level != null)
        {
            level.OnLevelCompleted += LevelComplete;
        }
        else
        {
            Debug.LogWarning("GameManager: ยังไม่ได้ใส่ Level");
        }
    }

    void OnDestroy()
    {
        if (player != null)
        {
            player.OnCaught -= GameOver;
        }
        if (level != null)
        {
            level.OnLevelCompleted -= LevelComplete;
        }
        if (Instance == this)
        {
            Instance = null;
        }
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }

        if (isLevelComplete && Input.GetKeyDown(KeyCode.Return))
        {
            LoadLevel(nextSceneName);
        }
    }

    public void GameOver()
    {
        if (isGameOver || isLevelComplete) return;

        isGameOver = true;
        Debug.Log("Game Over");
        Time.timeScale = 0f;   // หยุดเกม (Update ของ GameManager ยังทำงานอยู่)
        SetPanel(gameOverPanel, true);
    }

    public void LevelComplete()
    {
        if (isGameOver || isLevelComplete) return;

        isLevelComplete = true;
        Debug.Log("Level Complete");
        Time.timeScale = 0f;
        SetPanel(levelCompletePanel, true);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadLevel(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.Log("Last Meal!");
            return;
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    private void SetPanel(GameObject panel, bool active)
    {
        if (panel != null)
        {
            panel.SetActive(active);
        }
    }
}
