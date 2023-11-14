import { useState, useEffect, useRef } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  Select,
  Checkbox,
  InputNumber,
  Radio,
  Form,
  Input,
  Button,
  Row,
  Col,
  Modal,
} from "antd";
import { MinusOutlined, ExpandOutlined } from "@ant-design/icons";
import { ResizableBox } from "react-resizable";
import Draggable from "react-draggable";
import { removeOpenDialog } from "../../../../store/JobnetFormSlice";
import { getNextZIndex } from "../../../../store/JobnetFormSlice";
import { useTranslation } from "react-i18next";
import { getAvailableHosts } from "../../../../store/JobnetInitDataSlice";
import "./RebootIconDialog.scss";
import {
  getExecIconSettingNextZIndex,
  removeExecOpenDialog,
} from "../../../../store/JobExecutionSlice";
import { INVALID_STRING, REGEX_PATTERM } from "../../../../constants";
import { getShiftJISByteLength } from "../../../../common/Util";
import {
  isHankakuStrAndUnderbarAndFirstNotNum,
  isImpossibleStr,
  validateIsIdAlreadyExist,
} from "../../../../views/jobnetForm/Validation";
import { resetRebootTooltip } from "../../../../views/jobnetForm/ArrangeIconData";

const { Option } = Select;

const RebootIconDialog = ({
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
  const [hostFlag, setHostFlag] = useState(0);
  const [processModeFlag, setProcessModeFlag] = useState(0);
  const hostList =
    useSelector((state) => state.jobnetInitData.initData.host) || [];

  const [height, setHeight] = useState(240);
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
    dispatch(getAvailableHosts());
    let hostType = 0;
    if (cell.iconSetting.hasOwnProperty("hostFlag")) {
      hostType = cell.iconSetting.hostFlag;
      setHostFlag(hostType);
    }
    let processMode = 0;
    if (cell.iconSetting.hasOwnProperty("rebootModeFlag")) {
      processMode = cell.iconSetting.rebootModeFlag;
      setProcessModeFlag(processMode);
    }
    form.setFieldsValue({
      jobId: cell.jobId,
      jobName: cell.jobName || "",
      rdoHost: hostType,
      hostName: hostType === 0 ? cell.iconSetting.hostName || "" : "",
      varName: hostType === 1 ? cell.iconSetting.hostName || "" : "",
      processMode: processMode,
      timeoutSec: cell.iconSetting.hasOwnProperty("rebootWaitTime")
        ? cell.iconSetting.rebootWaitTime
        : "0",
      timeoutMin: cell.iconSetting.hasOwnProperty("timeout")
        ? cell.iconSetting.timeout
        : "0",
      forceFlag: cell.iconSetting.hasOwnProperty("forceFlag")
        ? cell.iconSetting.forceFlag
        : false,
    });
  }, [dispatch]);

  const hostFlagOnChange = (e) => {
    if (e.target.value === 0) {
      setHostFlag(0);
    } else {
      setHostFlag(1);
    }
  };

  const processModeOnChange = (e) => {
    if (e.target.value === 0) {
      setProcessModeFlag(0);
    } else {
      setProcessModeFlag(1);
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
      hostFlag: parseInt(form.getFieldValue("rdoHost")),
      hostName:
        form.getFieldValue("rdoHost") === 0
          ? form.getFieldValue("hostName")
          : form.getFieldValue("varName"),
      rebootModeFlag: processModeFlag,
      forceFlag: form.getFieldValue("forceFlag") ? 1 : 0,
      rebootWaitTime:
        processModeFlag === 1 ? form.getFieldValue("timeoutSec") : 0,
      timeout: form.getFieldValue("timeoutMin"),
    });
    graph.model.setTooltipLabel(cell, resetRebootTooltip(cell, t));
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
            {`${t("txt-reboot-icon")} (${jobnetIconId ? jobnetIconId + "/" : ""
              }${cell.jobId})`}
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
              <Col span={8}>
                <Form.Item name="rdoHost">
                  <Radio.Group onChange={hostFlagOnChange}>
                    <Radio value={0}>{t("txt-host-name")}</Radio>
                    <Radio value={1} className="mt-10x">
                      {t("txt-var")}
                    </Radio>
                  </Radio.Group>
                </Form.Item>
              </Col>
              <Col span={16}>
                <Form.Item
                  name="hostName"
                  rules={[
                    {
                      required: hostFlag === 0,
                      message: t("err-field-required-select", {
                        field: t("txt-host-name"),
                      }),
                    },
                  ]}
                >
                  <Select showSearch disabled={hostFlag !== 0}
                    filterOption={(input, option) =>
                      option.children.toLowerCase().indexOf(input.toLowerCase()) >= 0
                    }>
                    {hostList.map((host) => {
                      return (
                        <Option key={host.hostid} value={host.host}>
                          {host.host}
                        </Option>
                      );
                    })}
                  </Select>
                </Form.Item>
                <Form.Item
                  name="varName"
                  rules={[
                    {
                      whitespace: true,
                      required: hostFlag === 1,
                      message: t("err-field-required", { field: t("txt-var") }),
                    },
                    () => ({
                      validator(_, value) {
                        if (getShiftJISByteLength(value) > 128) {
                          return Promise.reject(
                            new Error(
                              t("err-msg-exceed-byte", {
                                field: t("txt-var"),
                                size: 128,
                              })
                            )
                          );
                        }
                        return Promise.resolve();
                      },
                    }),
                    () => ({
                      validator(_, value) {
                        if (!isHankakuStrAndUnderbarAndFirstNotNum(value)) {
                          return Promise.reject(
                            new Error(
                              t("err-msg-invalid-var", { field: t("txt-var") })
                            )
                          );
                        }
                        return Promise.resolve();
                      },
                    }),
                  ]}
                >
                  <Input disabled={hostFlag !== 1} className="mt-top-3px" />
                </Form.Item>
              </Col>
            </Row>
            <Row className="mt-top-2px">
              <Col span={6}>{t("txt-proc-mode")}</Col>
              <Col span={18}>
                <Form.Item name="processMode">
                  <Radio.Group onChange={processModeOnChange}>
                    <Radio value={0}>{t("txt-force-reboot")}</Radio>
                    <Radio value={1}>{t("txt-reboot-comp-job")}</Radio>
                  </Radio.Group>
                </Form.Item>
              </Col>
            </Row>
            <Row className="mt-top-2px">
              <Col span={10}>
                <Form.Item
                  name="timeoutSec"
                  label={t("txt-timeout-sec")}
                  rules={[
                    {
                      required: processModeFlag === 1,
                      message: t("err-field-required", {
                        field: t("txt-timeout-sec"),
                      }),
                    },
                  ]}
                >
                  <InputNumber
                    min="0"
                    max="9999"
                    disabled={processModeFlag === 0}
                  />
                </Form.Item>
              </Col>
              <Col span={14}>
                <Form.Item name="timeoutMin" label={t("lab-timeout")}>
                  <InputNumber min="0" max="99999" />
                </Form.Item>
              </Col>
            </Row>
            <Row className="mt-top-2px">
              <Col span={24}>
                <Form.Item name="forceFlag" valuePropName="checked">
                  <Checkbox disabled={formDisable}>
                    {t("btn-force-run")}
                  </Checkbox>
                </Form.Item>
              </Col>
            </Row>
          </Form>
        </div>
      </ResizableBox>
    </Modal>
  );
};

export default RebootIconDialog;
