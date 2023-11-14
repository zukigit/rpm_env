import { useState, useEffect, useRef } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  Radio,
  Modal,
  Form,
  Input,
  InputNumber,
  Select,
  Popconfirm,
  Checkbox,
  Button,
  Row,
  Col,
} from "antd";
import { MinusOutlined, ExpandOutlined } from "@ant-design/icons";
import { removeOpenDialog } from "../../../../store/JobnetFormSlice";
import EditableTable from "../../../tables/editableTable/EditableTable";
import { useTranslation } from "react-i18next";
import {
  getAvailableHosts,
  getDefineValueJobCon,
} from "../../../../store/JobnetInitDataSlice";
import { getNextZIndex } from "../../../../store/JobnetFormSlice";
import { ResizableBox } from "react-resizable";
import Draggable from "react-draggable";
import "./JobIconDialog.scss";
import {
  getExecIconSettingNextZIndex,
  removeExecOpenDialog,
} from "../../../../store/JobExecutionSlice";
import { INVALID_STRING, REGEX_PATTERM } from "../../../../constants";
import { getShiftJISByteLength } from "../../../../common/Util";
import {
  isHankaku,
  isHankakuStrAndUnderbarAndFirstNotNum,
  isImpossibleStr,
  validateIsIdAlreadyExist,
} from "../../../../views/jobnetForm/Validation";
import { resetJobIconTooltip } from "../../../../views/jobnetForm/ArrangeIconData";
import { alertError } from "../../CommonDialog";
const { Option } = Select;
const { TextArea } = Input;

const JobIconDialog = ({
  id,
  graph,
  visible = true,
  cell,
  graphIndexId,
  jobnetIconId = null,
  formDisable = false,
  innerJobnetId = null,
}) => {
  const [form] = Form.useForm();
  const { t } = useTranslation();
  const dispatch = useDispatch();

  const hostList =
    useSelector((state) => state.jobnetInitData.initData.host) || [];
  const defineValueJobConList =
    useSelector((state) => state.jobnetInitData.initData.defineValueJobCon) ||
    [];
  const [hostFlag, setHostFlag] = useState(0);
  const [stopCommandCheck, setStopCommandCheck] = useState(false);

  const [jobVariable, setJobVariable] = useState([]);
  const [count, setCount] = useState(0);

  const [height, setHeight] = useState(540);
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

  const defaultColumns = [
    {
      title: t("txt-var-name"),
      dataIndex: "valueName",
      width: "30%",
      editable: true,
    },
    {
      title: t("txt-val"),
      dataIndex: "value",
      editable: true,
    },
    {
      title: t("col-obj-action"),
      dataIndex: "operation",
      width: "20%",
      render: (_, record) =>
        jobVariable.length >= 1 ? (
          <Popconfirm
            title="Sure to delete?"
            onConfirm={() => handleDeleteJobVariable(record.key)}
          >
            <a>Delete</a>
          </Popconfirm>
        ) : null,
    },
  ];

  useEffect(() => {
    dispatch(getAvailableHosts());
    dispatch(getDefineValueJobCon());
    let isHost = 0,
      stopCodeFlag = false,
      exec = "";
    if (cell.iconSetting.hasOwnProperty("hostFlag")) {
      isHost = cell.iconSetting.hostFlag;
      setHostFlag(isHost);
    }

    if (cell.iconSetting.hasOwnProperty("stopFlag")) {
      if (cell.iconSetting.stopFlag == 1) {
        stopCodeFlag = true;
        setStopCommandCheck(true);
      }
    }

    if (cell.iconSetting.hasOwnProperty("exec")) {
      exec = cell.iconSetting.exec;
    }

    if (cell.iconSetting.hasOwnProperty("valueJob")) {
      if (cell.iconSetting.valueJob.length > 0) {
        setCount(
          cell.iconSetting.valueJob[cell.iconSetting.valueJob.length - 1].key +
          1
        );
      }
      setJobVariable(cell.iconSetting.valueJob);
    }

    form.setFieldsValue({
      jobId: cell.jobId,
      jobName: cell.jobName || "",
      rdoHost: isHost,
      hostName: isHost === 0 ? cell.iconSetting.hostName || "" : "",
      varName: isHost === 1 ? cell.iconSetting.hostName || "" : "",
      stopCommandFlag: stopCodeFlag,
      stopCommand: stopCodeFlag ? cell.iconSetting.stopCommand || "" : "",
      exec: cell.iconSetting.hasOwnProperty("exec")
        ? cell.iconSetting.exec
        : "",
      jobVar: jobVariable,
      jobConVar: cell.iconSetting.hasOwnProperty("valueJobCon")
        ? cell.iconSetting.valueJobCon
        : [],
      stopCode: cell.iconSetting.hasOwnProperty("stopCode")
        ? cell.iconSetting.stopCode
        : "",
      continueFlag: cell.iconSetting.hasOwnProperty("continueFlag")
        ? cell.iconSetting.continueFlag
        : false,
      forceFlag: cell.iconSetting.hasOwnProperty("forceFlag")
        ? cell.iconSetting.forceFlag
        : false,
      timeout: cell.iconSetting.hasOwnProperty("timeout")
        ? cell.iconSetting.timeout
        : 0,
      timeoutType: cell.iconSetting.hasOwnProperty("timeoutRunType")
        ? cell.iconSetting.timeoutRunType
        : "0",
      runUser: cell.iconSetting.hasOwnProperty("runUser")
        ? cell.iconSetting.runUser
        : "",
      runUserPassword: cell.iconSetting.hasOwnProperty("runUserPassword")
        ? cell.iconSetting.runUserPassword
        : "",
    });
  }, [dispatch, getAvailableHosts]);

  const handleDeleteJobVariable = (key) => {
    const newData = jobVariable.filter((item) => item.key !== key);
    setJobVariable(newData);
  };

  const handleSave = (row) => {
    const newData = [...jobVariable];
    const index = newData.findIndex((item) => row.key === item.key);
    const item = newData[index];
    if (!alreadyExists(row, newData)) {
      newData.splice(index, 1, { ...item, ...row });
      setJobVariable(newData);
    }
  };

  function alreadyExists(row, newData) {
    var duplicate = false;
    if (newData) {
      newData.forEach((obj) => {
        if (row.key !== obj["key"] && obj["valueName"] === row.valueName) {
          duplicate = true;
        }
      });
    }
    return duplicate;
  }
  const handleAdd = () => {
    if (!form.getFieldValue("jobVarName")) {
      alertError(
        t("title-error"),
        t("err-field-required", { field: t("txt-var-name") })
      );
      return;
    }

    if (
      !isHankakuStrAndUnderbarAndFirstNotNum(form.getFieldValue("jobVarName"))
    ) {
      alertError(
        t("title-error"),
        t("err-msg-invalid-var", { field: t("txt-var-name") })
      );
      return;
    }

    if (getShiftJISByteLength(form.getFieldValue("jobVarName")) > 128) {
      alertError(
        t("title-error"),
        t("err-msg-exceed-byte", { field: t("txt-var-name") })
      );
      return;
    }

    if (!form.getFieldValue("jobVarValue")) {
      alertError(
        t("title-error"),
        t("err-field-required", { field: t("txt-val") })
      );
      return;
    }

    if (getShiftJISByteLength(form.getFieldValue("jobVarValue")) > 4000) {
      alertError(
        t("title-error"),
        t("err-msg-exceed-byte", { field: t("txt-val") })
      );
      return;
    }

    const newData = {
      key: count,
      valueName: form.getFieldValue("jobVarName"),
      value: form.getFieldValue("jobVarValue"),
    };
    const data = jobVariable.filter(
      (item) => item.valueName !== form.getFieldValue("jobVarName")
    );
    form.setFieldsValue({ jobVarName: "" });
    form.setFieldsValue({ jobVarValue: "" });
    setJobVariable([...data, newData]);
    setCount(count + 1);
  };

  const onCancel = (e) => {
    if (innerJobnetId === null) {
      dispatch(removeOpenDialog({ id, graphIndexId }));
    } else {
      dispatch(removeExecOpenDialog({ id, innerJobnetId }));
    }
  };

  const layout = {
    labelCol: {
      span: 6,
    },
    wrapperCol: {
      span: 18,
    },
  };

  const hostFlagOnChange = (e) => {
    setHostFlag(e.target.value);
  };

  const stopCommandChange = (e) => {
    var dd = form.getFieldValue("stopCommandFlag");
    setStopCommandCheck(e.target.checked);
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
      hostFlag: form.getFieldValue("rdoHost"),
      hostName:
        form.getFieldValue("rdoHost") === 0
          ? form.getFieldValue("hostName")
          : form.getFieldValue("varName"),
      stopFlag: form.getFieldValue("stopCommandFlag") ? 1 : 0,
      stopCommand: form.getFieldValue("stopCommandFlag")
        ? form.getFieldValue("stopCommand")
        : null,
      exec: form.getFieldValue("exec"),
      valueJob: jobVariable,
      valueJobCon: form.getFieldValue("jobConVar"),
      stopCode: form.getFieldValue("stopCode"),
      forceFlag: form.getFieldValue("forceFlag") ? 1 : 0,
      continueFlag: form.getFieldValue("continueFlag") ? 1 : 0,
      timeout: parseInt(form.getFieldValue("timeout")) || 0,
      timeoutRunType: form.getFieldValue("timeoutType") || 0,
      runUser: form.getFieldValue("runUser") || "",
      runUserPassword: form.getFieldValue("runUserPassword") || "",
    });
    graph.model.setTooltipLabel(cell, resetJobIconTooltip(cell, t));
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
            }}
          >
            {`${t("txt-job-icon")} (${jobnetIconId ? jobnetIconId + "/" : ""}${cell.jobId
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
      wrapClassName="ant-multiple-modal-wrap"
      className={isMinimize && "hide-body-modal"}
      mask={false}
      zIndex={cell.zindex}
      maskClosable={false}
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
        minConstraints={[200, 200]}
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
            className="icon-dialog-form"
            size={"small"}
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
            <Row>
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
                    })
                  ]}
                >
                  <Input maxLength={64} />
                </Form.Item>
              </Col>
            </Row>
            <Row className="mt-top-3px">
              <Col span={8}>
                <Form.Item name="rdoHost">
                  <Radio.Group onChange={hostFlagOnChange}>
                    <Radio value={0}>{t("txt-host-name")}</Radio>
                    <Radio value={1} className="mt-6x">
                      {t("txt-var")}
                    </Radio>
                  </Radio.Group>
                </Form.Item>
              </Col>
              <Col span={16}>
                <Form.Item
                  name="hostName"
                  rules={[
                    ({ getFieldValue }) => ({
                      validator(_, value) {
                        if (getFieldValue("rdoHost") === 0 && !value) {
                          return Promise.reject(
                            new Error(
                              t("err-field-required-select", {
                                field: t("txt-host-name"),
                              })
                            )
                          );
                        }
                        return Promise.resolve();
                      },
                    }),
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
                    ({ getFieldValue }) => ({
                      validator(_, value) {
                        if (getFieldValue("rdoHost") === 1 && !value) {
                          return Promise.reject(
                            new Error(
                              t("err-field-required", { field: t("txt-var") })
                            )
                          );
                        }
                        return Promise.resolve();
                      },
                    }),
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
                  <Input
                    maxLength={128}
                    disabled={hostFlag === 0}
                    className="mt-top-3px"
                  />
                </Form.Item>
              </Col>
            </Row>
            <Row>
              <Col span={8}>
                <Form.Item name="stopCommandFlag" valuePropName="checked">
                  <Checkbox onChange={stopCommandChange} disabled={formDisable}>
                    {t("txt-stop-cmd")}
                  </Checkbox>
                </Form.Item>
              </Col>
              <Col span={16}>
                <Form.Item
                  name="stopCommand"
                  rules={[
                    ({ getFieldValue }) => ({
                      validator(_, value) {
                        if (getFieldValue("stopCommandFlag") && !value) {
                          return Promise.reject(
                            new Error(
                              t("err-field-required", {
                                field: t("txt-stop-cmd"),
                              })
                            )
                          );
                        }
                        return Promise.resolve();
                      },
                    }),
                    () => ({
                      validator(_, value) {
                        var regex = new RegExp(REGEX_PATTERM.MATCH_ASCII);
                        if (!regex.test(value)) {
                          return Promise.reject(
                            new Error(
                              t("err-msg-ascii-required", {
                                field: t("txt-stop-cmd"),
                              })
                            )
                          );
                        }
                        return Promise.resolve();
                      },
                    }),
                    () => ({
                      validator(_, value) {
                        if (getShiftJISByteLength(value) > 4000) {
                          return Promise.reject(
                            new Error(
                              t("err-msg-exceed-byte", {
                                field: t("txt-stop-cmd"),
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
                  <Input maxLength={4000} disabled={!stopCommandCheck} />
                </Form.Item>
              </Col>
            </Row>
            {t("txt-exec")}
            <Form.Item
              name="exec"
              rules={[
                {
                  whitespace: true,
                  required: true,
                  message: t("err-field-required", { field: t("txt-exec") }),
                },
                () => ({
                  validator(_, value) {
                    if (getShiftJISByteLength(value) > 4000) {
                      return Promise.reject(
                        new Error(
                          t("err-msg-exceed-byte", {
                            field: t("txt-exec"),
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
              <TextArea maxLength={4000} rows={4} />
            </Form.Item>
            {t("txt-job-var")}
            <EditableTable
              defaultColumns={defaultColumns}
              dataSource={jobVariable}
              handleSave={handleSave}
              tableHeight={"80px"}
            />

            <Row className="mt-3x">
              <Col span={9}>
                <Form.Item name="jobVarName">
                  <Input
                    placeholder="Variable name"
                    maxLength={128}
                    type="text"
                  />
                </Form.Item>
              </Col>
              <Col offset={1} span={9}>
                <Form.Item name="jobVarValue">
                  <Input placeholder="Value" maxLength={4000} type="text" />
                </Form.Item>
              </Col>
              <Col offset={1} span={4}>
                <Form.Item>
                  <Button size={"small"} onClick={handleAdd}>
                    Add
                  </Button>
                </Form.Item>
              </Col>
            </Row>
            {t("txt-job-ctr-var")}
            <Form.Item name="jobConVar">
              <Select mode="multiple">
                {defineValueJobConList.map((defineValueJobCon) => {
                  return (
                    <Option
                      key={defineValueJobCon.value_name}
                      value={defineValueJobCon.value_name}
                    >
                      {defineValueJobCon.value_name}
                    </Option>
                  );
                })}
              </Select>
            </Form.Item>
            <Row className="mt-top-2px">
              <Col span={12}>
                <Form.Item
                  name="stopCode"
                  label={t("txt-job-stop-code")}
                  rules={[
                    () => ({
                      validator(_, value) {
                        var regex = new RegExp(
                          REGEX_PATTERM.MATCH_HANKAKU_COMMA_HYPHEN
                        );
                        if (!regex.test(value)) {
                          return Promise.reject(
                            new Error(
                              t("err-msg-invalid-input", {
                                field: t("txt-job-stop-code"),
                              })
                            )
                          );
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
                                field: t("txt-job-stop-code"),
                                size: 32,
                              })
                            )
                          );
                        }
                        return Promise.resolve();
                      },
                    }),
                  ]}
                >
                  <Input />
                </Form.Item>
              </Col>
              <Col offset={1} span={5}>
                <Form.Item name="forceFlag" valuePropName="checked">
                  <Checkbox disabled={formDisable}>
                    {t("btn-force-run")}
                  </Checkbox>
                </Form.Item>
              </Col>
              <Col offset={1} span={5}>
                <Form.Item name="continueFlag" valuePropName="checked">
                  <Checkbox disabled={formDisable}>{t("txt-ctn1")}</Checkbox>
                </Form.Item>
              </Col>
            </Row>
            <Row className="mt-top-2px">
              <Col span={9}>{t("lab-timeout")}</Col>
              <Col span={6}>
                <Form.Item
                  name="timeout"
                  rules={[
                    {
                      required: true,
                      message: t("err-field-required", {
                        field: t("lab-timeout"),
                      }),
                    },
                  ]}
                >
                  <InputNumber min={0} max={99999} />
                </Form.Item>
              </Col>
              <Col offset={1} span={8}>
                <Form.Item name="timeoutType">
                  <Select>
                    <Option value="0">{t("sel-warning")}</Option>
                    <Option value="1">{t("txt-job-stop")}</Option>
                  </Select>
                </Form.Item>
              </Col>
            </Row>
            <Row className="mt-top-2px">
              <Col span={6}>{t("txt-run-usr")}</Col>
              <Col span={18}>
                <Form.Item
                  name="runUser"
                  rules={[
                    () => ({
                      validator(_, value) {
                        var regexPW = new RegExp(
                          REGEX_PATTERM.PROHIBITED_CHARACTER_USER_NAME
                        );
                        if (regexPW.test(value)) {
                          return Promise.reject(new Error(t("err-msg-usr")));
                        }
                        return Promise.resolve();
                      },
                    }),
                    () => ({
                      validator(_, value) {
                        if (getShiftJISByteLength(value) > 256) {
                          return Promise.reject(
                            new Error(
                              t("err-msg-exceed-byte", {
                                field: t("txt-run-usr"),
                                size: 256,
                              })
                            )
                          );
                        }
                        return Promise.resolve();
                      },
                    }),
                    ({ getFieldValue }) => ({
                      validator(_, value) {
                        if (getFieldValue("runUserPassword")) {
                          if (value.length === 0) {
                            return Promise.reject(
                              new Error(
                                t("err-field-required", {
                                  field: t("txt-run-usr"),
                                })
                              )
                            );
                          }
                        }
                        return Promise.resolve();
                      },
                    }),
                  ]}
                >
                  <Input maxLength={256} />
                </Form.Item>
              </Col>
            </Row>
            <Row className="mt-top-2px">
              <Col span={6}>{t("txt-pwd")}</Col>
              <Col span={18}>
                <Form.Item
                  name="runUserPassword"
                  rules={[
                    () => ({
                      validator(_, value) {
                        if (!isHankaku(value)) {
                          return Promise.reject(new Error(t("err-msg-pass")));
                        }
                        return Promise.resolve();
                      },
                    }),
                    () => ({
                      validator(_, value) {
                        var regexPW = new RegExp(
                          REGEX_PATTERM.PROHIBITED_CHARACTER_USER_PW
                        );
                        if (regexPW.test(value)) {
                          return Promise.reject(
                            new Error(t("err-run-pw-format"))
                          );
                        }
                        return Promise.resolve();
                      },
                    }),
                    () => ({
                      validator(_, value) {
                        if (getShiftJISByteLength(value) > 256) {
                          return Promise.reject(
                            new Error(
                              t("err-msg-exceed-byte", {
                                field: t("txt-pwd"),
                                size: 256,
                              })
                            )
                          );
                        }
                        return Promise.resolve();
                      },
                    }),
                  ]}
                >
                  <Input.Password
                    autoComplete="new-password"
                    maxLength={256}
                    visibilityToggle={!formDisable}
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

export default JobIconDialog;
