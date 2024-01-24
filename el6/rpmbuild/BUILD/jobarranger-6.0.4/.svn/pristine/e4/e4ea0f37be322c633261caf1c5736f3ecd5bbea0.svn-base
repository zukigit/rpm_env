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
using System.Windows.Media;
using System.Windows.Data;
using jp.co.ftf.jobcontroller.Common;
//*******************************************************************
//                                                                  *
//                                                                  *
//  Copyright (C) 2013 FitechForce, Inc. All Rights Reserved.       *
//                                                                  *
//  * @author KIM  2013/09/16 新規作成<BR>                          *
//                                                                  *
//                                                                  *
//*******************************************************************

namespace jp.co.ftf.jobcontroller.JobController
{
    /// <summary>
    /// Attached property provider which adds the read-only attached property
    /// <c>TextBlockService.IsTextTrimmed</c> to the framework's <see cref="TextBlock"/> control.
    /// </summary>
    public class TextBlockService
    {

        #region Attached Property [TextBlockService.IsTextTrimmed]

        /// <summary>
        /// Key returned upon registering the read-only attached property <c>IsTextTrimmed</c>.
        /// </summary>
        public static readonly DependencyPropertyKey IsTextTrimmedKey = DependencyProperty.RegisterAttachedReadOnly(
            "IsTextTrimmed",
            typeof( bool ),
            typeof( TextBlockService ),
            new PropertyMetadata( false ) );    // defaults to false

        /// <summary>
        /// Identifier associated with the read-only attached property <c>IsTextTrimmed</c>.
        /// </summary>
        public static readonly DependencyProperty IsTextTrimmedProperty = IsTextTrimmedKey.DependencyProperty;

        /// <summary>
        /// Returns the current effective value of the IsTextTrimmed attached property.
        /// </summary>
        /// <remarks>Invoked automatically by the framework when databound.</remarks>
        /// <param name="target"><see cref="TextBlock"/> to evaluate</param>
        /// <returns>Effective value of the IsTextTrimmed attached property</returns>
        [AttachedPropertyBrowsableForType( typeof( TextBlock ) )]
        public static Boolean GetIsTextTrimmed( TextBlock target )
        {
            return (Boolean) target.GetValue( IsTextTrimmedProperty );
        }

        #endregion (Attached Property [TextBlockService.IsTextTrimmed])


        /// <summary>
        /// Sets the instance value of read-only dependency property <see cref="IsTextTrimmed"/>.
        /// </summary>
        /// <param name="target">Associated <see cref="TextBlock"/> instance</param>
        /// <param name="value">New value for IsTextTrimmed</param>
        public static void SetIsTextTrimmed( TextBlock target, Boolean value )
        {
            target.SetValue( IsTextTrimmedKey, value );
        }
    }
}
