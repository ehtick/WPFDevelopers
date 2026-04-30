using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    public class ToastListBox : ListBox
    {
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ToastListBoxItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ToastListBoxItem();
        }

    }
}
