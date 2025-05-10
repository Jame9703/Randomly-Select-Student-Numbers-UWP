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
    public class SettingsHelper
    {
        private static readonly ApplicationDataContainer _localSettings =
            ApplicationData.Current.LocalSettings;

        // 默认值定义区域
        private const bool DefaultDarkMode = false;
        private const int DefaultFontSize = 14;
        private const string DefaultLanguage = "en-US";

        static SettingsHelper()
        {
            // 初始化时自动填充缺失的配置项
            EnsureSetting(nameof(IsDarkMode), DefaultDarkMode);
            EnsureSetting(nameof(FontSize), DefaultFontSize);
            EnsureSetting(nameof(AppLanguage), DefaultLanguage);
        }

        /// <summary>
        /// 确保配置项存在，不存在时写入默认值
        /// </summary>
        private static void EnsureSetting<T>(string key, T defaultValue)
        {
            if (!_localSettings.Values.ContainsKey(key))
            {
                _localSettings.Values[key] = defaultValue;
            }
        }

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
        private static void SetValue<T>(string key, T value)
        {
            _localSettings.Values[key] = value;
        }

        // 示例配置属性
        public static bool IsDarkMode
        {
            get => GetValue(nameof(IsDarkMode), DefaultDarkMode);
            set => SetValue(nameof(IsDarkMode), value);
        }

        public static int FontSize
        {
            get => GetValue(nameof(FontSize), DefaultFontSize);
            set => SetValue(nameof(FontSize), value);
        }

        public static string AppLanguage
        {
            get => GetValue(nameof(AppLanguage), DefaultLanguage);
            set => SetValue(nameof(AppLanguage), value);
        }

        /// <summary>
        /// 重置单个配置到默认值
        /// </summary>
        public static void ResetToDefault(string key)
        {
            switch (key)
            {
                case nameof(IsDarkMode):
                    SetValue(key, DefaultDarkMode);
                    break;
                case nameof(FontSize):
                    SetValue(key, DefaultFontSize);
                    break;
                case nameof(AppLanguage):
                    SetValue(key, DefaultLanguage);
                    break;
            }
        }

        /// <summary>
        /// 重置所有配置到默认值
        /// </summary>
        public static void ResetAll()
        {
            IsDarkMode = DefaultDarkMode;
            FontSize = DefaultFontSize;
            AppLanguage = DefaultLanguage;
        }
    }
}
