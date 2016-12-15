using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStampHolder : MonoBehaviour
{
    [SerializeField]
    private string timeStamp;

    public string TimeStamp
    {
        get { return timeStamp; }
        set { timeStamp = value; }
    }
}
