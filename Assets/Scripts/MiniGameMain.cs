using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class MiniGameMain : MonoBehaviour
{
    [SerializeField]private List<int> powerNeededForNewLayer = new List<int>();
    float currentPower;
    int currentLevel;
    [SerializeField] private List<GameObject> addSprites = new List<GameObject>();
    public TextMeshPro capacityTexter;
    [SerializeField] GameObject secondBar;
    private void Awake()
    {

    }
    private void Start()
    {
        powerNeededForNewLayer.Clear();
        for (int i = 0; i < GameManager.instance._gameSpecs.powerNeededForCapacity.Count; i++)
        {
            powerNeededForNewLayer.Add(GameManager.instance._gameSpecs.powerNeededForCapacity[i]);
        }
        AddPower(0);

    }
    public void AddPower(float hitPower)
    {
        currentPower += hitPower;
        int smallestLevel = 0;
        for(int i = 0; i < powerNeededForNewLayer.Count; i++)
        {
            if (currentPower >= powerNeededForNewLayer[i])
            {
                smallestLevel = i;
            }
        }
        for(int i = 0; i < addSprites.Count; i++)
        {
            if (i< smallestLevel)
            {
                if (!addSprites[i].gameObject.activeInHierarchy)
                {
                    addSprites[i].gameObject.SetActive(true);
                    Vector3 startScale = addSprites[i].transform.localScale;
                    addSprites[i].transform.localScale = Vector3.zero;
                    addSprites[i].transform.DOScale(startScale,.2f);
                }
            }
            else
            {
                addSprites[i].gameObject.SetActive(false);
            }
        }
        currentLevel = smallestLevel;
        if (currentLevel > 0)
        {
            if (!secondBar.gameObject.activeInHierarchy)
            {
                secondBar.gameObject.SetActive(true);
            }
        }
        capacityTexter.text = "+" + currentLevel.ToString() + " Capacity";
        SetCapacity(currentLevel);
    }


    public void SetCapacity(int capacityAddAmount)
    {
        for(int i = 0; i < addSprites.Count; i++)
        {
            if (i < capacityAddAmount)
            {
                if (!addSprites[i].gameObject.activeInHierarchy)
                {
                    addSprites[i].gameObject.SetActive(true);
                    Vector3 scaler = addSprites[i].transform.localScale;
                    addSprites[i].transform.localScale = Vector3.zero;
                    addSprites[i].transform.DOScale(scaler, .2f);
                }
            }
            else
            {
                addSprites[i].gameObject.SetActive(false);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.DOScale(Vector3.zero, .2f);
            GetComponent<Collider>().enabled = false;
            BarMain.instance.AddCapacity(currentLevel);
        }else if (other.CompareTag("Bullet"))
        {
            other.GetComponent<BulletScript>().BulletDeActivate(false, true);   
        }
    }
}
