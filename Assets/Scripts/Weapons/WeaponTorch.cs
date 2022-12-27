using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;


[System.Serializable]
public class TorchEvent : UnityEvent<float>
{
}

public class WeaponTorch : MonoBehaviour
{
    public GameObject player;
    private GameObject wand;
    private Rigidbody2D wandRB;
    private GameObject torch;
    public Transform wandOrigPos;

    public bool isThrown = false;
    public bool isGrab = false;

    private Vector3 mousePos;

    private float torchCharge = 1000;
    private float defaultTorchCharge;
    private float dischargeTime = 3;

    public Coroutine torchCoroutine;
    public Light2D torchIntensity;
    public float defaultTorchIntensity;
    public float defaultPercentageDown = 10;

    public TorchEvent torchEvent;
    public UIManager uimanager;

    public void Start()
    {
        wand = GameObject.Find("wand");
        wandRB = wand.GetComponent<Rigidbody2D>();
        torch = GameObject.Find("torch");
        torchIntensity = torch.GetComponent<Light2D>();
        defaultTorchCharge = torchCharge;
        defaultTorchIntensity = torchIntensity.intensity;

        if (torchEvent == null)
            torchEvent = new TorchEvent();
        torchEvent.AddListener(uimanager.SetTorchBar);
    }

    void Update()
    {
        SetTorchEnable();

        if (!isThrown)
            SetWandPos();

        if (Input.GetMouseButtonDown(1) && !isGrab && !isThrown)
        {
            isThrown = true;
        }
        else if (Input.GetMouseButtonDown(1) && isThrown)
        {
            isGrab = true;
        }
        else if (isThrown && !isGrab)
        {
            ThrowWand();
        }
        else if (isThrown && isGrab)
        {
            GrabWand();
        }

         TrackTorchCharge();
    }

    void SetTorchEnable()
    {
        if (Input.GetMouseButtonDown(0))
        {
            torch.SetActive(!torch.activeSelf);
        }
    }

    void SetWandPos()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseDir = (mousePos - wand.transform.position).normalized;

        float mouseAngle = Vector2.SignedAngle(Vector2.up, mouseDir);
        wand.transform.eulerAngles = new Vector3(0, 0, mouseAngle);
    }

    void ThrowWand()
    {
        wandRB.isKinematic = false;
        isThrown = true;
        isGrab = false;
        transform.parent = null;

        wand.transform.position = Vector2.MoveTowards(wand.transform.position, wand.transform.position + (mousePos - wand.transform.position).normalized, 5f * Time.deltaTime);

        Debug.Log("torch thrown!!");
    }

    void GrabWand()
    {
        wand.transform.position = Vector2.MoveTowards(wand.transform.position, wandOrigPos.position, 10f * Time.deltaTime);
        Debug.Log("grabbing torch");

        if (wand.transform.position == wandOrigPos.position)
        {
            wandRB.isKinematic = true;
            isThrown = false;
            isGrab = false;
            transform.parent = wandOrigPos;
        }
    }

    float tmpTimer;
    void TrackTorchCharge()
    {
        if (torch.activeSelf && torchCharge > 0)
        {
            tmpTimer += Time.deltaTime;
            if (tmpTimer > dischargeTime)
            {
                torchCharge -= getTorchPercentage(defaultPercentageDown);
                torchIntensity.intensity -=  getTorchIntensityPercentage(getTorchPercentage(defaultPercentageDown));
                
                tmpTimer = 0;
                torchEvent.Invoke(torchCharge / defaultTorchCharge);
            }
            
            Debug.Log("Torch charge : " + torchCharge);
        }
    }

    float getTorchPercentage(float percentageDown)
    {
        return (defaultTorchCharge / 100) * percentageDown;
    }

    float getTorchIntensityPercentage(float percentageDown)
    {
        return (percentageDown * defaultTorchIntensity) / defaultTorchCharge;
    }

    IEnumerator DischargeTorch()
    {
        torchCharge -= 1;
        yield return new WaitForSeconds(dischargeTime);
    }
}
