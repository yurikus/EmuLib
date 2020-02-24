using System;
using System.Reflection;
using EFT;
using EmuLib.Utils.Player;
using EmuLib.Utils.Reflection;
using UnityEngine;

namespace EmuLib.Monitors
{
    internal static class CreatePlayerOwnerMonitor
    {
        private static Func<Player, GamePlayerOwner> _createOwnerFunc;
        public static bool Catched;

        public static void CheckCreatePlayerOwnerCallBack(AbstractGame game)
        {
            if (Catched) return;

            FieldInfo createPlayerOwnerInfo = LocalGameUtils.GetCreatePlayerOwnerFunc(game);
            if (createPlayerOwnerInfo == null) return;

            if (!(createPlayerOwnerInfo.GetValue(game) is Func<Player, GamePlayerOwner> createPlayerOwnerFunc)) return;

            if (createPlayerOwnerFunc.Method.Name == "CreateOwner") return;

            _createOwnerFunc = createPlayerOwnerFunc;
            createPlayerOwnerInfo.SetValue(game, new Func<Player, GamePlayerOwner>(CreateOwner));
            Catched = true;
        }

        private static GamePlayerOwner CreateOwner(Player player)
        {
            try
            {
                player.HealthController.HealthChangedEvent += HealthStateUtils.OnHealthChangedEvent;
                player.HealthController.HydrationChangedEvent += HealthStateUtils.OnHydrationChangedEvent;
                player.HealthController.EnergyChangedEvent += HealthStateUtils.OnEnergyChangedEvent;
                player.HealthController.EffectAddedEvent += HealthStateUtils.OnEffectAddedEvent;
                player.HealthController.EffectRemovedEvent += HealthStateUtils.OnEffectRemovedEvent;
                player.HealthController.BodyPartDestroyedEvent += HealthStateUtils.OnBodyPartDestroyedEvent;
                player.HealthController.DiedEvent += HealthStateUtils.OnDiedEvent;
                EmuInstance.Player = player;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return _createOwnerFunc(player);
        }
    }
}