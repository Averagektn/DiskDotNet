﻿using System.Windows;
using System.Windows.Controls;

namespace Disk.Style
{
    public partial class Popup : UserControl
    {
        public Popup()
        {
            InitializeComponent();
        }

        public string PopupHeader
        {
            get { return (string)GetValue(PopupHeaderProperty); }
            set { SetValue(PopupHeaderProperty, value); }
        }

        public static readonly DependencyProperty PopupHeaderProperty =
            DependencyProperty.Register("PopupHeader", typeof(string), typeof(Popup), new PropertyMetadata(string.Empty, OnPopupHeaderChanged));

        private static void OnPopupHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Popup;
            if (control != null)
            {
                control.PART_Header.Text = e.NewValue as string;
            }
        }

        public string PopupMessage
        {
            get { return (string)GetValue(PopupMessageProperty); }
            set { SetValue(PopupMessageProperty, value); }
        }

        public static readonly DependencyProperty PopupMessageProperty =
            DependencyProperty.Register("PopupMessage", typeof(string), typeof(Popup), new PropertyMetadata(string.Empty, OnPopupMessageChanged));

        private static void OnPopupMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Popup;
            if (control != null)
            {
                control.PART_Message.Text = e.NewValue as string;
            }
        }

        public bool IsPopupOpen
        {
            get { return (bool)GetValue(IsPopupOpenProperty); }
            set { SetValue(IsPopupOpenProperty, value); }
        }

        public static readonly DependencyProperty IsPopupOpenProperty =
            DependencyProperty.Register("IsPopupOpen", typeof(bool), typeof(Popup), new PropertyMetadata(false, OnIsPopupOpenChanged));

        private static void OnIsPopupOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Popup;
            if (control != null)
            {
                control.PART_Popup.IsOpen = (bool)e.NewValue;
            }
        }
    }
}