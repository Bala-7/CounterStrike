using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager s;

    public Transform cam;
    public Transform cam2;
    public Image flashTexture;
    public RawImage secondCamImage;

    private void Awake()
    {
        s = this;
    }


    public Transform GetCamTransform() { return cam; }

    public Transform GetFlashCamTransform() { return cam2; }

    public Image GetFlashTexture() { return flashTexture; }

    public RawImage GetFlashCamImage() { return secondCamImage; }

}
