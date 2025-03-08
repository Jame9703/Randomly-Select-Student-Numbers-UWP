using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace 随机抽取学号.Views
{
    public sealed partial class PopupNotice : UserControl
    {
        //存放弹出框中的信息
        private string _popupContent;
        //创建一个popup对象
        private Popup _popup = null;

        public PopupNotice()
        {
            this.InitializeComponent();

            //将当前的长、宽赋值给控件
            this.Width = Window.Current.Bounds.Width;
            this.Height = Window.Current.Bounds.Height;

            //将当前的控价赋值给弹窗的Child属性，Child属性是弹窗需要显示的内容（当前的this是一个Grid控件）。
            _popup = new Popup();
            _popup.Child = this;
            //给当前的Grid添加一个Loaded事件，使用ShowAPopup()时，弹窗显示内容为Grid，此时打开动画。
            this.Loaded += PopupNoticeLoaded;

        }
        public PopupNotice(string popupContentString) : this()
        {
            //popupContentString:弹出消息内容
            _popupContent = popupContentString;
        }
        public void ShowPopup()
        {
            //显示弹窗
            _popup.IsOpen = true;
        }

        public void PopupNoticeLoaded(object sender, RoutedEventArgs e)
        {
            PopupContent.Message = _popupContent;

            //打开动画
            this.PopupIn.Begin();
            //动画执行，弹窗到达指定位置，等待一段时间：await Task.Delay()
            this.PopupIn.Completed += PopupInCompleted;
        }
        public async void PopupInCompleted(object sender, object e)
        {
            //在原地停留一坤秒
            await Task.Delay(2500);
            //将消失动画打开
            this.PopupOut.Begin();
            //popout动画完成后触发
            this.PopupOut.Completed += PopupOutCompleted;
        }


        //弹窗退出动画结束，代表整个过程结束，此时将弹窗关闭
        public void PopupOutCompleted(object sender, object e)
        {
            _popup.IsOpen = false;
        }
    }
}