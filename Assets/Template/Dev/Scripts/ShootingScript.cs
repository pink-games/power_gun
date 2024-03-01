using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using ElephantSDK;


public class ShootingScript : MonoBehaviour
{
    [SerializeField] float fireRate;
    [SerializeField] AnimationCurve animCurve;

    public Skills skillToPut;
    List<int> fullData = new List<int>();

    [SerializeField] private List<GameObject> _weaponsInside = new List<GameObject>();


    [Header("ShootPart")]
    [SerializeField] float currentRange;
    public float shootForce;
    [SerializeField] Transform shootPosition;
    private bool spreadShotUnlocked;
    bool canShoot;
    [SerializeField] List<ParticleSystem> muzzleParticles = new List<ParticleSystem>();
    float counter;
    float rateIncreaseAmount = .1f;
    float rangeIncreaseAmounter= .1f;
    float shootInterval;
    public int shootCounter;
    public string rateUpgradeCountRemoteName;
    public string rangeUpgradeCountRemoteName;
    public string incomeUpgradeCountRemoteName;
    public static ShootingScript instance;
    private void Awake()
    {
        instance = this;
        canShoot = true;
        rateIncreaseAmount = RemoteConfig.GetInstance().GetFloat("RateIncAmountDoor", .03f);
        rangeIncreaseAmounter= RemoteConfig.GetInstance().GetFloat("RangeIncAmountDoor", .03f);
        fireRate = RemoteConfig.GetInstance().GetFloat("StartRate", 1) + PlayerPrefs.GetInt(rateUpgradeCountRemoteName) * RemoteConfig.GetInstance().GetFloat("RateIncAmountUpgrade", .05f);
        currentRange= RemoteConfig.GetInstance().GetFloat("StartRange", 10) + PlayerPrefs.GetInt(rangeUpgradeCountRemoteName) * RemoteConfig.GetInstance().GetFloat("RangeIncAmountUpgrade", .05f);
        shootInterval = 1 / fireRate;
        counter = shootInterval;
    }
    public void FireRateUpgradeFromUI()
    {
        PlayerPrefs.SetInt(rateUpgradeCountRemoteName, PlayerPrefs.GetInt(rateUpgradeCountRemoteName) + 1);
        fireRate = RemoteConfig.GetInstance().GetFloat("StartRate", 1) + PlayerPrefs.GetInt(rateUpgradeCountRemoteName) * RemoteConfig.GetInstance().GetFloat("RateIncAmountUpgrade", .05f);
        FireRateChanged();
    }
    public void FireRangeUpgradeFromUI()
    {
        PlayerPrefs.SetInt(rangeUpgradeCountRemoteName, PlayerPrefs.GetInt(rangeUpgradeCountRemoteName) + 1);
        currentRange= RemoteConfig.GetInstance().GetFloat("StartRange", 10) + PlayerPrefs.GetInt(rangeUpgradeCountRemoteName) * RemoteConfig.GetInstance().GetFloat("RangeIncAmountUpgrade", .05f);
    }
    public List<Skills> GetActiveSkills()
    {
        List<Skills> skillsToGive = new List<Skills>();
        List<BarValues> _values = BarMain.instance.GiveCurrentValues();
        for (int i = 0; i < _values.Count; i++)
        {
            if (_values[i].skillBar)
            {
                skillsToGive.Add(_values[i]._barSkill);
            }
        }
        return skillsToGive;
    }
    public List<int> GetSkillLevelser()
    {
        List<BarValues> _values = BarMain.instance.GiveCurrentValues();
        List<int> skillLevels = new List<int>();
        for (int i = 0; i < _values.Count; i++)
        {
            if (_values[i].skillBar)
            {
                skillLevels.Add(_values[i].skillLeveler);
            }
        }
        Debug.Log(skillLevels.Count);
        return skillLevels;
    }
    public float GetActivePower()
    {
        List<BarValues> _values = BarMain.instance.GiveCurrentValues();
        float totalPower = 0;
        for (int i = 0; i < _values.Count; i++)
        {
            totalPower += _values[i].barPower;
        }
        return totalPower;
    }
    public void FireRateUpgrade(float _amount)
    {
        fireRate += _amount * rateIncreaseAmount;
        FireRateChanged();
    }
    public void FireRangeUpgrade(float _amount)
    {
        currentRange+= _amount * rangeIncreaseAmounter;
        FireRateChanged();
    }
    public void FirePowerUpgrade(float _amount)
    {

    }
    private void FireRateChanged()
    {
        shootInterval = 1 / fireRate;
        if (counter > shootInterval)
        {
            counter = shootInterval;
        }
    }
    private void Update()
    {
        if (GameManager.instance.started)
        {
            if (canShoot)
            {
                counter -= Time.deltaTime;
                if (counter < 0)
                {
                    int shootAmounter = 1;
                    int shootAmount = 1;
                    int iNumber = 0;
                    if (GetActiveSkills().Contains(Skills.MultiShoot))
                    {

                        for (int i = 0; i < GetActiveSkills().Count; i++)
                        {
                            if (GetActiveSkills()[i] == Skills.MultiShoot)
                            {
                                iNumber = i;
                            }
                        }
                        shootAmount = GetSkillLevelser()[iNumber];
                    }
                    Shoot(GetActivePower(), GetActiveSkills(), GetSkillLevelser());

                    counter = shootInterval;
                }
            }
        }
    }
    public void SingleShoot()
    {

    }
    private IEnumerator ShootEnum(float shootPower, List<Skills> _skillsInside, List<int> skillLevels, int shootAmount)
    {

        for (int i = 0; i < shootAmount; i++)
        {
            Shoot(shootPower, _skillsInside, skillLevels);
            yield return new WaitForSeconds(.08f);
        }
    }
    public void Shoot(float shootPower, List<Skills> _skillsInside, List<int> skillLevels)
    {
        int shootAmounter = 1;
        int iNumber = 0;
        if (GetActiveSkills().Contains(Skills.MultiShoot))
        {

            for (int i = 0; i < GetActiveSkills().Count; i++)
            {
                if (GetActiveSkills()[i] == Skills.MultiShoot)
                {
                    iNumber = i;
                }
            }
            shootAmounter = GetSkillLevelser()[iNumber];
        }
        if (shootAmounter == 0)
        {
            shootAmounter = 1;
        }
        if (canShoot && GameManager.instance.started)
        {
            GetComponent<Animator>().Play("ShootAnimation");
            if (shootAmounter == 1)
            {
                GameObject bullet = ObjectPooler.instance.SpawnFromPool("Bullet", shootPosition.position, Quaternion.identity);
                bullet.transform.eulerAngles = new Vector3(0, 0, 0);
                for (int i = 0; i < muzzleParticles.Count; i++)
                {
                    muzzleParticles[i].Play();
                }
                bullet.GetComponent<BulletScript>().ActivateBullet(shootPower, _skillsInside, skillLevels);
                bullet.GetComponent<BulletScript>().zMax = currentRange + transform.position.z;
                bullet.transform.position = shootPosition.position;
                Vector3 shootVector = Vector3.forward * shootForce;
                bullet.GetComponent<Rigidbody>().AddForce(shootVector);
            }
            else if (shootAmounter == 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    GameObject bullet = ObjectPooler.instance.SpawnFromPool("Bullet", shootPosition.position, Quaternion.identity);
                    for (int m = 0; m < muzzleParticles.Count; m++)
                    {
                        muzzleParticles[m].Play();
                    }
                    bullet.GetComponent<BulletScript>().ActivateBullet(shootPower, _skillsInside, skillLevels);

                    bullet.GetComponent<BulletScript>().zMax = currentRange + transform.position.z;
                    bullet.transform.position = shootPosition.position;

                    Vector3 shootVector = Vector3.forward * shootForce;
                    if (i == 0)
                    {
                        shootVector.x = -.05f * shootForce;
                        bullet.transform.eulerAngles = new Vector3(0, -3, 0);
                    }
                    else
                    {
                        shootVector.x = .05f * shootForce;
                        bullet.transform.eulerAngles = new Vector3(0, 3, 0);
                    }
                    bullet.GetComponent<Rigidbody>().AddForce(shootVector);
                }
            }
            else if (shootAmounter == 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    GameObject bullet = ObjectPooler.instance.SpawnFromPool("Bullet", shootPosition.position, Quaternion.identity);
                    for (int m = 0; m < muzzleParticles.Count; m++)
                    {
                        muzzleParticles[m].Play();
                    }
                    bullet.GetComponent<BulletScript>().ActivateBullet(shootPower, _skillsInside, skillLevels);

                    bullet.GetComponent<BulletScript>().zMax = currentRange + transform.position.z;
                    bullet.transform.position = shootPosition.position;

                    Vector3 shootVector = Vector3.forward * shootForce;
                    if (i == 0)
                    {
                        shootVector.x = -.05f * shootForce;
                        bullet.transform.eulerAngles = new Vector3(0, -3, 0);
                    }
                    else if (i == 2)
                    {
                        shootVector.x = .05f * shootForce;
                        bullet.transform.eulerAngles = new Vector3(0, 3, 0);
                    }
                    else if (i == 1)
                    {
                        shootVector.x = 0;
                        bullet.transform.eulerAngles = new Vector3(0, 0, 0);
                    }
                    bullet.GetComponent<Rigidbody>().AddForce(shootVector);
                }
            }
            else if (shootAmounter == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    GameObject bullet = ObjectPooler.instance.SpawnFromPool("Bullet", shootPosition.position, Quaternion.identity);
                    for (int m = 0; m < muzzleParticles.Count; m++)
                    {
                        muzzleParticles[m].Play();
                    }
                    bullet.GetComponent<BulletScript>().ActivateBullet(shootPower, _skillsInside, skillLevels);

                    bullet.GetComponent<BulletScript>().zMax = currentRange + transform.position.z;
                    bullet.transform.position = shootPosition.position;

                    Vector3 shootVector = Vector3.forward * shootForce;
                    if (i == 0)
                    {
                        shootVector.x = -.05f * shootForce;
                        bullet.transform.eulerAngles = new Vector3(0, -3, 0);
                    }
                    else if (i == 2)
                    {
                        shootVector.x = .05f * shootForce;
                        bullet.transform.eulerAngles = new Vector3(0, 3, 0);
                    }
                    else if (i == 1)
                    {
                        shootVector.x = .1f * shootForce;
                        bullet.transform.eulerAngles = new Vector3(0, 6, 0);
                    }
                    else if (i == 3)
                    {
                        shootVector.x = -.1f * shootForce;
                        bullet.transform.eulerAngles = new Vector3(0, -6, 0);
                    }
                    bullet.GetComponent<Rigidbody>().AddForce(shootVector);
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    GameObject bullet = ObjectPooler.instance.SpawnFromPool("Bullet", shootPosition.position, Quaternion.identity);
                    for (int m = 0; m < muzzleParticles.Count; m++)
                    {
                        muzzleParticles[m].Play();
                    }
                    bullet.GetComponent<BulletScript>().ActivateBullet(shootPower, _skillsInside, skillLevels);

                    bullet.GetComponent<BulletScript>().zMax = currentRange + transform.position.z;
                    bullet.transform.position = shootPosition.position;

                    Vector3 shootVector = Vector3.forward * shootForce;
                    if (i == 0)
                    {
                        shootVector.x = -.05f * shootForce;
                        bullet.transform.eulerAngles = new Vector3(0, -3, 0);
                    }
                    else if (i == 2)
                    {
                        shootVector.x = .05f * shootForce;
                        bullet.transform.eulerAngles = new Vector3(0, 3, 0);
                    }
                    else if (i == 1)
                    {
                        shootVector.x = .1f * shootForce;
                        bullet.transform.eulerAngles = new Vector3(0, 6, 0);
                    }
                    else if (i == 3)
                    {
                        shootVector.x = -.1f * shootForce;
                        bullet.transform.eulerAngles = new Vector3(0, -6, 0);
                    }
                    else if (i == 4)
                    {
                        shootVector.x = 0;
                        bullet.transform.eulerAngles = new Vector3(0, 0, 0);
                    }
                    bullet.GetComponent<Rigidbody>().AddForce(shootVector);
                }
            }
        }
    }
}
