using Comfort.Common;
using EFT;
using EmuLib.Monitors;
using EmuLib.Utils.Camera;
using EmuLib.Utils.Reflection;
using UnityEngine;
using EmuLib.MaociDebugger;
using GClass_Config = GClass261;

namespace EmuLib
{
    public class EmuInstance : MonoBehaviour
    {
        private const float MonitorPeriod = 1f;
        private float _monitorNextTime;
        public static Player Player;
        private static MaociDebuggerMain m_maociDebugger;
        public void Start()
        {

            //start from creating maociDebugger
            m_maociDebugger = gameObject.AddComponent<MaociDebuggerMain>(); // you dont need to use variable but it looks better :/

            if (!Singleton<EmuInstance>.Instantiated)
                Singleton<EmuInstance>.Create(this);

            ConfigUtils.ResetConfig();
            GClass_Config.LoadApplicationConfig();
            
            Debug.LogError("EmuInstance Start() method.");
        }

        private void Update()
        {
            CameraUtils.CheckSwitchCameraCombination();
        }

        public void FixedUpdate()
        {
            AbstractGame game = Singleton<AbstractGame>.Instance;
            if (game == null) return;

            // run monitoring utils
            RunMonitoringWithPeriod(game);
            CreatePlayerOwnerMonitor.CheckCreatePlayerOwnerCallBack(game);
        }

        #region MonitorsRegion

        private void RunMonitoringWithPeriod(AbstractGame game)
        {
            if (Time.time < _monitorNextTime) return;
            _monitorNextTime = Time.time + MonitorPeriod;

            // saving profile progress
            GameFinishCallBackMonitor.CheckFinishCallBack(game);
        }

        #endregion
    }
}