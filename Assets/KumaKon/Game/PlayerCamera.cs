using System;
using System.Collections.Generic;
using UnityEngine;

namespace AirKuma {


  [ExecuteAlways, RequireComponent(typeof(Camera), typeof(BoxCollider))]
  public class PlayerCamera : MonoBehaviour {

    [Range(30, 90)]
    public float rotationX = 60f;
    [Range(10, 50)]
    public float VerticalFieldOfView = 30f;
    [Range(10, 40)]
    public float DistanceFromPlayer = 25f; // set to 25f for vertical viewable length 15.85 when rotationX = 60f, VerticalFieldOfView = 30f

    [SerializeField]
    public GameObject thePlayer;



    public Vector3 OffsetFromPlayer => Quaternion.Euler(-(180 - rotationX), 0, 0) * Vector3.forward * DistanceFromPlayer;

    public float DistanceFactor => (Mathf.Sin(15 * Mathf.Deg2Rad) / Mathf.Sin((180 - rotationX - (VerticalFieldOfView / 2f)) * Mathf.Deg2Rad)
        + Mathf.Sin(15 * Mathf.Deg2Rad) / Mathf.Sin((rotationX - (VerticalFieldOfView / 2f)) * Mathf.Deg2Rad));

    public float CalDistanceFromPlayer(float viewableVerticalLength) {
      return viewableVerticalLength / DistanceFactor;
    }
    public float CalVerticalViewableLength(float distanceFromPlayer) {
      return distanceFromPlayer * DistanceFactor;
    }

    private void OnEnable() {

      GetComponent<Camera>().orthographic = false;
      //GetComponent<Camera>().backgroundColor = Color.black;
      //GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
    }

    private void Start() {
      if (Application.isPlaying) { UnityEngine.Object.DontDestroyOnLoad(this); }
    }

    private void LateUpdate() {

      GetComponent<Camera>().transform.position = thePlayer.transform.position + OffsetFromPlayer;
      GetComponent<Camera>().transform.rotation = Quaternion.Euler(this.rotationX, 0, 0);
      GetComponent<Camera>().fieldOfView = VerticalFieldOfView;

    }

    //private void InterpolationUpdate() {
    //  interpolatingCameraPosition += (thePlayer.transform.position - interpolatingCameraPosition) * ApproachRate;
    //  GetComponent<Camera>().transform.position = interpolatingCameraPosition + OffsetFromPlayer;
    //}

  }

}