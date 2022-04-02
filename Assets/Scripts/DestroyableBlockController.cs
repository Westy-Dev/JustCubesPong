using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableBlockController : MonoBehaviour
{
    [SerializeField] GameObject explosionObject;

    [SerializeField] Vector2 xGrid;
    [SerializeField] Vector2 yGrid;

    [SerializeField] int hitCountToDisable = 2;
    private int hitCount = 0;

    private float timePassed = 0f;

    [SerializeField]
    private BallController ballController;

    [SerializeField]
    private GameObject ball;

    [SerializeField]
    private AudioClip spriteHit;
    [SerializeField]
    private AudioClip spriteDestroyed;



    private Color initialColor;
    private AudioSource audioSource;
    private void Start()
    {
        AssignRandomPosition();
        initialColor = GetComponent<Renderer>().material.color;
        audioSource = Camera.main.GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            audioSource.Stop();
            audioSource.clip = spriteHit;
            audioSource.Play();
            hitCount++;

            GetComponent<Renderer>().material.color = Color.red;

            if (hitCount >= hitCountToDisable)
            {
                audioSource.Stop();
                audioSource.clip = spriteDestroyed;
                audioSource.Play();
                hitCount = 0;
                UIManager.Instance.AddPlayerScore(Random.Range(10,101));
                GameObject tempExplosionObject =Instantiate(explosionObject, transform.position, Quaternion.identity);
                Destroy(tempExplosionObject,2f);
                AssignRandomPosition();
                timePassed = 0;
                GetComponent<Renderer>().material.color = initialColor;
            }
        }
    }

    private void Update()
    {
        if (ballController.initFlag)
        {
            timePassed += Time.deltaTime;
            if (timePassed > 10f)
            {
                AssignRandomPosition();
                timePassed = 0;
            }
        }
    }

    void AssignRandomPosition()
    {
        transform.position = new Vector2(Random.Range(xGrid.x, xGrid.y), Random.Range(yGrid.x, yGrid.y));
        if (ball != null)
        {
            if (Vector2.Distance(transform.position, ball.transform.position) < 1)
            {
                AssignRandomPosition();
            }
        }
    }
}
