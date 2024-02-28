using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;



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
    float shootInterval;
    public static ShootingScript instance;
    private void Awake()
    {
        instance = this;
        canShoot = true;
        shootInterval = 1 / fireRate;
        counter = shootInterval;
    }
    
    public List<Skills> GetActiveSkills()
    {
        List<Skills> skillsToGive = new List<Skills>();
        return skillsToGive;
    }
    public float GetActivePower()
    {
        return 1;
    }
    public void FireRateUpgrade(float _amount)
    {
        fireRate += _amount * rateIncreaseAmount;
        FireRateChanged();
    }
    public void FireRangeUpgrade(float _amount)
    {

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
                    Shoot(1,GetActiveSkills());
                    counter = shootInterval;
                }
            }
        }
    }
    public void Shoot(float shootPower, List<Skills> _skillsInside)
    {
        if (canShoot && GameManager.instance.started)
        {
            GetComponent<Animator>().Play("ShootAnimation");
            if (!spreadShotUnlocked)
            {

                Taptic.Light();
                GameObject bullet = ObjectPooler.instance.SpawnFromPool("Bullet", shootPosition.position, Quaternion.identity);
                bullet.transform.eulerAngles = new Vector3(0, 0, 0);
                for (int i = 0; i < muzzleParticles.Count; i++)
                {
                    muzzleParticles[i].Play();
                }
                /*
                    muzzleParticles[0].GetComponent<AudioSource>().Play();
                    muzzleParticles[0].GetComponent<AudioSource>().pitch *= 1 + Random.Range(-5 / 100, 5 / 100);
                    power = 0;
                    for(int i = 0; i < currentPowers.Count; i++)
                    {
                        power += currentPowers[i];
                    }
                    */
                bullet.GetComponent<BulletScript>().ActivateBullet(shootPower, _skillsInside);
                bullet.GetComponent<BulletScript>().zMax = currentRange + transform.position.z;
                bullet.transform.position = shootPosition.position;
                Vector3 shootVector = Vector3.forward * shootForce;
                bullet.GetComponent<Rigidbody>().AddForce(shootVector);
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    Taptic.Light();
                    GameObject bullet = ObjectPooler.instance.SpawnFromPool("Bullet", shootPosition.position, Quaternion.identity);
                    for (int m = 0; m < muzzleParticles.Count; m++)
                    {
                        muzzleParticles[m].Play();
                    }
                    bullet.GetComponent<BulletScript>().ActivateBullet(shootPower, _skillsInside);

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
        }
    }
}
