import { useState, useEffect, useRef } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Select, Form, Input, Button, Row, Col, Modal } from "antd";
import { MinusOutlined, ExpandOutlined } from "@ant-design/icons";
import { ResizableBox } from "react-resizable";
import Draggable from "react-draggable";
import { removeOpenDialog } from "../../../../store/JobnetFormSlice";
import { getAllJobnet } from "../../../../store/JobnetInitDataSlice";
import { getNextZIndex } from "../../../../store/JobnetFormSlice";
import { useTranslation } from "react-i18next";
import "./TaskIconDialog.scss";
import {
  getExecIconSettingNextZIndex,
  removeExecOpenDialog,
} from "../../../../store/JobExecutionSlice";
import {
  isImpossibleStr,
  validateIsIdAlreadyExist,
} from "../../../../views/jobnetForm/Validation";
import { getShiftJISByteLength } from "../../../../common/Util";
import {
  INVALID_STRING,
  REGEX_PATTERM,
  SERVICE_RESPONSE,
} from "../../../../constants";
import store from "../../../../store";
import JobnetService from "../../../../services/JobnetService";
import { alertError } from "../../CommonDialog";
import { resetTaskIconTooltip } from "../../../../views/jobnetForm/ArrangeIconData";
const { Option } = Select;

const TaskIconDialog = ({
  id,
  graph,
  visible = true,
  cell,
  graphIndexId,
  jobnetIconId,
  formDisable = false,
  innerJobnetId = null,
}) => {
  const [form] = Form.useForm();
  const { t } = useTranslation();
  const dispatch = useDispatch();
  const [jobnetName, setJobnetName] = useState("");

  const allJobnetList =
    useSelector((state) => state.jobnetInitData.initData.allJobnetList) || [];

  const [height, setHeight] = useState(150);
  const [width, setWidth] = useState(480);
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
    dispatch(getAllJobnet());
    form.setFieldsValue({
      jobId: cell.jobId,
      jobName: cell.jobName || "",
      jobnetId: cell.iconSetting.hasOwnProperty("taskJobnetId")
        ? cell.iconSetting.taskJobnetId
        : "",
    });
    if(cell.iconSetting.hasOwnProperty("taskJobnetId")){
      if(cell.iconSetting.taskJobnetId){
        JobnetService.getJobnetInfo({
          jobnetId: cell.iconSetting.taskJobnetId,
        })
          .then((result) => {
            if (result.type === SERVICE_RESPONSE.OK) {
              setJobnetName(result.detail.data.jobnet_name)
            }
          })
          .catch((err) => {
            console.log(err);
          });
      }
    }
    
  }, [dispatch]);

  const onChangeJobnetId = (value) => {
    
    JobnetService.getJobnetInfo({
      jobnetId: value,
    })
      .then((result) => {
        if (result.type === SERVICE_RESPONSE.OK) {
          setJobnetName(result.detail.data.jobnet_name)
        }
      })
      .catch((err) => {
        console.log(err);
      });
  };

  const onCancel = (e) => {
    if (innerJobnetId === null) {
      dispatch(removeOpenDialog({ id, graphIndexId }));
    } else {
      dispatch(removeExecOpenDialog({ id, innerJobnetId }));
    }
  };

  const onFinish = (values) => {
    let cell = graph.getModel().getCell(id);
    JobnetService.checkJobnet({
      jobnetId: form.getFieldValue("jobnetId"),
    }).then((res) => {
      if ((res.type = SERVICE_RESPONSE.OK)) {
        if (res.detail.message === SERVICE_RESPONSE.RECORD_NOT_EXIST) {
          alertError(t("title-error"), t("err-jobnet-not-found"));
        } else {
          graph.model.setJobId(cell, form.getFieldValue("jobId"));
          let label = `<div style="height: 15px; width: 80px; overflow:hidden; white-space: nowrap; text-overflow: ellipsis;">${form.getFieldValue(
            "jobId"
          )}</div>`;
          if (form.getFieldValue("jobName")) {
            label += `<div style="height:15px; width: 80px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis;">${form.getFieldValue(
              "jobName"
            )}</div>`;
          }
          graph.model.setValue(cell, label);
          graph.model.setJobName(cell, form.getFieldValue("jobName"));
          graph.model.setIconSetting(cell, {
            jobId: form.getFieldValue("jobId"),
            jobName: form.getFieldValue("jobName"),
            taskJobnetId: form.getFieldValue("jobnetId"),
            taskJobnetName: jobnetName,
          });
          graph.model.setTooltipLabel(cell, resetTaskIconTooltip(cell, t));
          dispatch(removeOpenDialog({ id, graphIndexId }));
        }
      }
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
            onClick={(e) => {
              if (innerJobnetId === null) {
                dispatch(getNextZIndex(cell, graphIndexId));
              } else {
                dispatch(getExecIconSettingNextZIndex(cell, innerJobnetId));
              }
            }}
            onMouseOver={() => {
              if (disabled) {
                setDisabled(false);
              }
            }}
            onMouseDown={() => {
              if (innerJobnetId === null) {
                dispatch(getNextZIndex(cell, graphIndexId));
              } else {
                dispatch(getExecIconSettingNextZIndex(cell, innerJobnetId));
              }
            }}
            onMouseOut={() => {
              setDisabled(true);
            }} // fix eslintjsx-a11y/mouse-events-have-key-events
            // https://github.com/jsx-eslint/eslint-plugin-jsx-a11y/blob/master/docs/rules/mouse-events-have-key-events.md
          >
            {`${t("txt-task-icon")} (${jobnetIconId ? jobnetIconId + "/" : ""}${
              cell.jobId
            })`}
          </div>
          <Button
            style={{
              float: "right",
              marginRight: "32px",
              border: "none",
              height: "10px",
              marginTop: "4px",
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
        <Button
          key="submit"
          disabled={formDisable}
          type="primary"
          onClick={form.submit}
        >
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
      zIndex={cell.zindex}
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
            disabled={formDisable}
          >
            <Row>
              <Col span={6}>{t("lab-job-id")}</Col>
              <Col span={18}>
                <Form.Item
                  name="jobId"
                  rules={[
                    {
                      whitespace: true,
                      required: true,
                      message: t("err-field-required", {
                        field: t("lab-job-id"),
                      }),
                    },
                    () => ({
                      validator(_, value) {
                        var regex = new RegExp(
                          REGEX_PATTERM.MATCH_HANKAKU_HYPHEN_UNDERBAR
                        );
                        if (!regex.test(value)) {
                          return Promise.reject(new Error(t("err-id-format")));
                        }
                        return Promise.resolve();
                      },
                    }),
                    () => ({
                      validator(_, value) {
                        if (getShiftJISByteLength(value) > 32) {
                          return Promise.reject(
                            new Error(
                              t("err-msg-exceed-byte", {
                                field: t("lab-job-id"),
                                size: 32,
                              })
                            )
                          );
                        }
                        return Promise.resolve();
                      },
                    }),
                    () => ({
                      validator(_, value) {
                        if (value === INVALID_STRING.START) {
                          return Promise.reject(
                            new Error(t("err-msg-input-start"))
                          );
                        }
                        return Promise.resolve();
                      },
                    }),
                    () => ({
                      validator(_, value) {
                        if (validateIsIdAlreadyExist(value, cell.id, graph)) {
                          return Promise.reject(
                            new Error(
                              t("err-msg-id-already-exist", { id: value })
                            )
                          );
                        }
                        return Promise.resolve();
                      },
                    }),
                  ]}
                >
                  <Input maxLength={32} />
                </Form.Item>
              </Col>
            </Row>
            <Row className="mt-top-2px">
              <Col span={6}>{t("col-job-name")}</Col>
              <Col span={18}>
                <Form.Item
                  name="jobName"
                  rules={[
                    () => ({
                      validator(_, value) {
                        if (getShiftJISByteLength(value) > 64) {
                          return Promise.reject(
                            new Error(
                              t("err-msg-exceed-byte", {
                                field: t("col-job-name"),
                                size: 64,
                              })
                            )
                          );
                        }
                        return Promise.resolve();
                      },
                    }),
                    () => ({
                      validator(_, value) {
                        if (isImpossibleStr(value)) {
                          return Promise.reject(
                            new Error(
                              t("err-msg-invalid-string", {
                                field: t("col-job-name"),
                              })
                            )
                          );
                        }
                        return Promise.resolve();
                      },
                    }),
                  ]}
                >
                  <Input maxLength={64} />
                </Form.Item>
              </Col>
            </Row>
            <Row className="mt-top-2px">
              <Col span={6}>{t("label-jobnet-id")}</Col>
              <Col span={18}>
                <Form.Item
                  name="jobnetId"
                  rules={[
                    {
                      whitespace: true,
                      required: true,
                      message: t("err-field-required-select", {
                        field: t("label-jobnet-id"),
                      }),
                    },
                    ({ getFieldValue }) => ({
                      validator(_, value) {
                        if (
                          store.getState().jobnetForm.formObjList["1"].formData
                            .id === value
                        ) {
                          return Promise.reject(new Error(t("err-msg-self")));
                        }
                        return Promise.resolve();
                      },
                    }),
                  ]}
                >
                  <Select onChange={onChangeJobnetId}>
                    {allJobnetList.map((item) => {
                      return (
                        <Option key={item.jobnet_id} value={item.jobnet_id}>
                          {item.jobnet_id}
                        </Option>
                      );
                    })}
                  </Select>
                </Form.Item>
              </Col>
            </Row>
            <Row className="mt-top-2px">
              <Col span={6}>{t("label-jobnet-name")}</Col>
              <Col span={18}>{jobnetName}</Col>
            </Row>
          </Form>
        </div>
      </ResizableBox>
    </Modal>
  );
};

export default TaskIconDialog;
