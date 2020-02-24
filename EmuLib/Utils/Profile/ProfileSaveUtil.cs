using EFT;
using EmuLib.Utils.HTTP;

// ReSharper disable All

namespace EmuLib.Utils.Profile
{
    internal static class ProfileSaveUtil
    {
        public static void SaveProfileProgress(EFT.Profile profileData, ExitStatus exitStatus, string session, bool isPlayerScav)
        {
            var request = new SaveProfileRequest { exit = exitStatus.ToString().ToLower(), profile = profileData, isPlayerScav = isPlayerScav };
            var requestData = request.ToJson();

            new HttpUtils.Create(session).Post("/OfflineRaidSave", requestData, true);
        }
    }

    public class SaveProfileRequest
    {
        public string exit = "left";
        public EFT.Profile profile;
        public bool isPlayerScav;
    }
}