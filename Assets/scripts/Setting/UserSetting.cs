using System;

namespace Setting
{
    public class UserSetting
    {
        private static UserSetting _instance;

        public string UserID = "";
        public string DisplayName = "";
        public string ModelPath = "";
        public string ModelName = "";
        public string[] ModelParams = Array.Empty<string>();

        public static UserSetting Instance
        {
            get
            {
                return _instance ??= new UserSetting();
            }
        }

        private UserSetting(){}

        public static void Init()
        {
            _instance = null;
            _instance = new UserSetting();
        }
    }
}
