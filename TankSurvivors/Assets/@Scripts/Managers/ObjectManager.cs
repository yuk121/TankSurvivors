using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public PlayerController Player { get; private set; }

    public T Spawn<T>(Vector3 position, int implementID = 0) where T :BaseController
    {
        System.Type type = typeof(T);

        if(type == typeof(PlayerController))
        {
            GameObject go = Managers.Instance.ResourceManager.Instantiate("PlayerPrefab/PlayerTank.prefab");
            go.name = "Player";
            go.transform.position = position;

            PlayerController pc = Utils.GetOrAddComponent<PlayerController>(go);
            Player = pc;
            pc.Init();

            return pc as T;
        }

        return null;
    }
}
