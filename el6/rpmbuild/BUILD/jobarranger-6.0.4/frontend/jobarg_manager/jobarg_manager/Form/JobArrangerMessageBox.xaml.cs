/*
** Job Arranger Manager
** Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.
** Copyright (C) 2013 Daiwa Institute of Research Business Innovation Ltd. All Rights Reserved.
** Copyright (C) 2021 Daiwa Institute of Research Ltd. All Rights Reserved.
**
** Licensed to the Apache Software Foundation (ASF) under one or more
** contributor license agreements. See the NOTICE file distributed with
** this work for additional information regarding copyright ownership.
** The ASF licenses this file to you under the Apache License, Version 2.0
** (the "License"); you may not use this file except in compliance with
** the License. You may obtain a copy of the License at
**
** http://www.apache.org/licenses/LICENSE-2.0
**
** Unless required by applicable law or agreed to in writing, software
** distributed under the License is distributed on an "AS IS" BASIS,
** WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
** See the License for the specific language governing permissions and
** limitations under the License.
**
**/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;


namespace jp.co.ftf.jobcontroller.JobController
{
    /// <summary>
    /// Interaction logic for HakedisMessageBox.xaml
    /// </summary>
    ///


    public enum MessageBoxType
    {
        ConfirmationWithYesNo = 0,
        ConfirmationWithYesNoCancel,
        Information,
        Error,
        Warning
    }

    public enum MessageBoxImage
    {
        Warning = 0,
        Question,
        Information,
        Error,
        None
    }

    public partial class JobArrangerMessageBox : Window
    {
        protected override void OnSourceInitialized(EventArgs e)
        {

            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;

            if (hwndSource != null)
            {
                hwndSource.AddHook(HwndSourceHook);
            }

        }

        private bool allowClosing = false;

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        private const uint MF_BYCOMMAND = 0x00000000;
        private const uint MF_GRAYED = 0x00000001;
        private const uint WS_EX_DLGMODALFRAME = 0x0001;



        private const uint SC_CLOSE = 0xF060;

        private const int WM_SHOWWINDOW = 0x00000018;
        private const int WM_CLOSE = 0x10;


        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;


        private IntPtr HwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_SHOWWINDOW:
                    {
                        IntPtr hMenu = GetSystemMenu(hwnd, false);
                        if (hMenu != IntPtr.Zero)
                        {
                            EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | WS_EX_DLGMODALFRAME);
                        }
                    }
                    break;
                case WM_CLOSE:
                    if (!allowClosing)
                    {
                        handled = true;
                    }
                    break;
            }
            return IntPtr.Zero;
        }
        private JobArrangerMessageBox()
        {
            InitializeComponent();
        }

        static JobArrangerMessageBox messageBox;
        static MessageBoxResult result = MessageBoxResult.No;

        public static MessageBoxResult Show(string caption, string msg ,MessageBoxType type)
        {
            switch (type)
            {
                case MessageBoxType.ConfirmationWithYesNo:
                    return Show(caption, msg, MessageBoxButton.YesNo,MessageBoxImage.Question);
                case MessageBoxType.ConfirmationWithYesNoCancel:
                    return Show(caption, msg, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                case MessageBoxType.Information:
                    return Show(caption, msg, MessageBoxButton.OK, MessageBoxImage.Information);
                case MessageBoxType.Error:
                    return Show(caption, msg, MessageBoxButton.OK, MessageBoxImage.Error);
                case MessageBoxType.Warning:
                    return Show(caption, msg, MessageBoxButton.OK, MessageBoxImage.Warning);
                default:
                    return MessageBoxResult.No;
            }
        }

        public static MessageBoxResult Show(string msg, MessageBoxType type)
        {
            return Show(string.Empty, msg, type);
        }

        public static MessageBoxResult Show(string msg)
        {
            return Show(string.Empty, msg, MessageBoxButton.OK, MessageBoxImage.None);
        }

        public static MessageBoxResult Show(string caption, string text)
        {
            return Show(caption, text, MessageBoxButton.OK, MessageBoxImage.None);
        }

        public static MessageBoxResult Show(string caption, string text, MessageBoxButton button)
        {
            return Show(caption, text, button, MessageBoxImage.None);
        }

        public static MessageBoxResult Show(string caption,string text,MessageBoxButton button, MessageBoxImage image)
        {
            messageBox = new JobArrangerMessageBox();
            messageBox.txtMsg.Text = text;
            messageBox.Title = caption;
            setVisibilityOfButtons(button);
            setImageOfMessageBox(image);
            messageBox.ShowDialog();
            return result;
        }

        private static void setVisibilityOfButtons(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.OK:
                    messageBox.btnCancel.Visibility = Visibility.Collapsed;
                    messageBox.btnNo.Visibility = Visibility.Collapsed;
                    messageBox.btnYes.Visibility = Visibility.Collapsed;
                    messageBox.btnOk.Focus();
                    break;
                case MessageBoxButton.OKCancel:
                    messageBox.btnNo.Visibility = Visibility.Collapsed;
                    messageBox.btnYes.Visibility = Visibility.Collapsed;
                    messageBox.btnCancel.Focus();
                    break;
                case MessageBoxButton.YesNo:
                    messageBox.btnOk.Visibility = Visibility.Collapsed;
                    messageBox.btnCancel.Visibility = Visibility.Collapsed;
                    messageBox.btnNo.Focus();
                    break;
                case MessageBoxButton.YesNoCancel:
                    messageBox.btnOk.Visibility = Visibility.Collapsed;
                    messageBox.btnCancel.Focus();
                    break;
                default:
                    break;
            }
        }

        private static void setImageOfMessageBox(MessageBoxImage image)
        {
            switch (image)
            {
                case MessageBoxImage.Warning:
                    messageBox.setImage("Warning.png");
                    break;
                case MessageBoxImage.Question:
                    messageBox.setImage("Question.png");
                    break;
                case MessageBoxImage.Information:
                    messageBox.setImage("Information.png");
                    break;
                case MessageBoxImage.Error:
                    messageBox.setImage("Error.png");
                    break;
                default:
                    messageBox.img.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == btnOk)
                result = MessageBoxResult.OK;
            else if (sender == btnYes)
                result = MessageBoxResult.Yes;
            else if (sender == btnNo)
                result = MessageBoxResult.No;
            else if (sender == btnCancel)
                result = MessageBoxResult.Cancel;
            else
                result = MessageBoxResult.None;

            allowClosing = true;
            Close();
            messageBox = null;
        }



        private void setImage(string imageName)
        {
            string uri = string.Format("/Images/{0}", imageName);
            var uriSource = new Uri(uri, UriKind.RelativeOrAbsolute);
            img.Source = new BitmapImage(uriSource);
        }
    }
}
