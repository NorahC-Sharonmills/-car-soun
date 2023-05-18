using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ScreenTutorial : UICanvas
{
    [Header("-----------------------")]
    public Transform tab;
    private Transform tabSelectTraform;
    private int tabSelect;

    public Transform btnSkip;
    public Transform center;

    public override void Show()
    {
        base.Show();
        tabSelect = 0;
        tabSelectTraform = tab.FindChildByParent("Select");
        tabSelectTraform.localPosition = tab.GetChild(tabSelect).localPosition;
    }

    public void Next()
    {
        tabSelect += 1;
        if(tabSelect <= 2)
        {
            var nextTranform = tab.GetChild(tabSelect);
            tabSelectTraform.DOKill();
            tabSelectTraform.DOLocalMoveX(nextTranform.localPosition.x, 0.5f);

            var nowTab = center.GetChild(tabSelect - 1);
            var nextTab = center.GetChild(tabSelect);
            nextTab.localPosition = nextTab.localPosition.WithX(1500);
            nextTab.SetActive(true);

            nowTab.DOKill();
            nowTab.DOLocalMoveX(-1500f, 0.5f).OnComplete(() => nowTab.SetActive(false));
            nextTab.DOKill();
            nextTab.DOLocalMoveX(0f, 0.5f);

            if (tabSelect == 2)
                btnSkip.SetActive(false);
        }

        if(tabSelect == 3)
        {
            AdmobManager.Instance.ShowInter("InterTutorialId", () =>
            {
                ScreenHelper.FindScriptAndHide<ScreenSelect>().Show();
            }, () =>
            {
                AdmobManager.Instance.IsTutorial = false;
                PrefManager.SetBool(Manager.IsTutorial, true);
                Manager.Instance.CompleteOpenAd(0.01f);
            });
        }
    }

    public void Skip()
    {
        ScreenHelper.FindScriptAndHide<ScreenSelect>().Show();
        AdmobManager.Instance.IsTutorial = false;
        PrefManager.SetBool(Manager.IsTutorial, true);
        Manager.Instance.CompleteOpenAd();
    }
}
