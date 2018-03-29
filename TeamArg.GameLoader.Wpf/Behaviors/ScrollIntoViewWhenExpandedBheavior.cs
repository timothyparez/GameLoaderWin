using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace TeamArg.GameLoader.Behaviors
{
    public class ScrollIntoViewWhenExpandedBehavior : Behavior<Expander>
    {
        public static readonly DependencyProperty TargetScrollViewerProperty = DependencyProperty.Register("TargetScrollViewer", typeof(ScrollViewer), typeof(ScrollIntoViewWhenExpandedBehavior), new PropertyMetadata(null));

        public ScrollViewer TargetScrollViewer
        {
            get { return (ScrollViewer)GetValue(TargetScrollViewerProperty); }
            set { SetValue(TargetScrollViewerProperty, value); }
        }        
        
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Expanded += (s, e) => (AssociatedObject.Content as FrameworkElement).BringIntoView();
        }
    }
}