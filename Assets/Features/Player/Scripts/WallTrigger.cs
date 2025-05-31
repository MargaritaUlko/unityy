using UnityEngine;
using UnityEngine.SceneManagement;

public class WallTrigger : MonoBehaviour
{
    public string newSceneName; // Имя сцены для загрузки

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            Debug.Log("Сработал триггер со стеной! Загружаю сцену...");
            SceneManager.LoadScene(newSceneName);
        }
    }
}
