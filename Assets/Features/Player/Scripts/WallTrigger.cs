using UnityEngine;
using UnityEngine.SceneManagement;

public class WallTrigger : MonoBehaviour
{
    public string newSceneName; // Имя сцены для загрузки

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Объект столкнулся с: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Сработал триггер со стеной! Загружаю сцену...");
            SceneManager.LoadScene(newSceneName);
        }
    }


}
