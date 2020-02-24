using EFT;
using EmuLib.MaociDebugger.objectClasses;
using EmuLib.Utils.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmuLib.MaociDebugger
{
    class insPlayers : MonoBehaviour
    {
        private static List<Player>.Enumerator gw_PlayersList;
        private static Player gw_LocalPlayer;
        private static List<Player> dbgPlayersList;
        private static bool waitTillLoaded = false;
        public static class SWITCH {
            public static bool enable = false;
        }
        private static class VAR {
            public static Player temporalUpdateObj;
            public static Player temporalDrawObj;
            public static int DistanceToObject = 0;
            public static Vector3 objectOnScreen = Vector3.zero;
        }
        void Update() {
            if (intFunctions.DisAllowToRun()) 
            {
                return; 
            }
            PlayerCounters.Table.Clear();
            dbgPlayersList = new List<Player>();
            gw_PlayersList = insGameWorld.Access.getGameWorld().RegisteredPlayers.GetEnumerator();
            while (gw_PlayersList.MoveNext())
            {
                VAR.temporalUpdateObj = gw_PlayersList.Current;
                if (VAR.temporalUpdateObj.PointOfView == EPointOfView.FirstPerson)
                { // this is 100% local player
                    gw_LocalPlayer = VAR.temporalUpdateObj;
                    //AimedAt = intFunctions.BarrelRaycast();
                    continue; // skip the rest of code cause we dont need that shit there ...
                }
                VAR.objectOnScreen = Camera.main.WorldToScreenPoint(VAR.temporalUpdateObj.Transform.position);
                if(intFunctions.objectStrictOnScreen(VAR.objectOnScreen))
                    dbgPlayersList.Add(VAR.temporalUpdateObj);
                intFunctions.CalculateBotsCount(VAR.temporalUpdateObj.Profile.Info.Settings.Role);// count proper bots counters on map
            }
            if (!waitTillLoaded) waitTillLoaded = !waitTillLoaded;
        }
        private static List<Player>.Enumerator draw_PlayerList;
        private static class tPlayer {
            public static string name;
            public static string health;
            public static int distance;
            public static float textAboveHead = 5f;
            public static Vector3 posOnScreen = Vector3.zero;
            public static GUIContent guiContent = new GUIContent();
            public static GUIStyle guiStyle = new GUIStyle() { fontSize = 12 };
        }
        void OnGUI() {
            if (intFunctions.DisAllowToRun() || !waitTillLoaded || !SWITCH.enable)
            {
                return;
            }
            draw_PlayerList = dbgPlayersList.GetEnumerator();
            while (gw_PlayersList.MoveNext())
            {
                VAR.temporalDrawObj = gw_PlayersList.Current;
                if (VAR.temporalDrawObj == null) continue;
                tPlayer.posOnScreen = Camera.main.WorldToScreenPoint(VAR.temporalDrawObj.PlayerBones.Head.position);
                tPlayer.textAboveHead = Camera.main.WorldToScreenPoint(VAR.temporalDrawObj.PlayerBones.Head.position).y - Camera.main.WorldToScreenPoint(VAR.temporalDrawObj.PlayerBones.Neck.position).y * 6f;
                tPlayer.distance = (int)Vector3.Distance(Camera.main.transform.position, VAR.temporalDrawObj.Transform.position);
                tPlayer.health = "[" + VAR.temporalDrawObj.HealthController.GetBodyPartHealth(EBodyPart.Common).Current.ToString() + " hp] " + tPlayer.distance.ToString() + "m";

                GUIDraw.Text.Draw(tPlayer.posOnScreen.x, Screen.height - tPlayer.posOnScreen.y - tPlayer.textAboveHead, 200f, 15f, "", Color.green, tPlayer.guiStyle, true);
            }
        }
        public static class Access { }
        private static class intFunctions {
            public static bool DisAllowToRun() {
                return !insGameWorld.Access.IsGameWorldActive();
            }
            public static void CalculateBotsCount(WildSpawnType type) {
                switch (type)
                {
                    #region Switch cases WildSpawnType
                    case WildSpawnType.assault: 
                        PlayerCounters.Table.assault++; break;
                    case WildSpawnType.bossBully: 
                        PlayerCounters.Table.bossBully++; break;
                    case WildSpawnType.bossGluhar: 
                        PlayerCounters.Table.bossGluhar++; break;
                    case WildSpawnType.bossKilla: 
                        PlayerCounters.Table.bossKilla++; break;
                    case WildSpawnType.bossKojaniy: 
                        PlayerCounters.Table.bossKojaniy++; break;
                    case WildSpawnType.bossStormtrooper: 
                        PlayerCounters.Table.bossStormtrooper++; break;
                    case WildSpawnType.bossTest: 
                        PlayerCounters.Table.bossTest++; break;
                    case WildSpawnType.cursedAssault: 
                        PlayerCounters.Table.cursedAssault++; break;
                    case WildSpawnType.followerBully: 
                        PlayerCounters.Table.followerBully++; break;
                    case WildSpawnType.followerGluharAssault: 
                        PlayerCounters.Table.followerGluharAssault++; break;
                    case WildSpawnType.followerGluharScout: 
                        PlayerCounters.Table.followerGluharScout++; break;
                    case WildSpawnType.followerGluharSecurity: 
                        PlayerCounters.Table.followerGluharSecurity++; break;
                    case WildSpawnType.followerGluharSnipe: 
                        PlayerCounters.Table.followerGluharSnipe++; break;
                    case WildSpawnType.followerKojaniy: 
                        PlayerCounters.Table.followerKojaniy++; break;
                    case WildSpawnType.followerStormtrooper: 
                        PlayerCounters.Table.followerStormtrooper++; break;
                    case WildSpawnType.followerTest: 
                        PlayerCounters.Table.followerTest++; break;
                    case WildSpawnType.marksman: 
                        PlayerCounters.Table.marksman++; break;
                    case WildSpawnType.pmcBot: 
                        PlayerCounters.Table.pmcBot++; break;
                    case WildSpawnType.test: 
                        PlayerCounters.Table.test++; break;
                    default: break;
                    #endregion
                }
            }
            public static bool objectStrictOnScreen(Vector3 V)
            {
                return (V.x > 0.01f && V.y > 0.01f && V.x < Screen.width && V.y < Screen.height && V.z > 0.01f);
            }
            private static RaycastHit raycastHit;
            private static int mask = 1 << 12 | 1 << 16 | 1 << 18; // added some mast feel free to add more :)
            public static Vector3 BarrelRaycast()
            {
                try
                {
                    if (gw_LocalPlayer != null && gw_LocalPlayer.Fireport == null)
                        return Vector3.zero;
                    Physics.Linecast(gw_LocalPlayer.Fireport.position, gw_LocalPlayer.Fireport.position - gw_LocalPlayer.Fireport.up * 1000f, out raycastHit, mask);
                    return raycastHit.point;
                }
                catch { return Vector3.zero; }
            }
        }
    }
}
