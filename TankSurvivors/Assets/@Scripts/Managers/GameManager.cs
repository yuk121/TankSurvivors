using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager 
{
    PlayerController Player { get { return Managers.Instance.ObjectManager.Player; } }
}
