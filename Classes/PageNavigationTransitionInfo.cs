using System;
using System.Collections.Generic;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace 随机抽取学号.Classes
{
    public class PageNavigationTransitionInfo : NavigationTransitionInfo
    {
        private Compositor _compositor;
        private DrillInNavigationTransitionInfo transitionInfo;

        public PageNavigationTransitionInfo()
        {
            _compositor = Window.Current.Compositor;
        }

        //public override IReadOnlyList<UIElement> GetTransitionContent(Frame frame, object navigationParameter)
        //{
        //    return new List<UIElement> { frame.Content as UIElement };
        //}

        //public override CompositionAnimationGroup CreateAnimationGroup(Frame frame, object navigationParameter)
        //{
        //    var animationGroup = _compositor.CreateAnimationGroup();

        //    // 新页面的透明度动画
        //    var newPageOpacityAnimation = _compositor.CreateScalarKeyFrameAnimation();
        //    newPageOpacityAnimation.Duration = TimeSpan.FromSeconds(0.3);
        //    newPageOpacityAnimation.InsertKeyFrame(0f, 0f);
        //    newPageOpacityAnimation.InsertKeyFrame(1f, 1f);

        //    // 新页面的位移动画
        //    var newPageOffsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
        //    newPageOffsetAnimation.Duration = TimeSpan.FromSeconds(0.3);
        //    newPageOffsetAnimation.InsertKeyFrame(0f, new System.Numerics.Vector3(100, 0, 0));
        //    newPageOffsetAnimation.InsertKeyFrame(1f, new System.Numerics.Vector3(0, 0, 0));

        //    // 旧页面的透明度动画
        //    var oldPageOpacityAnimation = _compositor.CreateScalarKeyFrameAnimation();
        //    oldPageOpacityAnimation.Duration = TimeSpan.FromSeconds(0.3);
        //    oldPageOpacityAnimation.InsertKeyFrame(0f, 1f);
        //    oldPageOpacityAnimation.InsertKeyFrame(1f, 0f);

        //    // 旧页面的位移动画
        //    var oldPageOffsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
        //    oldPageOffsetAnimation.Duration = TimeSpan.FromSeconds(0.3);
        //    oldPageOffsetAnimation.InsertKeyFrame(0f, new System.Numerics.Vector3(0, 0, 0));
        //    oldPageOffsetAnimation.InsertKeyFrame(1f, new System.Numerics.Vector3(-100, 0, 0));

        //    animationGroup.Add(newPageOpacityAnimation);
        //    animationGroup.Add(newPageOffsetAnimation);
        //    animationGroup.Add(oldPageOpacityAnimation);
        //    animationGroup.Add(oldPageOffsetAnimation);

        //    return animationGroup;
        //}
    }
}
