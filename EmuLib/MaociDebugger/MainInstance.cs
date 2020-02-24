using EFT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using EmuLib.MaociDebugger.objectClasses;

namespace EmuLib.MaociDebugger
{
    class MaociDebuggerMain : MonoBehaviour
    {
        private static bool MaociDebugger = false;
        private static GameWorld mGameWorld = null;
        private static insPlayers instance_Players = null;
        private static List<Player>.Enumerator tP_list;
        private static Player LocalPlayer = null;
        private static Rect DebugWindow = new Rect(10f, 10f, 150f, 600f);
        private static Vector3 AimedAt = Vector3.zero;

        private static bool enableMenu = false;
        
        void Awake()
        {
            MaociDebugger = File.Exists(Path.GetPathRoot(Environment.SystemDirectory) + "maoci");
            if (!MaociDebugger) return;
            instance_Players = gameObject.AddComponent<insPlayers>(); // all players things instance
        }
        void Update()
        {
            if (!MaociDebugger) return;
            if (Input.GetKeyDown(KeyCode.Insert))
                enableMenu = !enableMenu;

            if (mGameWorld != null)
            {
                tP_list = mGameWorld.RegisteredPlayers.GetEnumerator();
                
            }
        }
        private static Rect info = new Rect(0f, Screen.height - 42f, 100f, 25f);
        private static Vector2 AimingAtScreenVector = Vector2.zero;
        private static Color Black = new Color(0f, 0f, 0f, 1f);
        private static Color White = new Color(1f, 1f, 1f, 1f);
        void OnGUI()
        {
            if (!MaociDebugger) return;
            
            if (enableMenu)
                GUI.Label(info, "DebugMenuActive!");

            if (!Comfort.Common.Singleton<GameWorld>.Instantiated)
                return; // do not proceede if there is no game worlds
            if (enableMenu)
            {
                DebugWindow = GUILayout.Window(0, DebugWindow, DrawMenu, new GUIContent("Maoci's DebugMenu"));
                #region performance draw aiming at place
                AimingAtScreenVector = Camera.main.WorldToScreenPoint(AimedAt);
                AimingAtScreenVector.x -= 2f;
                AimingAtScreenVector.y = Screen.height - AimingAtScreenVector.y - 2f;
                DrawPixel(AimingAtScreenVector, Black, 4f);
                AimingAtScreenVector.x += 1f;
                AimingAtScreenVector.y += 1f;
                DrawPixel(AimingAtScreenVector, White, 2f);
                #endregion
            }

        }
        private static Rect DragWindowDebugWindow = new Rect(0, 0, DebugWindow.width, 20);
        void DrawMenu(int id)
        {
            try
            {
                switch (id)
                {
                    case 0:
                        GUI.DragWindow(DragWindowDebugWindow);
                        GUILayout.Label(" - Players: " + mGameWorld.RegisteredPlayers.Count.ToString() + " - ");
                        insPlayers.SWITCH.enable = GUILayout.Toggle(insPlayers.SWITCH.enable, " - Player ESP - ");
                        GUILayout.Label(" - Aiming at - ");
                        GUILayout.Label(" x: " + AimedAt.x.ToString());
                        GUILayout.Label(" y: " + AimedAt.y.ToString());
                        GUILayout.Label(" z: " + AimedAt.z.ToString());
                        GUILayout.Label(" - PlayerPosition - ");
                        GUILayout.Label(" x: " + LocalPlayer.Transform.position.x.ToString());
                        GUILayout.Label(" y: " + LocalPlayer.Transform.position.y.ToString());
                        GUILayout.Label(" z: " + LocalPlayer.Transform.position.z.ToString());
                        GUILayout.Label(" - List - ");
                        if (PlayerCounters.Table.cursedAssault > 0) GUILayout.Label(" curseAssault: " + PlayerCounters.Table.cursedAssault.ToString());
                        if (PlayerCounters.Table.assault > 0) GUILayout.Label(" Assault: " + PlayerCounters.Table.assault.ToString());
                        if (PlayerCounters.Table.marksman > 0) GUILayout.Label(" marksman: " + PlayerCounters.Table.marksman.ToString());
                        if (PlayerCounters.Table.pmcBot > 0) GUILayout.Label(" pmcBot: " + PlayerCounters.Table.pmcBot.ToString());
                        if (PlayerCounters.Table.bossBully > 0) GUILayout.Label(" Bully: " + PlayerCounters.Table.bossBully.ToString());
                        if (PlayerCounters.Table.followerBully > 0) GUILayout.Label(" followerBully: " + PlayerCounters.Table.followerBully.ToString());
                        if (PlayerCounters.Table.bossGluhar > 0) GUILayout.Label(" Gluhar: " + PlayerCounters.Table.bossGluhar.ToString());
                        if (PlayerCounters.Table.followerGluharAssault > 0) GUILayout.Label(" f_GluharAssault: " + PlayerCounters.Table.followerGluharAssault.ToString());
                        if (PlayerCounters.Table.followerGluharScout > 0) GUILayout.Label(" f_GluharScout: " + PlayerCounters.Table.followerGluharScout.ToString());
                        if (PlayerCounters.Table.followerGluharSecurity > 0) GUILayout.Label(" f_GluharSecurity: " + PlayerCounters.Table.followerGluharSecurity.ToString());
                        if (PlayerCounters.Table.followerGluharSnipe > 0) GUILayout.Label(" f_GluharSnipe: " + PlayerCounters.Table.followerGluharSnipe.ToString());
                        if (PlayerCounters.Table.bossKilla > 0) GUILayout.Label(" Killa: " + PlayerCounters.Table.bossKilla.ToString());
                        if (PlayerCounters.Table.bossKojaniy > 0) GUILayout.Label(" Kojaniy: " + PlayerCounters.Table.bossKojaniy.ToString());
                        if (PlayerCounters.Table.followerKojaniy > 0) GUILayout.Label(" f_Kojaniy: " + PlayerCounters.Table.followerKojaniy.ToString());
                        if (PlayerCounters.Table.bossStormtrooper > 0) GUILayout.Label(" Stormtrooper: " + PlayerCounters.Table.bossStormtrooper.ToString());
                        if (PlayerCounters.Table.followerStormtrooper > 0) GUILayout.Label(" f_Stormtrooper: " + PlayerCounters.Table.followerStormtrooper.ToString());
                        if (PlayerCounters.Table.bossTest > 0) GUILayout.Label(" Test: " + PlayerCounters.Table.bossTest.ToString());
                        if (PlayerCounters.Table.followerTest > 0) GUILayout.Label(" followerTest: " + PlayerCounters.Table.followerTest.ToString());
                        if (PlayerCounters.Table.test > 0) GUILayout.Label(" test: " + PlayerCounters.Table.test.ToString());
                        break;
                    default:
                        break;
                }
            }
            catch { }
        }



        private static Texture2D lineTex = new Texture2D(1, 1);
        private static Color savedColor;
        private static float yOffset = 0f;
        public static void DrawPixel(Vector2 Position, Color color, float thickness)
        {
            if (!lineTex) { lineTex = new Texture2D(1, 1); } // incase it somehow clears itself ...
            yOffset = Mathf.Ceil(thickness / 2f);
            savedColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(new Rect(Position.x, Position.y - (float)yOffset, thickness, thickness), lineTex);
            GUI.color = savedColor;
        }
    }
}