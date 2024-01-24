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

namespace jp.co.ftf.jobcontroller.Common
{
    /// <summary>
    /// 共通変換ユーティリティクラス
    /// </summary>
    public class ConvertUtil
    {
        public static decimal ConverDate2IntYYYYMMDDHHMISS(DateTime date)
        {

            String str = date.ToString("yyyyMMddHHmmss");

            return Convert.ToDecimal(str);
        }

        public static DateTime ConverIntYYYYMMDDHHMISS2Date(decimal iDate)
        {
            String str = iDate.ToString();
            if (str.Length == 17)
            {
                return DateTime.ParseExact(str, "yyyyMMddHHmmssfff", null);
            }
            return DateTime.ParseExact(str, "yyyyMMddHHmmss", null);
        }

        public static decimal ConverDate2IntYYYYMMDD(DateTime date)
        {
            String str = date.ToString("yyyyMMdd");
            return Convert.ToDecimal(str);
        }

        public static DateTime ConverIntYYYYMMDD2Date(decimal iDate)
        {
            String str = iDate.ToString();

            return DateTime.ParseExact(str, "yyyyMMdd", null);
        }
        public static decimal ConverDate2IntYYYYMMDDHHMI(DateTime date)
        {
            String str = date.ToString("yyyyMMddHHmm");
            return Convert.ToDecimal(str);
        }
        public static DateTime ConverIntYYYYMMDDHHMI2Date(decimal iDate)
        {
            String str = iDate.ToString();

            return DateTime.ParseExact(str, "yyyyMMddHHmm", null);
        }
        public static decimal getCalendarFromDate(int year, int up)
        {
            return Convert.ToDecimal((year + up).ToString() + "0101");
        }

        public static decimal getCalendarToDate(int year, int up)
        {
            return Convert.ToDecimal((year + up).ToString() + "1231");
        }
        public static String convertDisplayFlag(int flag)
        {
            String displayFlag = "";
            if (flag == 1)
            {
                displayFlag = "○";
            }
            return displayFlag;
        }

        //Park.iggy Add
        public static String getPasswordFromString(String str)
        {
            string key = "199907";
            string enc = "1";
            string toX16 = "";

            int j;
            int b;
            j = 0;

            for (int i = 0; i < str.Length; i++)
            {
                b = (str[i] ^ key[j]);
                toX16 = "";
                if (b < 16)
                {
                    toX16 = "0" + Convert.ToString(b, 16);
                }
                else
                {
                    toX16 = Convert.ToString(b, 16);
                }

                //Console.WriteLine(i + "=encode=>" + b + "==" + toX16);
                enc = enc + toX16;

                j++;
                if (j == key.Length) j = 0;
            }

            return enc;
        }

        public static String getStringFromPassword(String password)
        {
            string key = "199907";
            string dec = "";
            int j;

            j = 0;
            for (int i = 0; i < password.Length; i++)
            {
                dec = dec + (char)(password[i] ^ key[j]);
                j++;
                if (j == key.Length) j = 0;
            }
            return dec;
        }

        //added by Park.iggy 2014/08/15
        /// <summary>パスワードの復号化</summary>
        ///
        public static String getStringFromX16Password(String password)
        {
            string enc_code = "";
            string de_code = "";
            string x16 = "";

            for (int kk = 0; kk < password.Length; kk++)
            {
                if (kk == 0)
                {
                    continue;
                }
                else if ((kk % 2) == 1)
                {
                    x16 = password[kk].ToString();
                }
                else
                {
                    x16 = x16 + password[kk].ToString();
                    enc_code = enc_code + (char)Convert.ToInt32(x16, 16);
                    x16 = "";
                }

            }
            de_code = getStringFromPassword(enc_code);

            return de_code;
        }

        //Park.iggy END
    }
}
