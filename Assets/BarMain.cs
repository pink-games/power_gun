using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class BarValues
{
    public bool skillBar;
    public float barPower;
    public Skills _barSkill;
    public int skillLeveler;
}
public class BarMain : MonoBehaviour
{
    [SerializeField] float fillTime;
    [SerializeField] float fillAmount;
    [SerializeField] float minValue;
    [SerializeField] float maxValue;
    [SerializeField] List<Renderer> singleBarRenderers = new List<Renderer>();
    [SerializeField] GameObject secondBar;
    int currentCapacity;
    [SerializeField] AnimationCurve _growCurver;
    public List<BarValues> _barValues;
    public GameObject singleBarRendererPrefab;
    [SerializeField] List<int> fullData = new List<int>();
    public Material activeMaterial;
    public Material inActiveMaterial;
    public static BarMain instance;
    private void Awake()
    {
        instance = this;
        currentCapacity = 5;
        fillAmount = minValue;
        StartCoroutine(WorkTheBar());
        for(int i = 0; i < singleBarRenderers.Count; i++)
        {
            singleBarRenderers[i].GetComponent<SinglePart>().active = true;
            if (i < 5)
            {
                fullData.Add(1);
            }
            else
            {
                fullData.Add(0);
            }
        }
    }
    private void Start()
    {
        AddCapacity(0);
    }
    private IEnumerator WorkTheBar()
    {
        DOTween.To(() => fillAmount, x => fillAmount = x, maxValue, fillTime/2).OnUpdate(delegate {
            for (int i = 0; i < singleBarRenderers.Count; i++)
            {
                if (fullData[i] == 1)
                {
                    singleBarRenderers[i].material.SetFloat("_FillRate", fillAmount);
                }
            }
        }).SetEase(Ease.Linear);
        yield return new WaitForSeconds(fillTime / 2 + Time.deltaTime);
        DOTween.To(() => fillAmount, x => fillAmount = x, minValue, fillTime/2).OnUpdate(delegate {
            for (int i = 0; i < singleBarRenderers.Count; i++)
            {
                if (fullData[i] == 1)
                {
                    singleBarRenderers[i].material.SetFloat("_FillRate", fillAmount);
                }
            }
        }).SetEase(Ease.Linear);
        yield return new WaitForSeconds(fillTime / 2 + Time.deltaTime);
        StartCoroutine(WorkTheBar());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            AddCapacity(2);
        }else if (Input.GetKeyDown(KeyCode.G))
        {
            List<BarValues> values = GiveCurrentValues();
            for(int i = 0; i < values.Count; i++)
            {
                Debug.Log(i + "_iNumber" + values[i].skillBar + "isSkillBar" + values[i]._barSkill + "whichSkill" + values[i].barPower + "BarPowerer");
            }
        }
    }
    public int GetBarNumber()
    {
        if(fillAmount < 0.685f)
        {
            return 0;
        }
        else if (fillAmount >= .685f && fillAmount < 0.76f)
        {
            return 1;
        }
        else if (fillAmount >= 0.76f && fillAmount < .84f)
        {
            return 2;
        }
        else if (fillAmount >= .84f && fillAmount < .92f)
        {
            return 3;
        }
        else if (fillAmount >= .92f && fillAmount <= maxValue)
        {
            return 4;
        }
        else
        {
            return 0;
        }
    }
    public List<int> GetActiveBars()
    {
        int barNumber = GetBarNumber();
        List<int> barsToUser = new List<int>();
        barsToUser.Add(barNumber);
        if (!_barValues[barNumber].skillBar)
        {
            singleBarRenderers[barNumber].GetComponent<SinglePart>()._powerTexter.color = Color.green;
            singleBarRenderers[barNumber].GetComponent<SinglePart>()._powerTexter.DOColor(Color.white,.1f).SetDelay(.1f);
            singleBarRenderers[barNumber].GetComponent<SinglePart>()._powerTexter.transform.DOScale(Vector3.one * 1.3f, .1f);
            singleBarRenderers[barNumber].GetComponent<SinglePart>()._powerTexter.transform.DOScale(Vector3.one, .1f).SetDelay(.1f);
        }
        else
        {
            //singleBarRenderers[barNumber].GetComponent<SinglePart>()._skillerParenter.transform.DOScale(Vector3.one * 1.3f, .1f);
            //singleBarRenderers[barNumber].GetComponent<SinglePart>()._skillerParenter.transform.DOScale(Vector3.one, .1f).SetDelay(.1f);
        }
        if (fullData[barNumber + 5] ==1)
        {
            if(fullData[barNumber + 5] == 1)
            {
                barsToUser.Add(barNumber + 5);
            }
        }
        return barsToUser;
    }
    public int GetSmallestEmptySlotNumber()
    {
        bool found = false;
        int smallestNumber = 0;
        for(int i = 0; i < _barValues.Count; i++)
        {
            if (fullData[i] == 0)
            {
                if (!found)
                {
                    smallestNumber = i;
                    found = true;
                }
            }
        }
        return smallestNumber;
    }
    public bool GotEmptySlot()
    {
        bool gotEmptySlot = false;
        for(int i = 0; i < currentCapacity; i++)
        {
            if (fullData[i] == 0)
            {
                gotEmptySlot = true;
            }
        }
        return gotEmptySlot;
    }
    public void GetPowerBullet(float bulletPowerer)
    {
        if (!GotEmptySlot())
        {
            if (_barValues[SmallestBarObjectInteger()].barPower < bulletPowerer)
            {
                GameObject originalSingleParter = singleBarRenderers[SmallestBarObjectInteger()].gameObject;
                _barValues[SmallestBarObjectInteger()].barPower = bulletPowerer;
                singleBarRenderers[SmallestBarObjectInteger()].GetComponent<SinglePart>()._powerTexter.text = bulletPowerer.ToString();
                GameObject showBar = Instantiate(originalSingleParter);
                Destroy(showBar.GetComponent<SinglePart>());
                showBar.transform.parent = singleBarRenderers[SmallestBarObjectInteger()].transform.parent;
                showBar.transform.position = singleBarRenderers[SmallestBarObjectInteger()].transform.position;
                showBar.transform.localScale = singleBarRenderers[SmallestBarObjectInteger()].transform.localScale;
                showBar.transform.localEulerAngles = singleBarRenderers[SmallestBarObjectInteger()].transform.localEulerAngles;
                Vector3 startScale = originalSingleParter.transform.localScale;
                originalSingleParter.transform.DOScale(startScale, .4f).SetEase(_growCurver);
                originalSingleParter.transform.localScale = Vector3.zero;
                showBar.AddComponent<Rigidbody>().AddForce(new Vector3(0, .4f, -1) * 200);
                showBar.transform.parent = null;
                showBar.AddComponent<BoxCollider>();
            }
        }
        else
        {
            _barValues[GetSmallestEmptySlotNumber()].barPower = bulletPowerer;
            _barValues[GetSmallestEmptySlotNumber()].skillBar = false;
            singleBarRenderers[GetSmallestEmptySlotNumber()].GetComponent<SinglePart>().active = true;
            singleBarRenderers[GetSmallestEmptySlotNumber()].GetComponent<SinglePart>().Set(_barValues[GetSmallestEmptySlotNumber()]);
            fullData[GetSmallestEmptySlotNumber()] = 1;
        }
        
    }
    public void GetSkillBullet(Skills _skillToGet)
    {

    }
    public void GetSkillPart(Skills skillToGetter,int skillLevel)
    {
        if (GotEmptySlot())
        {
            int emptySlotNumber = GetSmallestEmptySlotNumber();
            _barValues[emptySlotNumber].skillBar = true;
            _barValues[emptySlotNumber]._barSkill = skillToGetter;
            _barValues[emptySlotNumber].skillLeveler = skillLevel;
            singleBarRenderers[emptySlotNumber].GetComponent<SinglePart>().active = true;
            singleBarRenderers[emptySlotNumber].GetComponent<SinglePart>().Set(_barValues[emptySlotNumber]);
            fullData[emptySlotNumber] = 1;
        }
        else
        {

        }
    }
    public int SmallestBarObjectInteger()
    {
        float currentSmallestNumber = Mathf.Infinity;
        int iNumber = 0;
        for(int i = 0; i < _barValues.Count; i++)
        {
            if (!_barValues[i].skillBar)
            {
                if(currentSmallestNumber< _barValues[i].barPower)
                {
                    currentSmallestNumber = _barValues[i].barPower;
                    iNumber =i;
                }
            }
        }
        return iNumber;
    }
    public List<BarValues> GiveCurrentValues()
    {
        int currentBarNumber = GetBarNumber();
        List<BarValues> valuesToGive = new List<BarValues>();
        List<int> barsToUser = GetActiveBars();
        for(int i = 0; i < barsToUser.Count; i++)
        {
            valuesToGive.Add(_barValues[barsToUser[i]]);
        }
        return valuesToGive;
    }
    public void SetBars()
    {
        for(int i = 0; i < currentCapacity; i++)
        {
            BarValues value = _barValues[i];
            singleBarRenderers[i].GetComponent<SinglePart>().Set(value);
        }
    }
    public void AddCapacity(int addAmount)
    {
        currentCapacity += addAmount;
        if (currentCapacity > 5)
        {
            if (!secondBar.gameObject.activeInHierarchy)
            {
                secondBar.gameObject.SetActive(true);
                
                Vector3 localscale = secondBar.gameObject.transform.localScale;
                secondBar.transform.localScale = Vector3.zero;
                secondBar.transform.DOScale(localscale, .2f).SetEase(_growCurver);
            }
        }
        else
        {
            secondBar.gameObject.SetActive(false);
        }
        for (int i = 0; i < singleBarRenderers.Count; i++)
        {
            if (i < currentCapacity)
            {
                if (!singleBarRenderers[i].gameObject.activeInHierarchy)
                {
                    singleBarRenderers[i].gameObject.SetActive(true);
                    singleBarRenderers[i].GetComponent<SinglePart>().active = false;
                    singleBarRenderers[i].GetComponent<SinglePart>().Set(_barValues[0]);
                    Vector3 startScaler = singleBarRenderers[i].transform.localScale;
                    singleBarRenderers[i].transform.localScale = Vector3.zero;
                    singleBarRenderers[i].gameObject.transform.DOScale(startScaler, .2f).SetEase(_growCurver);
                }
            }
            else
            {
                singleBarRenderers[i].gameObject.SetActive(false);
            }
        }
        SetBars();
    }
}