using EFT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmuLib.MaociDebugger
{
    class insGameWorld : MonoBehaviour
    {
        private static GameWorld aGameWorld = null;
        void Update()
        {
            intFunctions.FindGameWorld();
        }
        //void OnGUI(){} // not used in gameworld instance
        public static class Access {
            public static bool IsGameWorldActive() {
                if (aGameWorld != null)
                    return true;
                return false;
            }
            public static GameWorld getGameWorld() {
                return aGameWorld;
            }
        }
        private static class intFunctions {
            public static void FindGameWorld()
            { // Function to find GameWorld Instrance - if its not acctive aGameWorld will be null
                if (Comfort.Common.Singleton<GameWorld>.Instantiated)
                {
                    aGameWorld = Comfort.Common.Singleton<GameWorld>.Instance;
                    return;
                }
                aGameWorld = null;
            }
        
        }
    }
}
