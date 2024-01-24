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

//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author DHC 郭 暁宇 2012/10/15 新規作成<BR>                    *
//                                                                  *
//                                                                  *
//*******************************************************************

namespace jp.co.ftf.jobcontroller.JobController
{
    /// <summary>
    /// メッセージの処理クラス
    /// </summary>
    public class MessageUtil
    {
        #region publicメソッド

        //************************************************************************
        /// <summary>
        /// メッセージの置換処理

        /// </summary>
        /// <param name="BaseMessage">変換対象を含む文字列</param>
        /// <param name="ChangeMessage">置換文字列</param>
        /// <returns>変換後の文字列</returns>
        //************************************************************************
        public static string GetReplaceMessage(string BaseMessage, string[] ChangeMessage)
        {
            return string.Format(BaseMessage, ChangeMessage);
        }

        //************************************************************************
        /// <summary>
        /// メッセージの取得

        /// </summary>
        /// <param name="msgId">情報ＩＤ</param>
        /// <return>メッセージの内容</return>
        //************************************************************************
        public static string GetMsgById(string msgId)
        {
            return Properties.Resources.ResourceManager.GetString(msgId, Properties.Resources.Culture);
        }

        #endregion
    }
}
