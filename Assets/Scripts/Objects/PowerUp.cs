using System;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
