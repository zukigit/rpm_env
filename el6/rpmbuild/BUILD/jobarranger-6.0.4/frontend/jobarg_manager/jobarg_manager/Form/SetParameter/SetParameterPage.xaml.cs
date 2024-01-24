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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using jp.co.ftf.jobcontroller.DAO;
using jp.co.ftf.jobcontroller.Common;
//using jp.co.ftf.jobcontroller.JobController.Form.JobEdit;
using System.Data;


namespace jp.co.ftf.jobcontroller.JobController.Form.SetParameter
{
    /// <summary>
    /// SetParameterPage.xaml �̑��ݍ��p���W�b�N
    /// </summary>
    public partial class SetParameterPage : BaseUserControl
    {
        #region �t�B�[���h

        /// <summary>DB�A�N�Z�X�C���X�^���X</summary>
        //private DBConnect dbAccess = new DBConnect(LoginSetting.ConnectStr);
        private DBConnect dbAccess;


        /// <summary> �p�����[�^�ݒ��e�[�u�� </summary>
        private ParameterTableDAO _parameterTableDAO;

        private String jobnetViewSpan = "";
        private String jobnetLoadSpan = "";
        private String jobnetKeepSpan = "";
        private String joblogKeepSpan = "";

        private String JobArrangerStandardTime = "";

        private String zabbixNotice = "";
        private String zabbixServerIPaddress = "";
        private String zabbixServerPortNumber = "";
        private String zabbixSenderCommand = "";
        private String messageDestinationServer = "";
        private String messageDestinationItemKey = "";
        private String retry = "";
        private String retryCount = "";
        private String retryInterval = "";

        private String sv_jobnetViewSpan = "";
        private String sv_jobnetLoadSpan = "";
        private String sv_jobnetKeepSpan = "";
        private String sv_joblogKeepSpan = "";

        private String sv_JobArrangerStandardTime = "";

        private String sv_zabbixNotice = "";
        private String sv_zabbixServerIPaddress = "";
        private String sv_zabbixServerPortNumber = "";
        private String sv_zabbixSenderCommand = "";
        private String sv_messageDestinationServer = "";
        private String sv_messageDestinationItemKey = "";
        private String sv_retry = "";
        private String sv_retryCount = "";
        private String sv_retryInterval = "";


        #endregion


        #region �R���X�g���N�^
        public SetParameterPage(JobArrangerWindow parent)
        {

            bool viewer = false;
            #if VIEWER
                viewer = true;
            #endif


            //Parent = parent;

            InitializeComponent();

            // �ݒ��l�擾�E�\��
            SetValues();


            // Zabbix���[�U�[�i���ʃ��[�U�[�j�A�Q�ƃ��[�h�̏ꍇ�͎Q�Ƃ̂�
            if (LoginSetting.Authority == Consts.AuthorityEnum.GENERAL || viewer)
            {
                // ���ʃR���g���[�����񊈐���
                SetItemIsEnabled(false, true);
            }
            else
            {
                SetItemIsEnabled(true, false);
            }

            _parent = parent;
            DataContext = this;
        }
        #endregion

        #region �v���p�e�B
        /// <summary>�W���u�l�b�g�ꗗ</summary>
        public ObservableCollection<JobnetExecInfo> JobnetExecList { get; set; }

        private JobArrangerWindow _parent;
        public JobArrangerWindow Parent
        {
            get
            {
                return _parent;
            }
        }

        /// <summary>�N���X��</summary>
        public override string ClassName
        {
            get
            {
                return "SetParameterPage";
            }
        }

        /// <summary>����ID</summary>
        public override string GamenId
        {
            get
            {
                return Consts.WINDOW_600;
            }
        }



        #endregion

        #region �C�x���g

        #endregion

        #region private���\�b�h

        /// <summary> �l�̃Z�b�g�ƕ\������</summary>
        /// <param name="sender">��</param>
        private void SetValues()
        {
            // ������R���{�{�b�N�X�쐬
            DataTable DtCombStdTime = new DataTable();

            DtCombStdTime.Columns.Add("ID", typeof(string));
            DtCombStdTime.Columns.Add("NAME", typeof(string));

            DataRow RowCombStdTime = DtCombStdTime.NewRow();
            RowCombStdTime["ID"] = "0";
            RowCombStdTime["NAME"] = jp.co.ftf.jobcontroller.JobController.Properties.Resources.settings_Job_Arranger_std_time0;
            DtCombStdTime.Rows.Add(RowCombStdTime);
            RowCombStdTime = DtCombStdTime.NewRow();
            RowCombStdTime["ID"] = "1";
            RowCombStdTime["NAME"] = jp.co.ftf.jobcontroller.JobController.Properties.Resources.settings_Job_Arranger_std_time1;
            DtCombStdTime.Rows.Add(RowCombStdTime);

            combStandardTime.Items.Clear();
            combStandardTime.ItemsSource = DtCombStdTime.DefaultView;
            combStandardTime.DisplayMemberPath = "NAME";
            combStandardTime.SelectedValuePath = "ID";


            // Zabbix�ʒm�R���{�{�b�N�X�쐬
            DataTable DtCombNotice = new DataTable();

            DtCombNotice.Columns.Add("ID", typeof(string));
            DtCombNotice.Columns.Add("NAME", typeof(string));

            DataRow RowCombNotice = DtCombNotice.NewRow();
            RowCombNotice["ID"] = "0";
            RowCombNotice["NAME"] = jp.co.ftf.jobcontroller.JobController.Properties.Resources.settings_zbxsnd_notice0;
            DtCombNotice.Rows.Add(RowCombNotice);
            RowCombNotice = DtCombNotice.NewRow();
            RowCombNotice["ID"] = "1";
            RowCombNotice["NAME"] = jp.co.ftf.jobcontroller.JobController.Properties.Resources.settings_zbxsnd_notice1;
            DtCombNotice.Rows.Add(RowCombNotice);

            combNotice.Items.Clear();
            combNotice.ItemsSource = DtCombNotice.DefaultView;
            combNotice.DisplayMemberPath = "NAME";
            combNotice.SelectedValuePath = "ID";


            // Zabbix�ʒm�đ��R���{�{�b�N�X�쐬
            DataTable DtCombRetry = new DataTable();

            DtCombRetry.Columns.Add("ID", typeof(string));
            DtCombRetry.Columns.Add("NAME", typeof(string));

            DataRow RowCombRetry = DtCombRetry.NewRow();
            RowCombRetry["ID"] = "0";
            RowCombRetry["NAME"] = jp.co.ftf.jobcontroller.JobController.Properties.Resources.settings_zbxsnd_retry0;
            DtCombRetry.Rows.Add(RowCombRetry);
            RowCombRetry = DtCombRetry.NewRow();
            RowCombRetry["ID"] = "1";
            RowCombRetry["NAME"] = jp.co.ftf.jobcontroller.JobController.Properties.Resources.settings_zbxsnd_retry1;
            DtCombRetry.Rows.Add(RowCombRetry);

            combRetry.Items.Clear();
            combRetry.ItemsSource = DtCombRetry.DefaultView;
            combRetry.DisplayMemberPath = "NAME";
            combRetry.SelectedValuePath = "ID";


            // DB�f�[�^�擾
            GetParamData();

            // ���ʍ��ڐݒ�
            SetParamData();

            // �C���O�f�[�^�ۑ�
            svParamData();


        }

        private void GetParamData()
        {
            jobnetViewSpan = DBUtil.GetParameterVelue("JOBNET_VIEW_SPAN");
            jobnetLoadSpan = DBUtil.GetParameterVelue("JOBNET_LOAD_SPAN");
            jobnetKeepSpan = DBUtil.GetParameterVelue("JOBNET_KEEP_SPAN");
            joblogKeepSpan = DBUtil.GetParameterVelue("JOBLOG_KEEP_SPAN");

            JobArrangerStandardTime = DBUtil.GetParameterVelue("MANAGER_TIME_SYNC");

            zabbixNotice = DBUtil.GetParameterVelueForStrData("ZBXSND_ON");
            zabbixServerIPaddress = DBUtil.GetParameterVelueForStrData("ZBXSND_ZABBIX_IP");
            zabbixServerPortNumber = DBUtil.GetParameterVelue("ZBXSND_ZABBIX_PORT");
            zabbixSenderCommand = DBUtil.GetParameterVelueForStrData("ZBXSND_SENDER");
            messageDestinationServer = DBUtil.GetParameterVelueForStrData("ZBXSND_ZABBIX_HOST");
            messageDestinationItemKey = DBUtil.GetParameterVelueForStrData("ZBXSND_ITEM_KEY");
            retry = DBUtil.GetParameterVelue("ZBXSND_RETRY");
            retryCount = DBUtil.GetParameterVelue("ZBXSND_RETRY_COUNT");
            retryInterval = DBUtil.GetParameterVelue("ZBXSND_RETRY_INTERVAL");

        }



        private void SetParamData()
        {

            tbxJobnetViewSpan.Text = jobnetViewSpan;
            tbxJobnetLoadSpan.Text = jobnetLoadSpan;
            tbxJobnetKeepSpan.Text = jobnetKeepSpan;
            tbxJoblogKeepSpan.Text = joblogKeepSpan;

            tbxZabbixServerIPaddress.Text = zabbixServerIPaddress;
            tbxZabbixServerPortNumber.Text = zabbixServerPortNumber;
            tbxZabbixSenderCommand.Text = zabbixSenderCommand;
            tbxMessageDestinationServer.Text = messageDestinationServer;
            tbxMessageDestinationItemKey.Text = messageDestinationItemKey;

            tbxRetryCount.Text = retryCount;
            tbxRetryInterval.Text = retryInterval;


            // ������R���{�{�b�N�X�ɍ��ڂ��ݒ�
            combStandardTime.SelectedValue = JobArrangerStandardTime;

            // Zabbix�ʒm�R���{�{�b�N�X�ɍ��ڂ��ݒ�
            combNotice.SelectedValue = zabbixNotice;

            // Zabbix�ʒm�đ��R���{�{�b�N�X�ɍ��ڂ��ݒ�
            combRetry.SelectedValue = retry;

        }

        private void svParamData()
        {
            sv_jobnetViewSpan = tbxJobnetViewSpan.Text;
            sv_jobnetLoadSpan = tbxJobnetLoadSpan.Text;
            sv_jobnetKeepSpan = tbxJobnetKeepSpan.Text;
            sv_joblogKeepSpan = tbxJoblogKeepSpan.Text;
            sv_JobArrangerStandardTime = combStandardTime.SelectedValue.ToString();

            sv_zabbixNotice = combNotice.SelectedValue.ToString();
            sv_zabbixServerIPaddress = tbxZabbixServerIPaddress.Text;
            sv_zabbixServerPortNumber = tbxZabbixServerPortNumber.Text;
            sv_zabbixSenderCommand = tbxZabbixSenderCommand.Text;
            sv_messageDestinationServer = tbxMessageDestinationServer.Text;
            sv_messageDestinationItemKey = tbxMessageDestinationItemKey.Text;
            sv_retry = combRetry.SelectedValue.ToString();
            sv_retryCount = tbxRetryCount.Text;
            sv_retryInterval = tbxRetryInterval.Text;

        }

        private bool IsChengedParamData()
        {

            if (tbxJobnetViewSpan.Text != sv_jobnetViewSpan)
                return true;

            if (tbxJobnetLoadSpan.Text != sv_jobnetLoadSpan)
                return true;

            if (tbxJobnetKeepSpan.Text != sv_jobnetKeepSpan)
                return true;

            if (tbxJoblogKeepSpan.Text != sv_joblogKeepSpan)
                return true;

            if (tbxZabbixServerIPaddress.Text != sv_zabbixServerIPaddress)
                return true;

            if (tbxZabbixServerPortNumber.Text != sv_zabbixServerPortNumber)
                return true;

            if (tbxZabbixSenderCommand.Text != sv_zabbixSenderCommand)
                return true;

            if (tbxMessageDestinationServer.Text != sv_messageDestinationServer)
                return true;

            if (tbxMessageDestinationItemKey.Text != sv_messageDestinationItemKey)
                return true;

            if (tbxRetryCount.Text != sv_retryCount)
                return true;

            if (tbxRetryInterval.Text != sv_retryInterval)
                return true;

            if (combStandardTime.SelectedValue.ToString() != sv_JobArrangerStandardTime)
                return true;

            if (combNotice.SelectedValue.ToString() != sv_zabbixNotice)
                return true;

            if (combRetry.SelectedValue.ToString() != sv_retry)
                return true;

            return false;

        }

        /// <summary> �e���ڂ̃`�F�b�N����(�o�^)</summary>
        private bool InputCheck()
        {
            Int64 chkData = 0;

            // �V�X�e���ݒ荀��
            // �W���u�l�b�g�^�s�����\�����ԁi���j
            string jobnetViewSpanForChange = Properties.Resources.err_message_settings_jobnet_view_span;
            String jobnetViewSpan = tbxJobnetViewSpan.Text;
            // �����͂̏ꍇ
            if (CheckUtil.IsNullOrEmpty(jobnetViewSpan))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001, new string[] { jobnetViewSpanForChange });
                return false;
            }
            // ���p�����̂݉�
            if (!CheckUtil.IsHankakuNum(jobnetViewSpan))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_007, new string[] { jobnetViewSpanForChange });
                return false;
            }
            // 1�`1059127200�i�j
            chkData = Convert.ToInt64(jobnetViewSpan);
            if (chkData < 1 || chkData > 1059127200)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_008,
                    new string[] { jobnetViewSpanForChange, "�u1�`1059127200�v" });
                return false;
            }

            // �\���W���u�l�b�g���O�W�J�J�n���ԁi���j
            string jobnetLoadSpanForChange = Properties.Resources.err_message_settings_jobnet_load_span;
            String jobnetLoadSpan = tbxJobnetLoadSpan.Text;
            // �����͂̏ꍇ
            if (CheckUtil.IsNullOrEmpty(jobnetLoadSpan))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001, new string[] { jobnetLoadSpanForChange });
                return false;
            }
            // ���p�����̂݉�
            if (!CheckUtil.IsHankakuNum(jobnetLoadSpan))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_007, new string[] { jobnetLoadSpanForChange });
                return false;
            }
            // 1�`2147483647�i32�r�b�g�ő��l�j
            chkData = Convert.ToInt64(jobnetLoadSpan);
            if (chkData < 1 || chkData > 2147483647)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_008,
                    new string[] { jobnetLoadSpanForChange, "�u1�`2147483647�v" });
                return false;
            }

            // �I���ς݃W���u�l�b�g�����ێ����ԁi���j
            string jobnetKeepSpanForChange = Properties.Resources.err_message_settings_jobnet_keep_span;
            String jobnetKeepSpan = tbxJobnetKeepSpan.Text;
            // �����͂̏ꍇ
            if (CheckUtil.IsNullOrEmpty(jobnetKeepSpan))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001, new string[] { jobnetKeepSpanForChange });
                return false;
            }
            // ���p�����̂݉�
            if (!CheckUtil.IsHankakuNum(jobnetKeepSpan))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_007, new string[] { jobnetKeepSpanForChange });
                return false;
            }
            // 1�`2147483647�i32�r�b�g�ő��l�j
            chkData = Convert.ToInt64(jobnetKeepSpan);
            if (chkData < 1 || chkData > 2147483647)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_008,
                    new string[] { jobnetKeepSpanForChange, "�u1�`2147483647�v" });
                return false;
            }

            // �W���u���s���ʃ��O�ێ����ԁi���j
            string joblogKeepSpanForChange = Properties.Resources.err_message_settings_joblog_keep_span;
            String joblogKeepSpan = tbxJoblogKeepSpan.Text;
            // �����͂̏ꍇ
            if (CheckUtil.IsNullOrEmpty(joblogKeepSpan))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001, new string[] { joblogKeepSpanForChange });
                return false;
            }
            // ���p�����̂݉�
            if (!CheckUtil.IsHankakuNum(joblogKeepSpan))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_007, new string[] { joblogKeepSpanForChange });
                return false;
            }
            // 1�`2147483647�i32�r�b�g�ő��l�j
            chkData = Convert.ToInt64(joblogKeepSpan);
            if (chkData < 1 || chkData > 2147483647)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_008,
                    new string[] { joblogKeepSpanForChange, "�u1�`2147483647�v" });
                return false;
            }

            // Zabbix�ʒm�ݒ荀��
            // Zabbix IP�A�h���X
            string zabbixServerIPaddressForChange = Properties.Resources.err_message_settings_zbxsnd_zabbix_ip;
            String zabbixServerIPaddress = tbxZabbixServerIPaddress.Text;
            // �����͂̏ꍇ
            if (CheckUtil.IsNullOrEmpty(zabbixServerIPaddress))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001, new string[] { zabbixServerIPaddressForChange });
                return false;
            }
            // �ő�2048�o�C�g
            if (CheckUtil.IsLenOver(zabbixServerIPaddress, 2048))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003, new string[] { zabbixServerIPaddressForChange, "2048" });
                return false;
            }
            //  ���p�p�����ƃA���_�[�o�[�A�n�C�t���A�s���I�h�̂݉�
            if (!CheckUtil.IsHankakuStrAndSpaceAndUnderbarAndHyphenAndPeriod(zabbixServerIPaddress))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_SETTING_002, new string[] { zabbixServerIPaddressForChange });
                return false;
            }
            // �n�C�t���A�s���I�h�́A�擪�A�Ō��̕����Ƃ��Ă͎g�p�s��
            String firstChr = zabbixServerIPaddress.Substring(0, 1);
            String lastChr = zabbixServerIPaddress.Substring(zabbixServerIPaddress.Length - 1, 1);
            if ((firstChr == "-" || lastChr == "-") || (firstChr == "." || lastChr == "."))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_SETTING_002, new string[] { zabbixServerIPaddressForChange });
                return false;
            }

            // �����݂̂̓��͕͂s��
            if (CheckUtil.IsHankakuNum(zabbixServerIPaddress))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_SETTING_002, new string[] { zabbixServerIPaddressForChange });
                return false;
            }



            // Zabbix �|�[�g�ԍ�
            string zabbixServerPortNumberForChange = Properties.Resources.err_message_settings_zbxsnd_zabbix_port;
            String zabbixServerPortNumber = tbxZabbixServerPortNumber.Text;
            // �����͂̏ꍇ
            if (CheckUtil.IsNullOrEmpty(zabbixServerPortNumber))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001, new string[] { zabbixServerPortNumberForChange });
                return false;
            }
            // ���p�����̂݉�
            if (!CheckUtil.IsHankakuNum(zabbixServerPortNumber))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_007, new string[] { zabbixServerPortNumberForChange });
                return false;
            }
            // 0�`65535
            Int32 PortNumber = Convert.ToInt32(zabbixServerPortNumber);
            if (PortNumber < 0 || PortNumber > 65535)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_017, new string[] { zabbixServerPortNumberForChange, "0", "65535" });
                return false;
            }

            // Zabbix Sender �R�}���h
            string zabbixSenderCommandForChange = Properties.Resources.err_message_settings_zbxsnd_sender;
            String zabbixSenderCommand = tbxZabbixSenderCommand.Text;
            // �����͂̏ꍇ
            if (CheckUtil.IsNullOrEmpty(zabbixSenderCommand))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001, new string[] { zabbixSenderCommandForChange });
                return false;
            }
            //  ASCII�����̂݉�
            if (!CheckUtil.IsASCIIStr(zabbixSenderCommand))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_007, new string[] { zabbixSenderCommandForChange });
                return false;
            }
            // �ő�2048�o�C�g
            if (CheckUtil.IsLenOver(zabbixSenderCommand, 2048))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003, new string[] { zabbixSenderCommandForChange, "2048" });
                return false;
            }

            // ���b�Z�[�W�ʒm��Zabbix�z�X�g
            string messageDestinationServerForChange = Properties.Resources.err_message_settings_zbxsnd_zabbix_host;
            String messageDestinationServer = tbxMessageDestinationServer.Text;
            // �����͂̏ꍇ
            if (CheckUtil.IsNullOrEmpty(messageDestinationServer))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001, new string[] { messageDestinationServerForChange });
                return false;
            }
            //  ���p�p�����Ɣ��p�󔒁A�A���_�[�o�[�A�n�C�t���A�s���I�h�̂݉�
            if (!CheckUtil.IsHankakuStrAndSpaceAndUnderbarAndHyphenAndPeriod(messageDestinationServer))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_028, new string[] { messageDestinationServerForChange });
                return false;
            }
            // �ő�64�o�C�g
            if (CheckUtil.IsLenOver(messageDestinationServer, 64))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003, new string[] { messageDestinationServerForChange, "64" });
                return false;
            }

            // �A�C�e���L�[
            string messageDestinationItemKeyForChange = Properties.Resources.err_message_settings_zbxsnd_zabbix_item;
            String messageDestinationItemKey = tbxMessageDestinationItemKey.Text;
            // �����͂̏ꍇ
            if (CheckUtil.IsNullOrEmpty(messageDestinationItemKey))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001, new string[] { messageDestinationItemKeyForChange });
                return false;
            }
            //  ���p�p�����A�A���_�[�o�[�A�n�C�t���̂݉�
            if (!CheckUtil.IsHankakuStrAndHyphenAndUnderbar(messageDestinationItemKey))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_013, new string[] { messageDestinationItemKeyForChange });
                return false;
            }
            // �ő�255�o�C�g
            if (CheckUtil.IsLenOver(messageDestinationItemKey, 255))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_003, new string[] { messageDestinationItemKeyForChange, "255" });
                return false;
            }

            // �đ�����
            string retryCountForChange = Properties.Resources.err_message_settings_zbxsnd_retry_count;
            String retryCount = tbxRetryCount.Text;
            // �����͂̏ꍇ
            if (CheckUtil.IsNullOrEmpty(retryCount))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001, new string[] { retryCountForChange });
                return false;
            }
            // ���p�����̂݉�
            if (!CheckUtil.IsHankakuNum(retryCount))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_007, new string[] { retryCountForChange });
                return false;
            }
            // 0�`2147483647
            chkData = Convert.ToInt64(retryCount);
            if (chkData < 0 || chkData > 2147483647)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_008,
                    new string[] { retryCountForChange, "�u0�`2147483647�v" });
                return false;
            }

            // �đ��C���^�[�o���i�b�j
            string retryIntervalForChange = Properties.Resources.err_message_settings_zbxsnd_retry_interval;
            String retryInterval = tbxRetryInterval.Text;
            // �����͂̏ꍇ
            if (CheckUtil.IsNullOrEmpty(retryInterval))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_001, new string[] { retryIntervalForChange });
                return false;
            }
            // ���p�����̂݉�
            if (!CheckUtil.IsHankakuNum(retryInterval))
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_COMMON_007, new string[] { retryIntervalForChange });
                return false;
            }
            // 1�`2147483647
            chkData = Convert.ToInt64(retryInterval);
            if (chkData < 1 || chkData > 2147483647)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_JOBEDIT_008,
                    new string[] { retryIntervalForChange, "�u1�`2147483647�v" });
                return false;
            }

            return true;

        }


        /// <summary>�ύX����</summary>
        /// <param name="sender">��</param>
        /// <param name="e">�C�x���g</param>
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            // ���̓`�F�b�N
            if (!InputCheck())
            {
                return;
            }

            // �ҏW�o�^�m�F�_�C�A���O�̕\��
            if (MessageBoxResult.Yes == CommonDialog.ShowEditRegistDialog())
            {
                dbAccess = new DBConnect(LoginSetting.ConnectStr);
                _parameterTableDAO = new ParameterTableDAO(dbAccess);

                // �p�����[�^�e�[�u�������b�N
                bool getLockFlag = GetLockForUpd(Consts.WINDOW_600);

                //200�b���s�����~���܂��B
                //System.Threading.Thread.Sleep(200000);

                //bool getLockFlag = true;

                 if (getLockFlag == true)
                {
                     RegistProcess();

                     // �C���O�f�[�^�ۑ�
                     svParamData();

                }
                dbAccess.CloseSqlConnect();
            }
        }

        //*******************************************************************
        /// <summary>�c�a�f�[�^�o�^</summary>
        //*******************************************************************
        private void RegistProcess()
        {
            //dbAccess.CreateSqlConnect();
            dbAccess.BeginTransaction();

            // �o�^
            RegistDataTable();

            dbAccess.TransactionCommit();
            //dbAccess.CloseSqlConnect();

        }


        private void RegistDataTable()
        {

            _parameterTableDAO.SetJaParameterTable("JOBNET_VIEW_SPAN", tbxJobnetViewSpan.Text);
            _parameterTableDAO.SetJaParameterTable("JOBNET_LOAD_SPAN", tbxJobnetLoadSpan.Text);
            _parameterTableDAO.SetJaParameterTable("JOBNET_KEEP_SPAN", tbxJobnetKeepSpan.Text);
            _parameterTableDAO.SetJaParameterTable("JOBLOG_KEEP_SPAN", tbxJoblogKeepSpan.Text);

            _parameterTableDAO.SetJaParameterTable("MANAGER_TIME_SYNC", combStandardTime.SelectedValue.ToString());

            _parameterTableDAO.SetJaParameterTable("ZBXSND_ON", combNotice.SelectedValue.ToString());
            _parameterTableDAO.SetJaParameterTable("ZBXSND_ZABBIX_IP", tbxZabbixServerIPaddress.Text);
            _parameterTableDAO.SetJaParameterTable("ZBXSND_ZABBIX_PORT", tbxZabbixServerPortNumber.Text);
            _parameterTableDAO.SetJaParameterTable("ZBXSND_SENDER", tbxZabbixSenderCommand.Text);
            _parameterTableDAO.SetJaParameterTable("ZBXSND_ZABBIX_HOST", tbxMessageDestinationServer.Text);
            _parameterTableDAO.SetJaParameterTable("ZBXSND_ITEM_KEY", tbxMessageDestinationItemKey.Text);
            _parameterTableDAO.SetJaParameterTable("ZBXSND_RETRY", combRetry.SelectedValue.ToString());

            _parameterTableDAO.SetJaParameterTable("ZBXSND_RETRY_COUNT", tbxRetryCount.Text);
            _parameterTableDAO.SetJaParameterTable("ZBXSND_RETRY_INTERVAL", tbxRetryInterval.Text);


        }



        /// <summary>�ēǍ�����</summary>
        /// <param name="sender">��</param>
        /// <param name="e">�C�x���g</param>
        private void btnReread_Click(object sender, RoutedEventArgs e)
        {
            // DB�f�[�^�擾
            GetParamData();

            // ���ڕҏW��
            if (IsChengedParamData() == true)
            {
                if (MessageBoxResult.Yes == CommonDialog.ShowCancelDialog())
                {
                    SetParamData();
                    // �C���O�f�[�^�ۑ�
                    svParamData();
                }
            }
            else
            {
                SetParamData();
                // �C���O�f�[�^�ۑ�
                svParamData();
            }
        }

        //*******************************************************************
        /// <summary> DB�̃��b�N�擾�A���݃`�F�b�N</summary>
        //*******************************************************************
        private bool GetLockForUpd(string windowId)
        {
            dbAccess.CreateSqlConnect();
            dbAccess.BeginTransaction();
            try
            {
                GetLock(windowId);
            }
            catch (DBException)
            {
                CommonDialog.ShowErrorDialog(Consts.ERROR_SETTING_001);
                return false;
            }
            return true;

        }


        //*******************************************************************
        /// <summary> �p�����[�^�e�[�u�����b�N</summary>
        //*******************************************************************
        private void GetLock(string windowId)
        {
            _parameterTableDAO.GetLock(windowId, LoginSetting.DBType);

        }

        //*******************************************************************
        /// <summary> ���ʍ��ڊ�������</summary>
        //*******************************************************************
        private void SetItemIsEnabled(bool setStat, bool generalMode)
        {
            tbxJobnetViewSpan.IsEnabled = setStat;
            tbxJobnetLoadSpan.IsEnabled = setStat;
            tbxJobnetKeepSpan.IsEnabled = setStat;
            tbxJoblogKeepSpan.IsEnabled = setStat;

            combStandardTime.IsEnabled = setStat;

            combNotice.IsEnabled = setStat;
            tbxZabbixServerIPaddress.IsEnabled = setStat;
            tbxZabbixServerPortNumber.IsEnabled = setStat;
            tbxZabbixSenderCommand.IsEnabled = setStat;
            tbxMessageDestinationServer.IsEnabled = setStat;
            tbxMessageDestinationItemKey.IsEnabled = setStat;
            combRetry.IsEnabled = setStat;
            tbxRetryCount.IsEnabled = setStat;
            tbxRetryInterval.IsEnabled = setStat;

            btnReread.IsEnabled = setStat;
            btnUpdate.IsEnabled = setStat;

            if (generalMode)
            {
                btnReread.IsEnabled = true;
            }

        }


        #endregion
    }
}
