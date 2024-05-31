using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProcedualAnimation : MonoBehaviour
{

   /* static Vector3[] ProjectOnSuface(Vector3 point, float halfRange, Vector3 up)
    {
        Vector3[] res = new Vector3[2];
        RaycastHit hit;
        Ray ray = new Ray(new Vector3(point.x, point.y + halfRange, point.z), -up);
        if (Physics.Raycast(ray, out hit, 2f * halfRange))
        {
            res[0] = hit.point;
            res[1] = hit.normal;
        }
        else
        {
            res[0] = point;
        }
        return res;
    }*/

    public Transform leftFootTarget;
    public Transform rightFootTarget;

    Vector3 initLeftFootPos;
    Vector3 initRightFootPos;

    Vector3 lastLeftFootPos;
    Vector3 lastRightFootPos;
    // Start is called before the first frame update
    void Start()
    {
        initLeftFootPos = leftFootTarget.localPosition;
        initRightFootPos = rightFootTarget.localPosition;

        lastLeftFootPos = leftFootTarget.localPosition;
        lastRightFootPos = rightFootTarget.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        leftFootTarget.position = lastLeftFootPos;
        rightFootTarget.position = lastRightFootPos;

        lastLeftFootPos = leftFootTarget.localPosition;
        lastRightFootPos = rightFootTarget.localPosition;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(leftFootTarget.position, 0.2f);
        Gizmos.DrawSphere(rightFootTarget.position, 0.2f);
        Debug.DrawLine(transform.position, transform.position + transform.up * 2f);
    }
}
