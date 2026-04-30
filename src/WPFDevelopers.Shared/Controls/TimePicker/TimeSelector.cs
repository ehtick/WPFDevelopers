using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WPFDevelopers.Controls
{
    [TemplatePart(Name = ListBoxHourTemplateName, Type = typeof(ListBox))]
    [TemplatePart(Name = ListBoxMinuteTemplateName, Type = typeof(ListBox))]
    [TemplatePart(Name = ListBoxSecondTemplateName, Type = typeof(ListBox))]
    public class TimeSelector : Control
    {
        private const string ListBoxHourTemplateName = "PART_ListBoxHour";
        private const string ListBoxMinuteTemplateName = "PART_ListBoxMinute";
        private const string ListBoxSecondTemplateName = "PART_ListBoxSecond";
        private const int VisibleItemCount = 10;
        private const int CenterIndex = 4;
        private int _hour;
        private int _minute;
        private int _second;
        private bool _isRefreshing;
        private ListBox _listBoxHour;
        private ListBox _listBoxMinute;
        private ListBox _listBoxSecond;

        public static readonly RoutedEvent SelectedTimeChangedEvent =
            EventManager.RegisterRoutedEvent(
                "SelectedTimeChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<DateTime?>),
                typeof(TimeSelector));

        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register(
                "SelectedTime",
                typeof(DateTime?),
                typeof(TimeSelector),
                new PropertyMetadata(null, OnSelectedTimeChanged));

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register(
                "ItemHeight",
                typeof(double),
                typeof(TimeSelector),
                new PropertyMetadata(0d));

        public static readonly DependencyProperty SelectorMarginProperty =
            DependencyProperty.Register(
                "SelectorMargin",
                typeof(Thickness),
                typeof(TimeSelector),
                new PropertyMetadata(new Thickness(0)));

        static TimeSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TimeSelector),
                new FrameworkPropertyMetadata(typeof(TimeSelector))
            );
        }

        public DateTime? SelectedTime
        {
            get => (DateTime?)GetValue(SelectedTimeProperty);
            set => SetValue(SelectedTimeProperty, value);
        }

        public double ItemHeight
        {
            get => (double)GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }

        public Thickness SelectorMargin
        {
            get => (Thickness)GetValue(SelectorMarginProperty);
            set => SetValue(SelectorMarginProperty, value);
        }

        public event RoutedPropertyChangedEventHandler<DateTime?> SelectedTimeChanged
        {
            add => AddHandler(SelectedTimeChangedEvent, value);
            remove => RemoveHandler(SelectedTimeChangedEvent, value);
        }

        public virtual void OnSelectedTimeChanged(DateTime? oldValue, DateTime? newValue)
        {
            var args = new RoutedPropertyChangedEventArgs<DateTime?>(
                oldValue,
                newValue,
                SelectedTimeChangedEvent
            );
            RaiseEvent(args);
        }

        private static void OnSelectedTimeChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            if (d is TimeSelector ctrl)
            {
                ctrl.HandleSelectedTimeChanged((DateTime?)e.OldValue, (DateTime?)e.NewValue);
                ctrl.OnSelectedTimeChanged((DateTime?)e.OldValue, (DateTime?)e.NewValue);
            }
        }

        private void HandleSelectedTimeChanged(DateTime? oldValue, DateTime? newValue)
        {
            if (_isRefreshing)
                return;

            if (!newValue.HasValue)
                return;

            _hour = newValue.Value.Hour;
            _minute = newValue.Value.Minute;
            _second = newValue.Value.Second;

            if (_listBoxHour != null && _listBoxMinute != null && _listBoxSecond != null)
            {
                RefreshAllColumns();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _listBoxHour = GetTemplateChild(ListBoxHourTemplateName) as ListBox;
            _listBoxMinute = GetTemplateChild(ListBoxMinuteTemplateName) as ListBox;
            _listBoxSecond = GetTemplateChild(ListBoxSecondTemplateName) as ListBox;

            if (_listBoxHour != null)
            {
                _listBoxHour.SelectionChanged -= ListBoxHour_SelectionChanged;
                _listBoxHour.SelectionChanged += ListBoxHour_SelectionChanged;

                _listBoxHour.Loaded -= ListBox_Loaded;
                _listBoxHour.Loaded += ListBox_Loaded;
            }

            if (_listBoxMinute != null)
            {
                _listBoxMinute.SelectionChanged -= ListBoxMinute_SelectionChanged;
                _listBoxMinute.SelectionChanged += ListBoxMinute_SelectionChanged;
            }

            if (_listBoxSecond != null)
            {
                _listBoxSecond.SelectionChanged -= ListBoxSecond_SelectionChanged;
                _listBoxSecond.SelectionChanged += ListBoxSecond_SelectionChanged;
            }

            if (SelectedTime.HasValue)
            {
                _hour = SelectedTime.Value.Hour;
                _minute = SelectedTime.Value.Minute;
                _second = SelectedTime.Value.Second;
            }
            else
            {
                var now = DateTime.Now;
                _hour = now.Hour;
                _minute = now.Minute;
                _second = now.Second;

                _isRefreshing = true;
                SelectedTime = new DateTime(
                    DateTime.Today.Year,
                    DateTime.Today.Month,
                    DateTime.Today.Day,
                    _hour,
                    _minute,
                    _second
                );
                _isRefreshing = false;
            }

            RefreshAllColumns();
        }

        private void ListBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (_listBoxHour == null)
                return;

            var h = GetItemHeight(_listBoxHour);
            if (h <= 0)
                return;

            ItemHeight = h;
            Height = h * VisibleItemCount;
            SelectorMargin = new Thickness(0, CenterIndex * h, 0, 0);
        }

        private double GetItemHeight(ListBox listBox)
        {
            if (listBox == null)
                return 0;

            if (listBox.Items.Count > 0)
            {
                listBox.UpdateLayout();
                var listBoxItem = listBox.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                if (listBoxItem != null)
                    return listBoxItem.ActualHeight;
            }

            return 0;
        }
        
        private List<string> BuildCyclicItems(int current, int maxValue)
        {
            var list = new List<string>(VisibleItemCount);

            for (int i = 0; i < VisibleItemCount; i++)
            {
                int value = (current - CenterIndex + i) % maxValue;
                if (value < 0)
                    value += maxValue;

                list.Add(value.ToString("D2"));
            }

            return list;
        }

        private void RefreshAllColumns()
        {
            RefreshHourItems();
            RefreshMinuteItems();
            RefreshSecondItems();
        }

        private void RefreshSecondItems()
        {
            if (_listBoxSecond == null)
                return;

            _listBoxSecond.SelectionChanged -= ListBoxSecond_SelectionChanged;
            _listBoxSecond.ItemsSource = BuildCyclicItems(_second, 60);
            _listBoxSecond.SelectedIndex = CenterIndex;
            _listBoxSecond.SelectionChanged += ListBoxSecond_SelectionChanged;
        }

        private void RefreshHourItems()
        {
            if (_listBoxHour == null)
                return;

            _listBoxHour.SelectionChanged -= ListBoxHour_SelectionChanged;
            _listBoxHour.ItemsSource = BuildCyclicItems(_hour, 24);
            _listBoxHour.SelectedIndex = CenterIndex;
            _listBoxHour.SelectionChanged += ListBoxHour_SelectionChanged;
        }

        private void RefreshMinuteItems()
        {
            if (_listBoxMinute == null)
                return;

            _listBoxMinute.SelectionChanged -= ListBoxMinute_SelectionChanged;
            _listBoxMinute.ItemsSource = BuildCyclicItems(_minute, 60);
            _listBoxMinute.SelectedIndex = CenterIndex;
            _listBoxMinute.SelectionChanged += ListBoxMinute_SelectionChanged;
        }

        private void ListBoxSecond_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isRefreshing)
                return;

            if (_listBoxSecond?.SelectedItem == null)
                return;

            if (!int.TryParse(_listBoxSecond.SelectedItem.ToString(), out var value))
                return;

            _second = value;
            UpdateSelectedTimeFromFields();
            RefreshSecondItems();
        }

        private void ListBoxMinute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isRefreshing)
                return;

            if (_listBoxMinute?.SelectedItem == null)
                return;

            if (!int.TryParse(_listBoxMinute.SelectedItem.ToString(), out var value))
                return;

            _minute = value;
            UpdateSelectedTimeFromFields();
            RefreshMinuteItems();
        }

        private void ListBoxHour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isRefreshing)
                return;

            if (_listBoxHour?.SelectedItem == null)
                return;

            if (!int.TryParse(_listBoxHour.SelectedItem.ToString(), out var value))
                return;

            _hour = value;
            UpdateSelectedTimeFromFields();
            RefreshHourItems();
        }

        private void UpdateSelectedTimeFromFields()
        {
            _isRefreshing = true;

            SelectedTime = new DateTime(
                DateTime.Today.Year,
                DateTime.Today.Month,
                DateTime.Today.Day,
                _hour,
                _minute,
                _second
            );

            _isRefreshing = false;
        }

        public void SetTime()
        {
            if (!SelectedTime.HasValue)
                return;

            _hour = SelectedTime.Value.Hour;
            _minute = SelectedTime.Value.Minute;
            _second = SelectedTime.Value.Second;
            RefreshAllColumns();
        }
    }
}