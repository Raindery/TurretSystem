using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private float _gameOverDuration;

    private void Awake()
    {
        PlayerEvents.CharacterDead.AddListener(GameOver);
    }

    private void OnDestroy()
    {
        PlayerEvents.CharacterDead.RemoveListener(GameOver);
    }

    private void GameOver()
    {
        StartCoroutine(GameOverCoroutine());
    }

    private IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSeconds(_gameOverDuration);
        SceneManager.LoadSceneAsync(0);
    }
}
