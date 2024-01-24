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
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2012 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author DHC 劉 旭 2012/10/15 新規作成<BR>                      *
//                                                                  *
//                                                                  *
//*******************************************************************
namespace jp.co.ftf.jobcontroller.Common
{
    /// <summary>
    /// 共通チェックユーティリティクラス.
    /// </summary>
    public class CheckUtil
    {
        #region フィールド

        /// <summary>半角英数字とハイフン</summary>
        private const string MATCH_HANKAKU_HYPHEN = "^[0-9a-zA-Z\\-]*$";

        /// <summary>半角数字</summary>
        private const string MATCH_HANKAKU = "^[0-9]*$";

        /// <summary>ASCII</summary>
        private const string MATCH_ASCII = "^[\\x00-\\x7F]*$";

        /// <summary>全半角英数字</summary>
        private const string MATCH_ZENKAKU_HANKAKU = "^[0-9a-zA-Z０-９ａ-ｚＡ-Ｚ]*$";

        /// <summary>半角数字、カンマ、ハイフン</summary>
        private const string MATCH_HANKAKU_COMMA_HYPHEN = "^[0-9\\,\\-]*$";

        /// <summary>半角英数字とハイフン、スラッシュ</summary>
        private const string MATCH_HANKAKU_HYPHEN_SLASH = "^[0-9a-zA-Z\\-\\/]*$";

        /// <summary>半角英数字とハイフン、アンダーバー、スラッシュ</summary>
        private const string MATCH_HANKAKU_HYPHEN_UNDERBAR_SLASH = "^[0-9a-zA-Z-_/]*$";

        /// <summary>半角英数字とハイフン、アンダーバー</summary>
        private const string MATCH_HANKAKU_HYPHEN_UNDERBAR = "^[0-9a-zA-Z_-]*$";

        /// <summary>半角英数字とアンダーバー</summary>
        private const string MATCH_HANKAKU_UNDERBAR = "^[0-9a-zA-Z_]*$";

        /// <summary>予約語（START）</summary>
        private const string HOLD_START = "START";

        public static Encoding EncSJis = Encoding.GetEncoding("shift_jis");

        //added by YAMA 2014/04/10
        /// <summary>半角数字とコロン</summary>
        private const string MATCH_HANKAKU_COLON = "^[0-9\\:]*$";

        //added by YAMA 2014/08/15
        /// <summary>実行ユーザー禁止文字 「" / \ [ ] : ; | = , + * ?< >」</summary>
        private const string PROHIBITED_CHARACTER_USER_NAME = "[\"/\\\\[\\];:|<>+=,?*]";

        //added by YAMA 2014/08/15
        /// <summary>実行ユーザーのパスワード禁止文字 「" / \ [ ] : ; | = , + * ?< >」</summary>
        //changed by Thiha 2022/03/24
        /// <summary>実行ユーザーパスワードの禁止文字 「" < >」チェック処理</summary>
        private const string PROHIBITED_CHARACTER_USER_PW = ".*[\"<>]+.*";

        //added by YAMA 2014/08/15
        static Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");

        //added by YAMA 2014/08/18
        /// <summary>半角英数字と半角空白、アンダーバー、ハイフン、ピリオド</summary>
        private const string MATCH_HANKAKU_SPACE_UNDERBAR_HYPHEN_PERIOD = "^[0-9a-zA-Z _\\-\\.]*$";

        //added by YAMA 2014/08/18
        /// <summary>半角英数字とアンダーバー、ハイフン、ピリオド</summary>
        private const string MATCH_HANKAKU_UNDERBAR_HYPHEN_PERIOD = "^[0-9a-zA-Z_\\-\\.]*$";

        //added by YAMA 2014/09/30
        /// <summary>半角英数字とドル記号、アンダーバー</summary>
        private const string MATCH_HANKAKU_DOLLAR_UNDERBAR = "^[0-9a-zA-Z$_]*$";

        //added by YAMA 2014/09/30
        /// <summary>半角英字</summary>
        private const string MATCH_HANKAKU_LETTER = "^[a-zA-Z]*$";

        //added by YAMA 2014/09/30
        /// <summary>半角英字とアンダーバー</summary>
        private const string MATCH_HANKAKU_LETTER_UNDERBAR = "^[a-zA-Z_]*$";

        #endregion

        #region publicメソッド

        /// <summary>必須チェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：null または空の文字列の場合」「False：それ以外」</return>
        public static bool IsNullOrEmpty(string str)
        {
            if (str == null || str.Length == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>ASCII文字チェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：ASCII文字」「False：それ以外」</return>
        public static bool IsASCIIStr(string str)
        {
            if (str == null || str.Length == 0
                || Regex.IsMatch(str, MATCH_ASCII))
            {
                return true;
            }
            return false;
        }

        /// <summary>文字列桁数チェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <param name="intLen">指定された長さ</param>
        /// <return>「True：指定された長さ以上の文字列」「False：指定された長さ以内の文字列」</return>
        public static bool IsGetaLenOver(string str, int intLen)
        {
            if (str == null || str.Length == 0)
            {
                return false;
            }
            return str.Length > intLen;
        }

        /// <summary>文字列バイト数チェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <param name="intLen">指定されたバイト長</param>
        /// <return>「True：指定された長さ以上の文字列」「False：指定された長さ以内の文字列」</return>
        public static bool IsLenOver(string str, int intLen)
        {
            if (str == null || str.Equals(""))
            {
                return false;
            }
            return (EncSJis.GetByteCount(str)) > intLen;
        }

        /// <summary>文字列バイト数チェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <param name="intLen">指定されたバイト長</param>
        /// <return>「True：指定された長さ以下の文字列」「False：指定された長さ以上の文字列」</return>
        public static bool IsLenUnder(string str, int intLen)
        {
            if (str == null || str.Equals(""))
            {
                return false;
            }
            return (EncSJis.GetByteCount(str)) < intLen;
        }

        /// <summary>文字列桁数チェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <param name="intLen">指定された長さ</param>
        /// <return>「True：指定された長さ」「False：指定された長さではない」</return>
        public static bool IsLen(string str, int intLen)
        {
            if (str == null || str.Length == 0)
            {
                return false;
            }
            return str.Length == intLen;
        }

        /// <summary>半角英数字とハイフンチェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：半角英数字とハイフン」「False：それ以外」</return>
        public static bool IsHankakuStrAndHyphen(string str)
        {
            if (str == null || str.Length == 0
                || Regex.IsMatch(str, MATCH_HANKAKU_HYPHEN))
            {
                return true;
            }
            return false;
        }

        /// <summary>全半角英数字チェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：全半角英数字」「False：それ以外」</return>
        public static bool IsZenkakuStrAndHankakuStr(string str)
        {
            if (str == null || str.Length == 0
                || Regex.IsMatch(str, MATCH_ZENKAKU_HANKAKU))
            {
                return true;
            }
            return false;
        }

        /// <summary>半角数字チェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：半角数字」「False：それ以外」</return>
        public static bool IsHankakuNum(string str)
        {
            if (str == null || str.Length == 0
                || Regex.IsMatch(str, MATCH_HANKAKU))
            {
                return true;
            }
            return false;
        }

        /// <summary>半角数字、カンマ、ハイフンチェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：半角数字、カンマ、ハイフン」「False：それ以外」</return>
        public static bool IsHankakuNumAndCommaAndHyphen(string str)
        {
            if (str == null || str.Length == 0
                || Regex.IsMatch(str, MATCH_HANKAKU_COMMA_HYPHEN))
            {
                return true;
            }
            return false;
        }

        /// <summary>カンマ、およびハイフンの前後が数字チェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：半角数字カンマ、およびハイフンの前後が数字」「False：それ以外」</return>
        public static bool IsHankakuNumBeforeOrAfterCommaAndHyphen(string str)
        {
            if (str == null || str.Length == 0)
            {
                return true;
            }
            String[] array = str.Split(new char[2] { ',', '-' });
            foreach (String i in array)
            {
                if (i.Length == 0 || !IsHankakuNum(i))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>半角英数字とハイフン、アンダーバー、スラッシュチェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：半角英数字、ハイフン、アンダーバー、スラッシュ」「False：それ以外」</return>
        public static bool IsHankakuStrAndHyphenUnderbarAndSlash(string str)
        {
            if (str == null || str.Length == 0
                || Regex.IsMatch(str, MATCH_HANKAKU_HYPHEN_UNDERBAR_SLASH))
            {
                return true;
            }
            return false;
        }

        /// <summary>半角英数字とハイフン、スラッシュチェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：半角英数字、ハイフン、スラッシュ」「False：それ以外」</return>
        public static bool IsHankakuStrAndHyphenAndSlash(string str)
        {
            if (str == null || str.Length == 0
                || Regex.IsMatch(str, MATCH_HANKAKU_HYPHEN_SLASH))
            {
                return true;
            }
            return false;
        }

        /// <summary>半角英数字とアンダーバー、最初文字数値以外チェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：半角英数字、アンダーバー、最初文字数値以外」「False：それ以外」</return>
        public static bool IsHankakuStrAndUnderbarAndFirstNotNum(string str)
        {
            if (str == null || str.Length == 0
                || (Regex.IsMatch(str, MATCH_HANKAKU_UNDERBAR) && !IsHankakuNum(str.Substring(0, 1))))
            {
                return true;
            }
            return false;
        }

        /// <summary>半角英数字とハイフン、アンダーバーチェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：半角英数字、ハイフン、アンダーバー」「False：それ以外」</return>
        public static bool IsHankakuStrAndHyphenAndUnderbar(string str)
        {
            if (str == null || str.Length == 0
                || Regex.IsMatch(str, MATCH_HANKAKU_HYPHEN_UNDERBAR))
            {
                return true;
            }
            return false;
        }

        /// <summary>半角英数字とハイフン、アンダーバー、スラッシュチェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：半角英数字、ハイフン、アンダーバー、スラッシュ」「False：それ以外」</return>
        public static bool IsHankakuStrAndHyphenAndUnderbarAndSlash(string str)
        {
            if (str == null || str.Length == 0
                || Regex.IsMatch(str, MATCH_HANKAKU_HYPHEN_UNDERBAR_SLASH))
            {
                return true;
            }
            return false;
        }

        /// <summary>予約語（START）チェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：予約語（START）」「False：それ以外」</return>
        public static bool IsHoldStrSTART(string str)
        {
            if (str == null || str.Length == 0)
            {
                return false;
            }
            if (HOLD_START.Equals(str))
            {
                return true;
            }
            return false;
        }

        /// <summary>日付の月であるかチェック</summary>
        /// <param name="month">チェックする数値</param>
        /// <return>「True：1-12の場合」「False：それ以外」</return>
        public static bool IsMonth(int month)
        {
            if (month >= 1 && month<=12)
            {
                return true;
            }
            return false;
        }

        /// <summary>時間の時が正しいかチェック</summary>
        /// <param name="hour">チェックする数値</param>
        /// <return>「True：0-23の場合」「False：それ以外」</return>
        public static bool IsHour(int hour)
        {
            if (hour >= 0 && hour <= 23)
            {
                return true;
            }
            return false;
        }

        /// <summary>時間の分が正しいかチェック</summary>
        /// <param name="min">チェックする数値</param>
        /// <return>「True：0-59の場合」「False：それ以外」</return>
        public static bool IsMin(int min)
        {
            if (min >= 0 && min <= 59)
            {
                return true;
            }
            return false;
        }

        /// <summary>日付の日であるかチェック</summary>
        /// <param name="day">チェックする数値</param>
        /// <return>「True：1-31の場合」「False：それ以外」</return>
        public static bool IsDay(int day)
        {
            if (day >= 1 && day <= 31)
            {
                return true;
            }
            return false;
        }

        /// <summary>対象のバイト長度を取得</summary>
        /// <param name="strValue">チェックする文字列</param>
        /// <returns>対象のバイト長度</returns>
        public static int Get_ByteLength(string strValue)
        {
            Encoding unEncoding = Encoding.Default;
            int intLength = 0;
            intLength = unEncoding.GetByteCount(strValue);
            return intLength;
        }

        /// <summary>ユーザーグループIDがログインユーザーグループリストに存在するかチェック</summary>
        /// <param name="loginUserGroupList">ログインユーザーグループリスト</param>
        /// <param name="objectUserGroupList">ユーザーグループID</param>
        /// <returns>「True：存在」「False：存在しない」</returns>
        public static bool isExistGroupId(List<Decimal> loginUserGroupList, List<Decimal> objectUserGroupList)
        {
            foreach (Decimal userGroupId in loginUserGroupList)
            {
                if (objectUserGroupList.Contains(userGroupId)) return true;
            }
            return false;
        }

        /// <summary>オブジェクト名、ジョブ名の入力不可文字存在チェック</summary>
        /// <param name="strValue">チェックする文字列</param>
        /// <returns>「True："'\,が存在する」「False："'\,が存在しない」</returns>
        public static bool IsImpossibleStr(string strValue)
        {

            if(strValue.IndexOf("\"") >= 0) {

                return true;
            }
            if (strValue.IndexOf("'") >= 0)
            {
                return true;
            }
            if (strValue.IndexOf(",") >= 0)
            {
                return true;
            }
            if (strValue.IndexOf("\\") >= 0)
            {
                return true;
            }
            return false;
        }

        //added by YAMA 2014/04/10
        /// <summary>半角数字とコロンチェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：半角数字とコロン」「False：それ以外」</return>
        public static bool IsHankakuStrAndColon(string str)
        {
            if (str == null || str.Length == 0
                || Regex.IsMatch(str, MATCH_HANKAKU_COLON))
            {
                return true;
            }
            return false;
        }

        //added by YAMA 2014/08/15
        /// <summary>実行ユーザーの禁止文字 「" / \ [ ] : ; | = , + * ?< >」チェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：禁止文字」「False：それ以外」</return>
        public static bool IsProhibitedCharacterUserName(string str)
        {
            if (str == null || str.Length == 0
                || Regex.IsMatch(str, PROHIBITED_CHARACTER_USER_NAME) == false)
            {
                return true;
            }
            return false;
        }

        //added by YAMA 2014/08/15
        /// <summary>実行ユーザーパスワードの禁止文字 チェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：禁止文字」「False：それ以外」</return>
        public static bool IsProhibitedCharacterUserPW(string str)
        {
            if (str == null || str.Length == 0
                || Regex.IsMatch(str, PROHIBITED_CHARACTER_USER_PW) == false)
            {
                return true;
            }
            return false;
        }

        //added by YAMA 2014/08/15
        /// <summary> 半角文字のみかを判定 </summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：半角文字のみ」「False：それ以外」</return>
        public static bool isHankaku(string str)
        {
            int num = sjisEnc.GetByteCount(str);
            return num == str.Length;
        }

        //added by YAMA 2014/08/18
        /// <summary>半角英数字と半角空白、アンダーバー、ピリオドチェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：半角英数字と半角空白、アンダーバー、ピリオド、ハイフン」「False：それ以外」</return>
        public static bool IsHankakuStrAndSpaceAndUnderbarAndHyphenAndPeriod(string str)
        {
            if (str == null || str.Length == 0
                || Regex.IsMatch(str, MATCH_HANKAKU_SPACE_UNDERBAR_HYPHEN_PERIOD))
            {
                return true;
            }
            return false;
        }


        //added by YAMA 2014/08/18
        /// <summary>半角英数字とアンダーバー、ハイフン、ピリオドチェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：半角英数字とアンダーバー、ハイフン、ピリオド」「False：それ以外」</return>
        public static bool IsHankakuStrAndUnderbarAndHyphenAndPeriod(string str)
        {
            if (str == null || str.Length == 0
                || Regex.IsMatch(str, MATCH_HANKAKU_UNDERBAR_HYPHEN_PERIOD))
            {
                return true;
            }
            return false;
        }


        //added by YAMA 2014/09/30
        /// <summary>半角英数字とドル記号、アンダーバーチェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：半角英数字、ドル記号、アンダーバー」「False：それ以外」</return>
        public static bool IsHankakuStrAndDollarAndUnderbar(string str) {
            if (str == null || str.Length == 0
                || Regex.IsMatch(str, MATCH_HANKAKU_DOLLAR_UNDERBAR))
            {
                return true;
            }
            return false;
        }

        //added by YAMA 2014/09/30
        /// <summary>半角英数字とアンダーバーチェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：半角英数字、アンダーバー」「False：それ以外」</return>
        public static bool IsHankakuStrAndUnderbar(string str) {
            if (str == null || str.Length == 0
                || Regex.IsMatch(str, MATCH_HANKAKU_UNDERBAR))
            {
                return true;
            }
            return false;
        }

        //added by YAMA 2014/09/30
        /// <summary>半角英字処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：半角英字」「False：それ以外」</return>
        public static bool IsHankakuLerrer(string str) {
            if (str == null || str.Length == 0
                || Regex.IsMatch(str, MATCH_HANKAKU_LETTER))
            {
                return true;
            }
            return false;
        }

        //added by YAMA 2014/09/30
        /// <summary>半角英字とアンダーバーチェック処理</summary>
        /// <param name="str">チェックする文字列</param>
        /// <return>「True：半角英字、アンダーバー」「False：それ以外」</return>
        public static bool IsHankakuLerrerAndUnderbar(string str) {
            if (str == null || str.Length == 0
                || Regex.IsMatch(str, MATCH_HANKAKU_LETTER_UNDERBAR))
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}
