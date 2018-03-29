using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace TeamArg.GameLoader.Behaviors
{
    public class DraggableWindowBehavior : Behavior<UIElement>
    {
        public static readonly DependencyProperty ResizeOnDoubleClickProperty = DependencyProperty.Register("ResizeOnDoubleClick", typeof(bool), typeof(DraggableWindowBehavior), new PropertyMetadata(false));

        public bool ResizeOnDoubleClick
        {
            get { return (bool)GetValue(ResizeOnDoubleClickProperty); }
            set { SetValue(ResizeOnDoubleClickProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
        }

        void AssociatedObject_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 2)
                {
                    if (ResizeOnDoubleClick)
                    {
                        AdjustWindowSize();
                    }
                }
                else
                {
                    var window = Window.GetWindow(AssociatedObject);
                    Window.GetWindow(AssociatedObject).DragMove();
                }
            }
        }

        private void AdjustWindowSize()
        {
            var window = Window.GetWindow(AssociatedObject);
            window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
        }
    }
}