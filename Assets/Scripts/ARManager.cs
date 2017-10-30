/*
 *  ARTrackedObject.cs
 *  ARToolKit for Unity
 *
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  ARToolKit for Unity is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with ARToolKit for Unity.  If not, see <http://www.gnu.org/licenses/>.
 *
 *  As a special exception, the copyright holders of this library give you
 *  permission to link this library with independent modules to produce an
 *  executable, regardless of the license terms of these independent modules, and to
 *  copy and distribute the resulting executable under terms of your choice,
 *  provided that you also meet, for each linked independent module, the terms and
 *  conditions of the license of that module. An independent module is a module
 *  which is neither derived from nor based on this library. If you modify this
 *  library, you may extend this exception to your version of the library, but you
 *  are not obligated to do so. If you do not wish to do so, delete this exception
 *  statement from your version.
 *
 *  Copyright 2015 Daqri, LLC.
 *  Copyright 2010-2015 ARToolworks, Inc.
 *
 *  Author(s): Philip Lamb
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(ARController))]
[ExecuteInEditMode]
public class ARManager : MonoBehaviour
{
    private ARController arController;
    private Dictionary<string, string> MarkerTagToVideo;
    [SerializeField]
    private ListController listController;
    private ARMarker[] arMarkers;
    [SerializeField]
    private GameObject LoadingPanel;
    [SerializeField]
    private Text LoadingText;
    private void Start()
    {
        arController = GetComponent<ARController>();
        arMarkers = FindObjectsOfType<ARMarker>();
        //StartCoroutine(Load());
    }
    public bool IsRunning
    {
        get
        {
            return arController.IsRunning;
        }
    }
    public void LoadAR(CanvasGroup canvasGroup)
    {
        if (!arController.enabled)
        {
            arController.enabled = true;
        }
        if (LoadingPanel != null && !LoadingPanel.activeSelf)
        {
            LoadingPanel.SetActive(true);
        }
        if (!arController.IsRunning)
        {
            StartCoroutine(Load(canvasGroup));
        }
    }

    public void StopAR()
    {
        LoadingPanel.SetActive(false);
        if (arController.IsRunning)
        {
            arController.StopAR();
        }
    }

    IEnumerator Load(CanvasGroup landingPageCG)
    {
        int loadingCount = 0;
        LoadingText.text = "0%";
        if (arMarkers.Length > 0)
        {
            foreach (ARMarker marker in arMarkers)
            {
                if (!marker.isActiveAndEnabled)
                {
                    marker.enabled = true;
                }
                loadingCount++;
                LoadingText.text = ((loadingCount * 100) / (arMarkers.Length + 1)).ToString() + "%";
                yield return new WaitForSeconds(0.7f);
            }
        }

        Debug.Log(arController.enabled + " : " + !arController.IsRunning);
        //yield return new WaitForSeconds(5);
        if (!arController.enabled)
        {
            arController.enabled = true;
        }
        else
        {
            if (!arController.IsRunning)
            {
                arController.StartAR();
            }
        }

        MarkerTagToVideo = new Dictionary<string, string>();
        MarkerTagToVideo.Add("1-A", "SmartLab02");
        MarkerTagToVideo.Add("1-B", "SmartLab02");
        MarkerTagToVideo.Add("2-A", "SmartLab08");
        MarkerTagToVideo.Add("2-B", "SmartLab08");
        MarkerTagToVideo.Add("3-A", "SmartLab07");
        MarkerTagToVideo.Add("3-B", "SmartLab07");
        MarkerTagToVideo.Add("4-A", "SmartLab01");
        MarkerTagToVideo.Add("4-B", "SmartLab01");
        MarkerTagToVideo.Add("5-A", "SmartLab03");
        MarkerTagToVideo.Add("5-B", "SmartLab03");
        MarkerTagToVideo.Add("6-A", "SmartLab05");
        MarkerTagToVideo.Add("6-B", "SmartLab05");
        MarkerTagToVideo.Add("7-A", "SmartLab04");
        MarkerTagToVideo.Add("7-B", "SmartLab04");
        MarkerTagToVideo.Add("8-A", "SmartLab06");
        MarkerTagToVideo.Add("8-B", "SmartLab06");
        MarkerTagToVideo.Add("8-C", "SmartLab06");



        

        //Destroy(LoadingPanel);
        //yield return new WaitUntil(() => arController.IsRunning);PluginFunctions.arwIsRunning()

        yield return new WaitUntil(() => PluginFunctions.arwIsRunning());
        LoadingText.text = "100%";
        yield return new WaitForSeconds(0.4f);
        LoadingPanel.SetActive(false);
        landingPageCG.alpha = 0;
        //yield return new WaitForSeconds(0.01f);
    }

    // Use LateUpdate to be sure the ARMarker has updated before we try and use the transformation.
    void LateUpdate()
    {
        // Update tracking if we are running in the Player.
        if (arMarkers != null && Application.isPlaying && !listController.IsPlaying()) {
            foreach (ARMarker marker in arMarkers)
            {
                if(marker.Visible && MarkerTagToVideo.ContainsKey(marker.Tag))
                {
                    Debug.Log("Found : " + marker.Tag);
                    listController.Play(MarkerTagToVideo[marker.Tag]);
                }
            }
        } 
    }

//    void ShowLoading()
//    {
//#if UNITY_IPHONE
//		Handheld.SetActivityIndicatorStyle(UnityEngine.iOS.ActivityIndicatorStyle.WhiteLarge);
//#elif UNITY_ANDROID
//        Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Small);
//#elif UNITY_TIZEN
//		Handheld.SetActivityIndicatorStyle(TizenActivityIndicatorStyle.Small);
//#endif

//        Handheld.StartActivityIndicator();
//    }

//    void HideLoading()
//    {
//        Handheld.StopActivityIndicator();
//    }
}

