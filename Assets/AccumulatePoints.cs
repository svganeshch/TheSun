using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AccumulatePoints : MonoBehaviour
{
    public List<Transform> getAwayPoints;

    private void Awake()
    {
        foreach (Transform mypoint in GetComponentsInChildren<Transform>().Skip(1))
        {
            getAwayPoints.Add(mypoint);
            //Debug.Log("points : " + mypoint.name);
        }
    }
}
