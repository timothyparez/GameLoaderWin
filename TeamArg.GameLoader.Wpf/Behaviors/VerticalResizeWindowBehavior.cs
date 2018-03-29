using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace TeamArg.GameLoader.Behaviors
{
    public class VerticalResizeWindowBehavior : Behavior<UIElement>
    {
        public static readonly DependencyProperty MinHeightProperty = DependencyProperty.Register("MinHeight", typeof(double), typeof(VerticalResizeWindowBehavior), new PropertyMetadata(600.0));
        public double MinHeight
        {
            get { return (double)GetValue(MinHeightProperty); }
            set { SetValue(MinHeightProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            var window = Window.GetWindow(AssociatedObject);
            var mouseDown = Observable.FromEventPattern<MouseButtonEventArgs>(AssociatedObject, "MouseLeftButtonDown")
                                        .Select(e => e.EventArgs.GetPosition(AssociatedObject));

            var mouseUp = Observable.FromEventPattern<MouseButtonEventArgs>(AssociatedObject, "MouseLeftButtonUp")
                                    .Select(e => e.EventArgs.GetPosition(AssociatedObject));

            var mouseMove = Observable.FromEventPattern<MouseEventArgs>(AssociatedObject, "MouseMove")
                                        .Select(e => e.EventArgs.GetPosition(AssociatedObject));

            var q = from start in mouseDown
                    from position in mouseMove.TakeUntil(mouseUp)
                    select new { X = position.X - start.X, Y = position.Y - start.Y };

            mouseDown.Subscribe(v => AssociatedObject.CaptureMouse());
            mouseUp.Subscribe(v => AssociatedObject.ReleaseMouseCapture());

            q.ObserveOnDispatcher().Subscribe(v =>
            {
                var newHeight = window.Height + v.Y;
                window.Height = newHeight < MinHeight ? MinHeight : newHeight;                
            });
        }
    }
}
