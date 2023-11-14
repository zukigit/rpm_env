import React, { useEffect, useRef, useState } from "react";
import YearCalendar from "../calendar/YearCalendar";
import {
  initCalendar,
  setCalendarFormData,
  initCalendarEdit,
} from "../../store/CalendarSlice";
import {
  lockObject,
  setObjectFormEditable,
  setunLock,
} from "../../store/ObjectListSlice";
import { createFormObject } from "../../factory/FormObjectFactory";
import Layout from "antd/lib/layout/layout";
import "./CalendarForm.scss";
import FormObject from "../form/formObject/FormObject";
import ScheduleFormObject from "../form/scheduleFormObject/ScheduleFormObject";
import { t } from "i18next";
import { useDispatch, useSelector } from "react-redux";
import { FORM_TYPE, OBJECT_CATEGORY } from "../../constants";
import { Form } from "antd";
import objectLockService from "../../services/objectLockService";

const CalendarFormComponent = ({
  objectId,
  date,
  objectType,
  formType = FORM_TYPE.EDIT,
  publicType,
}) => {
  const dispatch = useDispatch();
  const calendarInfo = useSelector((state) => state["calendar"].data);
  const isFormEditable = useSelector(
    (state) => state["objectList"].isObjectFormEditable
  );
  const subForm = Form.useFormInstance();
  const [isReloaded, reloaded] = useState(false);

  useEffect(() => {
    //unlock on reload
    if (formType != FORM_TYPE.SCHEDULE) {
      var localStoredValues = JSON.parse(
        sessionStorage.getItem("formIsRefreshed") || "[]"
      );
      if ("isLocked" in localStoredValues && localStoredValues.isLocked == 0) {
        let lockedObject = {
          objectId: localStoredValues.objectId,
          objectType: localStoredValues.objectType,
        };
        objectLockService.deleteLockAsync([lockedObject]).then((response) => {
          reloaded(true);
        });
        dispatch(setunLock());
      } else {
        reloaded(true);
      }
      sessionStorage.removeItem("formIsRefreshed");
    } else {
      reloaded(true);
    }
    //
  }, [objectId]);
  useEffect(() => {
    if (isReloaded == true) {
      if (formType === FORM_TYPE.CREATE) {

        let obj = {
          type: formType,
          isPublic: publicType,
        };
        dispatch(initCalendar(obj));
      } else {
        let object = {
          id: objectId,
          date: date,
          type: formType,
        };
        dispatch(initCalendarEdit(object));
      }
    }
  }, [objectId, isReloaded]);

  useEffect(() => {
    if (calendarInfo) {
      let formObject = {};
      let editable = 1;
      let idEditable = 1;
      let tmpId = "";
      if (Object.keys(calendarInfo).length > 0) {
        let formEditable = 1;
        if (formType != FORM_TYPE.SCHEDULE) {
          if (calendarInfo.editable == 0 || calendarInfo.isLocked == 1) {
            dispatch(setObjectFormEditable(false));
            formEditable = 0;
          } else {
            dispatch(setObjectFormEditable(true));
            formEditable = 1;
          }
        }
        switch (formType) {
          case FORM_TYPE.SCHEDULE:
            editable = 0;
            idEditable = 0;
            break;
          case FORM_TYPE.CREATE:
            editable = 1;
            idEditable = 1;
            break;
          case FORM_TYPE.EDIT:
            editable = formEditable;
            idEditable = 0;
            break;
          case FORM_TYPE.NEW_OBJECT:
            editable = calendarInfo.editable;
            idEditable = calendarInfo.editable;
            break;
          case FORM_TYPE.NEW_VERSION:
            editable = calendarInfo.editable;
            idEditable = 0;
            break;
        }
        if (!objectId) {
          formObject = createFormObject(
            `${t(`obj-${OBJECT_CATEGORY.CALENDAR}`)}_${calendarInfo.lastid}`,
            publicType,
            "",
            "",
            "",
            calendarInfo.name,
            true,
            "",
            "",
            "",
            [],
            "",
            editable,
            idEditable,
            0
          );
        } else {
          if (formType == FORM_TYPE.NEW_OBJECT) {
            tmpId = `${t(`obj-${OBJECT_CATEGORY.CALENDAR}`)}_${calendarInfo.calendarId
              }`;
          } else {
            tmpId = calendarInfo.calendarId;
            //lock if editable.
            if (formEditable == 1) {
              if (objectType != "schedule") {
                let object = {
                  objectId: tmpId,
                  date: calendarInfo.updateDate,
                  category: objectType,
                };
                dispatch(lockObject(object));
              }
            }
          }
          formObject = createFormObject(
            tmpId,
            parseInt(calendarInfo.publicFlag) ? true : false,
            "",
            calendarInfo.updateDate,
            calendarInfo.calendarName,
            calendarInfo.userName,
            calendarInfo.authority,
            calendarInfo.lastday,
            calendarInfo.desc,
            "",
            "",
            calendarInfo.dates,
            editable,
            idEditable,
            calendarInfo.isLocked
          );
        }
        dispatch(setCalendarFormData(formObject));
      }
    }
  }, [calendarInfo]);
  const selectorFunction = (state) => {
    return state["calendar"].data;
  };
  return (
    <Form id="calendar-sub-form" name="calendar-sub-form" form={subForm}>
      <Layout className="object-info-layout">
        {/* <FormObject formId="calendar" /> */}
        {objectType == FORM_TYPE.SCHEDULE ? (
          <ScheduleFormObject
            formId={OBJECT_CATEGORY.CALENDAR}
            // form={objectForm}
            //onFinishAction={submitAction}
            isCalendar={true}
            objectSlice="calendar"
            objectType={objectType}
          />
        ) : (
          <FormObject
            formId={OBJECT_CATEGORY.CALENDAR}
            // form={objectForm}
            //onFinishAction={submitAction}
            objectSlice="calendar"
            objectType={objectType}
          />
        )}
      </Layout>
      <Layout className="year-calendar-layout">
        <YearCalendar
          selectorFunction={selectorFunction}
          objectType={objectType}
        />
      </Layout>
    </Form>
  );
};

export default CalendarFormComponent;
