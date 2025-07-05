using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniStorm;
public class unistorm_time : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void setTime(float time)
    {
        UniStormSystem.Instance.m_TimeFloat = time;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
