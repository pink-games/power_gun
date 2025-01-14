using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;


public enum Skills
{
    BiggerBullets,
    Bomb,
    Critical,
    FireBullets,
    IceBullet,
    MultiShoot,
    Range,
    Richochet,
}
public class BulletScript : MonoBehaviour
{
    public float zMax;
    public float bulletPower;
    public List<Material> _bulletTipMaterials;
    public bool decisionGateBullet;
    public int health;
    bool gotCrit;
    public List<int> skillsInside;
    List<Ricochetable> richocetEncounter = new List<Ricochetable>();
    public List<Skills> _skillsGot;
    [SerializeField] List<GameObject> allTrails = new List<GameObject>();
    [SerializeField] GameObject _fireTrail;
    [SerializeField] GameObject _iceTrail;
    public void SetBullet(List<Skills> _skills,List<int> skillLevels)
    {
        _skillsGot.Clear();
        _skillsGot.AddRange(_skills);
        gotCrit = false;
     
        GetComponentInChildren<TMPro.TextMeshPro>().color = Color.green;
        GetComponentInChildren<TMPro.TextMeshPro>().DOColor(Color.white, .1f).SetDelay(.2f);
        for (int i = 0; i < allTrails.Count; i++)
        {
            allTrails[i].SetActive(false);
            allTrails[i].GetComponent<ParticleSystem>().Stop();
        }
        if (_skills.Contains(Skills.Bomb))
        {
            int indexOf = _skills.IndexOf(Skills.Bomb);
            int skillLevel = skillLevels[indexOf];
            bulletPower *= 2 + (skillLevel * .1f);
        }
        if (_skills.Contains(Skills.FireBullets))
        {
            int indexOf = _skills.IndexOf(Skills.FireBullets);
            int skillLevel = skillLevels[indexOf];
            _fireTrail.gameObject.SetActive(true);
            bulletPower *= 1.5f + (skillLevel * .1f);
        }
        if (_skills.Contains(Skills.IceBullet))
        {
            int indexOf = _skills.IndexOf(Skills.IceBullet);
            int skillLevel = skillLevels[indexOf];
            _iceTrail.gameObject.SetActive(true);
            bulletPower *= 1.5f + (skillLevel * .1f);
        }

        if (_skills.Contains(Skills.Critical))
        {
            int randomInt = Random.Range(0, 5);
            if (randomInt == 3)
            {
            }
            gotCrit = true;
            bulletPower *= 2;
        }
        if (_skills.Contains(Skills.Richochet))
        {
            richocetEncounter.Clear();
            int indexOf = _skills.IndexOf(Skills.Richochet);
            int skillLevel = skillLevels[indexOf];
            int addAmount = Mathf.RoundToInt((skillLevel * .5f) -.01f);
            health = 2;
        }
        if (_skills.Contains(Skills.BiggerBullets))
        {
            int indexOf = _skills.IndexOf(Skills.BiggerBullets);
            int skillLevel = skillLevels[indexOf];
            transform.localScale *= 1.5f + (1 + skillLevel * .2f);
        }
        if (_skills.Contains(Skills.Range))
        {
            int indexOf = _skills.IndexOf(Skills.BiggerBullets);
            int skillLevel = skillLevels[indexOf];
            zMax += 10 + (1 + skillLevel * 1f);
        }
        if (bulletPower % 1 == 0)
        {
            GetComponentInChildren<TMPro.TextMeshPro>().text = bulletPower.ToString("0");

        }
        else
        {
            GetComponentInChildren<TMPro.TextMeshPro>().text = bulletPower.ToString("0.0");
        }
    }
    public void BulletDeActivate(bool showPower, bool particledHit = false, Ricochetable _ricochetObject = null)
    {
        if (particledHit)
        {
            Taptic.Light();
            GameObject hitParticle = ObjectPooler.instance.SpawnFromPool("BulletHit", transform.position-new Vector3(0,0,.1f), Quaternion.identity);
            foreach (ParticleSystem ps in hitParticle.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Play();
            }


            if (_skillsGot.Contains(Skills.FireBullets))
            {
                GameObject fireHitParticle = ObjectPooler.instance.SpawnFromPool("FireExplosion", transform.position, Quaternion.identity);
                foreach (ParticleSystem ps in fireHitParticle.GetComponentsInChildren<ParticleSystem>())
                {
                    ps.Play();
                }
            }
            if (_skillsGot.Contains(Skills.IceBullet))
            {
                GameObject iceHitParticle = ObjectPooler.instance.SpawnFromPool("IceExplosioner", transform.position, Quaternion.identity);
                foreach (ParticleSystem ps in iceHitParticle.GetComponentsInChildren<ParticleSystem>())
                {
                    ps.Play();
                }
            }
            if (_skillsGot.Contains(Skills.Bomb))
            {
                Debug.Log("GotBomberExplosioner");
                GameObject bombHitParticle = ObjectPooler.instance.SpawnFromPool("BombExplosion", transform.position, Quaternion.identity);
                foreach (ParticleSystem ps in bombHitParticle.GetComponentsInChildren<ParticleSystem>())
                {
                    ps.Play();
                }
            }
            if (_skillsGot.Contains(Skills.Critical))
            {
                if (gotCrit)
                {
                    
                    GameObject bombHitParticle = ObjectPooler.instance.SpawnFromPool("GotCritExplosion", transform.position, Quaternion.identity);
                    foreach (ParticleSystem ps in bombHitParticle.GetComponentsInChildren<ParticleSystem>())
                    {
                        ps.Play();
                    }
                }
            }
            if (_skillsGot.Contains(Skills.Richochet))
            {
                health -= 1;
            }
        }
        if (showPower)
        {
            GameObject hitPowerTexter = ObjectPooler.instance.SpawnFromPool("HitPowerTexter", transform.position - new Vector3(0, 0, 1), Quaternion.identity);
            hitPowerTexter.transform.DOKill();
            hitPowerTexter.transform.DOComplete();
            Vector3 startPosition = transform.position - new Vector3(Random.Range(-.2f, .2f), 0, 1);
            hitPowerTexter.transform.position = startPosition;
            hitPowerTexter.transform.DOMove(startPosition + new Vector3(Random.Range(-1f, 1f), Random.Range(1, 1.5f), 0), .3f);
            if (bulletPower % 1 == 0)
            {
                hitPowerTexter.GetComponent<TextMeshPro>().text = bulletPower.ToString("0");
            }
            else
            {
                hitPowerTexter.GetComponent<TextMeshPro>().text = bulletPower.ToString("0.0");
            }
            hitPowerTexter.GetComponent<TextMeshPro>().color = Color.white;
            hitPowerTexter.GetComponent<TextMeshPro>().DOColor(new Color(), .2f).SetDelay(.5f);
        }
        if (!_skillsGot.Contains(Skills.Richochet))
        {
            transform.DOComplete();
            transform.DOKill();
            transform.DOScale(Vector3.zero, .1f);
            GetComponent<Collider>().enabled = false;
            GetComponent<TrailRenderer>().enabled = false;
            GetComponent<TrailRenderer>().Clear();
        }
        else
        {
            if (_ricochetObject != null)
            {
                if (health <= 0)
                {
                    transform.DOComplete();
                    transform.DOKill();
                    transform.DOScale(Vector3.zero, .1f);
                    GetComponent<Collider>().enabled = false;
                    GetComponent<TrailRenderer>().enabled = false;
                    GetComponent<TrailRenderer>().Clear();
                }
                else
                {
                    if (!richocetEncounter.Contains(_ricochetObject))
                    {
                        if (_ricochetObject != null)
                        {
                            GameObject ricohetTo = null;
                            float smallestDifference = Mathf.Infinity;
                            foreach (Ricochetable rr in FindObjectsOfType<Ricochetable>())
                            {
                                Debug.Log(_ricochetObject.gameObject);
                                if (rr.gameObject != _ricochetObject.gameObject)
                                {
                                    if (Vector3.Distance(rr.transform.position, _ricochetObject.transform.position) < smallestDifference)
                                    {
                                        smallestDifference = Vector3.Distance(rr.transform.position, _ricochetObject.transform.position);
                                        ricohetTo = rr.gameObject;
                                    }
                                }
                            }
                            Vector3 vectoralDifference = ricohetTo.transform.position - transform.position;
                            vectoralDifference.y = 0;
                            vectoralDifference.Normalize();
                            GetComponent<Rigidbody>().velocity = Vector3.zero;
                            transform.LookAt(new Vector3(ricohetTo.transform.position.x, transform.position.y, ricohetTo.transform.position.z));
                            GetComponent<Rigidbody>().AddForce(vectoralDifference * ShootingScript.instance.shootForce);
                            richocetEncounter.Add(_ricochetObject);
                        }
                    }
                    else
                    {

                    }
                }
            }
            else
            {
                transform.DOComplete();
                transform.DOKill();
                transform.DOScale(Vector3.zero, .1f);
                GetComponent<Collider>().enabled = false;
                GetComponent<TrailRenderer>().enabled = false;
                GetComponent<TrailRenderer>().Clear();
            }
        }
    }
    public void ActivateBullet(float _power, List<Skills> skill, List<int> _levels)
    {
        GetComponentInChildren<TextMeshPro>().text = _power.ToString();
        bulletPower = _power;
        transform.DOComplete();
        transform.DOKill();
        GetComponent<TrailRenderer>().Clear();
        GetComponent<BulletScript>().SetBullet(skill,_levels);
        transform.DOKill();
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().enabled = true;
        transform.localScale = Vector3.one * .2f;
    }
    private void Update()
    {
        if (!_skillsGot.Contains(Skills.Richochet))
        {
            if (transform.position.z > zMax)
            {
                if (GetComponent<Collider>())
                {
                    GetComponent<Collider>().enabled = false;
                    transform.DOScale(Vector3.zero, .2f);
                    GetComponent<TrailRenderer>().enabled = false;
                }
            }
        }
    }
}
