using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CollectableExecuter : MonoBehaviour
{
    public List<GameObject> referenceCubers;
    public bool execute;
    private void Update()
    {
        if (execute)
        {
            DizExecuter();
            execute = false;
        }
    }
    public void DizExecuter()
    {
        for(int i = 0; i < referenceCubers.Count; i++)
        {
            GameObject parentObject = new GameObject();
            parentObject.transform.parent = transform;
            parentObject.transform.localPosition = new Vector3(0,referenceCubers[i].transform.localPosition.y,0);
            parentObject.name = "LayerParent " + i.ToString();
            foreach(Transform cc in GetComponentsInChildren<Transform>())
            {
                if(cc.transform.position.y == referenceCubers[i].transform.position.y)
                {
                    cc.transform.parent = parentObject.transform;
                }
            }
        }
    }
}
