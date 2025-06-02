using UnityEngine;
using UnityEngine.SceneManagement;

public class WallTrigger : MonoBehaviour
{
    public string newSceneName; // ��� ����� ��� ��������

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("������ ���������� �: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("�������� ������� �� ������! �������� �����...");
            SceneManager.LoadScene(newSceneName);
        }
    }


}
