using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    public class CalibrationProfilesManager
    {
        private static CalibrationProfilesManager _calibrationProfilesManager;
        public static CalibrationProfilesManager Instance
        {
            get
            {
                if (_calibrationProfilesManager == null)
                    _calibrationProfilesManager = new CalibrationProfilesManager();
                return _calibrationProfilesManager;
            }
        }

        private List<CalibrationProfile> _calibrationProfiles;
        public List<CalibrationProfile> CalibrationProfiles
        {
            get => _calibrationProfiles;
        }

        public static string CalibrationProfilesFolder
        {
            // %USERPROFILE%/AvatarReadyProfiles , ie. 'C:/Users/<name>/AvatarReadyProfiles'
            get => Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "AvatarReadyProfiles");
        }


        private CalibrationProfilesManager()
        {
            if (!Directory.Exists(CalibrationProfilesFolder))
                Directory.CreateDirectory(CalibrationProfilesFolder);

            string[] calibrationProfileFiles = Directory.GetFiles(CalibrationProfilesFolder);

            if (_calibrationProfiles == null)
                _calibrationProfiles = new List<CalibrationProfile>();

            foreach (string calibrationProfileFile in calibrationProfileFiles)
            {
                _calibrationProfiles.Add(JsonUtility.FromJson<CalibrationProfile>(File.ReadAllText(calibrationProfileFile)));
            }
        }

        public void AddCalibrationProfile(CalibrationProfile calibrationProfile)
        {
            _calibrationProfiles.Add(calibrationProfile);
            SaveCalibrationProfile(calibrationProfile);
        }

        public void UpdateCalibrationProfile(CalibrationProfile calibrationProfile)
        {
            SaveCalibrationProfile(calibrationProfile);
        }

        public void DeleteCalibrationProfile(CalibrationProfile calibrationProfile)
        {
            _calibrationProfiles.Remove(calibrationProfile);

            // TODO add and store a GUID for the file name
            string fileName = string.Format("{0}.json", FilterName(calibrationProfile.Name));
            string filePath = Path.Combine(CalibrationProfilesFolder, fileName);
            File.Delete(filePath);
        }

        private void SaveCalibrationProfile(CalibrationProfile calibrationProfile)
        {
            string jsonUserProfile = JsonUtility.ToJson(calibrationProfile, true);

            string fileName = string.Format("{0}.json", FilterName(calibrationProfile.Name));
            string filePath = Path.Combine(CalibrationProfilesFolder, fileName);

            File.WriteAllText(filePath, jsonUserProfile);
        }

        public string FilterName(string name)
        {
            // TODO regex?
            // escape space and the invalid symbol in the windows filename
            return name.Replace(' ', '_').Replace('\\', '_').Replace('/', '_').Replace(':', '_')
                .Replace('*', '_').Replace('?', '_').Replace('"', '_').Replace('<', '_')
                .Replace('>', '_').Replace('|', '_');
        }
    }
}
