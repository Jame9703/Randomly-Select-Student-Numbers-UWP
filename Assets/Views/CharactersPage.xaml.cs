using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 随机抽取学号.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CharactersPage : Page
    {
        public CharactersPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            GC.Collect();
        }
        private string GenerateRandomChineseCharacter()
        {
            // 汉字Unicode编码范围：0x4E00 - 0x9FFF
            Random random = new Random();
            int code = random.Next(0x4E00, 0x9FFF);
            char character = (char)code;
            return character.ToString();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string randomCharacter = GenerateRandomChineseCharacter();
            TextBox.Text = randomCharacter;
        }
    }
}
