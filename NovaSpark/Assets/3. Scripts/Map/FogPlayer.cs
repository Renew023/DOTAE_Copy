using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogPlayer : MonoBehaviour
{
    public FogSystem fogSystem;
    public Transform secondaryFogSystem;
    [Range(0, 5)]
    public float sightDistance;
    public float checkInterval;

    private void Start()
    {
        StartCoroutine(CheckFogOfWar(checkInterval));
        secondaryFogSystem.localScale = new Vector2(sightDistance, sightDistance)*10f;
    }

    private IEnumerator CheckFogOfWar(float checkInterval)
    {
        while (true)
        {
            fogSystem.MakeHole(transform.localPosition, sightDistance); //좌표에 보이는 시야만큼의 구멍을 만듬.
            yield return new WaitForSeconds(checkInterval);
        }
    }
}
