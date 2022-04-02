using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    //white-blue-green-yellow-orange-red
    [SerializeField]
    private GameObject trail;

    [SerializeField]
    [Tooltip("Just for debugging, adds some velocity during OnEnable")]
    private Vector2 initialVelocity;

    [SerializeField]
    private float minVelocity = 10f;

    private Vector2 lastFrameVelocity;
    private Rigidbody2D rb;

    private TrailRenderer trailRenderer;

    [SerializeField] int hitCountToSpeed = 2;
    int hitCount = 0;
    [SerializeField] float speedToIncrease = 0.375f;

    [SerializeField] GameObject particleSystemObject;

    public bool initFlag = false;

    private AudioSource audioSource;

    [SerializeField]
    private AudioClip paddleHit;

    [SerializeField]
    private AudioClip wallHit;

    [SerializeField]
    private AudioClip gameOver;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trailRenderer = trail.GetComponent<TrailRenderer>();
        audioSource = Camera.main.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) &&!initFlag))
        {
            initFlag = true;
            rb.velocity = new Vector2(initialVelocity.x * (Random.Range(0, 2) == 0 ? 1 : -1), initialVelocity.y * (Random.Range(0, 2) == 0 ? 1 : -1));
        }

        lastFrameVelocity = rb.velocity;
        //Debug.Log("Ball velocity = " + lastFrameVelocity.magnitude);
        //Debug.Log("Percentage = " + (lastFrameVelocity.magnitude - 10) / (30 - 10));

        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        Color color = Color.Lerp(Color.green, Color.red, (lastFrameVelocity.magnitude - 10) / (30 - 10));
        gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
                           new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) });

        trailRenderer.colorGradient = gradient;
 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Right Wall") || collision.gameObject.CompareTag("Left Wall"))
        {
            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource.clip = gameOver;
                audioSource.Play();
            }

            UIManager.Instance.GameOver();
            Destroy(this.gameObject);
        }

        particleSystemObject.GetComponent<ParticleSystem>().Play();
        particleSystemObject.transform.position = collision.contacts[0].point;

        Bounce(collision.contacts[0].normal);

        if (collision.gameObject.CompareTag("Top Wall") || collision.gameObject.CompareTag("Bottom Wall"))
        {
            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource.clip = wallHit;
                audioSource.Play();
            }
        }

        if (collision.gameObject.CompareTag("Paddle"))
        {
            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource.clip = paddleHit;
                audioSource.Play();
            }

            hitCount++;

            if (hitCount >= hitCountToSpeed)
            {
                hitCount = 0;

                minVelocity += speedToIncrease;
            }
        }
    }

    private void Bounce(Vector2 collisionNormal)
    {
        var speed = lastFrameVelocity.magnitude;

        int randomNo = Random.Range(0, 4);

        Vector2 direction;

        if (randomNo==0)
        {
            direction = Vector2.Reflect(new Vector2(lastFrameVelocity.normalized.x - Random.Range(0, 0.1f), lastFrameVelocity.normalized.y + Random.Range(0, 0.1f)), collisionNormal);//+new Vector2(Random.Range(-0.01f,0.01f), Random.Range(-0.01f, 0.01f)));
        }
        else
        {
            direction = Vector2.Reflect(new Vector2(lastFrameVelocity.normalized.x, lastFrameVelocity.normalized.y), collisionNormal);//+new Vector2(Random.Range(-0.01f,0.01f), Random.Range(-0.01f, 0.01f)));
        }

        rb.velocity = direction * Mathf.Max(speed, minVelocity);
    }
}
