import { useState, useEffect, useRef } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Select, Form, Input, Button, Row, Col, Modal } from "antd";
import { MinusOutlined, ExpandOutlined } from "@ant-design/icons";
import { ResizableBox } from "react-resizable";
import Draggable from "react-draggable";
import { removeOpenDialog } from "../../../../store/JobnetFormSlice";
import { getNextZIndex } from "../../../../store/JobnetFormSlice";
import { getDefineExtendedJob } from "../../../../store/JobnetInitDataSlice";
import { useTranslation } from "react-i18next";
import "./ExtendedJobIconDialog.scss";
import {
  getExecIconSettingNextZIndex,
  removeExecOpenDialog,
} from "../../../../store/JobExecutionSlice";
import { getShiftJISByteLength } from "../../../../common/Util";
import {
  isImpossibleStr,
  validateIsIdAlreadyExist,
} from "../../../../views/jobnetForm/Validation";
import { INVALID_STRING, REGEX_PATTERM } from "../../../../constants";
import { resetExtendedJobIconTooltip } from "../../../../views/jobnetForm/ArrangeIconData";

const { Option } = Select;
const { TextArea } = Input;
const ExtendedJobIconDialog = ({
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
  const [description, setDescription] = useState("");
  const defineExtendedJobList =
    useSelector((state) => state.jobnetInitData.initData.defineExtendedJob) ||
    [];

  const [height, setHeight] = useState(280);
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
    dispatch(getDefineExtendedJob());

    var extendedJob;
    if (cell.iconSetting.hasOwnProperty("commandId")) {
      extendedJob = cell.iconSetting.commandId;
    }
    form.setFieldsValue({
      jobId: cell.jobId,
      jobName: cell.jobName || "",
      extendedJob: extendedJob,
      parameter: cell.iconSetting.hasOwnProperty("parameter")
        ? cell.iconSetting.parameter || ""
        : "",
    });
  }, [dispatch]);

  useEffect(() => {
    if (
      defineExtendedJobList.length > 0 &&
      cell.iconSetting.hasOwnProperty("commandId")
    ) {
      onChangeJob(cell.iconSetting.commandId);
    }
  }, [defineExtendedJobList]);

  const onChangeJob = (value) => {
    if (value) {
      const found = defineExtendedJobList.find((obj) => {
        return obj.command_id === value;
      });
      setDescription(found.memo);
    }
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
      commandId: form.getFieldValue("extendedJob"),
      parameter: form.getFieldValue("parameter"),
    });
    graph.model.setTooltipLabel(cell, resetExtendedJobIconTooltip(cell, t));
    dispatch(removeOpenDialog({ id, graphIndexId }));
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
            {`${t("txt-extd-job-icon")} (${
              jobnetIconId ? jobnetIconId + "/" : ""
            }${cell.jobId})`}
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
              <Col span={6}>{t("txt-extd-job")}</Col>
              <Col span={18}>
                <Form.Item
                  name="extendedJob"
                  rules={[
                    {
                      whitespace: true,
                      required: true,
                      message: t("err-field-required-select", {
                        field: t("txt-extd-job"),
                      }),
                    },
                  ]}
                >
                  <Select onChange={onChangeJob}>
                    {defineExtendedJobList.map((item) => {
                      return (
                        <Option key={item.command_id} value={item.command_id}>
                          {item.command_name}
                        </Option>
                      );
                    })}
                  </Select>
                </Form.Item>
              </Col>
            </Row>
            <Row className="mt-top-3px">
              <Col span={6}>{t("txt-par")}</Col>
              <Col span={18}>
                <Form.Item
                  name="parameter"
                  rules={[
                    () => ({
                      validator(_, value) {
                        if (getShiftJISByteLength(value) > 4000) {
                          return Promise.reject(
                            new Error(
                              t("err-msg-exceed-byte", {
                                field: t("txt-par"),
                                size: 4000,
                              })
                            )
                          );
                        }
                        return Promise.resolve();
                      },
                    }),
                  ]}
                >
                  <TextArea rows={4} />
                </Form.Item>
              </Col>
            </Row>
          </Form>

          <div className="description">
            {t("col-obj-des")}
            <br />
            <br />
            {description}
          </div>
        </div>
      </ResizableBox>
    </Modal>
  );
};

export default ExtendedJobIconDialog;
