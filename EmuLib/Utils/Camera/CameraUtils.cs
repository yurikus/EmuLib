using System;
using System.Collections.Generic;
using System.Reflection;
using EFT;
using EFT.HealthSystem;
using EFT.InventoryLogic;
using UnityEngine;
using static EmuLib.Utils.Reflection.PrivateValueAccessor;
using Object = UnityEngine.Object;
using HealthControllerInterface = GInterface134;
using HealthEffectInterface = GInterface106;
using BodyPartStruct = GStruct184;
using PlayerStatesInterface = GInterface106;
using StimulatorBuffEventInterface = GInterface105;
using HealthEffects = GClass1305;

namespace EmuLib.Utils.Camera
{
    public class CameraUtils
    {
        private static bool _leftShift;

        public static void CheckSwitchCameraCombination()
        {
            if (!EmuInstance.Player) return;

            if (Input.GetKeyDown(KeyCode.LeftShift))
                _leftShift = true;
            if (Input.GetKeyUp(KeyCode.LeftShift))
                _leftShift = false;

            if (_leftShift && (Input.GetKeyDown(KeyCode.KeypadPlus)))
            {
                SwitchCamera<Free>();
            }

            if (_leftShift && (Input.GetKeyDown(KeyCode.KeypadMinus)))
            {
                SwitchCamera<FreeCamera>();
            }
        }

        private static void SwitchCamera<T>() where T : FreeCamera
        {
            var disabled = EmuInstance.Player.PointOfView == EPointOfView.FirstPerson;

            EmuInstance.Player.PointOfView = disabled ? EPointOfView.FreeCamera : EPointOfView.FirstPerson;
            var playerCameraController = EmuInstance.Player.gameObject.GetComponent<PlayerCameraController>();
            var freeCamera = playerCameraController.Camera.gameObject.GetOrAddComponent<T>();

            if (!disabled)
            {
                Object.Destroy(freeCamera);
            }

            ReplaceHealthController();
        }

        private static void ReplaceHealthController()
        {
            FieldInfo healthControllerInfo = GetPrivateFieldInfo(EmuInstance.Player.GetType(), "_healthController");
            if (!(healthControllerInfo.GetValue(EmuInstance.Player) is HealthControllerInterface healthController)) return;

            if (Math.Abs(healthController.GetBodyPartHealth(EBodyPart.Chest).Current - float.MaxValue) <= 0f)
            {
                healthControllerInfo.SetValue(EmuInstance.Player, _previousHealthController);
                return;
            }

            _previousHealthController = healthController;
            healthControllerInfo.SetValue(EmuInstance.Player, new ReplaceHealthController());
        }

        private static HealthControllerInterface _previousHealthController;
    }

    internal class ReplaceHealthController : HealthControllerInterface
    {
        public BodyPartStruct GetBodyPartHealth(EBodyPart bodyPart, bool rounded = false)
        {
            return new BodyPartStruct { Current = float.MaxValue, Maximum = float.MaxValue};
        }

        public bool IsBodyPartBroken(EBodyPart bodyPart)
        {
            return false;
        }

        public bool IsBodyPartDestroyed(EBodyPart bodyPart)
        {
            return false;
        }

        public void GetBodyPartsInCriticalCondition(float treshold, out int all, out int vital)
        {
            all = 0;
            vital = 0;
        }

        public TEffect FindExistingEffect<TEffect>(EBodyPart bodyPart = EBodyPart.Common) where TEffect : PlayerStatesInterface
        {
            return (TEffect) (object) null;
        }

        public TEffect FindActiveEffect<TEffect>(EBodyPart bodyPart = EBodyPart.Common) where TEffect : PlayerStatesInterface
        {
            return (TEffect) (object) null;
        }

        public PlayerStatesInterface[] GetAllEffects(EBodyPart bodyPart)
        {
            return new PlayerStatesInterface[] { };
        }

        public PlayerStatesInterface[] GetActiveEffects(EBodyPart bodyPart)
        {
            return new PlayerStatesInterface[] { };
        }

        public PlayerStatesInterface[] GetResidualEffects(EBodyPart bodyPart)
        {
            return new PlayerStatesInterface[] { };
        }

        public IEnumerable<TEffect> FindActiveEffects<TEffect>(EBodyPart bodyPart = EBodyPart.Common)
            where TEffect : PlayerStatesInterface
        {
            return new List<TEffect>();
        }

        public bool IsItemForHealing(Item item)
        {
            return false;
        }

        public EBodyPart? CanApplyItem(Item item, EBodyPart bodyPart)
        {
            return null;
        }

        public void ApplyItem(Item item, EBodyPart bodyPart, float? amount = null)
        {
        }

        public void CancelApplyingItem()
        {
        }

        public void ManualUpdate(float delatTime)
        {
        }

        public void PropagateAllEffects()
        {
        }

        public bool IsAlive
        {
            get { return true; }
        }

        public BodyPartStruct Energy => new BodyPartStruct() {Current = float.MaxValue, Maximum = float.MaxValue};

        public BodyPartStruct Hydration
        {
            get { return new BodyPartStruct() {Current = float.MaxValue, Maximum = float.MaxValue}; }
        }

        public float HealthRate
        {
            get { return float.MaxValue; }
        }

        public float EnergyRate
        {
            get { return float.MaxValue; }
        }

        public float HydrationRate
        {
            get { return float.MaxValue; }
        }

        public float DamageCoeff
        {
            get { return float.MaxValue; }
        }

        public EFT.Player Player => EmuInstance.Player;

        public HealthEffects BodyPartEffects
        {
            get { return new HealthEffects(); }
        }

        public event Action<HealthEffectInterface> EffectAddedEvent;
        public event Action<HealthEffectInterface> EffectStartedEvent;
        public event Action<HealthEffectInterface> EffectResidualEvent;
        public event Action<HealthEffectInterface> EffectRemovedEvent;
        public event Action<EBodyPart, float, EDamageType> ApplyDamageEvent;
        public event Action<EBodyPart, float, HealthEffectInterface> HealthChangedEvent;
        public event Action<float> EnergyChangedEvent;
        public event Action<float> HydrationChangedEvent;
        public event Action<EBodyPart, EDamageType> BodyPartDestroyedEvent;
        public event Action<EDamageType> DiedEvent;
        public event Action<HealthEffectInterface> HealerDoneEvent;
        public event Action<Vector3, float, float> BurnEyesEvent;
        public event Action<StimulatorBuffEventInterface> StimulatorBuffEvent;
    }
}