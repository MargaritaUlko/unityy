using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BgInf : MonoBehaviour
{
    public float scrollspeed = 0.5f;
    private Renderer quadRenderer;
    private Transform playerTransform;
    void Start()
    {
        quadRenderer = GetComponent<Renderer>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    private void Update()
    {
        float offset = playerTransform.position.x * scrollspeed;
        Vector2 newOffset = new Vector2(offset, 0);
        quadRenderer.material.mainTextureOffset = newOffset;
    }
}
 