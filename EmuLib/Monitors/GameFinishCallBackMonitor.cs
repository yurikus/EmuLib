using System;
using System.Reflection;
using Comfort.Common;
using EFT;
using EmuLib.Utils.Profile;
using EmuLib.Utils.Reflection;
using UnityEngine;
using GClass_GameFinish = GClass1224;

namespace EmuLib.Monitors
{
    internal static class GameFinishCallBackMonitor
    {
        private static Callback<ExitStatus, TimeSpan, GClass_GameFinish> _gameCallBack;

        public static void CheckFinishCallBack(AbstractGame game)
        {
            FieldInfo callBackField = LocalGameUtils.GetFinishCallBack(game);

            if (callBackField == null) return;

            if (!(callBackField.GetValue(game) is Callback<ExitStatus, TimeSpan, GClass_GameFinish> finishCallBack)) return;

            if (finishCallBack.Method.Name == "OnGameFinish") return;

            _gameCallBack = finishCallBack;
            callBackField.SetValue(game, new Callback<ExitStatus, TimeSpan, GClass_GameFinish>(OnGameFinish));
        }

        private static void OnGameFinish(Result<ExitStatus, TimeSpan, GClass_GameFinish> result)
        {
            GInterface22 backend = ClientAppUtils.GetBackendSession();
            MainApplication mainApplication = MainAppUtils.GetMainApp();
            if (backend?.Profile == null || mainApplication == null)
            {
                _gameCallBack(result);
                return;
            }

            ESideType? eSideType = PrivateValueAccessor.GetPrivateFieldValue(mainApplication.GetType(),
                    "esideType_0", mainApplication) as ESideType?;
            Profile profile = backend.Profile;
            bool isPlayerScav = false;
            if (eSideType != null && eSideType.GetValueOrDefault() == ESideType.Savage)
            {
                profile = backend.ProfileOfPet;
                isPlayerScav = true;
            }

            try
            {
                CreatePlayerOwnerMonitor.Catched = false;
                ProfileSaveUtil.SaveProfileProgress(backend.Profile, result.Value0, backend.GetPhpSessionId(), isPlayerScav);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            _gameCallBack(result);
        }
    }
}