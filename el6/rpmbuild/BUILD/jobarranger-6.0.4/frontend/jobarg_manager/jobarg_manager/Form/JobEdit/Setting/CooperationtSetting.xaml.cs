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
using System.Windows;
using System.Data;
using System;
using System.Windows.Input;
using jp.co.ftf.jobcontroller.Common;
using jp.co.ftf.jobcontroller.DAO;
using System.Windows.Controls;
using System.Collections.Generic;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author CEC 2014/02/05 新規作成<BR>                           *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.JobEdit
{
    /// <summary>
    /// RebootSetting.xaml の相互作用ロジック
    /// </summary>
    public partial class CooperationSetting : Window
    {
        #region フィールド

        // ホストグループデータ取得 特権管理者用
        private string _selectForHostGroupSqlSuper = "select groups1.groupid, groups1.name as group_name, '3' as permission from hstgrp as groups1 order by groups1.groupid ";

        // ホストデータ取得 特権管理者用 ZabbixVer1.8
        private string _selectForHostSqlSuperZabbixVer1_8 = "select groups1.groupid, groups1.name as group_name, hosts.hostid, hosts.host, '3' as permission " +
            "from hstgrp as groups1 inner join hosts_groups on groups1.groupid = hosts_groups.groupid inner join hosts " +
            //added by YAMA 2014/08/08    （ホスト名でソート）
            //"on hosts_groups.hostid = hosts.hostid where (hosts.status=0 or hosts.status=1) and groups1.groupid = ? order by hostid ASC ";
            "on hosts_groups.hostid = hosts.hostid where (hosts.status=0 or hosts.status=1) and groups1.groupid = ? order by host ASC ";

        // ホストデータ取得 特権管理者用 ZabbixVer2.0
        private string _selectForHostSqlSuperZabbixVer2_0 = "select groups1.groupid, groups1.name as group_name, hosts.hostid, hosts.host, '3' as permission " +
            "from hstgrp as groups1 inner join hosts_groups on groups1.groupid = hosts_groups.groupid " +
            "inner join hosts  on hosts_groups.hostid = hosts.hostid " +
            //added by YAMA 2014/08/08    （ホスト名でソート）
            //"where (hosts.status=0 or hosts.status=1) and groups1.groupid = ? order by hostid ASC ";
            "where (hosts.status=0 or hosts.status=1) and groups1.groupid = ? order by host ASC ";

        // ホストデータ取得 特権管理者用 ZabbixVer2.2
        private string _selectForHostSqlSuperZabbixVer2_2 = "select groups1.groupid, groups1.name as group_name, hosts.hostid, hosts.host, '3' as permission " +
            "from hstgrp as groups1 inner join hosts_groups on groups1.groupid = hosts_groups.groupid inner join hosts " +
            "on hosts_groups.hostid = hosts.hostid where (hosts.status=0 or hosts.status=1) and " +
            //added by YAMA 2014/08/08    （ホスト名でソート）
            //"(hosts.flags=0 or hosts.flags=4) and groups1.groupid = ? order by hostid ASC";
            "(hosts.flags=0 or hosts.flags=4) and groups1.groupid = ? order by host ASC";

        // アイテムデータ取得 ZabbixVer1.8
        private string _selectForItemSqlZabbixVer1_8 = "select hosts.hostid, hosts.host, items.itemid, items.description as item_name, " +
            "items.key_ as item_key, items.type from hosts inner join items on hosts.hostid = items.hostid " +
            "where hosts.hostid = ? and (hosts.status=0 or hosts.status=1) and items.type <> 9 order by hosts.hostid, items.itemid ";

        // アイテムデータ取得 ZabbixVer2.0
        private string _selectForItemSqlZabbixVer2_0 = "select hosts.hostid, hosts.host, items.itemid, " +
            "items.name as item_name, items.key_ as item_key, items.type " +
            "from hosts inner join items on hosts.hostid = items.hostid  " +
            "where hosts.hostid = ? and (hosts.status=0 or hosts.status=1) and items.type <> 9  " +
            "and (items.flags=0 or items.flags=4) order by hosts.hostid, items.itemid  ";

        // アイテムデータ取得 ZabbixVer2.2
        private string _selectForItemSqlZabbixVer2_2 = "select hosts.hostid, hosts.host, items.itemid, items.name as item_name, items.key_ as item_key, items.type " +
            "from hosts inner join items on hosts.hostid = items.hostid where hosts.hostid = ? and (hosts.status=0 or hosts.status=1) " +
            "and (hosts.flags=0 or hosts.flags=4) and items.type <> 9 and (items.flags=0 or items.flags=4) order by hosts.hostid, items.itemid ";

        // トリガーデータ取得 ZabbixVer1.8
        private string _selectForTriggerSqlZabbixVer1_8 = "select hosts.hostid, hosts.host, triggers.triggerid, triggers.expression, triggers.description " +
            "from hosts inner join items on hosts.hostid = items.hostid inner join functions on items.itemid= functions.itemid " +
            "inner join triggers on functions.triggerid = triggers.triggerid where hosts.hostid = ? and (hosts.status=0 or hosts.status=1) " +
            "and items.type <> 9  order by hosts.hostid, items.itemid, triggers.triggerid ";

        // トリガーデータ取得 ZabbixVer2.0
        private string _selectForTriggerSqlZabbixVer2_0 = "select hosts.hostid, hosts.host, triggers.triggerid, " +
            "triggers.expression, triggers.description from hosts inner join items on hosts.hostid = items.hostid inner join functions " +
            "on items.itemid= functions.itemid inner join triggers on functions.triggerid = triggers.triggerid " +
            "where hosts.hostid = ? and (hosts.status=0 or hosts.status=1) and items.type <> 9 " +
            "and (items.flags=0 or items.flags=4) and (triggers.flags=0 or triggers.flags=4) " +
            "order by hosts.hostid, items.itemid, triggers.triggerid";

        // トリガーデータ取得 ZabbixVer2.2
        private string _selectForTriggerSqlZabbixVer2_2 = "select hosts.hostid, hosts.host, triggers.triggerid, triggers.expression, triggers.description " +
            "from hosts inner join items on hosts.hostid = items.hostid inner join functions on items.itemid= functions.itemid " +
            "inner join triggers on functions.triggerid = triggers.triggerid where hosts.hostid = ? and (hosts.status=0 or hosts.status=1) " +
            "and (hosts.flags=0 or hosts.flags=4) and items.type <> 9 and (items.flags=0 or items.flags=4) and (triggers.flags=0 or triggers.flags=4) " +
            "order by hosts.hostid, items.itemid, triggers.triggerid ";

        //added by YAMA 2014/07/25
        // トリガーデータ（条件式表示用）取得 ZabbixVer1.8
        private string _selectForTriggerExpressionSqlZabbixVer1_8 = "select hosts.host,items.key_,functions.name,functions.parameter " +
            "from functions " +
            "inner join items on functions.itemid = items.itemid inner join hosts on items.hostid= hosts.hostid " +
            "where functions.functionid = ? and (hosts.status=0 or hosts.status=1) and items.type <> 9 ";

        // トリガーデータ（条件式表示用）取得 ZabbixVer2.0
        private string _selectForTriggerExpressionSqlZabbixVer2_0 = "select hosts.host,items.key_,functions.name,functions.parameter " +
            "from functions " +
            "inner join items on functions.itemid = items.itemid inner join hosts on items.hostid= hosts.hostid " +
            "where functions.functionid = ? " +
            "and (hosts.status=0 or hosts.status=1) and items.type <> 9 and (items.flags=0 or items.flags=4) ";

        // トリガーデータ（条件式表示用）取得 ZabbixVer2.2
        private string _selectForTriggerExpressionSqlZabbixVer2_2 = "select hosts.host,items.key_,functions.name,functions.parameter " +
            "from functions " +
            "inner join items on functions.itemid = items.itemid inner join hosts on items.hostid= hosts.hostid " +
            "where functions.functionid = ? " +
            "and (hosts.status=0 or hosts.status=1) and (hosts.flags=0 or hosts.flags=4) and items.type <> 9 and (items.flags=0 or items.flags=4) ";

        // 一般ユーザー用SQL

        // ホストグループデータ取得 一般ユーザー用
        private string _selectForHostGroupSql = "select distinct groups1.groupid, groups1.name as group_name, rights.permission " +
            "from users inner join users_groups on users.userid = users_groups.userid " +
            "inner join rights on users_groups.usrgrpid = rights.groupid " +
            "inner join hstgrp as groups1 on rights.id = groups1.groupid " +
            "inner join hosts_groups on groups1.groupid = hosts_groups.groupid " +
            "where users.username = ? and rights.permission >= 0 " +
            // 2014/06/25 PostgreSQL Err対応
            "order by groups1.groupid  ";
//            "order by hosts_groups.groupid  ";

        // ホストデータ取得 一般ユーザー用 ZabbixVer1.8
        private string _selectForHostSqlZabbixVer1_8 = "select users.userid, users.username, rights.id, groups1.groupid, groups1.name  as group_name, " +
            "hosts_groups.hostid, hosts.host, rights.permission from users inner join users_groups on users.userid = users_groups.userid " +
            "inner join rights on users_groups.usrgrpid = rights.groupid inner join hstgrp as groups1 on rights.id = groups1.groupid " +
            "inner join hosts_groups on groups1.groupid = hosts_groups.groupid inner join hosts on hosts_groups.hostid = hosts.hostid " +
            //added by YAMA 2014/08/08    （ホスト名でソート）
            //"where users.alias = ? and rights.permission >= 0 and (hosts.status=0 or hosts.status=1) and groups1.groupid = ? order by hosts_groups.groupid, hosts.hostid ";
            "where users.username = ? and rights.permission >= 0 and (hosts.status=0 or hosts.status=1) and groups1.groupid = ? order by hosts_groups.groupid, hosts.host ";


        // ホストデータ取得 一般ユーザー用 ZabbixVer2.0
        private string _selectForHostSqlZabbixVer2_0 = "select groups1.groupid, groups1.name  as group_name, " +
        "hosts_groups1.hostid, hosts.host, rights.permission from users inner join users_groups on users.userid = users_groups.userid " +
        "inner join rights on users_groups.usrgrpid = rights.groupid inner join hstgrp as groups1 on rights.id = groups1.groupid " +
        "inner join hosts_groups on groups1.groupid = hosts_groups.groupid inner join hosts on hosts_groups.hostid = hosts.hostid " +
            //added by YAMA 2014/08/08    （ホスト名でソート）
        //"where users.alias = ? and rights.permission >= 0 and (hosts.status=0 or hosts.status=1) and groups1.groupid = ? order by hosts_groups.groupid, hosts.hostid ";
        "where users.username = ? and rights.permission >= 0 and (hosts.status=0 or hosts.status=1) and groups1.groupid = ? order by hosts_groups.groupid, hosts.host ";


        // ホストデータ取得 一般ユーザー用 ZabbixVer2.2
        private string _selectForHostSqlZabbixVer2_2 = "select users.userid, users.username, rights.id, groups1.groupid, groups1.name  as group_name, " +
            "hosts_groups.hostid, hosts.host, rights.permission from users inner join users_groups on users.userid = users_groups.userid " +
            "inner join rights on users_groups.usrgrpid = rights.groupid inner join hstgrp as groups1 on rights.id = groups1.groupid " +
            "inner join hosts_groups on groups1.groupid = hosts_groups.groupid inner join hosts on hosts_groups.hostid = hosts.hostid " +
            "where users.username = ? and rights.permission >= 0 and (hosts.status=0 or hosts.status=1) and (hosts.flags=0 or hosts.flags=4) and groups1.groupid = ? " +
            //added by YAMA 2014/08/08    （ホスト名でソート）
            //"order by hosts_groups.groupid, hosts.hostid ";
            "order by hosts_groups.groupid, hosts.host ";



        // 連携対象データを格納
        // ホストグループを格納
        DataTable dtHostGroupData;

        // ホストデータを格納：作業用（ホストグループ単位に格納）
        DataTable dtHostData;

        // ホストデータを格納（正式）
        DataTable dtHostData2 = new DataTable();

        // ホストのアイテムデータを格納
        DataTable dtItemData;
        // ホストのトリガーデータを格納
        DataTable dtTriggerData;

        //added by YAMA 2014/07/25
        // ホストのトリガーデータ（条件式表示用）を格納
        DataTable dtTriggerDataExpression;
        // ホストのトリガーデータの条件式を格納
        DataTable dtTriggerExpression = new DataTable();

        // ホストグループのアクセス権限情報を格納
        Dictionary<string, string> hostGroupPermissionInfo = new Dictionary<string, string>();

        // ホストのアクセス権限情報を格納
        Dictionary<string, string> hostPermissionInfo = new Dictionary<string, string>();

        enum CooperationType {HOSTGROUP, HOST, ITEM, TRIGGER };

        #endregion

        #region コンストラクタ

        public CooperationSetting(IRoom room, string jobId)
        {
            InitializeComponent();

            _myJob = room;

            _oldJobId = jobId;

            SetValues(jobId, Consts.EditType.Modify);

            if (_myJob.ContentItem.InnerJobId != null)
            {
                ChangeButton4DetailRef();
            }
        }
        public CooperationSetting(IRoom room, string jobId, Consts.EditType editType)
        {
            InitializeComponent();

            _myJob = room;

            _oldJobId = jobId;

            SetValues(jobId, editType);

            if (_myJob.ContentItem.InnerJobId != null)
            {
                ChangeButton4DetailRef();
            }
        }
        #endregion

        #region プロパティ
        /// <summary>ジョブ</summary>
        private IRoom _myJob;
        public IRoom MyJob
        {
            get
            {
                return _myJob;
            }
            set
            {
                _myJob = value;
            }
        }

        /// <summary>ジョブID</summary>
        private string _oldJobId;
        public string OldJobId
        {
            get
            {
                return _oldJobId;
            }
            set
            {
                _oldJobId = value;
            }
        }
        #endregion

        #region イベント

        /// <summary>登録処理</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void btnToroku_Click(object sender, RoutedEventArgs e)
        {
            // 入力チェック
            if (!InputCheck())
            {
                return;
            }

            //処理前現在データで履歴を作成
            ((jp.co.ftf.jobcontroller.JobController.Form.JobEdit.Container)_myJob.Container).CreateHistData();

            // 入力されたジョブID
            string newJobId = txtJobId.Text;
            // 入力されたジョブ名
            string newJobNm = txtJobName.Text;

            // ジョブ管理テーブルの更新
            DataRow[] rowJobCon = _myJob.Container.JobControlTable.Select("job_id='" + _oldJobId + "'");
            if (rowJobCon != null && rowJobCon.Length > 0)
            {
                rowJobCon[0]["job_id"] = newJobId;
                rowJobCon[0]["job_name"] = newJobNm;
            }

            // Zabbix連携アイコン設定テーブルの更新
            DataRow[] rowIconCooperation = _myJob.Container.IconCooperationTable.Select("job_id='" + _oldJobId + "'");
            if (rowIconCooperation != null && rowIconCooperation.Length > 0)
            {
                // ジョブID
                rowIconCooperation[0]["job_id"] = newJobId;

                // ホストグループを選択
                if (rbHostGroup.IsChecked == true)
                {
                    // 連携対象
                    rowIconCooperation[0]["link_target"] = CooperationType.HOSTGROUP;

                    // ホストグループID
                    rowIconCooperation[0]["groupid"] = combHostGroup.SelectedValue.ToString();

                    // ホストID
                    rowIconCooperation[0]["hostid"] = "0";

                    // アイテムID
                    rowIconCooperation[0]["itemid"] = "0";

                    // トリガーID
                    rowIconCooperation[0]["triggerid"] = "0";

                } // ホストを選択
                else if (rbHostName.IsChecked == true)
                {
                    // 連携対象
                    rowIconCooperation[0]["link_target"] = CooperationType.HOST;

                    // ホストグループID
                    rowIconCooperation[0]["groupid"] = chgALL2ZeroIdNo(combHostGroup.SelectedValue.ToString());

                    // ホストID
                    rowIconCooperation[0]["hostid"] = combHostName.SelectedValue.ToString();

                    // アイテムID
                    rowIconCooperation[0]["itemid"] = "0";

                    // トリガーID
                    rowIconCooperation[0]["triggerid"] = "0";

                } // アイテムを選択
                else if (rbItemName.IsChecked == true)
                {
                    // 連携対象
                    rowIconCooperation[0]["link_target"] = CooperationType.ITEM;

                    // ホストグループID
                    rowIconCooperation[0]["groupid"] = chgALL2ZeroIdNo(combHostGroup.SelectedValue.ToString());

                    // ホストID
                    rowIconCooperation[0]["hostid"] = combHostName.SelectedValue.ToString();

                    // アイテムID
                    rowIconCooperation[0]["itemid"] = combItemName.SelectedValue.ToString();

                    // トリガーID
                    rowIconCooperation[0]["triggerid"] = "0";

                } // トリガーを選択
                else
                {
                    // 連携対象
                    rowIconCooperation[0]["link_target"] = CooperationType.TRIGGER;

                    // ホストグループID
                    rowIconCooperation[0]["groupid"] = chgALL2ZeroIdNo(combHostGroup.SelectedValue.ToString());

                    // ホストID
                    rowIconCooperation[0]["hostid"] = combHostName.SelectedValue.ToString();

                    // アイテムID
                    rowIconCooperation[0]["itemid"] = "0";

                    // トリガーID
                    rowIconCooperation[0]["triggerid"] = combTriggerName.SelectedValue.ToString();

                }

                // 連携動作
                RadioButton[] rbMode = new RadioButton[4];
                rbMode[0] = rbEnabled;
                rbMode[1] = rbDisabled;
                rbMode[2] = rbGetStat;
                rbMode[3] = rbGetData;
                int idx = 0;
                for (idx = 0; idx < 4; idx++)
                {
                    if (rbMode[idx].IsChecked == true)
                    {
                        break;
                    }
                }
                rowIconCooperation[0]["link_operation"] = idx;
            }

            // ジョブIDが変更された場合、フロー管理テーブルを更新
            if (!_oldJobId.Equals(newJobId))
                CommonUtil.UpdateFlowForJobId(_myJob.Container.FlowControlTable, _oldJobId, newJobId);

            // 画面再表示
            _myJob.Container.JobItems.Remove(_oldJobId);
            _myJob.Container.JobItems.Add(newJobId, _myJob);
            _myJob.JobId = newJobId;
            _myJob.JobName = newJobNm;
            _myJob.Container.SetedJobIds[_myJob.JobId] = "1";
            this.Close();
        }

        /// <summary>ホストグループ名を選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void rbHostGroup_Checked(object sender, RoutedEventArgs e)
        {
            combHostGroup.IsEnabled = true;
            combHostName.IsEnabled = false;
            combItemName.IsEnabled = false;
            combTriggerName.IsEnabled = false;

            // ホストグループ、ホスト共に未選択時
            if (combHostGroup.SelectedValue == null && combHostName.SelectedValue == null)
            {
                // 初期設定
                rbEnabled.IsEnabled = true;
                rbDisabled.IsEnabled = true;
                rbGetStat.IsEnabled = false;
                rbGetData.IsEnabled = false;
            }
            // ホストグループ「全て」選択、ホスト未選択時
            else if (combHostGroup.SelectedValue.ToString() == "ALL" && combHostName.SelectedValue == null)
            {
                // 初期設定
                rbEnabled.IsEnabled = true;
                rbDisabled.IsEnabled = true;
                rbGetStat.IsEnabled = false;
                rbGetData.IsEnabled = false;
            }
            // ホストグループ「全て」選択、ホスト選択時
            else if (combHostGroup.SelectedValue.ToString() == "ALL" && combHostName.SelectedValue != null)
            {
                // ホストの権限チェック
                //CtlHostPermission(combHostName.SelectedValue.ToString());
                // 初期設定
                rbEnabled.IsEnabled = true;
                rbDisabled.IsEnabled = true;
                rbGetStat.IsEnabled = false;
                rbGetData.IsEnabled = false;

            }
            // ホストグループ選択、ホスト未選択時
            else if (combHostName.SelectedValue == null)
            {
                // ホストグループの権限チェック
                CtlHostGrpPermission(combHostGroup.SelectedValue.ToString());
            }
            // ホストグループ、ホスト共に選択時
            else
            {

                // ホストグループの権限チェック
                CtlHostGrpPermission(combHostGroup.SelectedValue.ToString());
            }

 }


        /// <summary>ホスト名を選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void rbHostName_Checked(object sender, RoutedEventArgs e)
        {
            combHostName.IsEnabled = true;
            combHostGroup.IsEnabled = true;
            combItemName.IsEnabled = false;
            combTriggerName.IsEnabled = false;

            // ホストグループ、ホスト共に未選択時
            if (combHostGroup.SelectedValue == null && combHostName.SelectedValue == null)
            {
                // 初期設定
                rbEnabled.IsEnabled = true;
                rbDisabled.IsEnabled = true;
                rbGetStat.IsEnabled = true;
                rbGetData.IsEnabled = false;
            }
            // ホストグループ「全て」選択、ホスト未選択時
            else if (combHostGroup.SelectedValue.ToString() == "ALL" && combHostName.SelectedValue == null)
            {
                // 初期設定
                rbEnabled.IsEnabled = true;
                rbDisabled.IsEnabled = true;
                rbGetStat.IsEnabled = true;
                rbGetData.IsEnabled = false;
            }
            // ホストグループ「全て」選択、ホスト選択時
            else if (combHostGroup.SelectedValue.ToString() == "ALL" && combHostName.SelectedValue != null)
            {
                // ホストの権限チェック
                CtlHostPermission(combHostName.SelectedValue.ToString());
            }
            // ホストグループ選択、ホスト未選択時
            else if (combHostName.SelectedValue == null)
            {
                // ホストグループの権限チェック
                CtlHostGrpPermission(combHostGroup.SelectedValue.ToString());
            }
            // ホストグループ、ホスト共に選択時
            else
            {
                // ホストの権限チェック
                CtlHostPermission(combHostName.SelectedValue.ToString());
            }

        }


        /// <summary>アイテム名を選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void rbItemName_Checked(object sender, RoutedEventArgs e)
        {
            combItemName.IsEnabled = true;
            combHostGroup.IsEnabled = true;
            combHostName.IsEnabled = true;
            combTriggerName.IsEnabled = false;

            // ホストグループ、ホスト共に未選択時
            if (combHostGroup.SelectedValue == null && combHostName.SelectedValue == null)
            {
                // 初期設定
                rbEnabled.IsEnabled = true;
                rbDisabled.IsEnabled = true;
                rbGetStat.IsEnabled = true;
                rbGetData.IsEnabled = true;
            }
            // ホストグループ「全て」選択、ホスト未選択時
            else if (combHostGroup.SelectedValue.ToString() == "ALL" && combHostName.SelectedValue == null)
            {
                // 初期設定
                rbEnabled.IsEnabled = true;
                rbDisabled.IsEnabled = true;
                rbGetStat.IsEnabled = true;
                rbGetData.IsEnabled = true;
            }
            // ホストグループ「全て」選択、ホスト選択時
            else if (combHostGroup.SelectedValue.ToString() == "ALL" && combHostName.SelectedValue != null)
            {
                // ホストの権限チェック
                CtlHostPermission(combHostName.SelectedValue.ToString());
            }
            // ホストグループ選択、ホスト未選択時
            else if (combHostName.SelectedValue == null)
            {
                // ホストグループの権限チェック
                CtlHostGrpPermission(combHostGroup.SelectedValue.ToString());
            }
            // ホストグループ、ホスト共に選択時
            else
            {
                // ホストの権限チェック
                CtlHostPermission(combHostName.SelectedValue.ToString());
            }

        }


        /// <summary>トリガー名を選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void rbTriggerName_Checked(object sender, RoutedEventArgs e)
        {
            combTriggerName.IsEnabled = true;
            combHostGroup.IsEnabled = true;
            combHostName.IsEnabled = true;
            combItemName.IsEnabled = false;

            // ホストグループ、ホスト共に未選択時
            if (combHostGroup.SelectedValue == null && combHostName.SelectedValue == null)
            {
                // 初期設定
                rbEnabled.IsEnabled = true;
                rbDisabled.IsEnabled = true;
                rbGetStat.IsEnabled = true;
                rbGetData.IsEnabled = false;
            }
            // ホストグループ「全て」選択、ホスト未選択時
            else if (combHostGroup.SelectedValue.ToString() == "ALL" && combHostName.SelectedValue == null)
            {
                // 初期設定
                rbEnabled.IsEnabled = true;
                rbDisabled.IsEnabled = true;
                rbGetStat.IsEnabled = true;
                rbGetData.IsEnabled = false;
            }
            // ホストグループ「全て」選択、ホスト選択時
            else if (combHostGroup.SelectedValue.ToString() == "ALL" && combHostName.SelectedValue != null)
            {
                // ホストの権限チェック
                CtlHostPermission(combHostName.SelectedValue.ToString());
            }
            // ホストグループ選択、ホスト未選択時
            else if (combHostName.SelectedValue == null)
            {
                // ホストグループの権限チェック
                CtlHostGrpPermission(combHostGroup.SelectedValue.ToString());
            }
            // ホストグループ、ホスト共に選択時
            else
            {
                // ホストの権限チェック
                CtlHostPermission(combHostName.SelectedValue.ToString());
            }
        }


        /// <summary>有効化を選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>


        /// <summary>無効化を選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>


        /// <summary>状態取得を選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>


        /// <summary>データ取得を選択</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>

        /// <summary>キャンセルをクリック</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //*******************************************************************
        /// <summary>画面を閉める</summary>
        /// <param name="sender">源</param>
        /// <param name="e">イベント</param>
        //*******************************************************************
        private void Window_Closed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_myJob.ItemEditType == Consts.EditType.READ && _myJob.Container.ParantWindow is JobEdit)
                _myJob.ResetInitColor();
        }

        #endregion

        #region publicメッソド
        public void SetDisable()
        {
            txtJobId.IsEnabled = false;
            txtJobName.IsEnabled = false;
            btnToroku.IsEnabled = false;

            rbHostGroup.IsEnabled = false;
            rbHostName.IsEnabled = false;
            rbItemName.IsEnabled = false;
            rbTriggerName.IsEnabled = false;

            combHostGroup.IsEnabled = false;
            combHostName.IsEnabled = false;
            combItemName.IsEnabled = false;
            combTriggerName.IsEnabled = false;

            rbEnabled.IsEnabled = false;
            rbDisabled.IsEnabled = false;
            rbGetStat.IsEnabled = false;
            rbGetData.IsEnabled = false;

        }
        #endregion

        #region privateメッソド

        /// <summary> 値のセットと表示処理</summary>
        /// <param name="sender">源</param>
        private void SetValues(string jobId, Consts.EditType editType)
        {

            bool existFlg;

            // ジョブ管理テーブルのデータを取得
            DataRow[] rowJob = _myJob.Container.JobControlTable.Select("job_id='" + jobId + "'");
            if (rowJob != null && rowJob.Length > 0)
            {
                txtJobId.Text = jobId;
                txtJobName.Text = Convert.ToString(rowJob[0]["job_name"]);
            }

            DBConnect dbAccess = new DBConnect(LoginSetting.ConnectStr);
            dbAccess.CreateSqlConnect();


            // 列を定義します。
            // 最終的にホストを格納する
            dtHostData2.Columns.Add("hostid", Type.GetType("System.String"));
            dtHostData2.Columns.Add("host", Type.GetType("System.String"));
            dtHostData2.Columns.Add("permission", Type.GetType("System.String"));
            //added by YAMA 2014/07/25
            dtTriggerExpression.Columns.Add("expression", Type.GetType("System.String"));


            // ホストグループデータ取得
            if (LoginSetting.Authority == Consts.AuthorityEnum.SUPER)
            {
                dtHostGroupData = dbAccess.ExecuteQuery(_selectForHostGroupSqlSuper);
            }
            else
            {
                List<ComSqlParam> sqlParams = new List<ComSqlParam>();
                sqlParams.Add(new ComSqlParam(DbType.String, "@username", LoginSetting.UserName));


                dtHostGroupData = dbAccess.ExecuteQuery(_selectForHostGroupSql, sqlParams);
            }


            // 取得したホストグループのアクセス権限を求める
            // ホストグループは複数のユーザーグループに属すことが可能なので、アクセス権限も重複することがある
            // 重複する場合のアクセス権限の優先順位は 0 > 3 > 2 で一つに集約する
            hostGroupPermissionInfo.Clear();
            chkHostGroupPermission();


            // 取得したホストグループに属する重複を含む全てのホストのアクセス権限を求める
            // ホストは複数のホストグループに属すことが可能なので、アクセス権限も重複することがある
            // 重複する場合のアクセス権限の優先順位は 0 > 3 > 2 で一つに集約する
            hostPermissionInfo.Clear();
            chkHostPermission();

            // Zabbix連携アイコン設定テーブルのデータを取得
            DataRow[] rowIconCooperation;
            if (_myJob.ContentItem.InnerJobId == null)
            {
                rowIconCooperation = _myJob.Container.IconCooperationTable.Select("job_id='" + jobId + "'");
            }
            else
            {
                rowIconCooperation = _myJob.Container.IconCooperationTable.Select("inner_job_id=" + _myJob.ContentItem.InnerJobId);
            }

            if (rowIconCooperation != null && rowIconCooperation.Length > 0)
            {

                // 前回選択された設定内容を取得

                // 連携対象
                int linkTarget = Convert.ToInt16(rowIconCooperation[0]["link_target"]);

                // 前回選択されたホストグループID
                string selectedHostGrpID = Convert.ToString(rowIconCooperation[0]["groupid"]);

                // 前回選択されたホストID
                string selectedHostID = Convert.ToString(rowIconCooperation[0]["hostid"]);

                // 前回選択されたアイテムID
                string selectedItemID = Convert.ToString(rowIconCooperation[0]["itemid"]);

                // 前回選択されたトリガーID
                string selectedTriggerID = Convert.ToString(rowIconCooperation[0]["triggerid"]);

                // ホストグループコンボボックスにホストグループデータを設定
                setcombHostGroup2();


                // 新規作成以外は、前回状態を再現する
                //if (editType != Consts.EditType.Add)
                if (selectedHostGrpID != "")
                {
                    existFlg = false;
                    // 前回選択したホストグループをコンボボックスに表示
                    for (int i = 0; i < dtHostGroupData.Rows.Count; i++)
                    {
                        if (selectedHostGrpID == dtHostGroupData.Rows[i]["groupid"].ToString())
                        {
                            combHostGroup.SelectedValue = selectedHostGrpID;
                            existFlg = true;
                            break;
                        }
                    }

                    // 前回、ALLを選択していた場合
                    if (selectedHostGrpID == "0")
                    {
                        combHostGroup.SelectedValue = "ALL";
                        existFlg = true;
                    }
                    // 前回選択したホストグループが削除等により無い場合
                    if (existFlg == false)
                    {
                        // 何もしない
                    }
                    else
                    {
                        // 選択されたホストグループに属するホストデータをホストコンボボックスに設定
                        setcombHostName2();
                    }

                }

                if (linkTarget == (int)CooperationType.HOSTGROUP)    // 連携対象はホストグループ
                {
                    // ホストグループを選択
                    rbHostGroup.IsChecked = true;

                    combHostGroup.IsEnabled = true;
                    combHostName.IsEnabled = false;
                    combItemName.IsEnabled = false;
                    combTriggerName.IsEnabled = false;

                }
                else
                {
                    // 新規作成以外は、前回状態を再現する
                    //if (editType != Consts.EditType.Add)
                    if (selectedHostID != "")
                    {
                        existFlg = false;
                        // 前回選択したホスト名をコンボボックスに表示
                        for (int i = 0; i < dtHostData2.Rows.Count; i++)
                        {
                            if (selectedHostID == dtHostData2.Rows[i]["hostid"].ToString())
                            {
                                combHostName.SelectedValue = selectedHostID;
                                existFlg = true;
                                break;
                            }
                        }

                        // 前回選択したホストが削除等により無い場合は設定しない
                        // 前回選択したホストに属するアイテムとトリガーのデータをコンボボックスに設定
                        if (existFlg == true)
                        {
                            setcomboItemAndTrigger2(combHostName.SelectedValue.ToString());
                        }

                        switch (linkTarget)
                        {
                            // ホストを選択
                            case (int)CooperationType.HOST:
                                rbHostName.IsChecked = true;

                                combHostName.IsEnabled = true;
                                combHostGroup.IsEnabled = true;
                                combItemName.IsEnabled = false;
                                combTriggerName.IsEnabled = false;

                                break;
                            // アイテムを選択
                            case (int)CooperationType.ITEM:
                                // 前回選択したアイテム名をコンボボックスに表示
                                if (existFlg == true)
                                {
                                    for (int i = 0; i < dtItemData.Rows.Count; i++)
                                    {
                                        if (selectedItemID == dtItemData.Rows[i]["itemid"].ToString())
                                        {
                                            combItemName.SelectedValue = selectedItemID;
                                            break;
                                        }
                                    }
                                }
                                rbItemName.IsChecked = true;

                                combItemName.IsEnabled = true;
                                combHostGroup.IsEnabled = true;
                                combHostName.IsEnabled = true;
                                combTriggerName.IsEnabled = false;
                                break;

                            // トリガーを選択
                            case (int)CooperationType.TRIGGER:
                                // 前回選択したトリガー名をコンボボックスに表示
                                if (existFlg == true)
                                {
                                    for (int i = 0; i < dtTriggerData.Rows.Count; i++)
                                    {
                                        if (selectedTriggerID == dtTriggerData.Rows[i]["triggerid"].ToString())
                                        {
                                            combTriggerName.SelectedValue = selectedTriggerID;
                                            break;
                                        }
                                    }
                                }
                                rbTriggerName.IsChecked = true;

                                combTriggerName.IsEnabled = true;
                                combHostGroup.IsEnabled = true;
                                combHostName.IsEnabled = true;
                                combItemName.IsEnabled = false;
                                break;
                        }

                        if (existFlg == true)
                        {
                            // 選択したホストのアクセス権限に応じて連係動作を制御
                            CtlHostPermission(selectedHostID);
                        }
                    }
                }

                // 連携動作
                int linkOperation = Convert.ToInt16 (rowIconCooperation[0]["link_operation"]);

                switch (linkOperation)
                {
                    // 有効化
                    case 0:
                        //added by YAMA 2014/07/25  (新規作成時は『"有効"にチェック』を外す)
                        //rbEnabled.IsChecked = true;
                        if (selectedHostGrpID != "")
                        {   // 新規作成でない
                            rbEnabled.IsChecked = true;
                        }
                        else
                        {   // 新規作成
                            rbEnabled.IsChecked = false;
                        }
                        break;
                    // 無効化
                    case 1:
                        rbDisabled.IsChecked = true;
                        break;
                    // 状態取得
                    case 2:
                        rbGetStat.IsChecked = true;
                        break;
                    // データ取得
                    case 3:
                        rbGetData.IsChecked = true;
                        break;
                }
            }
            else
            {

            }
        }


        /// <summary> 各項目のチェック処理(登録)</summary>
        private bool InputCheck()
        {
            // ジョブID
            string jobIdForChange = Properties.Resources.err_message_job_id;
            String jobId = txtJobId.Text;
            // 未入力の場合
            if (CheckUtil.IsNullOrEmpty(jobId))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { jobIdForChange });
                return false;
            }
            // バイト数チェック
            if (CheckUtil.IsLenOver(jobId, 32))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { jobIdForChange, "32" });
                return false;
            }
            // 半角英数値、「-」、「_」チェック
            if (!CheckUtil.IsHankakuStrAndHyphenAndUnderbar(jobId))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_013,
                    new string[] { jobIdForChange });
                return false;
            }
            // 予約語（START）チェック
            if (CheckUtil.IsHoldStrSTART(jobId))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_001);
                return false;
            }
            // すでに登録済みの場合
            DataRow[] rowJob = _myJob.Container.JobControlTable.Select("job_id='" + jobId + "'");
            if (rowJob != null && rowJob.Length > 0)
            {
                foreach (DataRow row in rowJob)
                {
                    if (!jobId.Equals(_oldJobId) && jobId.Equals(row["job_id"]))
                    {
                        CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_004,
                            new string[] { jobIdForChange });
                        return false;
                    }
                }
            }

            // ジョブ名
            string jobNameForChange = Properties.Resources.err_message_job_name;
            String jobName = txtJobName.Text;
            // バイト数チェック
            if (CheckUtil.IsLenOver(jobName, 64))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003,
                    new string[] { jobNameForChange, "64" });
                return false;
            }

            // 入力不可文字「"'\,」チェック
            if (CheckUtil.IsImpossibleStr(jobName))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_025,
                    new string[] { jobNameForChange });
                return false;
            }

            // ホストグループ未選択
            if (combHostGroup.SelectedIndex == -1)
            {
                string hostNameGropuForChange = Properties.Resources.err_message_host_gropu_name;
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                new string[] { hostNameGropuForChange });
                return false;
            }

            // ホストグループ選択時
            if (rbHostGroup.IsChecked == true)
            {
                // ホストグループ名に「全て」が選択されていないこと
                if (combHostGroup.SelectedValue.ToString() == "ALL")
                {
                    string hostNameGropuForChange = Properties.Resources.err_message_host_gropu_name;
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { hostNameGropuForChange });
                    return false;
                }

                // 連携動作「状態取得」は無効
                if (rbGetStat.IsChecked == true)
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_ZABBIX_LINK_002);
                    return false;
                }
            }
            else
            {
                // ホスト未選択
                if (combHostName.SelectedIndex == -1)
                {
                    string hostNameForChange = Properties.Resources.err_message_host_name;
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { hostNameForChange });
                    return false;
                }
            }

            // ホスト選択時
            if (rbHostName.IsChecked == true)
            {
            }

            // アイテム選択時
            if (rbItemName.IsChecked == true)
            {
                // アイテム未選択
                if (combItemName.SelectedIndex == -1)
                {
                    string itemNameForChange = Properties.Resources.err_message_item_name;
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { itemNameForChange });
                    return false;
                }
            }

            // トリガー選択時
            if (rbTriggerName.IsChecked == true)
            {
                // トリガー未選択
                if (combTriggerName.SelectedIndex == -1)
                {
                    string triggerNameForChange = Properties.Resources.err_message_trigger_name;
                    CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001,
                    new string[] { triggerNameForChange });
                    return false;
                }
            }

            // 連携対象がアイテム以外選択時、連携動作「データ取得」は無効
            if (rbItemName.IsChecked == false)
            {
                if (rbGetData.IsChecked == true)
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_ZABBIX_LINK_001);
                    return false;
                }
            }

            // 非活性連携動作が選択されている場合
            RadioButton[] rbMode = new RadioButton[4];
            rbMode[0] = rbEnabled;
            rbMode[1] = rbDisabled;
            rbMode[2] = rbGetStat;
            rbMode[3] = rbGetData;
            int idx = 0;
            for (idx = 0; idx < 4; idx++)
            {
                if (rbMode[idx].IsChecked == true && rbMode[idx].IsEnabled == false)
                {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_ZABBIX_LINK_004);
                    return false;
                }
            }

            //added by YAMA 2014/07/25
            // 連携動作のチェックボックスが選択されているか否かの確認
            int num = 0;
            idx = 0;
            for (idx = 0; idx < rbMode.Length; idx++)
            {
                if (rbMode[idx].IsChecked == true)
                {
                    num++;
                    break;
                }
            }
            if (num == 0)
            {
                    CommonDialog.ShowErrorDialog(Consts.ERROR_ZABBIX_LINK_004);
                    return false;
            }

            return true;
        }

        /// <summary> 詳細画面からの参照時のボタンの切り替え</summary>
        private void ChangeButton4DetailRef()
        {
            GridSetting.Children.Remove(btnToroku);
            System.Windows.Controls.Button button = new System.Windows.Controls.Button();
            btnCancel.Content = Properties.Resources.close_button_text;
            btnCancel.IsDefault = true;
        }
        #endregion


        private void combHostGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 設定画面表示時の前回選択データを再現時は除く
            if (combHostGroup.SelectedValue != null)
            {
                // setcombHostName();

                setcombHostName2();

                // アイテムコンボボックスはクリア
                combItemName.ItemsSource = null;
                combItemName.Items.Clear();

                // トリガーコンボボックスはクリア
                combTriggerName.ItemsSource = null;
                combTriggerName.Items.Clear();

                // 選択したホストグループのアクセス権限に応じて連係動作を制御
                CtlHostGrpPermission(combHostGroup.SelectedValue.ToString());

            }


        }


        private void combHostName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ホストが選択されている場合
            if (combHostName.SelectedIndex != -1)
            {
                if (combHostName.SelectedValue != null)
                {
                    // ホストに属するアイテムとトリガーのデータをコンボボックスに設定
                    setcomboItemAndTrigger2(combHostName.SelectedValue.ToString());

                    // 選択したホストのアクセス権限に応じて連係動作を制御
                    CtlHostPermission(combHostName.SelectedValue.ToString());
                }
            }
        }



        private void setcombHostGroup2()
        {
            //DataTableオブジェクトを用意
            DataTable CombHstGrp = new DataTable();

            //DataTableに列を追加
            CombHstGrp.Columns.Add("ID", typeof(string));
            CombHstGrp.Columns.Add("NAME", typeof(string));

            CombHstGrp.Columns.Add("PERMISSION", typeof(string));

            // 「全て」を1行目に設定する
            DataRow CombHstGrpRow = CombHstGrp.NewRow();
            //各列に値をセット
            CombHstGrpRow["ID"] = "ALL";
            CombHstGrpRow["NAME"] = jp.co.ftf.jobcontroller.JobController.Properties.Resources.cooperation_all_label_text;
            CombHstGrpRow["PERMISSION"] = "3";

            //DataTableに「全て」行を追加
            CombHstGrp.Rows.Add(CombHstGrpRow);

            // ホストグループ名を設定する
            string stHostGroupID;
            string stHostGroupPermission;

            List<string> hostGroupDataList = new List<string>();

            for (int i = 0; i < dtHostGroupData.Rows.Count; i++)
            {
                // 重複しないホストグループデータを設定する
                stHostGroupID = dtHostGroupData.Rows[i]["groupid"].ToString();

                if (hostGroupPermissionInfo.TryGetValue(stHostGroupID, out stHostGroupPermission) == true)
                {
                    if (stHostGroupPermission != "0")
                    {
                        if (hostGroupDataList.Contains(stHostGroupID) == false)
                        {
                            stHostGroupPermission = dtHostGroupData.Rows[i]["permission"].ToString();
                            DataRow CombHstGrpRow2 = CombHstGrp.NewRow();

                            CombHstGrpRow2["ID"] = dtHostGroupData.Rows[i]["groupid"].ToString();
                            CombHstGrpRow2["NAME"] = dtHostGroupData.Rows[i]["group_name"].ToString();
                            CombHstGrpRow2["PERMISSION"] = stHostGroupPermission;
                            CombHstGrp.Rows.Add(CombHstGrpRow2);

                            hostGroupDataList.Add(stHostGroupID);

                        }
                    }
                }
            }

            CombHstGrp.AcceptChanges();

            // ホストグループコンボボックスにホストグループを設定
            combHostGroup.Items.Clear();
            combHostGroup.ItemsSource = CombHstGrp.DefaultView;
            combHostGroup.DisplayMemberPath = "NAME";
            combHostGroup.SelectedValuePath = "ID";
        }





        // ALL を変換する
        private string chgALL2ZeroIdNo(string id)
        {
            if (id == "ALL")
            {
                return "0";
            }
            else
            {
                return id;
            }
        }


        private void getHostData(string selectedHostGroupId)
        {
            DBConnect dbAccess = new DBConnect(LoginSetting.ConnectStr);
            dbAccess.CreateSqlConnect();

            // ホストデータ取得
            if (LoginSetting.Authority == Consts.AuthorityEnum.SUPER)
            {
                List<ComSqlParam> sqlParams = new List<ComSqlParam>();
                sqlParams.Add(new ComSqlParam(DbType.String, "@hostgroupid", selectedHostGroupId));

                // Zabbixバージョンに応じたSQLを設定
                switch (LoginSetting.JaZabbixVersion)
                {
                    // Ver1.8
                    case 1:
                        dtHostData = dbAccess.ExecuteQuery(_selectForHostSqlSuperZabbixVer1_8, sqlParams);
                        break;
                    // Ver2.0
                    case 2:
                        dtHostData = dbAccess.ExecuteQuery(_selectForHostSqlSuperZabbixVer2_0, sqlParams);
                        break;

                    // Ver2.2
                    case 3:
                        dtHostData = dbAccess.ExecuteQuery(_selectForHostSqlSuperZabbixVer2_2, sqlParams);
                        break;
                }
            }
            else
            {
                List<ComSqlParam> sqlParams = new List<ComSqlParam>();
                sqlParams.Add(new ComSqlParam(DbType.String,  "@username", LoginSetting.UserName));
                sqlParams.Add(new ComSqlParam(DbType.String, "@groupid", selectedHostGroupId));

                // Zabbixバージョンに応じたSQLを設定
                switch (LoginSetting.JaZabbixVersion)
                {
                    case 1:
                        dtHostData = dbAccess.ExecuteQuery(_selectForHostSqlZabbixVer1_8, sqlParams);
                        break;
                    case 2:
                        dtHostData = dbAccess.ExecuteQuery(_selectForHostSqlZabbixVer2_0, sqlParams);
                        break;
                    case 3:
                        dtHostData = dbAccess.ExecuteQuery(_selectForHostSqlZabbixVer2_2, sqlParams);
                        break;
                }
            }


        }

        private void setcombHostName2()
        {

            DBConnect dbAccess = new DBConnect(LoginSetting.ConnectStr);
            dbAccess.CreateSqlConnect();

            //DataTableオブジェクトを用意
            DataTable CombHstTbl = new DataTable();


            dtHostData2.Clear();

            //DataTableに列を追加
            CombHstTbl.Columns.Add("ID", typeof(string));
            CombHstTbl.Columns.Add("NAME", typeof(string));

            CombHstTbl.Columns.Add("PERMISSION", typeof(string));

            string stHostID, stHostPermission;

            List<string> hostDataList = new List<string>();

            if (combHostGroup.SelectedValue.ToString() == "ALL")
            {
                // 全てのホストグループに属するホスト
                for (int i = 0; i < dtHostGroupData.Rows.Count; i++)
                {
                    getHostData(dtHostGroupData.Rows[i]["groupid"].ToString());
                    for (int j = 0; j < dtHostData.Rows.Count; j++)
                    {
                        // 重複しないホストデータを設定する
                        stHostID = dtHostData.Rows[j]["hostid"].ToString();
                        if (hostPermissionInfo.TryGetValue(stHostID, out stHostPermission) == true)
                        {
                            if (stHostPermission != "0")
                            {
                                if (hostDataList.Contains(stHostID) == false)
                                {
                                    DataRow CombHstRow = CombHstTbl.NewRow();

                                    CombHstRow["ID"] = stHostID;
                                    CombHstRow["NAME"] = dtHostData.Rows[j]["host"].ToString();
                                    CombHstRow["PERMISSION"] = stHostPermission;

                                    CombHstTbl.Rows.Add(CombHstRow);

                                    DataRow newRow = dtHostData2.NewRow();
                                    newRow["hostid"] = stHostID;
                                    newRow["host"] = dtHostData.Rows[j]["host"].ToString();
                                    newRow["permission"] = stHostPermission;
                                    dtHostData2.Rows.Add(newRow);

                                    hostDataList.Add(stHostID);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // 選択されたホストグループに属するホストをコンボボックスに設定
                getHostData(combHostGroup.SelectedValue.ToString());

                for (int j = 0; j < dtHostData.Rows.Count; j++)
                {
                    // 重複しないホストデータを設定する
                    stHostID = dtHostData.Rows[j]["hostid"].ToString();
                    if (hostPermissionInfo.TryGetValue(stHostID, out stHostPermission) == true)
                    {
                        if (stHostPermission != "0")
                        {
                            if (hostDataList.Contains(stHostID) == false)
                            {
                                DataRow CombHstRow = CombHstTbl.NewRow();

                                CombHstRow["ID"] = stHostID;
                                CombHstRow["NAME"] = dtHostData.Rows[j]["host"].ToString();
                                CombHstRow["PERMISSION"] = stHostPermission;

                                CombHstTbl.Rows.Add(CombHstRow);

                                DataRow newRow = dtHostData2.NewRow();
                                newRow["hostid"] = stHostID;
                                newRow["host"] = dtHostData.Rows[j]["host"].ToString();
                                newRow["permission"] = stHostPermission;
                                dtHostData2.Rows.Add(newRow);

                                hostDataList.Add(stHostID);
                            }
                        }
                    }
                }
            }

            CombHstTbl.AcceptChanges();

            //combHostName.Items.Clear();
            combHostName.ItemsSource = CombHstTbl.DefaultView;
            combHostName.DisplayMemberPath = "NAME";
            combHostName.SelectedValuePath = "ID";
        }



        private void getItemAndTriggerData2(string selectedHostId)
        {
            DBConnect dbAccess = new DBConnect(LoginSetting.ConnectStr);
            dbAccess.CreateSqlConnect();

            List<ComSqlParam> sqlParams = new List<ComSqlParam>();
            sqlParams.Add(new ComSqlParam(DbType.String, "@hostid", selectedHostId));

            // アイテム・トリガーデータ取得
            // Zabbixバージョンに応じたSQLを設定
            switch (LoginSetting.JaZabbixVersion)
            {
                // Ver1.8
                case 1:
                    dtItemData = dbAccess.ExecuteQuery(_selectForItemSqlZabbixVer1_8, sqlParams);
                    dtTriggerData = dbAccess.ExecuteQuery(_selectForTriggerSqlZabbixVer1_8, sqlParams);
                    //added by YAMA 2014/07/25  [条件式の置き換え]
                    setConditionalExpression(LoginSetting.JaZabbixVersion, dbAccess);
                    break;
                // Ver2.0
                case 2:
                    dtItemData = dbAccess.ExecuteQuery(_selectForItemSqlZabbixVer2_0, sqlParams);
                    dtTriggerData = dbAccess.ExecuteQuery(_selectForTriggerSqlZabbixVer2_0, sqlParams);
                    //added by YAMA 2014/07/25  [条件式の置き換え]
                    setConditionalExpression(LoginSetting.JaZabbixVersion, dbAccess);
                    break;

                // Ver2.2
                case 3:
                    dtItemData = dbAccess.ExecuteQuery(_selectForItemSqlZabbixVer2_2, sqlParams);
                    dtTriggerData = dbAccess.ExecuteQuery(_selectForTriggerSqlZabbixVer2_2, sqlParams);
                    //added by YAMA 2014/07/25  [条件式の置き換え]
                    setConditionalExpression(LoginSetting.JaZabbixVersion, dbAccess);
                    break;
            }
        }


        //added by YAMA 2014/07/25
        private void setConditionalExpression(int zabbixVersion, DBConnect dbAccess)
        {
            String strExpression = "";                              // 'expression'を格納
            String strWk_Expression = "";                           // 'expression'を格納(work)
            String strFunctionid = "";                              // 'functionid'を格納(work)
            String strWk_Functionid = "";                           // 'functionid'を格納
            int foundIndex = 0;                                     // 区切り記号"{"の位置
            String strHost = "";                                    // ホスト名
            String strKey = "";                                     // キー
            String strFunction = "";                                // function
            String strParameter = "";                               // parameter
            String strConditionalExpression = "";                   // 条件式


            dtTriggerExpression.Clear();

            for (int i = 0; i < dtTriggerData.Rows.Count; i++)
            {
                // 'expression'を取得
                strExpression = dtTriggerData.Rows[i]["expression"].ToString();
                strWk_Expression = strExpression;

                // 'expression'内の'functionid'を取得
                foundIndex = strWk_Expression.IndexOf("{");        // 最初の"{"の位置
                while (0 <= foundIndex)
                {
                    // '{ }'を含めた値を取得
                    strFunctionid = strWk_Expression.Substring(foundIndex, strWk_Expression.IndexOf("}") - foundIndex + 1);
                    strWk_Functionid = strFunctionid;
                    if (char.IsNumber(strFunctionid, 1))
                    {
                        // 取得した値が、'functionid'の場合(数字)、'functionid'を検索キーとして置き換える値(ホスト名、アイテムキー、パラメータの値)を取得する
                        strWk_Functionid = strWk_Functionid.Substring(1, strWk_Functionid.Length - 2);
                        List<ComSqlParam> sqlParams = new List<ComSqlParam>();
                        sqlParams.Add(new ComSqlParam(DbType.String, "@functions.functionid", strWk_Functionid));
//                        sqlParams.Add(new ComSqlParam(DbType.String, "@functionid", strFunctionid));
                        switch (zabbixVersion)
                        {
                            // Ver1.8
                            case 1:
                                dtTriggerDataExpression = dbAccess.ExecuteQuery(_selectForTriggerExpressionSqlZabbixVer1_8, sqlParams);
                                break;

                            // Ver2.0
                            case 2:
                                dtTriggerDataExpression = dbAccess.ExecuteQuery(_selectForTriggerExpressionSqlZabbixVer2_0, sqlParams);
                                break;

                            // Ver2.2
                            case 3:
                                dtTriggerDataExpression = dbAccess.ExecuteQuery(_selectForTriggerExpressionSqlZabbixVer2_2, sqlParams);
                                break;
                        }
                        strHost = dtTriggerDataExpression.Rows[0]["host"].ToString();
                        strKey = dtTriggerDataExpression.Rows[0]["key_"].ToString();
                        strFunction = dtTriggerDataExpression.Rows[0]["name"].ToString();
                        strParameter = dtTriggerDataExpression.Rows[0]["parameter"].ToString();

                        // 取得した値を組み立て、条件式を作成する
                        strConditionalExpression = "{" +strHost + ":" + strKey + "." + strFunction + "(" + strParameter + ")" + "}";

                        // 'expression'内の'functionid'を条件式に置き換える
                        strExpression = strExpression.Replace(strFunctionid, strConditionalExpression);
                    }
                    else
                    {
                        // 'functionid'でない場合(数字以外)、何もしない
                    }

                    // strExpression内の'{ }'を含めた値を任意の値に変更する
                    strWk_Expression = strWk_Expression.Replace(strFunctionid, "AA22");
                    foundIndex = strWk_Expression.IndexOf("{");    // 次の"{"の位置
                }

                // 条件式を格納
                //dtTriggerData.Rows[i]["expression"] = strExpression;
                DataRow newRow = dtTriggerExpression.NewRow();
                newRow["expression"] = strExpression;
                dtTriggerExpression.Rows.Add(newRow);
            }

        }


        private void setcomboItemAndTrigger2(string selectedHostId)
        {
            // 選択されたホストに属するアイテムをコンボボックスに設定
            DataTable CombItemTbl = new DataTable();

            //DataTableに列を追加
            CombItemTbl.Columns.Add("ID", typeof(string));
            CombItemTbl.Columns.Add("NAME", typeof(string));

            DataTable CombTriggerTbl = new DataTable();

            //DataTableに列を追加
            CombTriggerTbl.Columns.Add("ID", typeof(string));
            CombTriggerTbl.Columns.Add("NAME", typeof(string));

            getItemAndTriggerData2(selectedHostId);

            string wk_id = "";

            // アイテムデータを設定
            for (int i = 0; i < dtItemData.Rows.Count; i++)
            {
                if (wk_id != dtItemData.Rows[i]["itemid"].ToString())
                {
                    DataRow CombItemRow = CombItemTbl.NewRow();
                    CombItemRow["ID"] = dtItemData.Rows[i]["itemid"].ToString();

                    //added by YAMA 2014/07/25    (アイテム名の表示)
//                    CombItemRow["NAME"] = dtItemData.Rows[i]["item_name"].ToString() +
//                                            "(" + dtItemData.Rows[i]["item_key"].ToString() + ")";
                    String strName = dtItemData.Rows[i]["item_name"].ToString();
                    if (0 <= strName.IndexOf("$")){
                        // 名前の編集を行う
                        /// キーの[]内から置き換える文字列を配列に変換
                        String str = dtItemData.Rows[i]["item_key"].ToString();
                        String str2 = str.Substring(str.IndexOf("[") + 1, str.IndexOf("]") - str.IndexOf("[") - 1);
                        String[] stArrayKey = str2.Split(',');

                        /// 名前内の"$数字"を置き換える
                        int foundIndex = strName.IndexOf("$");                  // 最初の"$"の位置
                        int num = 0;
                        int numOfChar = 0;
                        int numOfNumber = 0;
                        while (0 <= foundIndex)
                        {
                            str = strName.Substring(foundIndex);                // "$"以降の最後までを取得
                            int j = 0;
                            numOfNumber = 0;
                            for (j = 1; j < str.Length; j++)
                            {
                                if (char.IsNumber(str, j))
                                {
                                    numOfNumber++;
                                }else{
                                    break;
                                }
                            }
                            numOfChar = j;

                            num = int.Parse(str.Substring(1, numOfNumber));     // "$"の次の文字（配列のindexを取得）
                            String str3 = stArrayKey[num - 1];                  // 置き換える文字列を抽出

                            str2 = str.Substring(0, numOfChar);                 // 置き換えられる文字列($数字)を取得

                            strName = strName.Replace(str2, str3);              // 文字の置き換え

                            foundIndex = strName.IndexOf("$");                  // 次の"$"の位置
                        }
                        CombItemRow["NAME"] = strName;
                    }else{
                        CombItemRow["NAME"] = dtItemData.Rows[i]["item_name"].ToString();
                    }


                    wk_id = dtItemData.Rows[i]["itemid"].ToString();
                    CombItemTbl.Rows.Add(CombItemRow);
                }
            }

            // トリガーデータを設定
            for (int i = 0; i < dtTriggerData.Rows.Count; i++)
            {
                DataRow CombTriggerRow = CombTriggerTbl.NewRow();
                CombTriggerRow["ID"] = dtTriggerData.Rows[i]["triggerid"].ToString();

                //added by YAMA 2014/07/25
                //CombTriggerRow["NAME"] = dtTriggerData.Rows[i]["description"].ToString() +
                //                           "(" + dtTriggerData.Rows[i]["expression"].ToString() + ")";
                CombTriggerRow["NAME"] = dtTriggerData.Rows[i]["description"].ToString() +
                                           "(" + dtTriggerExpression.Rows[i]["expression"].ToString() + ")";

                CombTriggerTbl.Rows.Add(CombTriggerRow);
            }

            CombItemTbl.AcceptChanges();

            combItemName.ItemsSource = CombItemTbl.DefaultView;
            combItemName.DisplayMemberPath = "NAME";
            combItemName.SelectedValuePath = "ID";

            CombTriggerTbl.AcceptChanges();

            combTriggerName.ItemsSource = CombTriggerTbl.DefaultView;
            combTriggerName.DisplayMemberPath = "NAME";
            combTriggerName.SelectedValuePath = "ID";
        }

        private void CtlHostGrpPermission(String SelectedId)
        {
            // 選択したホストグループのアクセス権限に応じて連係動作を制御
            for (int i = 0; i < dtHostGroupData.Rows.Count; i++)
            {
                if (SelectedId == "ALL")
                {
                    rbEnabled.IsEnabled = true;
                    rbDisabled.IsEnabled = true;
                    rbGetStat.IsEnabled = false;
                    rbGetData.IsEnabled = false;
                    break;
                }

                if (SelectedId == dtHostGroupData.Rows[i]["groupid"].ToString())
                {
                    switch (dtHostGroupData.Rows[i]["permission"].ToString())
                    {
                        case "2":
                            if (rbHostGroup.IsChecked == true)
                            {
                                rbEnabled.IsEnabled = false;
                                rbDisabled.IsEnabled = false;
                                rbGetStat.IsEnabled = false;
                                rbGetData.IsEnabled = false;
                            }
                            else if (rbHostName.IsChecked == true)
                            {
                                rbEnabled.IsEnabled = false;
                                rbDisabled.IsEnabled = false;
                                rbGetStat.IsEnabled = true;
                                rbGetData.IsEnabled = false;
                            }
                            else if (rbItemName.IsChecked == true)
                            {
                                rbEnabled.IsEnabled = false;
                                rbDisabled.IsEnabled = false;
                                rbGetStat.IsEnabled = true;
                                rbGetData.IsEnabled = true;
                            }
                            else
                            {
                                rbEnabled.IsEnabled = false;
                                rbDisabled.IsEnabled = false;
                                rbGetStat.IsEnabled = true;
                                rbGetData.IsEnabled = false;
                            }

                            break;
                        case "3":
                            if (rbHostGroup.IsChecked == true)
                            {
                                rbEnabled.IsEnabled = true;
                                rbDisabled.IsEnabled = true;
                                rbGetStat.IsEnabled = false;
                                rbGetData.IsEnabled = false;
                            }
                            else if (rbHostName.IsChecked == true)
                            {
                                rbEnabled.IsEnabled = true;
                                rbDisabled.IsEnabled = true;
                                rbGetStat.IsEnabled = true;
                                rbGetData.IsEnabled = false;
                            }
                            else if (rbItemName.IsChecked == true)
                            {
                                rbEnabled.IsEnabled = true;
                                rbDisabled.IsEnabled = true;
                                rbGetStat.IsEnabled = true;
                                rbGetData.IsEnabled = true;
                            }
                            else
                            {
                                rbEnabled.IsEnabled = true;
                                rbDisabled.IsEnabled = true;
                                rbGetStat.IsEnabled = true;
                                rbGetData.IsEnabled = false;
                            }

                            break;
                    }
                    break;
                }
            }
        }


        private void CtlHostPermission(String SelectedId)
        {
            // 選択したホストのアクセス権限に応じて連係動作を制御

            switch (hostPermissionInfo[SelectedId])
            {
                case "2":
                        if (rbHostGroup.IsChecked == true)
                        {
                            rbEnabled.IsEnabled = false;
                            rbDisabled.IsEnabled = false;
                            rbGetStat.IsEnabled = false;
                            rbGetData.IsEnabled = false;
                        }
                        else if (rbHostName.IsChecked == true)
                        {
                            rbEnabled.IsEnabled = false;
                            rbDisabled.IsEnabled = false;
                            rbGetStat.IsEnabled = true;
                            rbGetData.IsEnabled = false;
                        }
                        else if (rbItemName.IsChecked == true)
                        {
                            rbEnabled.IsEnabled = false;
                            rbDisabled.IsEnabled = false;
                            rbGetStat.IsEnabled = true;
                            rbGetData.IsEnabled = true;
                        }
                        else
                        {
                            rbEnabled.IsEnabled = false;
                            rbDisabled.IsEnabled = false;
                            rbGetStat.IsEnabled = true;
                            rbGetData.IsEnabled = false;
                        }
                        break;

                case "3":
                        if (rbHostGroup.IsChecked == true)
                        {
                            rbEnabled.IsEnabled = true;
                            rbDisabled.IsEnabled = true;
                            rbGetStat.IsEnabled = false;
                            rbGetData.IsEnabled = false;
                        }
                        else if (rbHostName.IsChecked == true)
                        {
                            rbEnabled.IsEnabled = true;
                            rbDisabled.IsEnabled = true;
                            rbGetStat.IsEnabled = true;
                            rbGetData.IsEnabled = false;
                        }
                        else if (rbItemName.IsChecked == true)
                        {
                            rbEnabled.IsEnabled = true;
                            rbDisabled.IsEnabled = true;
                            rbGetStat.IsEnabled = true;
                            rbGetData.IsEnabled = true;
                        }
                        else
                        {
                            rbEnabled.IsEnabled = true;
                            rbDisabled.IsEnabled = true;
                            rbGetStat.IsEnabled = true;
                            rbGetData.IsEnabled = false;
                        }

                        break;
                }
        }

        private void chkHostGroupPermission()
        {
            string stValue;
            string stHostGroupID, stHostGroupPermission;

            // 全てのホストグループ
            for (int i = 0; i < dtHostGroupData.Rows.Count; i++)
            {
                stHostGroupID = dtHostGroupData.Rows[i]["groupid"].ToString();
                stHostGroupPermission = dtHostGroupData.Rows[i]["permission"].ToString();

                if (hostGroupPermissionInfo.TryGetValue(stHostGroupID, out stValue) == true)
                {
                    // ホストグループが存在する場合、権限を更新
                    switch (stHostGroupPermission)
                    {
                        case "0":
                            hostGroupPermissionInfo[stHostGroupID] = stHostGroupPermission;
                            break;

                        case "3":
                            if (stValue != "0")
                            {
                                hostGroupPermissionInfo[stHostGroupID] = stHostGroupPermission;
                            }
                            break;

                        case "2":
                            break;
                    }
                }
                else
                {
                    // ホストグループが無ければ追加する
                    hostGroupPermissionInfo.Add(stHostGroupID, stHostGroupPermission);
                }
            }
        }

        private void chkHostPermission()
        {
            string stValue;
            string stHostID, stHostPermission;

            // 全てのホストグループに属するホスト
            for (int i = 0; i < dtHostGroupData.Rows.Count; i++)
            {
                getHostData(dtHostGroupData.Rows[i]["groupid"].ToString());

                for (int j = 0; j < dtHostData.Rows.Count; j++)
                {

                    stHostID = dtHostData.Rows[j]["hostid"].ToString();
                    stHostPermission = dtHostData.Rows[j]["permission"].ToString();

                    if (hostPermissionInfo.TryGetValue(stHostID, out stValue) == true)
                    {
                        // ホストが存在する場合、権限を更新
                        switch (stHostPermission)
                        {
                            case "0":
                                    hostPermissionInfo[stHostID] = stHostPermission;
                                break;

                            case "3":
                                if (stValue != "0")
                                {
                                    hostPermissionInfo[stHostID] = stHostPermission;
                                }
                                break;

                            case "2":
                                break;
                        }
                    }
                    else
                    {
                        // ホストが無ければ追加する
                        hostPermissionInfo.Add(stHostID, stHostPermission);
                    }
                }
            }

        }


    }
}
