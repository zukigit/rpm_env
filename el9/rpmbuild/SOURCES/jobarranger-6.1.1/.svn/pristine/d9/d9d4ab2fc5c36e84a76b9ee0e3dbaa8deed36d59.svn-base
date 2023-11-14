import React, { useEffect, useRef, useState, useCallback } from "react";
import FilterFormComponent from "../../components/filterForm/FilterForm";
import Layout from "antd/lib/layout/layout";
import {
  confirmDialog,
  alertSuccess,
  alertError,
  alertInfo,
} from "../../components/dialogs/CommonDialog";
import { SaveFilled, CloseCircleFilled } from "@ant-design/icons/";
import FloatingButtons from "../../components/button/floatingButtons/FloatingButtons";
import { t } from "i18next";
import { Form, Spin } from "antd";
import { useParams } from "react-router-dom";
import store from "../../store";
import { createFilterFormObjectRequest } from "../../factory/FormObjectFactory";
import moment from "moment";
import { useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { createFilterObject, cleanupFilterForm } from "../../store/FilterSlice";
import objectLockService from "../../services/objectLockService";
import { setunLock } from "../../store/ObjectListSlice";
import { FORM_TYPE, OBJECT_CATEGORY, SERVICE_RESPONSE, USER_TYPE } from "../../constants";
import usePrompt from "../../components/dialogs/usePrompt";

const FilterForm = ({ formType, publicType }) => {
  const dispatch = useDispatch();
  const isDialog = useRef(false);
  const [backKeyEvent, setBackKeyEvent] = useState(false);
  let { objectId, date } = useParams();
  const navigate = useNavigate();
  const [mainForm] = Form.useForm();
  const disabledAlert = useRef(0);
  const isFormEdited = useRef(false);
  const objectLockObject = useRef({
    objectId: "",
    objectType: 0,
  });

  const responseData = useSelector((state) => state["filter"].responseData);
  const isLoading = useSelector((state) => state["filter"].isLoading);
  const filterInfo = useSelector((state) => state["filter"].data);
  const isFormEditable = useSelector(
    (state) => state["objectList"].isObjectFormEditable
  );
  const userType = useSelector((state) => state["user"].userInfo.userType);

  const cancelPopState = useCallback((event) => {
    var r = window.confirm(t("warn-mess-redisplay"));
    if (r == false) {
      window.history.pushState(
        "fake-route",
        document.title,
        window.location.href
      );
      event.preventDefault();
    } else {
      navigate(-1);
    }
  }, []);

  useEffect(() => {
    if (
      (isFormEdited.current ||
        formType === FORM_TYPE.NEW_OBJECT ||
        formType === FORM_TYPE.CREATE) &&
      backKeyEvent === false
    ) {
      // Add a fake history event so that the back button does nothing if pressed once
      window.history.pushState(
        "fake-route",
        document.title,
        window.location.href
      );
      window.addEventListener("popstate", cancelPopState);
      setBackKeyEvent(true);
    }
  }, [isFormEdited.current, backKeyEvent, cancelPopState]);

  useEffect(() => {
    return () => {
      if (window.history.state === "fake-route") {
        window.history.back();
      } else {
        window.removeEventListener("popstate", cancelPopState);
      }
    };
  }, []);

  const heartbeatIntervalTime =
    useSelector((state) => state.user.userInfo.heartbeatIntervalTime) || 30;
  const [isBlocked, setIsBlocked] = useState(false);
  const [isSave, setIsSave] = useState(false);

  useEffect(() => {
    if (!isBlocked && !isSave) {
      setIsBlocked(isFormEdited.current);
    }
  }, [isFormEdited.current]);

  useEffect(() => {
    if (filterInfo) {
      if (Object.keys(filterInfo).length !== 0) {
        if (userType == 1 && (formType === FORM_TYPE.NEW_OBJECT || formType === FORM_TYPE.CREATE)) {
          alertInfo("", t("txt-permission-denied"));
          navigate(
            `/object-list/filter/${publicType ? "public" : "private"}/`
          );
          return;
        }
      }
      if (
        (filterInfo.formType === FORM_TYPE.EDIT ||
          filterInfo.formType === FORM_TYPE.NEW_VERSION) &&
        filterInfo.editable === 1 &&
        (!filterInfo.hasOwnProperty("isLocked") || filterInfo.isLocked === 0)
      ) {
        const intervalId = setInterval(() => {
          if (!document.hidden) {
            objectLockService.heartbeat({
              objectId,
              objectType: OBJECT_CATEGORY.FILTER,
            });
          }
        }, heartbeatIntervalTime * 1000);
        return () => {
          clearInterval(intervalId);
        };
      }
    }
  }, [filterInfo]);

  const navigateToVersionHandler = () => {
    navigate(
      `/object-version/filter/${mainForm.getFieldValue("isPublic") ? "public" : "private"
      }/${mainForm.getFieldValue("id")}`
    );
  };

  const cancelConfirm = () => {
    if (objectId) {
      if (objectId.length > 0) {
        navigate(
          `/object-version/filter/${mainForm.getFieldValue("isPublic") ? "public" : "private"
          }/${objectId}`
        );
      }
    } else {
      navigate(
        `/object-list/filter/${mainForm.getFieldValue("isPublic") ? "public" : "private"
        }/`
      );
    }
  };
  const cancelFloatBtn = () => {
    if (
      (!isFormEdited.current && formType === FORM_TYPE.EDIT) ||
      (!isFormEdited.current && formType === FORM_TYPE.NEW_VERSION)
    ) {
      cancelConfirm();
    } else {
      setIsBlocked(false);
      setIsSave(true);
      confirmDialog(
        t("title-msg-confirm"),
        t("warn-mess-redisplay"),
        cancelConfirm,
        () => {
          setIsBlocked(true);
          setIsSave(false);
          return false;
        }
      );
    }
  };
  const onFormFinishAction = (name, { values, forms }) => {
    isDialog.current = true;
    confirmDialog(
      t("title-msg-confirm"),
      t("txt-data-fil"),
      createFilter,
      cancel
    );
  };
  const onFormChangedAction = (name, { changedFields, forms }) => {
    if (changedFields[0].value != "" && changedFields[0].value != undefined) {
      isFormEdited.current = true;
      if (!isBlocked) {
        setIsBlocked(true);
      }
    }
  };
  //create Filter process
  const createFilter = () => {
    //remove window prompt
    setIsBlocked(false);
    setIsSave(true);
    let formObject = store.getState().filter.formData;
    let filterUpdateRequest = createFilterFormObjectRequest(
      formObject.updateDate,
      formObject.id,
      mainForm.getFieldValue("id"),
      mainForm.getFieldValue("name"),
      formObject.userName,
      mainForm.getFieldValue("isPublic") ? 1 : 0,
      moment().format("YYYYMMDDHHmmss"),
      mainForm.getFieldValue("description"),
      formType,
      formObject.createdDate,
      formObject.validFlag,
      formObject.editable,
      formObject.authority,
      mainForm.getFieldValue("baseDays"),
      mainForm.getFieldValue("designatedDay"),
      mainForm.getFieldValue("shiftDay"),
      mainForm.getFieldValue("getCalendar"),
      ""
    );
    dispatch(createFilterObject(filterUpdateRequest));
    isDialog.current = false;
  };

  const submitBtnAction = () => {
    mainForm.submit();
  };
  const cancel = () => {
    isDialog.current = false;
  };
  const buttons = [
    {
      label: t("btn-save"),
      icon: <SaveFilled />,
      clickAction: submitBtnAction,
      disabled: !isFormEditable,
    },
    {
      label: t("btn-close"),
      icon: <CloseCircleFilled />,
      clickAction: cancelFloatBtn,
      disabled: false,
    },
  ];

  useEffect(() => {
    if (disabledAlert.current < 1) {
      disabledAlert.current++;
      return;
    }
    if (responseData) {
      switch (responseData.type) {
        case SERVICE_RESPONSE.OK:
          alertSuccess(
            t("title-success"),
            `${t("label-success")} : ${mainForm.getFieldValue("id")}`
          );
          navigateToVersionHandler();
          break;
        case SERVICE_RESPONSE.INCOMEPLETE:
          if (
            responseData.detail.message === SERVICE_RESPONSE.NO_LOCK_SESSION
          ) {
            alertError(t("title-error"), t("err-no-lock-exist"));
            navigateToVersionHandler(objectId);
          } else {
            alertError(
              t("title-error"),
              `${responseData.detail["message-objectid"]
                ? t(responseData.detail["message-objectid"]) + " :"
                : ""
              }  ${t(
                responseData.detail["message-detail"]
                  ? t(responseData.detail["message-detail"])
                  : t("err-msg-fail")
              )}`
            );
            navigate(
              `/object-list/filter/${publicType ? "public" : "private"}/`
            );
          }
          break;
        case SERVICE_RESPONSE.RECORD_EXIST:
          alertError(t("title-error"), t("txt-fild-val-id"));
          break;
        default:
          alertError(
            t("title-error"),
            `${responseData.detail["message-objectid"]
              ? t(responseData.detail["message-objectid"]) + " :"
              : ""
            }  ${t(
              responseData.detail["message-detail"]
                ? t(responseData.detail["message-detail"])
                : t("err-msg-fail")
            )}`
          );
          navigate(`/object-list/filter/${publicType ? "public" : "private"}/`);
      }
    }
  }, [responseData]);

  const handleFormKeydown = (e) => {
    if (!isDialog.current) {
      if (e.keyCode === 13) {
        submitBtnAction();
      }
    }
  };
  // cleanup data.
  useEffect(() => {
    window.addEventListener("keydown", handleFormKeydown);
    return () => {
      let formObject = store.getState().filter.formData;
      let isEditable = store.getState().objectList.isObjectFormEditable;
      objectLockObject.current.objectId = formObject.id;
      objectLockObject.current.objectType = "FILTER";
      if (isEditable) {
        objectLockService.deleteLock([objectLockObject.current]);
        dispatch(setunLock());
      }
      dispatch(cleanupFilterForm());
      window.removeEventListener("keydown", handleFormKeydown);
    };
  }, []);

  useEffect(() => {
    const unloadCallback = async (event) => {
      event.preventDefault();
      event.returnValue = "";
      return "";
    };
    if (
      isFormEdited.current ||
      formType === FORM_TYPE.NEW_OBJECT ||
      formType === FORM_TYPE.CREATE
    ) {
      window.addEventListener("beforeunload", unloadCallback);
      return () => window.removeEventListener("beforeunload", unloadCallback);
    }
  }, [isFormEdited.current, filterInfo]);
  usePrompt(
    t("warn-mess-redisplay"),
    isSave === false &&
    ((formType === FORM_TYPE.NEW_OBJECT || formType === FORM_TYPE.CREATE) && userType != USER_TYPE.USER_TYPE_GENERAL),
    isBlocked
  );
  return (
    <Form.Provider
      onFormFinish={(name, { values, forms }) =>
        onFormFinishAction(name, { values, forms })
      }
      onFormChange={(name, { changedFields, forms }) =>
        onFormChangedAction(name, { changedFields, forms })
      }
    >
      <Spin size="large" spinning={isLoading}>
        <Form id="main-form" name="main-form" form={mainForm}>
          <Layout>
            <FilterFormComponent
              objectId={objectId}
              date={date}
              objectType="filter"
              formType={formType}
              publicType={publicType}
            />
          </Layout>
          <FloatingButtons buttons={buttons} />
        </Form>
      </Spin>
    </Form.Provider>
  );
};

export default FilterForm;
