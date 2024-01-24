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
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2013 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author DHC �� �� 2013/09/27 �V�K�쐬<BR>                      *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.JobController.Form.JobEdit
{
    public class IconViewData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        /// <summary>�W���uID</summary>
        private string _jobId="";
        public string JobId
        {
            get { return this._jobId; }
            set
            {
                if (value != this._jobId)
                {
                    this._jobId = value;
                    NotifyPropertyChanged("JobId");
                }
            }
        }

        /// <summary>�W���u��</summary>
        private string _jobName;
        public string JobName
        {
            get { return this._jobName; }
            set
            {
                if (value != this._jobName)
                {
                    this._jobName = value;
                    NotifyPropertyChanged("JobName");
                }
            }
        }
    }
}
