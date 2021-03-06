﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
[RequireComponent(typeof(VideoPlayer))]
public class ListController : MonoBehaviour {

    [SerializeField]
    private ARManager arManager;
    //Info
    public string Info;
    //public TextAsset Info;
    private Product[] products;
    [SerializeField]
    private List<Button> buttonList = new List<Button>();
    [SerializeField]
    private List<Sprite> thumbnailList = new List<Sprite>();
    [SerializeField]
    private CanvasGroup LandingPage;
    [SerializeField]
    private CanvasGroup infoPanel;
    [SerializeField]
    private Text infoHeader;
    [SerializeField]
    private Image infoThumbnail;
    [SerializeField]
    private Text infoDesc;


    [SerializeField]
    private AudioSource sound;
    [SerializeField]
    private AudioClip clickSound;
    //Video
    private VideoPlayer videoPlayer;
    private VideoClip videoClip;
    [SerializeField]
    private CanvasGroup videoPanel;
    [SerializeField]
    private CanvasGroup productList;
    [SerializeField]
    private Text Header;
    private bool isInfo = true;
    [SerializeField]
    private Slider videoTimeSlider;

    [SerializeField]
    private Sprite play;
    [SerializeField]
    private Sprite pause;
    [SerializeField]
    private Image videoControl;

    [SerializeField]
    private CanvasGroup navbarPanel;
    [SerializeField]
    private CanvasGroup videoControlPanel;
    public enum Page
    {
        Landing,
        List,
        Video,
        Info,
        None
    }
    Dictionary<Page, CanvasGroup> PageToCanvasGroup = new Dictionary<Page, CanvasGroup>();
    private void Start()
    {
        PageToCanvasGroup.Add(Page.Landing, LandingPage);
        PageToCanvasGroup.Add(Page.List, productList);
        PageToCanvasGroup.Add(Page.Video, videoPanel);
        PageToCanvasGroup.Add(Page.Info, infoPanel);

        videoPlayer = GetComponent<VideoPlayer>();
        //videoPlayer.SetTargetAudioSource(GetComponent<AudioSource>());
        products = JsonUtility.FromJson<Info>(Info).ProductList;
        for (int i = 0; i < buttonList.Count; i++)
        {
            int buf = i;
            buttonList[i].onClick.AddListener(() =>
            {
                sound.PlayOneShot(clickSound);
                if (isInfo)
                {
                    infoHeader.text = products[buf].Header;
                    infoThumbnail.sprite = thumbnailList[buf];
                    infoDesc.text = products[buf].Desc;

                    infoPanel.alpha = 1;
                    infoPanel.interactable = true;
                    infoPanel.blocksRaycasts = true;

                    Canvas.ForceUpdateCanvases();
                }
                else
                {
						Debug.Log("Click : " + buf);
                    Play("SmartLab0" + (buf+1));
                }
            });
        }

        videoTimeSlider.onValueChanged.AddListener((time) =>{
            if (videoPlayer.canSetTime &&( time < videoPlayer.time - 2 || time > videoPlayer.time + 2)) {
                videoPlayer.time = time;
                }
        });
    }

    public void ShowLadingPage()
    {
        ShowPage(Page.Landing);
    }

    public void ShowPage(Page page)
    {
        foreach (KeyValuePair<Page, CanvasGroup> pair in PageToCanvasGroup)
        {
            if(page == pair.Key)
            {
                pair.Value.alpha = 1;
                if (page != Page.Landing)
                {
                    pair.Value.interactable = true;
                    pair.Value.blocksRaycasts = true;
                }
            }
            else{
                pair.Value.alpha = 0;
                if (page != Page.Landing)
                {
                    pair.Value.interactable = false;
                    pair.Value.blocksRaycasts = false;
                }
            }
        }
    }

    void HidePage(Page page)
    {
        foreach (KeyValuePair<Page, CanvasGroup> pair in PageToCanvasGroup)
        {
            if (page == pair.Key)
            {
                pair.Value.alpha = 1;
                pair.Value.interactable = false;
                pair.Value.blocksRaycasts = false;
            }
        }
    }

    public void StartAR()
    {
        ShowPage(Page.Landing);
        sound.PlayOneShot(clickSound);
        if (!arManager.IsRunning)
        {
            sound.PlayOneShot(clickSound);
            arManager.LoadAR(LandingPage);
        }
    }

    public void StopAR()
    {
        if (arManager != null && arManager.isActiveAndEnabled && arManager.IsRunning)
        {
            arManager.StopAR();
        }
    }

    public void ShowInfoList(bool isOn)
    {
        if(isOn)
        {
            sound.PlayOneShot(clickSound);
            StopAR();
            ShowInfoList();
        }
    }
    public void ShowInfoList()
    {
        isInfo = true;
        Header.text = "ผลิตภัณฑ์ของเรา";
        ShowPage(Page.List);
    }
    public void ShowVideoList(bool isOn)
    {
        if (isOn)
        {
            sound.PlayOneShot(clickSound);
            StopAR();
            ShowVideoList();
        }
    }
    public void ShowVideoList()
    {
        isInfo = false;
        Header.text = "วิดีโอ";
        ShowPage(Page.List);
    }

    public void Play(string clipName)
    {
        //string path = ContentController.ContentDirectory + "/" + clipName;
        videoClip = Resources.Load<VideoClip>(clipName);
        if(videoClip != null)
        {
            videoPlayer.clip = videoClip;
            videoPlayer.EnableAudioTrack(0, true);
            videoPlayer.SetTargetAudioSource(0, GetComponent<AudioSource>());
            videoPlayer.Play();
            videoPanel.alpha = 1;
            videoPanel.interactable = true;
            videoPanel.blocksRaycasts = true;

            videoControl.sprite = pause;

            videoTimeSlider.maxValue = (float)videoClip.length;



            ShowPage(Page.Video);

            FadeIn();

            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = true;

            navbarPanel.alpha = 0;
            navbarPanel.interactable = false;
            navbarPanel.blocksRaycasts = false;
        }
    }

    public void StopVideo()
    {
        FadeCount = 0;
        videoPlayer.Stop();
        videoPlayer.clip = null;
        if (!arManager.IsRunning)
        {
            ShowPage(Page.List);
        }
        else
        {
            ShowPage(Page.None);
        }
        //Destroy(videoClip);
        videoClip = null;
        Resources.UnloadUnusedAssets();
        Screen.orientation = ScreenOrientation.Portrait;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = false;

        navbarPanel.alpha = 1;
        navbarPanel.interactable = true;
        navbarPanel.blocksRaycasts = true;
    }
    public bool IsPlaying()
    {
        //return productList.interactable;
        return videoPlayer.clip != null;
    }
    
    private void Update()
    {
        if (videoPlayer.isPlaying)
        {
            videoTimeSlider.value = (float)videoPlayer.time;
            
        }
        if(fade==null && Input.GetMouseButtonDown(0) && videoPlayer.clip != null)
        {
           FadeIn();
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                FadeCount = FadeCountMax;
            }
        }
    }
    

    public void PlayOrPause()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            videoControl.sprite = play;
        }
        else
        {
            videoPlayer.Play();
            videoControl.sprite = pause;
        }
    }

    public void FadeIn()
    {
        //Debug.Log("fadein");
        LeanTween.alphaCanvas(videoControlPanel, 1, 0.4f);
        videoControlPanel.interactable = true;
        videoControlPanel.blocksRaycasts = true;
        fade  = StartCoroutine(WaitForFadeOut());
    }
    private Coroutine fade;
    private float FadeCount;
    private const float FadeCountMax = 3;
    IEnumerator WaitForFadeOut()
    {
        FadeCount = FadeCountMax;
        while (FadeCount > 0)
        {
            yield return new WaitForSeconds(0.5f);
            FadeCount -= 0.5f;
        }
        LeanTween.alphaCanvas(videoControlPanel, 0, 0.4f);
        videoControlPanel.interactable = false;
        videoControlPanel.blocksRaycasts = false;

        fade = null;
        //Debug.Log("Fade out " + FadeCount);
        yield return true;
    }
}
