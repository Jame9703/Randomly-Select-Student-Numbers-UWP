using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;

namespace 随机抽取学号.Classes
{
    //public class SettingsHelper
    //{
    //    public static bool NoReturn;
    //    public static bool AutoStop;
    //    public static bool Optimize;
    //    public static bool SaveRange;
    //    public static bool SaveHistory;
    //    public static int Theme;
    //    public static int MainPageBackground;
    //    public static double MainPageBackgroundOpacity;
    //    public static int ContentFrameBackground;
    //    public static double ContentFrameBackgroundOpacity;
    //    public static int MainPageBackground;
    //}
    public class SettingsHelper:INotifyPropertyChanged
    {
        private static readonly ApplicationDataContainer _localSettings =
            ApplicationData.Current.LocalSettings;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            SaveSetting(propertyName);  // 属性变更时触发保存
        }

        // 默认值定义区域
        private const bool DefaultDarkMode = false;
        private const int DefaultFontSize = 14;
        private const string DefaultLanguage = "en-US";

        static SettingsHelper()
        {
            // 初始化时自动填充缺失的配置项
            //EnsureSetting(nameof(IsDarkMode), DefaultDarkMode);
            //EnsureSetting(nameof(FontSize), DefaultFontSize);
            //EnsureSetting(nameof(AppLanguage), DefaultLanguage);
        }
        protected void SaveSetting(string key)
        {
            var property = this.GetType().GetProperty(key);
            if (property != null)
            {
                _localSettings.Values[key] = property.GetValue(this);
            }
        }
        /// <summary>
        /// 确保配置项存在，不存在时写入默认值
        /// </summary>
        //private static void EnsureSetting<T>(string key, T defaultValue)
        //{
        //    if (!_localSettings.Values.ContainsKey(key))
        //    {
        //        _localSettings.Values[key] = defaultValue;
        //    }
        //}

        /// <summary>
        /// 泛型方法获取配置值
        /// </summary>
        private static T GetValue<T>(string key, T defaultValue = default)
        {
            if (_localSettings.Values.TryGetValue(key, out object value))
            {
                try
                {
                    return (T)value;
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// 泛型方法设置配置值
        /// </summary>
        //private static void SetValue<T>(string key, T value)
        //{
        //    _localSettings.Values[key] = value;
        //}
        protected T GetSetting<T>(T defaultValue, [CallerMemberName] string key = null)
        {
            if (_localSettings.Values.TryGetValue(key, out object value))
            {
                return (T)value;
            }
            return defaultValue;
        }
        private string _userName;
        private int _fontSize;

        public string UserName
        {
            get => GetSetting("DefaultUser");
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged();
                }
            }
        }

        public int FontSize
        {
            get => GetSetting(14);
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 重置单个配置到默认值
        /// </summary>
        public static void ResetToDefault(string key)
        {
            switch (key)
            {
                //case nameof(IsDarkMode):
                //    SetValue(key, DefaultDarkMode);
                //    break;
                //case nameof(FontSize):
                //    SetValue(key, DefaultFontSize);
                //    break;
                //case nameof(AppLanguage):
                //    SetValue(key, DefaultLanguage);
                //    break;
            }
        }

        /// <summary>
        /// 重置所有配置到默认值
        /// </summary>
        public static void ResetAll()
        {
            //IsDarkMode = DefaultDarkMode;
            //FontSize = DefaultFontSize;
            //AppLanguage = DefaultLanguage;
        }
    }
}
