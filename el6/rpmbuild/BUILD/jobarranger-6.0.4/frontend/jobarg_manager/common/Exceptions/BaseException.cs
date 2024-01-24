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

//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author DHC 郭 暁宇 2012/10/15 新規作成<BR>                    *
//                                                                  *
//                                                                  *
//*******************************************************************

namespace jp.co.ftf.jobcontroller.Common
{
    /// <summary>
    /// 例外の処理.
    /// </summary>
    public class BaseException:Exception
    {
        #region フィールド


        /// <summary>エラーメッセージ</summary>
        private string _strMessage = string.Empty;

        /// <summary>発生元プログラムID</summary>
        private string _strSource = string.Empty;

        /// <summary>メッセージID</summary>
        private string _strMessageID = string.Empty;

        #endregion

        #region プロパティ
        /// <summary>エラーメッセージ</summary>
        public override string Message
        {
            get
            {
                return _strMessage;
            }
        }

        /// <summary>発生元プログラムID</summary>
        public override string Source
        {
            get
            {
                return _strSource;
            }
            set
            {
                _strSource = value;
            }
        }

        /// <summary>メッセージID</summary>
        public string MessageID
        {
            get
            {
                return _strMessageID;
            }
            set
            {
                _strMessageID = value;
            }
        }

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        /// <param name="messageID">例外メッセージＩＤ</param>
        public BaseException()
            : this(string.Empty, string.Empty, string.Empty, null) { }

        /// <summary>コンストラクタ</summary>
        /// <param name="messageID">例外メッセージＩＤ</param>
        public BaseException(string messageID)
            : this(string.Empty, messageID, string.Empty, null) { }

        /// <summary>コンストラクタ</summary>
        /// <param name="messageID">例外メッセージＩＤ</param>
        /// <param name="ex">現在の例外の原因である例外。</param>
        public BaseException(string messageID, Exception ex)
            : this(string.Empty, messageID, string.Empty, ex) { }

        /// <summary>コンストラクタ</summary>
        /// <param name="source">発生元プログラムID</param>
        /// <param name="messageID">例外メッセージID</param>
        /// <param name="ex">現在の例外の原因である例外。</param>
        public BaseException(string source, string messageID, Exception ex)
            : this(source, messageID, string.Empty, ex) { }

        /// <summary>コンストラクタ</summary>
        /// <param name="source">発生元プログラムID</param>
        /// <param name="messageID">例外メッセージID</param>
        /// <param name="message">例外メッセージ</param>
        /// <param name="ex">現在の例外の原因である例外。</param>
        public BaseException(string source, string messageID, string message, Exception ex)
            : base(message, ex)
        {
            this._strSource = source;
            this._strMessageID = messageID;
            this._strMessage = message;
        }
		#endregion

    }
}
