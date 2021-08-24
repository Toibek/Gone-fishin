using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LureControl : MonoBehaviour
{
    internal Transform ReturnPoint;
    [Header("Settings")]
    public float reelSpeed;
    public float awaySpeed;
    public float approachSpeed;
    [Header("Physics")]
    public float watVelo;
    public float regVelo;
    public float watGrav;
    public float regGrav;
    [Header("Strain")]
    public float strain;
    public float reelStrain;
    public float looseStrain;
    public float maxStrain;


    float spawnTime;
    Rigidbody2D rb;
    bool reeling;
    bool submerged;
    // Start is called before the first frame update
    void Start()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        spawnTime = Time.timeSinceLevelLoad;
        rb.gravityScale = regGrav;
    }
    private void FixedUpdate()
    {
        Vector3 difference = ReturnPoint.position - transform.position;
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);

        if (Input.GetKey(KeyCode.W))
            Reel();
        else
            strain = Mathf.Clamp(strain - looseStrain * Time.deltaTime, 0, maxStrain);

        
        if (Input.GetKey(KeyCode.D))
            strafe(true);
        else if (Input.GetKey(KeyCode.A))
            strafe(false);


        if (submerged)
        {
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, watVelo);
            Fishing.Instance.Strain(strain);
        }
        else
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, regVelo);
    }
    void Reel()
    {
        if (!reeling && submerged)
        {
            reeling = true;
            rb.gravityScale = watGrav / 2;
        }

        Vector2 dir = (Vector2)Vector3.Normalize(ReturnPoint.position - transform.position);
        strain = Mathf.Clamp(strain + reelStrain * Time.deltaTime,0,maxStrain);
        rb.AddForce(dir * reelSpeed);
    }
    void strafe(bool right)
    {
        float dir = -1;
        if (right)
            dir = 1;
        if (Mathf.Sign(dir) == Mathf.Sign(Vector2.Distance(transform.position, ReturnPoint.position)))
            rb.AddForce(new Vector2(approachSpeed * dir, 0));
        else
            rb.AddForce(new Vector2(awaySpeed * dir, 0));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Water")
        {
            submerged = true;
            rb.gravityScale = watGrav;
        }
        else if (collision.tag == "Player" && Time.timeSinceLevelLoad - spawnTime > 2f)
        {
            Fishing.Instance.Catch();
            submerged = false;
            Fishing.Instance.Strain(0);
            Destroy(gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Water")
        {
            submerged = false;
            rb.gravityScale = regGrav;
        }
    }
}
