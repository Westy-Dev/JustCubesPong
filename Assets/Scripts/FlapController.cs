using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlapController : MonoBehaviour
{
    [SerializeField] float flapSpeed = 3;

    [SerializeField] int hitCountToShrink = 2;
    int hitCount = 0;

    [SerializeField] KeyCode key1;
    [SerializeField] KeyCode key2;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(key1) || Input.GetKeyDown(key2))
        {
            rb.velocity=new Vector2(0,flapSpeed*Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ball"))
        {
            UIManager.Instance.AddPaddleCollisionScore();
        }
    }
}
