using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DisatnceToPlayer {
  None,
  Near,
  Mid,
  Far,
}

[RequireComponent(typeof(BoxCollider))]
public class PlayerExtent : MonoBehaviour
{
  public DisatnceToPlayer Distance;
}
