using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = ListBoxTemplateName, Type = typeof(ListBox))]
    [TemplatePart(Name = TextBoxTemplateName, Type = typeof(TextBox))]
    [TemplatePart(Name = SelectAllCheckBoxTemplateName, Type = typeof(CheckBox))]
    [TemplatePart(Name = ApplyButtonTemplateName, Type = typeof(Button))]
    public class Filter : Control
    {
        private const string ListBoxTemplateName = "PART_ListBox";
        private const string TextBoxTemplateName = "PART_TextBox";
        private const string SelectAllCheckBoxTemplateName = "PART_SelectAll";
        private const string ApplyButtonTemplateName = "PART_ApplyButton";
        private ListBox _list;
        private TextBox _textBox;
        private CheckBox _checkBox;
        private Button _applyButton;
        private List<FilterItem> _allItems = new List<FilterItem>();

        public static readonly RoutedEvent ApplyClickEvent = EventManager.RegisterRoutedEvent("ApplyClick", RoutingStrategy.Bubble,
         typeof(RoutedEventHandler), typeof(Filter));

        public event RoutedEventHandler ApplyClick
        {
            add => AddHandler(ApplyClickEvent, value);
            remove => RemoveHandler(ApplyClickEvent, value);
        }

        public ICommand ApplyCommand
        {
            get => (ICommand)GetValue(ApplyCommandProperty);
            set => SetValue(ApplyCommandProperty, value);
        }

        public static readonly DependencyProperty ApplyCommandProperty =
            DependencyProperty.Register("ApplyCommand", typeof(ICommand), typeof(Filter),new PropertyMetadata(null));

        public IEnumerable<object> Values
        {
            get { return (IEnumerable<object>)GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }

        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.Register("Values", typeof(IEnumerable<object>), typeof(Filter), new PropertyMetadata(null));

        public List<object> SelectedValues
        {
            get { return (List<object>)GetValue(SelectedValuesProperty); }
            set { SetValue(SelectedValuesProperty, value); }
        }

        public static readonly DependencyProperty SelectedValuesProperty =
            DependencyProperty.Register("SelectedValues", typeof(List<object>), typeof(Filter), new PropertyMetadata(null));


        public bool? IsSelectAll
        {
            get { return (bool?)GetValue(IsSelectAllProperty); }
            set { SetValue(IsSelectAllProperty, value); }
        }

        public static readonly DependencyProperty IsSelectAllProperty =
            DependencyProperty.Register("IsSelectAll", typeof(bool?), typeof(Filter), new PropertyMetadata(null));


        static Filter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Filter),
                new FrameworkPropertyMetadata(typeof(Filter)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _list = GetTemplateChild(ListBoxTemplateName) as ListBox;
            _textBox = GetTemplateChild(TextBoxTemplateName) as TextBox;
            if (_textBox != null)
            {
                _textBox.TextChanged -= OnTextBox_TextChanged;
                _textBox.TextChanged += OnTextBox_TextChanged;
            }
            _checkBox = GetTemplateChild(SelectAllCheckBoxTemplateName) as CheckBox;
            if (_checkBox != null)
            {
                _checkBox.Checked -= OnCheckBox_Checked;
                _checkBox.Checked += OnCheckBox_Checked;
                _checkBox.Unchecked -= OnCheckBox_Unchecked;
                _checkBox.Unchecked += OnCheckBox_Unchecked;
            }
            _applyButton = GetTemplateChild(ApplyButtonTemplateName) as Button;
            if (_applyButton != null)
            {
                _applyButton.Click -= OnApplyButton_Click;
                _applyButton.Click += OnApplyButton_Click;
            }
            UpdateSelectAllState();
        }

        private void OnCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ModifyAllItemsChecked();
        }

        private void OnCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ModifyAllItemsChecked(true);
        }

        private void OnTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_list == null)
                return;

            var keyword = _textBox.Text;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                _list.ItemsSource = _allItems;
            }
            else
            {
                _list.ItemsSource = _allItems.Where(i => i.Value != null && i.Value.ToString().IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            UpdateSelectAllState();
        }

        private void OnApplyButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedValues = _list.Items
                .Cast<FilterItem>()
                .Where(i => i.IsChecked)
                .Select(i => i.Value)
                .ToList();
            RaiseEvent(new RoutedEventArgs(ApplyClickEvent));
            if (ApplyCommand?.CanExecute(SelectedValues) == true)
            {
                ApplyCommand?.Execute(SelectedValues);
            }
        }

        public Filter(IEnumerable<object> values, HashSet<object> selectedValues)
        {
            foreach (var v in values)
            {
                if (v == null) continue;

                var item = new FilterItem
                {
                    Value = v,
                    IsChecked = selectedValues?.Contains(v) ?? true
                };

                item.PropertyChanged += (_, __) => UpdateSelectAllState();
                _allItems.Add(item);
            }
            Values = _allItems;
        }

        private void UpdateSelectAllState()
        {
            if (_list == null || _list.Items.Count == 0)
            {
                IsSelectAll = null;
                return;
            }

            var items = _list.Items.Cast<FilterItem>();

            bool allChecked = items.All(i => i.IsChecked);
            bool noneChecked = items.All(i => !i.IsChecked);

            if (allChecked)
            {
                IsSelectAll = true;
            }
            else if (noneChecked)
            {
                IsSelectAll = false;
            }
            else
            {
                IsSelectAll = null;
            }
        }

        private void ModifyAllItemsChecked(bool isChecked = false)
        {
            if (_list == null)
                return;

            foreach (FilterItem item in _list.Items)
            {
                item.IsChecked = isChecked;
            }

            _list.Items.Refresh();

            UpdateSelectAllState();
        }
    }
}
