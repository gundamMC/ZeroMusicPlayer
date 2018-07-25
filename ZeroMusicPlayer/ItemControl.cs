using System;
using System.Windows;
using System.Windows.Controls;

namespace ZeroMusicPlayer
{
    public class ItemControl : UserControl
    {
        public string ItemName
        {
            get { return (String)GetValue(ItemNameProperty); }
            set { SetValue(ItemNameProperty, value.ToUpper()); }
        }

        public Boolean Selected
        {
            get { return (Boolean)GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }
        }

        public static readonly DependencyProperty ItemNameProperty = DependencyProperty.Register("ItemName", typeof(String), typeof(ItemControl), new PropertyMetadata(""));
        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register("Selected", typeof(Boolean), typeof(ItemControl), new PropertyMetadata(false));

    }
}
