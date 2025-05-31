using UnityEngine;
using UnityEngine.SceneManagement;

public class WallTrigger : MonoBehaviour
{
    public string newSceneName; // ��� ����� ��� ��������

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            Debug.Log("�������� ������� �� ������! �������� �����...");
            SceneManager.LoadScene(newSceneName);
        }
    }
}
