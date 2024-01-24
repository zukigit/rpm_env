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
using System.Windows.Input;


namespace jp.co.ftf.jobcontroller.JobController
{
    public class JobArrangerCommands
    {
        public JobArrangerWindow window;
        public static readonly RoutedUICommand File = new RoutedUICommand(
                        Properties.Resources.file_menu_text, "File", typeof(JobArrangerCommands));

        public static readonly RoutedUICommand Import = new RoutedUICommand(
                        Properties.Resources.import_menu_text, "Import", typeof(JobArrangerCommands));

        public static readonly RoutedUICommand Exit = new RoutedUICommand(
                        Properties.Resources.exit_menu_text, "Exit", typeof(JobArrangerCommands));

        public static readonly RoutedUICommand Edit = new RoutedUICommand(
                        Properties.Resources.edit_menu_text, "Edit", typeof(JobArrangerCommands));

        public static readonly RoutedUICommand View = new RoutedUICommand(
                        Properties.Resources.view_menu_text, "View", typeof(JobArrangerCommands));

        public static readonly RoutedUICommand ViewJobnet = new RoutedUICommand(
                        Properties.Resources.jobnet_situation_menu_text, "ViewJobnet", typeof(JobArrangerCommands));

        public static readonly RoutedUICommand ViewErrJobnet = new RoutedUICommand(
                        Properties.Resources.err_jobnet_situation_menu_text, "ViewErrJobnet", typeof(JobArrangerCommands));

        public static readonly RoutedUICommand ViewRunningJobnet = new RoutedUICommand(
                        Properties.Resources.during_jobnet_situation_menu_text, "ViewRunningJobnet", typeof(JobArrangerCommands));

        public static readonly RoutedUICommand Run = new RoutedUICommand(
                        Properties.Resources.run_menu_text, "Run", typeof(JobArrangerCommands));

        public static readonly RoutedUICommand RunImmediate = new RoutedUICommand(
                        Properties.Resources.immediate_run_menu_text, "RunImmediate", typeof(JobArrangerCommands));

        public static readonly RoutedUICommand RunReservation = new RoutedUICommand(
                        Properties.Resources.reservation_run_menu_text, "RunReservation", typeof(JobArrangerCommands));

        public static readonly RoutedUICommand RunTest = new RoutedUICommand(
                        Properties.Resources.test_run_menu_text, "RunTest", typeof(JobArrangerCommands));

        public static readonly RoutedUICommand Ver = new RoutedUICommand(
                        Properties.Resources.ver_menu_text, "Ver", typeof(JobArrangerCommands));

        public static readonly RoutedUICommand New = new RoutedUICommand(
                        Properties.Resources.new_context_menu_text, "New", typeof(JobArrangerCommands));

        public static readonly RoutedUICommand CopyNew = new RoutedUICommand(
                        Properties.Resources.new_version_context_menu_text, "CopyNew", typeof(JobArrangerCommands));

        public static readonly RoutedUICommand Update = new RoutedUICommand(
                        Properties.Resources.update_context_menu_text, "Update", typeof(JobArrangerCommands));

        public static readonly RoutedUICommand Del = new RoutedUICommand(
                        Properties.Resources.del_menu_text, "Del", typeof(JobArrangerCommands));

        public static readonly RoutedUICommand Valid = new RoutedUICommand(
                        Properties.Resources.valid_col_head_text, "Valid", typeof(JobArrangerCommands));

        public static readonly RoutedUICommand Invalid = new RoutedUICommand(
                        Properties.Resources.invalidity_context_menu_text, "Invalid", typeof(JobArrangerCommands));

        public static readonly RoutedUICommand Export = new RoutedUICommand(
                        Properties.Resources.export_menu_text, "Export", typeof(JobArrangerCommands));

        public static readonly RoutedUICommand Hide = new RoutedUICommand(
                        Properties.Resources.hide_context_menu_text, "Hide", typeof(JobArrangerCommands));

        public static readonly RoutedUICommand AllStop = new RoutedUICommand(
                        Properties.Resources.stop_context_menu_text, "AllStop", typeof(JobArrangerCommands));
        public static readonly RoutedUICommand ErrStop = new RoutedUICommand(
                Properties.Resources.stop_context_menu_text, "ErrStop", typeof(JobArrangerCommands));
        public static readonly RoutedUICommand RunningStop = new RoutedUICommand(
                Properties.Resources.stop_context_menu_text, "RunningStop", typeof(JobArrangerCommands));

        //added by YAMA 2014/04/25
        public static readonly RoutedUICommand AllDelayed = new RoutedUICommand(
                Properties.Resources.stop_context_menu_text, "AllDelayed", typeof(JobArrangerCommands));

        //added by YAMA 2014/04/25
        public static readonly RoutedUICommand ErrDelayed = new RoutedUICommand(
                Properties.Resources.stop_context_menu_text, "ErrDelayed", typeof(JobArrangerCommands));

        //added by YAMA 2014/04/25
        public static readonly RoutedUICommand RunningDelayed = new RoutedUICommand(
                Properties.Resources.stop_context_menu_text, "RunningDelayed", typeof(JobArrangerCommands));

        // added by YAMA 2014/10/14    実行予定リスト起動時刻変更
        public static readonly RoutedUICommand Updt = new RoutedUICommand(
                Properties.Resources.updt_context_menu_text, "Updt", typeof(JobArrangerCommands));
        public static readonly RoutedUICommand Reserve = new RoutedUICommand(
                Properties.Resources.reserve_context_menu_text, "Reserve", typeof(JobArrangerCommands));
        public static readonly RoutedUICommand Release = new RoutedUICommand(
                Properties.Resources.release_context_menu_text, "Release", typeof(JobArrangerCommands));

        public static readonly RoutedUICommand Schedule_Delete = new RoutedUICommand(
                Properties.Resources.release_context_menu_text, "Schedule_Delete", typeof(JobArrangerCommands));


        public JobArrangerCommands(JobArrangerWindow window)
        {
            this.window = window;
        }
    }
}
