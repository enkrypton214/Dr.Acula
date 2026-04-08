
using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] GameObject pointA;
    [SerializeField] GameObject pointB;
    [SerializeField] float speed;
    [SerializeField] float delay;

    [SerializeField] GameObject platform;
    private Vector3 targetPosistion;
    
    void Start()
    {
        platform.transform.position = pointA.transform.position;
        targetPosistion= pointB.transform.position;
        StartCoroutine(MovePlatform());
    }


    IEnumerator MovePlatform()
    {
        while (true)
        {
            while ((targetPosistion - platform.transform.position).sqrMagnitude > 0.01f)
            {
                platform.transform.position = Vector3.MoveTowards(platform.transform.position,targetPosistion,speed*Time.deltaTime);
                yield return null;
            }
            targetPosistion = targetPosistion == pointA.transform.position ? pointB.transform.position : pointA.transform.position;
            yield return new WaitForSeconds(delay);
        }
    }
}
