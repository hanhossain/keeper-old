using System;
using Foundation;

namespace Keeper.iOS
{
    public class UserDefaults : IUserDefaults
    {
        private readonly NSUserDefaults _userDefaults = NSUserDefaults.StandardUserDefaults;

        public DateTime SleeperLastUpdated
        {
            get
            {
                var raw = _userDefaults.StringForKey(nameof(SleeperLastUpdated));
                return DateTime.TryParse(raw, out var result) ? result : DateTime.MinValue;
            }

            set
            {
                var raw = value.ToString("O");
                _userDefaults.SetString(raw, nameof(SleeperLastUpdated));
            }
        }
    }
}

