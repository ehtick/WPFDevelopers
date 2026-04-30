using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using WPFDevelopers.Controls;

namespace WPFDevelopers.Helpers
{
    public class DataGridHelper
    {
        public static readonly DependencyProperty IsFilterProperty =
            DependencyProperty.RegisterAttached(
                "IsFilter",
                typeof(bool),
                typeof(DataGridHelper),
                new PropertyMetadata(false, OnIsFilterChanged));

        public static readonly DependencyProperty FilterEngineProperty =
        DependencyProperty.RegisterAttached(
            "FilterEngine",
            typeof(object),
            typeof(DataGridHelper),
            new PropertyMetadata(null));

        private static void OnIsFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ToggleButton toggleButton)
            {
                toggleButton.Checked -= OnToggleButton_Checked;

                if (e.NewValue != null)
                {
                    toggleButton.Checked += OnToggleButton_Checked;
                }
            }
        }

        private static void OnToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
            {
                var dataGrid = ControlsHelper.FindParent<DataGrid>(toggleButton);
                if (dataGrid == null) return;
                var header = ControlsHelper.FindParent<DataGridColumnHeader>(toggleButton);
                if (header == null)
                    return;
                var column = header.Column;
                string property =
                    column.SortMemberPath;
                var engine = GetFilterEngine(dataGrid) as IFilterEngine;
                if (engine == null)
                    return;
                var values = DistinctHelper.GetDistinctValues(engine.Source, property, engine.ItemType);
                var cornerRadius = ElementHelper.GetCornerRadius(dataGrid);
                var selected = engine.GetFilterValues(property);
                var filter = new Filter(values, selected);
                ElementHelper.SetCornerRadius(filter, cornerRadius);
                var popup = new Popup();
                popup.AllowsTransparency = true;
                popup.Closed += delegate 
                {
                    toggleButton.IsChecked = false;
                };
                filter.ApplyClick += (s,y) =>
                {
                    var selectedValues = filter.SelectedValues;
                    popup.IsOpen = false;
                    toggleButton.IsChecked = false;
                    if (selectedValues.Count == values.Count)
                        engine.ClearFilter(property);
                    else
                        engine.ApplyFilter(property, selectedValues);
                };
                popup.Child = filter;
                popup.Placement = PlacementMode.MousePoint;
                popup.StaysOpen = false;
                popup.IsOpen = true;
            }
        }

        public static bool GetIsFilter(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFilterProperty);
        }

        public static void SetIsFilter(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFilterProperty, value);
        }

        public static object GetFilterEngine(DependencyObject obj)
        {
            return obj.GetValue(FilterEngineProperty);
        }

        public static void SetFilterEngine(DependencyObject obj, object value)
        {
            obj.SetValue(FilterEngineProperty, value);
        }
    }
}
