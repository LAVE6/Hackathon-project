using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoTool : MonoBehaviour
{
    // Rect region to take screenshot

    public int side;
    public int updown;
    public int offset;

    Texture2D tex2d_photo;
    Rect photoRegion;

    // An event which will be triggered when the photo is taken
    public delegate void PhotoTaken(Texture2D _photoTaken);
    public static event PhotoTaken OnPhotoTaken;

    public void TakePhoto()
    {


        StartCoroutine(E_TakePhoto());
    }

    IEnumerator E_TakePhoto()
    {
        #region Hide Debug Region If Needed

        //if (debugOnGUI)
        //{
        //    debugOnGUI = false;
        //    shouldTurnOnDebugOnGUI = true;
        //}
        //else
        //{
        //    shouldTurnOnDebugOnGUI = false;
        //}

        #endregion Hide Debug Region If Needed

        yield return new WaitForEndOfFrame();

        float ratio = (float)Screen.height / 1920f;
        tex2d_photo = new Texture2D((int)(side * 2 * ratio), (int)(updown * 2 * ratio), TextureFormat.RGB24, false);
        photoRegion = new Rect(
            (Screen.width / 2) - (side * ratio),
            (Screen.height / 2) - (updown * ratio) - (offset * ratio),
            (side * 2 * ratio),
            (updown * 2 * ratio)
        );

        tex2d_photo.ReadPixels(photoRegion, 0, 0);
        tex2d_photo.Apply();

        if (OnPhotoTaken != null)
            OnPhotoTaken(tex2d_photo);

        Debug.Log("Photo taken!");

        #region Turn On Debug Region If Needed

        if (shouldTurnOnDebugOnGUI)
        {
            debugOnGUI = true;
        }

        #endregion Turn On Debug Region If Needed
    }

    #region Debug Region to take photo

    public bool debugOnGUI;
    bool shouldTurnOnDebugOnGUI;
    public Texture debugRegionTex;
    public Color debugRegionColor = Color.white;

    void OnGUI()
    {
#if UNITY_EDITOR 
        // Leave bool debugOnGUI as is
#elif UNITY_STANDALONE
        debugOnGUI = false;
#endif
        if (debugOnGUI)
        {
            float ratio = (float)Screen.height / 1920f;
            photoRegion = new Rect(
                (Screen.width / 2) - (side * ratio),
                (Screen.height / 2) - (updown * ratio) + (offset * ratio),
                side * 2 * ratio,
                updown * 2 * ratio
            );

            GUI.color = debugRegionColor;
            GUI.DrawTexture(photoRegion, debugRegionTex);
        }
    }

    #endregion Debug Region to take photo
}
