<?php
/*
** Job Arranger Manager
** Copyright (C) 2023 Daiwa Institute of Research Ltd. All Rights Reserved.
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

namespace App\Controllers;

use App\Utils\Controller;
use App\Services\RelatedObjectService;

/**
 * This controller is used to manage the pages.
 *
 * @version    6.1.0
 * @since      Class available since version 6.1.0
 */
class Pages extends Controller
{
    public function __construct()
    {
        $this->relatedObjectService = new RelatedObjectService;
    }

    /**
     * It redirects to index screen.
     *
     * @since   Method available since version 6.1.0
     */
    public function index(): void
    {
        $this->view('login');
    }

    /**
     * It redirects to error_page screen.
     *
     * @since   Method available since version 6.1.0
     */
    public function error(): void
    {
        $this->view('screens/error_page');
    }

    /**
     * It redirects to error_not_found screen.
     *
     * @since   Method available since version 6.1.0
     */
    public function notFound()
    {
        $this->view('screens/error_not_found');
    }

    /**
     * It gets language file.
     *
     * @since   Method available since version 6.1.0
     */
    public function getLang(): void
    {
        $lang = $_SESSION['userInfo']['language'] ?? null;
        if ($lang == "JP") {
            echo json_encode(file_get_contents('../app/views/language/lang_jp.json'), true);
        } else {
            echo json_encode(file_get_contents('../app/views/language/lang_en.json'), true);
        }
    }
}
