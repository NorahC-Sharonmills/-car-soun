using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupRefill : UICanvas
{
    public void Refill()
    {
        if(FirebaseManager.Instance.IsRewardRefill)
        {
            AdmobManager.Instance.ShowReward("RewardRefillId", () =>
            {
                GameEvent.OnRefillMethod();
            }, () =>
            {
                Hide();
            }, () =>
            {
                Hide();
                var notification = PopupHelper.FindScript<PopupNotification>();
                notification.Initialized("warning", "No internet\nplease connect to refill gas");
            });
        }
        else
        {
            Hide();
            GameEvent.OnRefillMethod();
        }

    }
}
