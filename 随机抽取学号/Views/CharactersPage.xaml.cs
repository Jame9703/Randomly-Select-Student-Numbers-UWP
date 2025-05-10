using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.Media.SpeechSynthesis;
using System.Threading.Tasks;

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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
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
        private async Task SpeakChinese(string text)
        {
            using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
            {
                // 获取中文语音
                var voices = SpeechSynthesizer.AllVoices;
                VoiceInformation voice = null;
                foreach (var v in voices)
                {
                    if (v.Language.StartsWith("zh-"))
                    {
                        voice = v;
                        break;
                    }
                }
                if (voice != null)
                {
                    synthesizer.Voice = voice;
                }

                // 合成语音
                SpeechSynthesisStream stream = await synthesizer.SynthesizeTextToStreamAsync(text);

                // 播放语音
                //MediaElement mediaElement = new MediaElement();
                //mediaElement.SetSource(stream, stream.ContentType);
                //mediaElement.Play();
            }
        }

        private async void SpeakChineseButton_Click(object sender, RoutedEventArgs e)
        {
            if (TextBox.Text != "")
            {
                await SpeakChinese(TextBox.Text);
            }
        }
    }
}
