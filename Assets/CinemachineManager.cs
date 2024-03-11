using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CinemachineManager : MonoBehaviour
{
    public List<CinemachineVirtualCamera> _cameras;
    public static CinemachineManager instance;
    private void Awake()
    {
        instance = this;
    }
    public void SetCam(int _camNumber)
    {
        for(int i = 0; i < _cameras.Count; i++)
        {
            if (i == _camNumber)
            {
                _cameras[i].Priority = 11;
            }
            else
            {
                _cameras[i].Priority = 0;
            }
        }
    }
}
