import { useRoutes } from "react-router-dom";
import React from "react";
import { OBJECT_CATEGORY, FORM_TYPE } from "../constants";
import Login from "../views/login/Login";
import Setup from "../views/setup/Setup";
import Home from "../views/home/Home";
import PrivateRoute from "../components/layout/private/PrivateRoute";
import MainLayout from "../components/layout/main/MainLayout";
import CalendarForm from "../views/calendarForm/CalendarForm";
import ObjectList from "../views/objectList/ObjectList";
import ScheduleForm from "../views/scheduleForm/ScheduleForm";
import FilterForm from "../views/filterForm/FilterForm";
import JobnetForm from "../views/jobnetForm/JobnetForm";
import ObjectVersion from "../views/objectVersion/ObjectVersion";
import GeneralSetting from "../views/generalSetting/GeneralSetting";
import LockManagement from "../views/lockManagement/LockManagement";
import JobExecutionResult from "../views/jobExecutionResult/JobExecutionResult";
import JobExecutionManagement from "../views/jobExecutionManagement/JobExecutionManagement";
import { Err404, Err500, RandomError, NetworkError } from "../views/error/Error";
import { t } from "i18next";

export function MainRoutes() {
  const _Login = (
    <PrivateRoute
      element={Login}
      meta={{
        requiresAuth: false,
        title:t('title-login'),
      }}
    />
  );

  const elements = useRoutes([
    {
      path: "/",
      element: _Login,
    },
    {
      path: "/404",
      element: (
        <PrivateRoute
          element={Err404}
          meta={{
            requiresAuth: false,
            title: t('title-err'),
          }}
        />
      ),
    },
    {
      path: "/error",
      element: (
        <PrivateRoute
          element={RandomError}
          meta={{
            requiresAuth: false,
            title: t('title-err'),
          }}
        />
      ),
    },
    {
      path: "/500",
      element: (
        <PrivateRoute
          element={Err500}
          meta={{
            requiresAuth: false,
            title:t('title-err'),
          }}
        />
      ),
    },
    {
      path: "/networkErr",
      element: (
        <PrivateRoute
          element={NetworkError}
          meta={{
            requiresAuth: false,
            title:t("title-err"),
          }}
        />
      ),
    },
    {
      path: "/login",
      element: _Login,
    },
    {
      path: "/setup",
      element: (
        <PrivateRoute
          element={Setup}
          meta={{
            requiresAuth: false,
            title: t('title-setup'),
          }}
        />
      ),
    },
    {
      path: "/",
      element: <MainLayout />,
      children: [
        {
          path: "home",
          element: (
            <PrivateRoute
              element={Home}
              meta={{
                title:t('title-home'),
                requiresAuth: true,
              }}
            />
          ),
        },
        {
          path: "object-list/calendar/public",
          element: (
            <PrivateRoute
              element={ObjectList}
              meta={{
                title:t('title-pub-cal-lists'),
                requiresAuth: true,
              }}
              category={OBJECT_CATEGORY.CALENDAR}
              publicType={true}
            />
          ),
        },
        {
          path: "object-list/calendar/private",
          element: (
            <PrivateRoute
              element={ObjectList}
              meta={{
                title:t('title-pri-cal-lists'),
                requiresAuth: true,
              }}
              category={OBJECT_CATEGORY.CALENDAR}
              publicType={false}
            />
          ),
        },
        {
          path: "object-list/filter/public",
          element: (
            <PrivateRoute
              element={ObjectList}
              meta={{
                title:t('title-pub-fil-lists'),
                requiresAuth: true,
              }}
              category={OBJECT_CATEGORY.FILTER}
              publicType={true}
            />
          ),
        },
        {
          path: "object-list/filter/private",
          element: (
            <PrivateRoute
              element={ObjectList}
              meta={{
                title:t('title-pri-fil-lists'),
                requiresAuth: true,
              }}
              category={OBJECT_CATEGORY.FILTER}
              publicType={false}
            />
          ),
        },
        {
          path: "object-list/schedule/public",
          element: (
            <PrivateRoute
              element={ObjectList}
              meta={{
                title:t('title-pub-sch-lists'),
                requiresAuth: true,
              }}
              category={OBJECT_CATEGORY.SCHEDULE}
              publicType={true}
            />
          ),
        },
        {
          path: "object-list/schedule/private",
          element: (
            <PrivateRoute
              element={ObjectList}
              meta={{
                title:t('title-pri-sch-lists'),
                requiresAuth: true,
              }}
              category={OBJECT_CATEGORY.SCHEDULE}
              publicType={false}
            />
          ),
        },
        {
          path: "schedule/edit/public/:objectId/:date",
          element: (
            <PrivateRoute
              element={ScheduleForm}
              meta={{
                title:t('title-sch-edi'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.EDIT}
              publicType={true}
            />
          ),
        },
        {
          path: "schedule/edit/private/:objectId/:date",
          element: (
            <PrivateRoute
              element={ScheduleForm}
              meta={{
                title:t('title-sch-edi'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.EDIT}
              publicType={false}
            />
          ),
        },
        {
          path: "object-list/jobnet/public",
          element: (
            <PrivateRoute
              element={ObjectList}
              meta={{
                title:t('title-pub-job-lists'),
                requiresAuth: true,
              }}
              category={OBJECT_CATEGORY.JOBNET}
              publicType={true}
            />
          ),
        },
        {
          path: "object-list/jobnet/private",
          element: (
            <PrivateRoute
              element={ObjectList}
              meta={{
                title:t('title-pri-job-lists'),
                requiresAuth: true,
              }}
              category={OBJECT_CATEGORY.JOBNET}
              publicType={false}
            />
          ),
        },
        {
          path: "calendar/create/public/:id",
          element: (
            <PrivateRoute
              element={CalendarForm}
              meta={{
                
                requiresAuth: true,
              }}
              formType={FORM_TYPE.CREATE}
              publicType={true}
            />
          ),
        },
        {
          path: "calendar/create/public/",
          element: (
            <PrivateRoute
              element={CalendarForm}
              meta={{
                title:t('title-cal-create'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.CREATE}
              publicType={true}
            />
          ),
        },
        {
          path: "calendar/create/private",
          element: (
            <PrivateRoute
              element={CalendarForm}
              meta={{
                title:t('title-cal-create'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.CREATE}
              publicType={false}
            />
          ),
        },
        {
          path: "calendar/create/private/:id",
          element: (
            <PrivateRoute
              element={CalendarForm}
              meta={{
                
                requiresAuth: true,
              }}
              formType={FORM_TYPE.CREATE}
              publicType={false}
            />
          ),
        },
        {
          path: "calendar/edit/private/:objectId/:date",
          element: (
            <PrivateRoute
              element={CalendarForm}
              meta={{
                title:t('title-cal-edi'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.EDIT}
              publicType={false}
            />
          ),
        },
        {
          path: "calendar/new-object/private/:objectId/:date",
          element: (
            <PrivateRoute
              element={CalendarForm}
              meta={{
                title:t('title-cal-new-obj'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.NEW_OBJECT}
              publicType={true}
            />
          ),
        },
        {
          path: "calendar/new-version/private/:objectId/:date",
          element: (
            <PrivateRoute
              element={CalendarForm}
              meta={{
                title:t('title-cal-new-ver'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.NEW_VERSION}
              publicType={true}
            />
          ),
        },
        {
          path: "calendar/edit/public/:objectId/:date",
          element: (
            <PrivateRoute
              element={CalendarForm}
              meta={{
                title:t('title-cal-edi'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.EDIT}
              publicType={true}
            />
          ),
        },
        {
          path: "calendar/new-object/public/:objectId/:date",
          element: (
            <PrivateRoute
              element={CalendarForm}
              meta={{
                title:t('title-cal-new-obj'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.NEW_OBJECT}
              publicType={true}
            />
          ),
        },
        {
          path: "calendar/new-version/public/:objectId/:date",
          element: (
            <PrivateRoute
              element={CalendarForm}
              meta={{
                title:t('title-cal-new-ver'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.NEW_VERSION}
              publicType={true}
            />
          ),
        },
        {
          path: "filter/create/public",
          element: (
            <PrivateRoute
              element={FilterForm}
              meta={{
                title:t('title-fil-create'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.CREATE}
              publicType={true}
            />
          ),
        },
        {
          path: "filter/edit/private/:objectId/:date",
          element: (
            <PrivateRoute
              element={FilterForm}
              meta={{
                title:t('title-fil-edi'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.EDIT}
              publicType={false}
            />
          ),
        },
        {
          path: "filter/new-object/private/:objectId/:date",
          element: (
            <PrivateRoute
              element={FilterForm}
              meta={{
                title:t('title-fil-new-obj'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.NEW_OBJECT}
              publicType={false}
            />
          ),
        },
        {
          path: "filter/new-object/public/:objectId/:date",
          element: (
            <PrivateRoute
              element={FilterForm}
              meta={{
                title:t('title-fil-new-obj'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.NEW_OBJECT}
              publicType={true}
            />
          ),
        },
        {
          path: "filter/edit/public/:objectId/:date",
          element: (
            <PrivateRoute
              element={FilterForm}
              meta={{
                title:t('title-fil-edi'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.EDIT}
              publicType={true}
            />
          ),
        },
        {
          path: "filter/create/private",
          element: (
            <PrivateRoute
              element={FilterForm}
              meta={{
                title:t('title-fil-create'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.CREATE}
              publicType={false}
            />
          ),
        },
        {
          path: "schedule/create/public",
          element: (
            <PrivateRoute
              element={ScheduleForm}
              meta={{
                title:t('title-sch-create'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.CREATE}
              publicType={true}
            />
          ),
        },
        {
          path: "schedule/new-object/public/:objectId/:date",
          element: (
            <PrivateRoute
              element={ScheduleForm}
              meta={{
                requiresAuth: true,
                title:t('title-sch-new-obj'),
              }}
              formType={FORM_TYPE.NEW_OBJECT}
              publicType={true}
            />
          ),
        },
        {
          path: "schedule/new-version/public/:objectId/:date",
          element: (
            <PrivateRoute
              element={ScheduleForm}
              meta={{
                title:t('title-sch-new-ver'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.NEW_VERSION}
              publicType={true}
            />
          ),
        },
        {
          path: "schedule/create/private",
          element: (
            <PrivateRoute
              element={ScheduleForm}
              meta={{
                title:t('title-sch-create'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.CREATE}
              publicType={false}
            />
          ),
        },
        {
          path: "schedule/new-object/private/:objectId/:date",
          element: (
            <PrivateRoute
              element={ScheduleForm}
              meta={{
                title:t('title-sch-new-obj'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.NEW_OBJECT}
              publicType={false}
            />
          ),
        },
        {
          path: "schedule/new-version/private/:objectId/:date",
          element: (
            <PrivateRoute
              element={ScheduleForm}
              meta={{
                title:t('title-sch-new-ver'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.NEW_VERSION}
              publicType={false}
            />
          ),
        },
        {
          path: "jobnet/create/public",
          element: (
            <PrivateRoute
              element={JobnetForm}
              meta={{
                title:t('title-job-create'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.CREATE}
              publicType={true}
            />
          ),
        },
        {
          path: "jobnet/create/private",
          element: (
            <PrivateRoute
              element={JobnetForm}
              meta={{
                title:t('title-job-create'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.CREATE}
              publicType={false}
            />
          ),
        },
        {
          path: "jobnet/edit/public/:objectId/:date",
          element: (
            <PrivateRoute
              element={JobnetForm}
              meta={{
                title:t('title-job-edi'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.EDIT}
              publicType={true}
            />
          ),
        },
        {
          path: "jobnet/edit/private/:objectId/:date",
          element: (
            <PrivateRoute
              element={JobnetForm}
              meta={{
                title:t('title-job-edi'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.EDIT}
              publicType={false}
            />
          ),
        },
        {
          path: "jobnet/new-version/public/:objectId/:date",
          element: (
            <PrivateRoute
              element={JobnetForm}
              meta={{
                title:t('title-job-new-ver'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.NEW_VERSION}
              publicType={true}
            />
          ),
        },
        {
          path: "jobnet/new-version/private/:objectId/:date",
          element: (
            <PrivateRoute
              element={JobnetForm}
              meta={{
                title:t('title-job-new-ver'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.NEW_VERSION}
              publicType={false}
            />
          ),
        },
        {
          path: "jobnet/new-object/public/:objectId/:date",
          element: (
            <PrivateRoute
              element={JobnetForm}
              meta={{
                title:t('title-job-new-obj'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.NEW_OBJECT}
              publicType={true}
            />
          ),
        },
        {
          path: "jobnet/new-object/private/:objectId/:date",
          element: (
            <PrivateRoute
              element={JobnetForm}
              meta={{
                title:t('title-cal-new-obj'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.NEW_OBJECT}
              publicType={false}
            />
          ),
        },
        {
          path: "object-version/calendar/public/:objectId",
          element: (
            <PrivateRoute
              element={ObjectVersion}
              meta={{
                title:t('title-cal-ver'),
                requiresAuth: true,
              }}
              category={OBJECT_CATEGORY.CALENDAR}
              publicType={true}
            />
          ),
        },
        {
          path: "object-version/calendar/private/:objectId",
          element: (
            <PrivateRoute
              element={ObjectVersion}
              meta={{
                title:t('title-cal-ver'),
                requiresAuth: true,
              }}
              category={OBJECT_CATEGORY.CALENDAR}
              publicType={false}
            />
          ),
        },
        {
          path: "object-version/filter/public/:objectId",
          element: (
            <PrivateRoute
              element={ObjectVersion}
              meta={{
                title:t('title-fil-ver'),
                requiresAuth: true,
              }}
              category={OBJECT_CATEGORY.FILTER}
              publicType={true}
            />
          ),
        },
        {
          path: "object-version/filter/private/:objectId",
          element: (
            <PrivateRoute
              element={ObjectVersion}
              meta={{
                title:t('title-fil-ver'),
                requiresAuth: true,
              }}
              category={OBJECT_CATEGORY.FILTER}
              publicType={false}
            />
          ),
        },
        {
          path: "filter/new-version/public/:objectId/:date",
          element: (
            <PrivateRoute
              element={FilterForm}
              meta={{
                title:t('title-fil-new-ver'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.NEW_VERSION}
              publicType={true}
            />
          ),
        },
        {
          path: "filter/new-version/private/:objectId/:date",
          element: (
            <PrivateRoute
              element={FilterForm}
              meta={{
                title:t('title-fil-new-ver'),
                requiresAuth: true,
              }}
              formType={FORM_TYPE.NEW_VERSION}
              publicType={false}
            />
          ),
        },
        {
          path: "object-version/schedule/public/:objectId",
          element: (
            <PrivateRoute
              element={ObjectVersion}
              meta={{
                title:t('title-sch-ver'),
                requiresAuth: true,
              }}
              category={OBJECT_CATEGORY.SCHEDULE}
              publicType={true}
            />
          ),
        },
        {
          path: "object-version/schedule/private/:objectId",
          element: (
            <PrivateRoute
              element={ObjectVersion}
              meta={{
                title:t('title-sch-ver'),
                requiresAuth: true,
              }}
              category={OBJECT_CATEGORY.SCHEDULE}
              publicType={false}
            />
          ),
        },
        {
          path: "object-version/jobnet/public/:objectId",
          element: (
            <PrivateRoute
              element={ObjectVersion}
              meta={{
                title:t('title-job-ver'),
                requiresAuth: true,
              }}
              category={OBJECT_CATEGORY.JOBNET}
              publicType={true}
            />
          ),
        },
        {
          path: "object-version/jobnet/private/:objectId",
          element: (
            <PrivateRoute
              element={ObjectVersion}
              meta={{
                title:t('title-job-ver'),
                requiresAuth: true,
              }}
              category={OBJECT_CATEGORY.JOBNET}
              publicType={false}
            />
          ),
        },
        {
          path: "general-setting",
          element: (
            <PrivateRoute
              element={GeneralSetting}
              meta={{
                title:t('title-general-setting'),
                requiresAuth: true,
              }}
            />
          ),
        },
        {
          path: "lock-management",
          element: (
            <PrivateRoute
              element={LockManagement}
              meta={{
                title:t('title-lock-management'),
                requiresAuth: true,
              }}
            />
          ),
        },
        {
          path: "job-execution-result",
          element: (
            <PrivateRoute
              element={JobExecutionResult}
              meta={{
                title:t('title-job-exe-result'),
                requiresAuth: true,
              }}
            />
          ),
        },
        {
          path: "job-execution-management",
          element: (
            <PrivateRoute
              element={JobExecutionManagement}
              meta={{
                title:t('title-job-exe-management'),
                requiresAuth: true,
              }}
            />
          ),
        },
        {
          path: "*",
          element: (
            <PrivateRoute
              element={Err404}
              meta={{
                title:t('title-err'),
                requiresAuth: true,
              }}
            />
          ),
        },
      ],
    },
    {
      path: "*",
      element: (
        <PrivateRoute
          element={Err404}
          meta={{
            requiresAuth: false,
            title: "404 Not Found",
          }}
        />
      ),
    },
  ]);

  return elements;
}
