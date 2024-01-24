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
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows.Forms;


namespace jp.co.ftf.jobcontroller.JobController.Form.FilterEdit
{
    public class ScrollViewerWindowsFormsHost : WindowsFormsHost
    {
        private PresentationSource _presentationSource;


        public ScrollViewerWindowsFormsHost()
        {
            PresentationSource.AddSourceChangedHandler(this, SourceChangedEventHandler);
        }

        protected override void OnWindowPositionChanged(Rect rcBoundingBox)
        {
            base.OnWindowPositionChanged(rcBoundingBox);

            if (ParentScrollViewer == null)
                return;

            GeneralTransform tr = RootVisual.TransformToDescendant(ParentScrollViewer);
            var scrollRect = new Rect(new Size(ParentScrollViewer.ViewportWidth, ParentScrollViewer.ViewportHeight));

            var intersect = Rect.Intersect(scrollRect, tr.TransformBounds(rcBoundingBox));
            if (!intersect.IsEmpty)
            {
                tr = ParentScrollViewer.TransformToDescendant(this);
                intersect = tr.TransformBounds(intersect);
            }
            else
                intersect = new Rect();

            int x1 = (int)Math.Round(intersect.Left);
            int y1 = (int)Math.Round(intersect.Top);
            int x2 = (int)Math.Round(intersect.Right);
            int y2 = (int)Math.Round(intersect.Bottom);

            SetRegion(x1, y1, x2, y2);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                PresentationSource.RemoveSourceChangedHandler(this, SourceChangedEventHandler);
        }

        private void SourceChangedEventHandler(Object sender, SourceChangedEventArgs e)
        {
            ParentScrollViewer = FindParentScrollViewer();
        }

        private ScrollViewer FindParentScrollViewer()
        {
            DependencyObject vParent = this;
            ScrollViewer parentScroll = null;
            while (vParent != null)
            {
                parentScroll = vParent as ScrollViewer;
                if (parentScroll != null)
                    break;

                vParent = LogicalTreeHelper.GetParent(vParent);
            }
            return parentScroll;
        }

        private void SetRegion(int x1, int y1, int x2, int y2)
        {
            SetWindowRgn(Handle, CreateRectRgn(x1, y1, x2, y2), true);
        }

        Visual RootVisual
        {
            get
            {
                if (_presentationSource == null)
                    _presentationSource = PresentationSource.FromVisual(this);

                return _presentationSource.RootVisual;
            }
        }

        ScrollViewer ParentScrollViewer { get; set; }

        [DllImport("User32.dll", SetLastError = true)]
        static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
    }
}
