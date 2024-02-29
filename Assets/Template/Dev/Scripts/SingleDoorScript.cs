using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public enum DoorType
{
    FireRate,
    FireRange,
    FirePower,
    SkillDoor,
    BulletDoor
}
public class SingleDoorScript : MonoBehaviour
{
    [Header("Door Properties")]
    public DoorType _doorType;
    public Skills _doorSkill;
    public float amount;
    private Vector3 startScale;
    [SerializeField] List<GameObject> skillSprites = new List<GameObject>();
    [SerializeField] List<Transform> skillBalls = new List<Transform>();
    [SerializeField] List<Vector3> skillBallsStartScales = new List<Vector3>();
    [SerializeField] Transform _bulletPosition;
    [Header("Lock Properties")]
    public bool locked;
    [SerializeField] private int lockPower;
    [SerializeField] private Transform lockTriangle;
    [SerializeField] private Transform lockCanvas;
    [SerializeField] private Vector2 trianglePositions;
    private float triangleXMoveAmount;
    private float currentTrianglePosition;
    public GameObject iceDoor;
    bool iced;

    [SerializeField] private TextMeshPro amountText;
    [SerializeField] private TextMeshPro typeText;
    [SerializeField] private List<GameObject> doors = new List<GameObject>();
    private bool shaking;


    [SerializeField] private List<GameObject> lockObjects = new List<GameObject>();
    float objectThrowAmount;
    float currentObjectNumber;
    [Header("Skill Doors")]
    [SerializeField] List<TextMeshPro> _levelTexts = new List<TextMeshPro>();
    [SerializeField] GameObject _barRenderer;
    [SerializeField] List<int> powerNeededForNewLevelSkill = new List<int>();
    public SingleDoorScript otherDoor;
    [SerializeField] int startLevel;
    float currentPower;
    bool canStamina;
    [SerializeField] float staminaSpeed;
    int currentLevel;
    public GameObject _bulletInside;
    public TextMeshPro _bulletPowerTexter;


    public TextMeshPro _multiTexter;

    public Vector3 barStartScale;
    public Vector3 barFinalScale;
    public Vector3 barStartPosition;
    public Vector3 barFinalPosition;


    private void Awake()
    {
        for (int i = 0; i < skillBalls.Count; i++)
        {
            skillBallsStartScales.Add(skillBalls[i].transform.localScale);
        }
        if (_doorType == DoorType.SkillDoor || _doorType == DoorType.BulletDoor)
        {
            currentPower = powerNeededForNewLevelSkill[startLevel];
            GetHit(0);
        }
        foreach (SingleDoorScript sds in FindObjectsOfType<SingleDoorScript>())
        {
            if (sds.transform.position.z == transform.position.z)
            {
                if (sds.gameObject != this.gameObject)
                {
                    otherDoor = sds;
                }
            }
        }
        if (lockPower > 0)
        {
            triangleXMoveAmount = (trianglePositions.y - trianglePositions.x) / lockPower;
            currentTrianglePosition = trianglePositions.x;
            lockTriangle.transform.localPosition = new Vector3(currentTrianglePosition, lockTriangle.transform.localPosition.y, lockTriangle.transform.localPosition.z);
            locked = true;
        }
        else
        {
            locked = false;
            for (int i = 0; i < lockObjects.Count; i++)
            {
                lockObjects[i].SetActive(false);
            }
            lockTriangle.transform.parent.gameObject.SetActive(false);
        }
        startScale = transform.localScale;
        objectThrowAmount = (float)lockObjects.Count / (float)lockPower;
        SetDoorProperties();
    }
    public void ThrowObjects(float hitPower)
    {
        currentObjectNumber += hitPower * objectThrowAmount;
        for (int i = 0; i < currentObjectNumber; i++)
        {
            if (i < lockObjects.Count)
            {
                if (lockObjects[i].GetComponent<Rigidbody>() == null)
                {
                    lockObjects[i].AddComponent<Rigidbody>().AddForce(new Vector3(0, 0, -1) * 80 * Random.Range(0.8f, 1.3f));
                    lockObjects[i].GetComponent<Rigidbody>().AddTorque(Vector3.one * 200);
                    lockObjects[i].AddComponent<BoxCollider>();
                    lockObjects[i].transform.parent = null;
                }
            }
        }
    }
    private void SetDoorProperties()
    {
        switch (_doorType)
        {
            case DoorType.FireRate:
                typeText.text = "RATE";
                break;
            case DoorType.FireRange:
                typeText.text = "RANGE";
                break;
            case DoorType.FirePower:
                typeText.text = "POWER";
                break;
            case DoorType.SkillDoor:
                
                switch (_doorSkill)
                {
                    case Skills.BiggerBullets:
                        typeText.text = "SIZE";
                        break;
                    case Skills.Bomb:
                        typeText.text = "BOMB";
                        break;
                    case Skills.Critical:
                        typeText.text = "CRITICAL";
                        break;
                    case Skills.FireBullets:
                        typeText.text = "FIRE";
                        break;
                    case Skills.IceBullet:
                        typeText.text = "ICE";
                        break;
                    case Skills.Range:
                        typeText.text = "RANGE";
                        break;
                    case Skills.Richochet:
                        typeText.text = "RICOCHET";
                        break;
                    case Skills.MultiShoot:
                        typeText.text = "MULTI";
                        break;
                }
                break;
        }
        if (amount < 0)
        {
            amountText.text = amount.ToString("0");

        }
        else
        {
            amountText.text = "+" + amount.ToString("0");
        }
        int doorNumber;
        if (locked)
        {
            doorNumber = 2;
        }
        else
        {
            if (amount >= 0)
            {
                doorNumber = 0;
            }
            else
            {
                doorNumber = 1;
            }
            if (_doorType == DoorType.SkillDoor )
            {
                doorNumber = 3;
            }
            if(_doorType == DoorType.BulletDoor)
            {
                doorNumber = 3;
            }
        }
        for (int i = 0; i < doors.Count; i++)
        {
            if (i == doorNumber)
            {
                doors[i].SetActive(true);
            }
            else
            {
                doors[i].SetActive(false);
            }
        }
        if (_doorType == DoorType.SkillDoor)
        {
            int skillNumber = ((int)_doorSkill);
            for (int i = 0; i < skillSprites.Count; i++)
            {
                if (i == skillNumber)
                {
                    skillSprites[i].SetActive(true);
                }
                else
                {
                    skillSprites[i].SetActive(false);
                }
            }
        }
        if(_doorType ==DoorType.SkillDoor||_doorType == DoorType.BulletDoor)
        {

        }
        else
        {
            if (iced)
            {
                for(int i = 0; i < doors.Count; i++)
                {
                    doors[i].gameObject.SetActive(false);
                }
                iceDoor.SetActive(true);
            }
            else
            {
                iceDoor.SetActive(false);
            }
        }
    }
    private void LockHit()
    {
        StartCoroutine(ShakeLockCanvas());
        lockPower -= 1;
        currentTrianglePosition += triangleXMoveAmount * 1;
        lockTriangle.transform.DOLocalMoveX(currentTrianglePosition, .2f);
        ThrowObjects(1);
        if (lockPower < 0)
        {
            lockCanvas.transform.DOScale(Vector3.zero, .2f);
            locked = false;
            SetDoorProperties();
        }
    }
    private IEnumerator ShakeLockCanvas()
    {
        lockCanvas.transform.DORotate(new Vector3(0, 0, 3), .1f);
        yield return new WaitForSeconds(.1f);
        lockCanvas.transform.DORotate(new Vector3(0, 0, -3), .1f);
        yield return new WaitForSeconds(.1f);
        lockCanvas.transform.DORotate(new Vector3(0, 0, 0), .1f);
    }
    private void IncreaseAmount(float bulletPower)
    {
        float beforeAmount = amount;
        amount += bulletPower;
        SetDoorProperties();
        if (!shaking)
        {
            StartCoroutine(Shake());
            shaking = true;
        }
    }
    private IEnumerator Shake()
    {
        amountText.transform.DOScale(Vector3.one * 1.1f, .1f);
        amountText.transform.DORotate(new Vector3(0, 0, 5), .1f);
        transform.DOScaleY(startScale.y * 1.1f, .1f);
        transform.DOScaleX(startScale.x * 1.05f, .1f);
        yield return new WaitForSeconds(.1f + Time.deltaTime);
        amountText.transform.DORotate(new Vector3(0, 0, -5), .1f);
        transform.DOScaleY(startScale.y, .1f);
        transform.DOScaleX(startScale.x, .1f);
        yield return new WaitForSeconds(.1f + Time.deltaTime);
        amountText.transform.DORotate(new Vector3(0, 0, 0), .1f);
        shaking = false;
    }
    public void GetHit(float hitPower)
    {
        canStamina = false; 
        currentPower += hitPower;
        int smallestLevel = 0;
        for (int i = 0; i < powerNeededForNewLevelSkill.Count; i++)
        {
            if (currentPower >= powerNeededForNewLevelSkill[i])
            {
                smallestLevel = i;
            }
        }
        _levelTexts[0].text = (smallestLevel + 1).ToString();
        _levelTexts[1].text = (smallestLevel + 2).ToString();
        if(_doorType == DoorType.SkillDoor)
        {
            _levelTexts[2].text = "lvl" + (smallestLevel + 1).ToString();
        }
        else
        {
            _levelTexts[2].text = (smallestLevel + 1).ToString();
        }
        if(smallestLevel >= powerNeededForNewLevelSkill.Count-1)
        {
            int lastPower = powerNeededForNewLevelSkill[^1];
            for (int i = 0; i < 10; i++)
            {
                powerNeededForNewLevelSkill.Add(lastPower+(i+1)*10);
            }
        }
        float fillAmount = currentPower - (float)powerNeededForNewLevelSkill[smallestLevel];
        fillAmount /= (float)powerNeededForNewLevelSkill[smallestLevel + 1] - (float)powerNeededForNewLevelSkill[smallestLevel];

        Vector3 barCurrentScale = barStartScale + ((barFinalScale - barStartScale) * fillAmount);
        Vector3 barCurrentPosition = barStartPosition + ((barFinalPosition - barStartPosition) * fillAmount);
        _barRenderer.transform.DOScale(barCurrentScale, .2f).OnComplete(delegate {
            canStamina = true;
        });
        _barRenderer.transform.DOLocalMove(barCurrentPosition, .2f).OnComplete(delegate {
            canStamina = true;
        });
        /*
        DOTween.To(() => currentFillAmount, x => currentFillAmount = x, fillAmount, .2f).OnUpdate(delegate {
            
        }).OnComplete(delegate {
            canStamina = true;
        });*/
        currentLevel = smallestLevel;
        if(_doorType == DoorType.SkillDoor)
        {
            int skillNumber = ((int)_doorSkill);
            for (int i = 0; i < skillBalls.Count; i++)
            {
                if (i == skillNumber)
                {
                    skillBalls[i].transform.DOScale(skillBallsStartScales[i] * 1.2f, .1f);
                    skillBalls[i].transform.DOScale(skillBallsStartScales[i], .1f).SetDelay(.1f + Time.deltaTime);
                }
            }
            if(_doorSkill == Skills.MultiShoot)
            {
                _multiTexter.text ="X"+ (currentLevel + 2).ToString();
            }
        }
        else if(_doorType ==DoorType.BulletDoor)
        {
            _bulletInside.transform.DOScale(new Vector3(361.7169f, 361.7169f, 361.7169f) * 1.2f, .1f);
            _bulletInside.transform.DOScale(new Vector3(361.7169f, 361.7169f, 361.7169f), .1f).SetDelay(.1f + Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            /*
            GameObject bulletHit = ObjectPooler.instance.SpawnFromPool("DoorHit", other.transform.position, Quaternion.identity);
            foreach (ParticleSystem ps in bulletHit.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Play();
            }
            if (amount >= 0)
            {
                GameObject doorPositiveHit = ObjectPooler.instance.SpawnFromPool("DoorPositiveHit", other.transform.position, Quaternion.identity);
                foreach (ParticleSystem ps in doorPositiveHit.GetComponentsInChildren<ParticleSystem>())
                {
                    ps.Play();
                }
            }
            */
            other.GetComponent<BulletScript>().BulletDeActivate(true, true, GetComponent<Ricochetable>());
            if(_doorType == DoorType.SkillDoor || _doorType == DoorType.BulletDoor)
            {
                GetHit(other.GetComponent<BulletScript>().bulletPower);

            }
            else 
            {
                if (other.GetComponent<BulletScript>()._skillsGot.Contains(Skills.IceBullet))
                {
                    iced = true;
                }
                if (!locked)
                {
                    IncreaseAmount(other.GetComponent<BulletScript>().bulletPower);
                    Taptic.Light();
                }
                else
                {
                    LockHit();
                    Taptic.Medium();
                }
            }
        }
        else if (other.CompareTag("Player"))
        {
            if (!locked)
            {
                transform.DOMoveY(transform.position.y - 5, .2f);
                GetComponent<Collider>().enabled = false;
                switch (_doorType)
                {
                    case DoorType.FireRate:
                        ShootingScript.instance.FireRateUpgrade(amount);
                        break;
                    case DoorType.FireRange:
                        ShootingScript.instance.FireRangeUpgrade(amount);
                        break;
                    case DoorType.FirePower:
                        ShootingScript.instance.FirePowerUpgrade(amount);
                        break;
                    case DoorType.SkillDoor:
                        BarMain.instance.GetSkillPart(_doorSkill,currentLevel+1);
                        //StartCoroutine(ShootingScript.instance.GetSkillBullet(_bulletPosition, _doorSkill));
                        //ShootingScript.instance.GetSkillBullet(_doorSkill);
                        break;
                    case DoorType.BulletDoor:
                        BarMain.instance.GetPowerBullet(currentLevel+1);
                        break;
                }
            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            GetHit(1);
        }
        if (canStamina)
        {
            if (currentPower > powerNeededForNewLevelSkill[currentLevel])
            {
                currentPower -= Time.deltaTime * staminaSpeed;
                float fillAmount = currentPower - (float)powerNeededForNewLevelSkill[currentLevel];
                fillAmount /= (float)powerNeededForNewLevelSkill[currentLevel + 1] - (float)powerNeededForNewLevelSkill[currentLevel];
                Vector3 barCurrentScale = barStartScale + ((barFinalScale - barStartScale) * fillAmount);
                Vector3 barCurrentPosition = barStartPosition + ((barFinalPosition - barStartPosition) * fillAmount);
                _barRenderer.transform.localScale = barCurrentScale;
                _barRenderer.transform.localPosition= barCurrentPosition;
            }
        }
    }
}
