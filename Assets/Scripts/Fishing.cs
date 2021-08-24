using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishing : MonoBehaviour
{
    #region Singleton
    public static Fishing Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
    #endregion

    [Header("Throw")]
    public GameObject prefabLure;
    public GameObject throwStart;
    [Space]
    public GameObject ThrowField;
    public float Distance;
    public float Speed;
    public float Target;
    public float MaxForce;
    public float ForceMultiplier;
    [Header("Fisherman")]
    public Animator Fisherman;
    public float Reel;
    public bool Draw;
    public bool Cast;
    public bool Cought;
    [Header("Cameras")]
    public Camera boatCam;
    public Camera lureCam;
    public float splitTime;
    public float SplitAmount;

    float throwForce;
    bool throwing;
    bool fishing;
    GameObject point;
    float throwPoint = 0;
    Coroutine CoThrow;
    Coroutine CoSpace;
    Coroutine CoSplit;

    private void Start()
    {
        ThrowField.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && CoSpace == null && fishing == false)
        {
            CoSpace = StartCoroutine(ThrowCommand());
        }
    }
    IEnumerator ThrowCommand()
    {
        if (CoThrow != null)
        {
            StartCoroutine(triggerAnimation("Cast"));
            fishing = true;
            throwing = false;
           // CoSplit = StartCoroutine(splitCam(true));
        }
        else
        {
            StartCoroutine(triggerAnimation("Draw"));
            yield return new WaitForSeconds(0.5f);
            CoThrow = StartCoroutine(ThrowStrength());
        }
        CoSpace = null;
    }
    IEnumerator ThrowStrength()
    {
        throwPoint = Random.Range(-Distance, Distance);
        GameObject point = ThrowField.transform.GetChild(1).gameObject;
        ThrowField.SetActive(true);
        bool increasing = Random.Range(0,2).Equals(1);

        throwing = true;
        while (throwing)
        {
            if (increasing)
            {
                if (throwPoint <= Distance)
                    throwPoint += Speed * Time.deltaTime;
                else
                    increasing = false;
            }
            else
            {
                if (throwPoint >= -Distance)
                    throwPoint -= Speed * Time.deltaTime;
                else
                    increasing = true;
            }
            Vector3 pos = point.transform.position;
            point.transform.position = new Vector3(throwPoint, pos.y, pos.z);
            yield return new WaitForEndOfFrame();
        }
        if (throwPoint > -(Distance * Target) && throwPoint < (Distance * Target))
            throwForce = (Distance * 1.5f);
        else
            throwForce = (Distance - Mathf.Abs(throwPoint));

        GameObject Thrown = Instantiate(prefabLure, throwStart.transform.position, Quaternion.identity);
        Thrown.GetComponent<LureControl>().ReturnPoint = Fisherman.transform.GetChild(0);
        Thrown.GetComponent<Rigidbody2D>().velocity = (new Vector3(-1, 1, 0) * throwForce * ForceMultiplier);
        ThrowField.SetActive(false);
        CoThrow = null;
    }
    public void Strain(float strain)
    {
        Reel = strain;
        Fisherman.SetInteger("Reel", Mathf.RoundToInt(Reel));
    }
    public void Catch()
    {
        StartCoroutine(triggerAnimation("Cought"));
        //CoSplit = StartCoroutine(splitCam(false));
        fishing = false;
    }
    IEnumerator triggerAnimation(string animation)
    {
        Fisherman.SetBool(animation, true);
        yield return new WaitForSeconds(0.1f);
        Fisherman.SetBool(animation, false);
    }
}
