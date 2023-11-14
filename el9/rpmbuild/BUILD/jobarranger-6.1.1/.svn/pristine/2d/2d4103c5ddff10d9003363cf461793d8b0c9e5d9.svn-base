import { useState, useEffect, useRef } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Form, Input, Button, Row, Col, Modal, DatePicker } from "antd";
import { MinusOutlined, ExpandOutlined } from "@ant-design/icons";
import { ResizableBox } from "react-resizable";
import Draggable from "react-draggable";
import { useTranslation } from "react-i18next";
import "./UpdateScheduleDialog.scss";
import {
  getAllOperationList,
  setSelectedObject,
  setSelectedRowKeys,
  updateVisibleScheduleDialog,
} from "../../../store/AllOperationListSlice";
import moment from "moment";
import JobExecutionService from "../../../services/jobExecutionService";
import { getAllObjectList } from "../../../store/ObjectListSlice";
import { alertError } from "../CommonDialog";
import { SERVICE_RESPONSE } from "../../../constants";
import store from "../../../store";

const UpdateScheduleDialog = () => {
  const [form] = Form.useForm();
  const { t } = useTranslation();
  const dispatch = useDispatch();
  const visible = useSelector(
    (state) => state.allOperationList.updateScheduleDialogVisible
  );
  const selectedObject = useSelector(
    (state) => state.allOperationList.selectedObject[0]
  );

  const [height, setHeight] = useState(150);
  const [width, setWidth] = useState(420);
  const [disabled, setDisabled] = useState(false);
  const [isMinimize, setIsMinimize] = useState(false);

  const [bounds, setBounds] = useState({
    left: 0,
    top: 0,
    bottom: 0,
    right: 0,
  });

  const draggleRef = useRef(null);

  const onResize = (event, { size }) => {
    setHeight(size.height);
    setWidth(size.width);
  };

  const onMinimize = () => {
    setIsMinimize(!isMinimize);
  };

  const onStart = (_event, uiData) => {
    const { innerHeight, innerWidth } = window;
    const targetRect = draggleRef.current?.getBoundingClientRect();
    if (!targetRect) {
      return;
    }
    setBounds({
      left: -targetRect.left + uiData.x,
      right: innerWidth - (targetRect.right - uiData.x),
      top: -targetRect.top + uiData.y,
      bottom: innerHeight - (targetRect.bottom - uiData.y),
    });
  };

  useEffect(() => {
    if (selectedObject) {
      form.setFieldsValue({
        manageId: selectedObject.managementId,
        jobnetId: selectedObject.jobnetId,
        scheduleTime: selectedObject.scheduledTime,
        scheduleStartTime: moment(
          selectedObject.scheduledTime,
          "YYYY/MM/DD HH:mm"
        ),
      });
    }
  }, [dispatch, selectedObject]);

  const onOk = (value) => {};

  const onCancel = (e) => {
    dispatch(updateVisibleScheduleDialog(false));
  };

  const onFinish = (values) => {
    JobExecutionService.updateSchedule({
      innerJobnetId: form.getFieldValue("manageId"),
      scheduleTime: moment(
        form.getFieldValue("scheduleStartTime"),
        "YYYY/MM/DD HH:mm"
      ).format("YYYYMMDDHHmm"),
    }).then((res) => {
      if (res.type === SERVICE_RESPONSE.OK) {
        dispatch(getAllOperationList());
        dispatch(updateVisibleScheduleDialog(false));
      } else {
        alertError(t("title-error"), res.detail.message);
        dispatch(updateVisibleScheduleDialog(false));
      }
      store.dispatch(setSelectedRowKeys([]));
      store.dispatch(setSelectedObject([]));
    });
  };

  return (
    <Modal
      title={
        <div
          style={{
            width: "100%",
            height: "30px",
          }}
        >
          <div
            style={{
              width: "70%",
              cursor: "move",
              float: "left",
            }}
            onClick={(e) => {}}
            onMouseOver={() => {
              if (disabled) {
                setDisabled(false);
              }
            }}
            onMouseDown={() => {}}
            onMouseOut={() => {
              setDisabled(true);
            }} // fix eslintjsx-a11y/mouse-events-have-key-events
            // https://github.com/jsx-eslint/eslint-plugin-jsx-a11y/blob/master/docs/rules/mouse-events-have-key-events.md
          >
            {t("col-upd-schd-screen")}
          </div>
          <Button
            style={{
              float: "right",
              marginRight: "32px",
              border: "none",
              height: "10px",
              marginTop: "3px",
            }}
            onClick={onMinimize}
            shape="circle"
            icon={!isMinimize ? <MinusOutlined /> : <ExpandOutlined />}
          />
        </div>
      }
      visible={visible}
      onCancel={() => {
        onCancel();
      }}
      footer={[
        <Button key="submit" type="primary" onClick={form.submit}>
          {t("btn-apply")}
        </Button>,
        <Button
          key="cancel"
          onClick={() => {
            onCancel();
          }}
        >
          {t("btn-cancel")}
        </Button>,
      ]}
      wrapClassName="ant-multiple-modal-wrap"
      className={isMinimize && "hide-body-modal"}
      mask={false}
      zIndex={3}
      maskClosable={false}
      modalRender={(modal) => (
        <Draggable
          disabled={disabled}
          bounds={bounds}
          onStart={(event, uiData) => onStart(event, uiData)}
        >
          <div ref={draggleRef}>{modal}</div>
        </Draggable>
      )}
      width={width + 50}
    >
      <ResizableBox
        width={width}
        height={height}
        maxConstraints={[window.innerWidth - 100, window.innerHeight - 180]}
        // resizeHandles={["sw", "nw", "se", "ne"]}
        minConstraints={[200, 60]}
        onResize={onResize}
      >
        <div
          className="dialog-wrapper"
          style={{ width: width, height: height, overflow: "auto" }}
          onMouseOver={() => {
            setDisabled(true);
          }}
        >
          <Form
            form={form}
            name="control-hooks"
            size={"small"}
            className="icon-dialog-form"
            labelAlign="left"
            onFinish={onFinish}
          >
            <Row>
              <Col span={8}>{t("col-mgt-id")}</Col>
              <Col span={16}>{form.getFieldValue("manageId")}</Col>
            </Row>
            <Row>
              <Col span={8}>{t("label-jobnet-id")}</Col>
              <Col span={16}>{form.getFieldValue("jobnetId")}</Col>
            </Row>
            <Row>
              <Col span={8}>{t("lab-schedule-time")}</Col>
              <Col span={16}>{form.getFieldValue("scheduleTime")}</Col>
            </Row>
            <Row>
              <Col span={8}>{t("col-schd-srt-time")}</Col>
              <Col span={16}>
                <Form.Item
                  name="scheduleStartTime"
                  rules={[
                    {
                      type: "object",
                      required: true,
                      message: t("err-field-required", {
                        field: t("col-schd-srt-time"),
                      }),
                    },
                    ({ getFieldValue }) => ({
                      validator(_, value) {
                        if (
                          value.isSameOrBefore(
                            moment().format("YYYY/MM/DD HH:mm")
                          )
                        ) {
                          return Promise.reject(
                            new Error(t("err-msg-not-allow-past-time"))
                          );
                        }
                        return Promise.resolve();
                      },
                    }),
                  ]}
                >
                  <DatePicker
                    showTime
                    format={"YYYY/MM/DD HH:mm"}
                    showNow={false}
                    onOk={onOk}
                  />
                </Form.Item>
              </Col>
            </Row>
          </Form>
        </div>
      </ResizableBox>
    </Modal>
  );
};

export default UpdateScheduleDialog;
