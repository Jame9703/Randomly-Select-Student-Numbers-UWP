using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;
using 随机抽取学号.Views;

namespace 随机抽取学号.Classes
{
    public class SettingsHelper
    {
        private static readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private static bool isFirstRun;
        private static int theme;

        private static int contentFrameBackground;
        private static double contentFrameBackgroundOpacity;
        private static string currentClassName;
        private static bool noReturn;
        private static bool autoStop;
        private static bool optimize;
        private static bool saveRange;
        private static bool saveHistory;
        private static int mainPageBackground;
        private static double mainPageNoBackgroundOpacity;
        private static double mainPageAcrylicBackgroundOpacity;
        private static double mainPageMicaBackgroundOpacity;
        private static double mainPageImageBackgroundOpacity;

        static SettingsHelper()
        {
            // 初始化所有设置值
            isFirstRun = GetSetting(nameof(IsFirstRun), true);
            theme = GetSetting(nameof(Theme), 0);
            mainPageBackground = GetSetting(nameof(MainPageBackground), 2);
            mainPageNoBackgroundOpacity = GetSetting(nameof(MainPageNoBackgroundOpacity), 0.5);
            mainPageAcrylicBackgroundOpacity = GetSetting(nameof(MainPageAcrylicBackgroundOpacity), 0.5);
            mainPageMicaBackgroundOpacity = GetSetting(nameof(MainPageMicaBackgroundOpacity), 0.5);
            mainPageImageBackgroundOpacity = GetSetting(nameof(MainPageImageBackgroundOpacity), 0.5);
            contentFrameBackground = GetSetting(nameof(ContentFrameBackground), 0);
            contentFrameBackgroundOpacity = GetSetting(nameof(ContentFrameBackgroundOpacity), 0.5);
            currentClassName = GetSetting(nameof(CurrentClassName), "我的班级");
            noReturn = GetSetting(nameof(NoReturn), false);
            autoStop = GetSetting(nameof(AutoStop), false);
            optimize = GetSetting(nameof(Optimize), false);
            saveRange = GetSetting(nameof(SaveRange), true);
            saveHistory = GetSetting(nameof(SaveHistory), true);
        }
        /// <summary>
        /// 从本地设置中获取指定键的值，如果不存在则返回默认值
        /// </summary>
        private static T GetSetting<T>(string key, T defaultValue)
        {
            if (localSettings.Values.TryGetValue(key, out object value))
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch (Exception ex)
                {
                    PopupNotice popupNotice = new PopupNotice("应用设置加载失败" + ex.Message);
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                    popupNotice.ShowPopup();
                }
            }
            localSettings.Values[key] = defaultValue;
            return defaultValue;
        }
        /// <summary>
        /// 将单个设置项重置为默认值
        /// </summary>
        public static void ResetToDefault(string key)
        {
            switch (key)
            {
                case nameof(Theme):
                    theme = 0;
                    localSettings.Values[nameof(Theme)] = 0;
                    break;
            }
        }

        /// <summary>
        /// 将所有设置项重置为默认值
        /// </summary>
        public static void ResetAll()
        {

        }
        public static bool IsFirstRun
        {
            get => isFirstRun;
            set
            {
                if (isFirstRun != value)
                {
                    isFirstRun = value;
                    localSettings.Values[nameof(IsFirstRun)] = value;
                }
            }
        }
        public static int Theme
        {
            get => theme;
            set
            {
                if (theme != value)
                {
                    theme = value;
                    localSettings.Values[nameof(Theme)] = value;
                }
            }
        }
        /// <summary>
        /// 获取或设置MainPage的背景，0：纯色，1：Acrylic，2：Mica，3：图片
        /// </summary>
        public static int MainPageBackground
        {
            get => mainPageBackground;
            set
            {
                if (mainPageBackground != value)
                {
                    mainPageBackground = value;
                    localSettings.Values[nameof(mainPageBackground)] = value;
                }
            }
        }
        /// <summary>
        /// 获取或设置MainPage的纯色背景透明度
        /// </summary>
        public static double MainPageNoBackgroundOpacity
        {
            get => mainPageNoBackgroundOpacity;
            set
            {
                if (mainPageNoBackgroundOpacity != value)
                {
                    mainPageNoBackgroundOpacity = value;
                    localSettings.Values[nameof(mainPageNoBackgroundOpacity)] = value;
                }
            }
        }
        /// <summary>
        /// 获取或设置MainPage的Acrylic背景透明度
        /// </summary>
        public static double MainPageAcrylicBackgroundOpacity
        {
            get => mainPageAcrylicBackgroundOpacity;
            set
            {
                if (mainPageAcrylicBackgroundOpacity != value)
                {
                    mainPageAcrylicBackgroundOpacity = value;
                    localSettings.Values[nameof(mainPageAcrylicBackgroundOpacity)] = value;
                }
            }
        }
        /// <summary>
        /// 获取或设置MainPage的Mica背景透明度
        /// </summary>
        public static double MainPageMicaBackgroundOpacity
        {
            get => mainPageMicaBackgroundOpacity;
            set
            {
                if (mainPageMicaBackgroundOpacity != value)
                {
                    mainPageMicaBackgroundOpacity = value;
                    localSettings.Values[nameof(mainPageMicaBackgroundOpacity)] = value;
                }
            }
        }
        /// <summary>
        /// 获取或设置MainPage的图片背景透明度
        /// </summary>
        public static double MainPageImageBackgroundOpacity
        {
            get => mainPageImageBackgroundOpacity;
            set
            {
                if (mainPageImageBackgroundOpacity != value)
                {
                    mainPageImageBackgroundOpacity = value;
                    localSettings.Values[nameof(mainPageImageBackgroundOpacity)] = value;
                }
            }
        }
        public static int ContentFrameBackground
        {
            get => contentFrameBackground;
            set
            {
                if (contentFrameBackground != value)
                {
                    contentFrameBackground = value;
                    localSettings.Values[nameof(contentFrameBackground)] = value;
                }
            }
        }

        public static double ContentFrameBackgroundOpacity
        {
            get => contentFrameBackgroundOpacity;
            set
            {
                if (contentFrameBackgroundOpacity != value)
                {
                    contentFrameBackgroundOpacity = value;
                    localSettings.Values[nameof(contentFrameBackgroundOpacity)] = value;
                }
            }
        }

        public static string CurrentClassName
        {
            get => currentClassName;
            set
            {
                if (currentClassName != value)
                {
                    currentClassName = value;
                    localSettings.Values[nameof(currentClassName)] = value;
                }
            }
        }

        public static bool NoReturn
        {
            get => noReturn;
            set
            {
                if (noReturn != value)
                {
                    noReturn = value;
                    localSettings.Values[nameof(noReturn)] = value;
                }
            }
        }

        public static bool AutoStop
        {
            get => autoStop;
            set
            {
                if (autoStop != value)
                {
                    autoStop = value;
                    localSettings.Values[nameof(autoStop)] = value;
                }
            }
        }

        public static bool Optimize
        {
            get => optimize;
            set
            {
                if (optimize != value)
                {
                    optimize = value;
                    localSettings.Values[nameof(optimize)] = value;
                }
            }
        }

        public static bool SaveRange
        {
            get => saveRange;
            set
            {
                if (saveRange != value)
                {
                    saveRange = value;
                    localSettings.Values[nameof(saveRange)] = value;
                }
            }
        }

        public static bool SaveHistory
        {
            get => saveHistory;
            set
            {
                if (saveHistory != value)
                {
                    saveHistory = value;
                    localSettings.Values[nameof(saveHistory)] = value;
                }
            }
        }

    }
}
