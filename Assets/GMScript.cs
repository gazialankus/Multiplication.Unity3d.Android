using UnityEngine;
using System.Collections;

public class GMScript : MonoBehaviour
{

    string url = "YOURURL/api/hello";

    // Use this for initialization
    IEnumerator Start()
    {
        Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Large);
        Handheld.StartActivityIndicator();
        WWW www = new WWW(url);
        while (!www.isDone)
        {
            yield return null;
        }

        PlayerPrefs.SetString("score", www.text);
        Handheld.StopActivityIndicator();
        Application.LoadLevel(1);
    }
}
