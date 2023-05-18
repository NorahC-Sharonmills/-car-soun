using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoSingletonGlobal<Manager>
{
    public ScreenSplash screenSplash;

    public static readonly string IsLanguage = "key_language_install";
    public static readonly string IsTutorial = "key_tutorial_install";
    public static readonly string FirstCreateData = "key_first_create_data";

    private bool createData = false;
    private float timerCreateData = 0;
    protected override void Awake()
    {
        base.Awake();
        if (PrefManager.GetBool(FirstCreateData, true))
        {
            PrefManager.SetBool(FirstCreateData, false);
            RuntimeStorageData.CreateData();
        }
        else
            RuntimeStorageData.ReadData();

    }

    private IEnumerator Start()
    {
        createData = true;
        screenSplash.Show();
        yield return AdmobManager.Instance.AdmobInitialized();
        yield return new WaitUntil(() => RuntimeStorageData.IsReady);
        createData = false;
        yield return FirebaseManager.Instance.InitializedFirebase();
        SceneManager.LoadScene("Game");
    }

    private void Update()
    {
        if(createData)
        {
            timerCreateData += Time.deltaTime;
            if (timerCreateData > 15f)
            {
                Debug.Log("fork data because load data false");
                createData = false;
                RuntimeStorageData.CreateData();
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            RuntimeStorageData.SaveAllData();
    }

    private void OnApplicationQuit()
    {
        RuntimeStorageData.SaveAllData();
    }

    public void HideSplash()
    {
        screenSplash.Hide();
    }

    public void CompleteOpenAd()
    {
        HideSplash();
        bool IsLanguage = PrefManager.GetBool(Manager.IsLanguage);
        bool IsTutorial = PrefManager.GetBool(Manager.IsTutorial);
        AdmobManager.Instance.IsBannerAd = IsLanguage && IsTutorial;
        if (IsLanguage && IsTutorial)
            AdmobManager.Instance.ConitinuteShowBanner();
#if UNITY_EDITOR
        if (IsLanguage && IsTutorial) Debug.Log("Complete Open Ad And Show Banner");
        else Debug.Log("Complete Open Ad");
#endif 
    }

    public void CompleteOpenAd(float timer)
    {
        HideSplash();
        bool IsLanguage = PrefManager.GetBool(Manager.IsLanguage);
        bool IsTutorial = PrefManager.GetBool(Manager.IsTutorial);
        AdmobManager.Instance.IsBannerAd = IsLanguage && IsTutorial;
        if (IsLanguage && IsTutorial)
            AdmobManager.Instance.ContinuteShowBannerAfter(timer);
#if UNITY_EDITOR
        if (IsLanguage && IsTutorial) Debug.Log("Complete Open Ad And Show Banner");
        else Debug.Log("Complete Open Ad");
#endif 
    }

}
