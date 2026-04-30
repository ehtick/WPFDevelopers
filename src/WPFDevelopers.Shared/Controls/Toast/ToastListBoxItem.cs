using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPFDevelopers.Controls
{
    public class ToastListBoxItem : ListBoxItem
    {
        public ToastImage ToastType
        {
            get { return (ToastImage)GetValue(ToastTypeProperty); }
            set { SetValue(ToastTypeProperty, value); }
        }
        public static readonly DependencyProperty ToastTypeProperty =
            DependencyProperty.Register("ToastType", typeof(ToastImage), typeof(ToastListBoxItem), new PropertyMetadata(ToastImage.Info));

        public bool IsCenter
        {
            get { return (bool)GetValue(IsCenterProperty); }
            set { SetValue(IsCenterProperty, value); }
        }

        public static readonly DependencyProperty IsCenterProperty =
            DependencyProperty.Register("IsCenter", typeof(bool), typeof(ToastListBoxItem), new PropertyMetadata(false));

        public ToastListBoxItem()
        {
            Loaded += OnMessageListBoxItem_Loaded;
        }

        private void OnMessageListBoxItem_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnMessageListBoxItem_Loaded;
            var item = sender as ListBoxItem;
            if (item == null) return;
            var dpd = DependencyPropertyDescriptor
                .FromProperty(UIElement.OpacityProperty, typeof(UIElement));
            EventHandler handler = null;
            handler = (s, args) =>
            {
                if (item.Opacity < 0.1)
                {
                    dpd.RemoveValueChanged(item, handler);
                    var parent = ItemsControl.ItemsControlFromItemContainer(item);
                    if (parent != null)
                    {
                        var selectedItem = parent.ItemContainerGenerator.ItemFromContainer(item);
                        parent.Items.Remove(selectedItem);
                    }
                }
            };
            dpd.AddValueChanged(item, handler);
        }
    }
}
