using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTileController : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerArea") == false)
            return;

        Vector3 playerPos = Managers.Instance.ObjectManager.Player.transform.position;
        Vector3 tilePos = transform.position;

        float diffX = Mathf.Abs(playerPos.x - tilePos.x);
        float diffZ = Mathf.Abs(playerPos.z - tilePos.z);

        Vector3 dir = Managers.Instance.ObjectManager.Player.MoveDir;
        float dirX = dir.x < 0 ? -1 : 1;
        float dirY = dir.y < 0 ? -1 : 1;

        if (diffX > diffZ)
            transform.Translate(Vector3.right * dirX * 120);
        else if(diffX <diffZ)
            transform.Translate(Vector3.forward * dirY * 120);
    }

}
