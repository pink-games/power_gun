using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SinglePart : MonoBehaviour
{
    public SkillSpriteParent _skillerParenter;
    public TextMeshPro _powerTexter;
    public bool active;
    public int skillLevel;
    private void Awake()
    {
        _skillerParenter = GetComponentInChildren<SkillSpriteParent>();
        foreach(Transform cc in GetComponentsInChildren<Transform>())
        {
            if (cc.gameObject.name.Contains("PowerText"))
            {
                _powerTexter = cc.GetComponent<TextMeshPro>();
            }
        }
    }
    public void Set(BarValues _barValue)
    {
        if (active)
        {
            GetComponent<Renderer>().material = BarMain.instance.activeMaterial;
            if (_barValue.skillBar)
            {
                _powerTexter.gameObject.SetActive(false);
                _skillerParenter.gameObject.SetActive(true);
                int skillNumber = (int)_barValue._barSkill;
                Debug.Log("SkillName" + _barValue._barSkill.ToString() + "SkillNumber" + skillNumber);
                for(int i = 0; i < _skillerParenter.spritesInside.Count; i++)
                {
                    if (i == skillNumber)
                    {
                        _skillerParenter.spritesInside[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        _skillerParenter.spritesInside[i].gameObject.SetActive(false);
                    }
                }
                if(_barValue._barSkill == Skills.MultiShoot)
                {
                    _skillerParenter.GetComponentInChildren<TextMeshPro>().text = "X" + ((_barValue.skillLeveler)+1).ToString();
                }
            }
            else
            {
                Debug.Log("Deneme");
                _powerTexter.text = _barValue.barPower.ToString();
                _powerTexter.gameObject.SetActive(true);
                _skillerParenter.gameObject.SetActive(false);
            }

        }
        else
        {
            Debug.Log("InActive");
            GetComponent<Renderer>().material = BarMain.instance.inActiveMaterial;
            _powerTexter.gameObject.SetActive(false);
            _skillerParenter.gameObject.SetActive(false);
        }
    }
}
