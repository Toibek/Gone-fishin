using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [Header("Idle")]
    public Vector2 speed;
    public Vector2 turnFrequency;
    public float lureSense;
    [Header("Approach")]
    public float approachSpeed;
    [Header("Hooked")]
    Coroutine active;
    Transform lure;
    void Start()
    {
        active = StartCoroutine(idle());
    }
    IEnumerator idle()
    {
        float t = Random.Range(turnFrequency.x,turnFrequency.y);
        Vector2 currentDirection = new Vector2(Random.Range(speed.x, speed.y), Random.Range(speed.x,speed.y));
        if (currentDirection.x > 0)
            GetComponent<SpriteRenderer>().flipX = false;
        else
            GetComponent<SpriteRenderer>().flipX = true;

        while (true)
        {
            while (t > 0)
            {
                transform.position += (Vector3)currentDirection * Time.deltaTime;
                t -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
                if(lure == null && GameObject.FindGameObjectWithTag("Lure") != null)
                    lure = GameObject.FindGameObjectWithTag("Lure").transform;
                else if (lure != null && Vector2.Distance(transform.position,lure.position) < lureSense)
                {
                    active = StartCoroutine(approach());
                    yield break;
                }
            }
            t = Random.Range(turnFrequency.x, turnFrequency.y);
            currentDirection = new Vector2(Random.Range(speed.x, speed.y), Random.Range(speed.x, speed.y));
            if (currentDirection.x > 0)
                GetComponent<SpriteRenderer>().flipX = false;
            else
                GetComponent<SpriteRenderer>().flipX = true;
        }
    }
    IEnumerator approach()
    {
        while(lure != null && Vector2.Distance(transform.position,lure.position) < lureSense)
        {
            transform.position = Vector2.MoveTowards(transform.position, lure.position, approachSpeed * Time.deltaTime);
            if (lure.tag != "Lure")
            {
                lure = null;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        active = StartCoroutine(idle());
    }
    IEnumerator hooked()
    {
        transform.parent = lure;
        transform.localPosition = Vector3.zero;
        GetComponent<SpriteRenderer>().flipX = false;
        transform.rotation = lure.rotation;
        GetComponent<Collider2D>().isTrigger = true;
        Destroy(GetComponent<Rigidbody2D>());
        StopAllCoroutines();
        lure.tag = "Untagged";
        Destroy(this);
        yield return new WaitForEndOfFrame();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Water")
        {
            GetComponent<Rigidbody2D>().gravityScale = 0;
        }
        else if (collision.tag == "Lure")
        {
            StopCoroutine(active);
            active = StartCoroutine(hooked());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Water")
        {
            GetComponent<Rigidbody2D>().gravityScale = 1;
        }
    }
}
