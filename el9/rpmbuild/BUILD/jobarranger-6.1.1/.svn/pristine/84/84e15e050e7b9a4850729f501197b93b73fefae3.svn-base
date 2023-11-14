import React, { useRef, useState, useEffect, useCallback } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useSelector } from "react-redux";
import { Form, Spin } from "antd";
import FloatingButtons from "../../components/button/floatingButtons/FloatingButtons";
import JobnetFormComponent from "../../components/jobnet/jobnetForm/JobnetForm";
import store from "../../store";
import {
  FORM_TYPE,
  OBJECT_CATEGORY,
  RESPONSE_OBJ_KEY,
  SERVICE_RESPONSE,
  USER_TYPE,
} from "../../constants";
import {
  alertError,
  alertSuccess,
  confirmDialog,
  alertInfo,
} from "../../components/dialogs/CommonDialog";
import JobnetService from "../../services/JobnetService";
import {
  SaveFilled,
  FilePdfFilled,
  CloseCircleFilled,
} from "@ant-design/icons/";
import { t } from "i18next";
import "ant-design-draggable-modal/dist/index.css";
import "./JobnetForm.scss";
import objectLockService from "../../services/objectLockService";
import usePrompt from "../../components/dialogs/usePrompt";
import { set } from "lodash";

const JobnetForm = ({ formType, publicType }) => {
  const { objectId, date } = useParams();
  const isDialog = useRef(false);
  const [backKeyEvent, setBackKeyEvent] = useState(false);
  const [graphDataChange, setGraphDataChange] = useState(false);
  const isOtherDialog = useRef(false);
  const [mainForm] = Form.useForm();
  const [formLoading, setFormLoading] = useState(false);
  const modifyForm = useRef(false);
  const jobnetInfo = useSelector(
    (state) => state.jobnetForm.formObjList["1"].data
  );

  const heartbeatIntervalTime =
    useSelector((state) => state.user.userInfo.heartbeatIntervalTime) || 30;

  const childRef = useRef(null);
  const [isBlocked, setIsBlocked] = useState(false);
  const [isSave, setIsSave] = useState(false);
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
    if (!isBlocked && !isSave) {
      if (modifyForm.current || graphDataChange) {
        setIsBlocked(true);
      }
    }
  }, [modifyForm.current, graphDataChange]);

  useEffect(() => {
    if (
      (modifyForm.current ||
        graphDataChange ||
        formType === FORM_TYPE.NEW_OBJECT ||
        formType === FORM_TYPE.CREATE) && userType !== USER_TYPE.USER_TYPE_GENERAL &&
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
  }, [modifyForm.current, backKeyEvent, cancelPopState, graphDataChange]);

  useEffect(() => {
    return () => {
      if (window.history.state === "fake-route") {
        window.history.back();
      } else {
        window.removeEventListener("popstate", cancelPopState);
      }
    };
  }, []);

  useEffect(() => {
    const unloadCallback = async (event) => {
      event.preventDefault();
      event.returnValue = "";
      return "";
    };
    if (
      modifyForm.current ||
      graphDataChange ||
      formType === FORM_TYPE.NEW_OBJECT ||
      formType === FORM_TYPE.CREATE && userType !== USER_TYPE.USER_TYPE_GENERAL
    ) {
      window.addEventListener("beforeunload", unloadCallback);
      return () => window.removeEventListener("beforeunload", unloadCallback);
    }
  }, [modifyForm.current, jobnetInfo]);

  usePrompt(
    t("warn-mess-redisplay"),
    isSave === false &&
    ((formType === FORM_TYPE.NEW_OBJECT || formType === FORM_TYPE.CREATE) && userType !== USER_TYPE.USER_TYPE_GENERAL),
    isBlocked
  );

  useEffect(() => {
    if (jobnetInfo) {
      if (Object.keys(jobnetInfo).length !== 0) {
        if (userType == USER_TYPE.USER_TYPE_GENERAL && (formType === FORM_TYPE.NEW_OBJECT || formType === FORM_TYPE.CREATE)) {
          alertInfo("", t("txt-permission-denied"));
          navigate(
            `/object-list/jobnet/${publicType ? "public" : "private"}/`
          );
          return;
        }
      }
      if (
        (jobnetInfo.type === FORM_TYPE.EDIT ||
          jobnetInfo.type === FORM_TYPE.NEW_VERSION) &&
        jobnetInfo.editable === 1 &&
        (!jobnetInfo.hasOwnProperty("isLocked") || jobnetInfo.isLocked === 0)
      ) {
        const intervalId = setInterval(() => {
          if (!document.hidden) {
            objectLockService.heartbeat({
              objectId,
              objectType: OBJECT_CATEGORY.JOBNET,
            });
          }
        }, heartbeatIntervalTime * 1000);
        return () => {
          clearInterval(intervalId);
        };
      }
    }
  }, [jobnetInfo]);

  const navigate = useNavigate();
  const navigateToVersionHandler = (id) => {
    navigate(
      `/object-version/jobnet/${mainForm.getFieldValue("isPublic") ? "public" : "private"
      }/${id}`
    );
  };

  const isEditable = () => {
    if (jobnetInfo) {
      return jobnetInfo.editable == 0 || jobnetInfo.isLocked == 1
        ? true
        : false;
    }
    return false;
  };

  const detectedGraphChange = () => {
    setGraphDataChange(true);
  };

  const onFormCancelAction = () => {
    if (formType === FORM_TYPE.CREATE) {
      navigate(
        `/object-list/jobnet/${mainForm.getFieldValue("isPublic") ? "public" : "private"
        }`
      );
    } else {
      navigateToVersionHandler(jobnetInfo.detail.jobnet_id);
    }
  };
  const cancel = () => {
    isDialog.current = false;
  };

  onbeforeunload = (event) => { 
  };

  const buttons = [
    {
      label: t("btn-pdf"),
      icon: <FilePdfFilled />,
      disabled: true,
    },
    {
      label: t("btn-save"),
      icon: <SaveFilled />,
      disabled: isEditable(),
      clickAction() {
        mainForm.submit();
      },
    },
    {
      label: t("btn-close"),
      icon: <CloseCircleFilled />,
      disabled: false,
      clickAction() {
        if (
          (!modifyForm.current &&
            !graphDataChange &&
            formType === FORM_TYPE.EDIT) ||
          (!modifyForm.current &&
            !graphDataChange &&
            formType === FORM_TYPE.NEW_VERSION)
        ) {
          onFormCancelAction();
        } else {
          setIsBlocked(false);
          setIsSave(true);
          confirmDialog(
            t("title-msg-confirm"),
            t("warn-mess-redisplay"),
            onFormCancelAction,
            () => {
              setIsBlocked(true);
              setIsSave(false);
              return false;
            }
          );
        }
      },
    },
  ];

  const onFormFinishAction = (name, { values, forms }) => {
    if (name === "jobnet-header-form") {
      isDialog.current = true;
      confirmDialog(
        t("title-msg-confirm"),
        t("txt-data-fil"),
        () => {
          //remove window prompt
          setIsBlocked(false);
          setIsSave(true);
          setFormLoading(true);
          let result = childRef.current.validateGraphData();

          if (!result[0]) {
            setFormLoading(false);
            isDialog.current = false;
            isOtherDialog.current = true;
            return;
          }

          let data = {
            jobnetId: values.id,
            multiple: values.multiple,
            public: values.isPublic ? 1 : 0,
            jobnetName: values.name,
            description: values.description || "",
            timeoutSec: parseInt(values.timeout),
            timeoutType: parseInt(values.timeoutType),
            icon: result[1],
            flow: result[2],
            userName:
              formType === FORM_TYPE.EDIT || formType === FORM_TYPE.NEW_VERSION
                ? jobnetInfo.detail.user_name
                : store.getState().user.userInfo.userName,
            type: formType,
          };

          if (
            formType === FORM_TYPE.EDIT ||
            formType === FORM_TYPE.NEW_VERSION
          ) {
            data.updateDate = jobnetInfo.detail.update_date;
          } else {
            data.updateDate = "";
          }

          JobnetService.saveJobnet(data).then((result) => {
            if (result.type === SERVICE_RESPONSE.OK) {
              alertSuccess(
                t("title-success"),
                `${t("label-success")} : ${result.detail.data.jobnetId}`
              );
              navigateToVersionHandler(result.detail.data.jobnetId);
            } else {
              if (result.detail.hasOwnProperty(RESPONSE_OBJ_KEY.MESSAGE_CODE)) {
                alertError(
                  t("title-error"),
                  t(result.detail[RESPONSE_OBJ_KEY.MESSAGE_CODE]),
                  416,
                  enableEnterSubmit
                );
              } else if (
                result.detail.message === SERVICE_RESPONSE.RECORD_EXIST
              ) {
                alertError(
                  t("title-error"),
                  t("txt-jobnet-val-id"),
                  416,
                  enableEnterSubmit
                );
              } else if (
                result.detail.message === SERVICE_RESPONSE.RECORD_NOT_EXIST
              ) {
                alertError(
                  t("title-error"),
                  t("data-not-exist"),
                  416,
                  enableEnterSubmit
                );
              } else if (
                result.detail.message === SERVICE_RESPONSE.NO_LOCK_SESSION
              ) {
                alertError(
                  t("title-error"),
                  t("err-no-lock-exist"),
                  416,
                  enableEnterSubmit
                );
                // navigateToVersionHandler(data.jobnetId);
              } else {
                if (result.detail.message instanceof Object) {
                  for (const [key, value] of Object.entries(
                    result.detail.message
                  )) {
                    alertError(
                      t("title-error"),
                      `${value}`,
                      416,
                      enableEnterSubmit
                    );
                  }
                } else {
                  alertError(
                    t("title-error"),
                    result.detail.message,
                    416,
                    enableEnterSubmit
                  );
                }
              }
            }
            setFormLoading(false);
          });
        },
        cancel
      );
    }
  };
  const enableEnterSubmit = () => {
    isDialog.current = false;
  };
  const submitBtnAction = () => {
    mainForm.submit();
  };
  const handleFormKeydown = (e) => {
    if (!isDialog.current) {
      if (isOtherDialog.current) {
        isOtherDialog.current = false;
      } else {
        if (e.keyCode === 13 && store.getState().jobnetForm.formObjList["1"].modalState.modals.length === 0) {
          submitBtnAction();
        }
      }
    }
  };
  useEffect(() => {
    window.addEventListener("keydown", handleFormKeydown);
    return () => {
      window.removeEventListener("keydown", handleFormKeydown);
    };
  }, []);

  return (
    <Form.Provider
      onFormFinish={(name, { values, forms }) =>
        onFormFinishAction(name, { values, forms })
      }
      onFormChange={() => {
        modifyForm.current = true;
        if (!isBlocked) {
          setIsBlocked(true);
        }
      }}
    >
      <Spin spinning={formLoading}>
        <Form id="main-form" form={mainForm}>
          <JobnetFormComponent
            ref={childRef}
            formType={formType}
            publicType={publicType}
            objectId={objectId}
            date={date}
            form={mainForm}
            detectedGraphChange={detectedGraphChange}
          />
          <FloatingButtons buttons={buttons} />
        </Form>
      </Spin>
    </Form.Provider>
  );
};

export default JobnetForm;
