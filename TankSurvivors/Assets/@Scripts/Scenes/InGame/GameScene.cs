using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Managers.Instance.ResourceManager.LoadAllAsyncWithLabel<Object>("preload", (key, count, totlaCount) =>
        {
            if(count == totlaCount) 
            {
                StartLoaded();       
            }
        });
    }
    private void StartLoaded()
    {
        var player = Managers.Instance.ObjectManager.Spawn<PlayerController>(Vector3.zero);
        Camera.main.GetComponent<CameraController>().Init(player.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
