using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace App
{
    public class AppUpdateCheck : MonoBehaviour
    {
        public static string RemoteVersion = "not downloaded yet";
        public static bool versionOk = true;
        public string ServerAddress = "https://www.myserver.com";

        private void Start()
        {
            StartCoroutine(GetVersion());
        }

        IEnumerator GetVersion()
        {
            UnityWebRequest www = UnityWebRequest.Get(ServerAddress + "/version/version.txt");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogWarning(www.error);
            }
            else
            {
                RemoteVersion = www.downloadHandler.text;
                if (Application.version == RemoteVersion)
                {
                    versionOk = true;
                }
                else
                {
                    versionOk = false;
                }
            }
        }
    }

}
