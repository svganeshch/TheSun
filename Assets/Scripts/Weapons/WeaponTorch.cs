using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class TorchEvent : UnityEvent<float> {}

public class WeaponTorch : MonoBehaviour
{
    private Transform wandOrigPos;

    private Rigidbody2D wandRB;
    private GameObject torch;

    private bool isThrown = false;
    private bool isGrab = false;

    private Vector3 mousePos;
    private Vector3 mousePosDir;

    private float torchCharge = 1000;
    private float defaultTorchCharge;
    private float dischargeTime = 3;
    private float currentTorchTime;
    public float throwSpeed = 400;
    public float grabSpeed = 400;

    private Light2D torchIntensity;
    private float defaultTorchIntensity;
    public float defaultPercentageDown = 5f;

    private TorchEvent torchEvent;
    private UIManager uimanager;

    private MyInputActions inputActions;
    private InputAction enableTorchAction;
    private InputAction throwGrabAction;
    private InputAction mousePositionAction;

    public float TorchCharge { get { return torchCharge; }  }

    private void Awake()
    {
        wandOrigPos = transform.parent.transform;

        wandRB = GetComponent<Rigidbody2D>();
        torch = GameObject.Find("torch");
        torchIntensity = torch.GetComponent<Light2D>();
        uimanager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        inputActions= new MyInputActions();
    }

    public void Start()
    {
        defaultTorchCharge = torchCharge;
        defaultTorchIntensity = torchIntensity.intensity;

        torchEvent ??= new TorchEvent();
        torchEvent.AddListener(uimanager.SetTorchBar);
    }

    public void Update()
    {
        mousePos = mousePositionAction.ReadValue<Vector2>();

        if (!isThrown)
            SetWandDirection();

        TrackTorchCharge();
    }

    private void FixedUpdate()
    {
        if (isThrown && !isGrab)
        {
            ThrowWand();
        }
        else if (isThrown && isGrab)
        {
            GrabWand();
        }
    }

    public void ThrowGrabAction(InputAction.CallbackContext context)
    {
        Debug.Log("clicked throw grab");
        if (!isGrab && !isThrown)
        {
            isThrown = true;
            Debug.Log("clicked : Throw");
        }
        else if (isThrown)
        {
            isGrab = true;
            Debug.Log("clicked : Grab");
        }
    }

    public void TorchEnableAction(InputAction.CallbackContext context)
    {
        torch.SetActive(!torch.activeSelf);
        Debug.Log("clicked : torch enable");
    }

    void SetWandDirection()
    {
        mousePosDir = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 mouseDir = (mousePosDir - transform.position).normalized;

        float mouseAngle = Vector2.SignedAngle(Vector2.up, mouseDir);
        transform.eulerAngles = new Vector3(0, 0, mouseAngle);
    }

    void ThrowWand()
    {
        wandRB.isKinematic = false;
        isThrown = true;
        isGrab = false;
        transform.parent = null;

        Vector3 throwTarget = transform.position + (mousePosDir - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, throwTarget, throwSpeed * Time.fixedDeltaTime);

        Debug.Log("torch thrown!!");
    }

    void GrabWand()
    {
        transform.position = Vector2.MoveTowards(transform.position, wandOrigPos.position, grabSpeed * Time.fixedDeltaTime);
        Debug.Log("grabbing torch");

        if (transform.position == wandOrigPos.position)
        {
            wandRB.isKinematic = true;
            isThrown = false;
            isGrab = false;
            transform.parent = wandOrigPos;
        }
    }

    void TrackTorchCharge()
    {
        if (torch.activeSelf && torchCharge > 0)
        {
            currentTorchTime += Time.deltaTime;
            if (currentTorchTime > dischargeTime)
            {
                torchCharge -= GetTorchPercentage(defaultPercentageDown);
                float currentIntensity = torchIntensity.intensity - Mathf.Abs(GetTorchIntensityPercentage(GetTorchPercentage(defaultPercentageDown)));
                torchIntensity.intensity = currentIntensity < 0 ? 0 : currentIntensity;
                torchCharge = torchIntensity.intensity <= 0 ? 0 : torchCharge;
                
                currentTorchTime = 0;
            }
        }

        if (torchCharge >= 0)
        {
            torchEvent.Invoke(torchCharge / defaultTorchCharge);
            Debug.Log("Torch charge : " + torchCharge);
        }
    }

    float GetTorchPercentage(float percentageDown)
    {
        return (defaultTorchCharge / 100) * percentageDown;
    }

    float GetTorchIntensityPercentage(float percentageDown)
    {
        return (percentageDown * defaultTorchIntensity) / defaultTorchCharge;
    }

    public void ChargeTorch(float chargeAmount)
    {
        Debug.Log("charging torch");
        if (torchCharge < defaultTorchCharge)
        {
            torchCharge += chargeAmount;

            float currentIntensity = torchIntensity.intensity + Mathf.Abs(GetTorchIntensityPercentage(GetTorchPercentage(defaultPercentageDown)));
            torchIntensity.intensity = currentIntensity > defaultTorchIntensity ? defaultTorchIntensity : currentIntensity;

            if (torchCharge > defaultTorchCharge) torchCharge = defaultTorchCharge;
        }
    }

    private void OnEnable()
    {
        inputActions.Enable();

        throwGrabAction = inputActions.Player.ThrowGrab;
        throwGrabAction.Enable();
        throwGrabAction.performed += ThrowGrabAction;

        enableTorchAction = inputActions.Player.EnableTorch;
        enableTorchAction.Enable();
        enableTorchAction.performed += TorchEnableAction;

        mousePositionAction = inputActions.Player.MousePosition;
        mousePositionAction.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
        throwGrabAction.Disable();
        enableTorchAction.Disable();
        mousePositionAction.Disable();
    }
}
