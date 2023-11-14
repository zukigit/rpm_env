import { useState, useEffect, useRef } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  Tabs,
  InputNumber,
  Divider,
  Checkbox,
  Select,
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
import "./AgentLessIconDialog.scss";
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
import { resetAgentLessTooltip } from "../../../../views/jobnetForm/ArrangeIconData";

const { TabPane } = Tabs;
const { TextArea } = Input;
const { Option } = Select;

const AgentLessIconDialog = ({
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
  const [sessionFlag, setSessionFlag] = useState(0);
  const [authFlag, setAuthFlag] = useState(0);
  const [hostFlag, setHostFlag] = useState(0);
  const hostList =
    useSelector((state) => state.jobnetInitData.initData.host) || [];

  const [height, setHeight] = useState(500);
  const [width, setWidth] = useState(500);
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

    var hostType = 0;
    if (cell.iconSetting.hasOwnProperty("hostFlag")) {
      hostType = cell.iconSetting.hostFlag;
      setHostFlag(hostType);
    }

    if (cell.iconSetting.hasOwnProperty("sessionFlag")) {
      setSessionFlag(cell.iconSetting.sessionFlag);
    }

    if (cell.iconSetting.hasOwnProperty("authMethod")) {
      setAuthFlag(cell.iconSetting.authMethod);
    }

    form.setFieldsValue({
      jobId: cell.jobId,
      jobName: cell.jobName || "",
      sessionType: cell.iconSetting.hasOwnProperty("sessionFlag")
        ? cell.iconSetting.sessionFlag
        : 0,
      sessionId: cell.iconSetting.hasOwnProperty("sessionId")
        ? cell.iconSetting.sessionId
        : "",
      rdoHost: hostType,
      hostName: hostType === 0 ? cell.iconSetting.hostName || "" : "",
      varName: hostType === 1 ? cell.iconSetting.hostName || "" : "",
      authFlag: cell.iconSetting.hasOwnProperty("authMethod")
        ? cell.iconSetting.authMethod
        : 0,
      interactFlag: cell.iconSetting.hasOwnProperty("runMode")
        ? cell.iconSetting.runMode
        : 0,
      username: cell.iconSetting.hasOwnProperty("loginUser")
        ? cell.iconSetting.loginUser
        : "",
      password: cell.iconSetting.hasOwnProperty("loginPassword")
        ? cell.iconSetting.loginPassword
        : "",
      publicKey: cell.iconSetting.hasOwnProperty("publicKey")
        ? cell.iconSetting.publicKey
        : "",
      privateKey: cell.iconSetting.hasOwnProperty("privateKey")
        ? cell.iconSetting.privateKey
        : "",
      passPhrase: cell.iconSetting.hasOwnProperty("passPhrase")
        ? cell.iconSetting.passPhrase
        : "",
      exec: cell.iconSetting.hasOwnProperty("command")
        ? cell.iconSetting.command
        : "",
      prompt: cell.iconSetting.hasOwnProperty("promptString")
        ? cell.iconSetting.promptString
        : "",
      charCode: cell.iconSetting.hasOwnProperty("characterCode")
        ? cell.iconSetting.characterCode
        : "Unspecified",
      feedCode: cell.iconSetting.hasOwnProperty("lineFeedCode")
        ? cell.iconSetting.lineFeedCode
        : 0,
      timeout: cell.iconSetting.hasOwnProperty("timeout")
        ? cell.iconSetting.timeout
        : "0",
      stopCode: cell.iconSetting.hasOwnProperty("stopCode")
        ? cell.iconSetting.stopCode
        : "",
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

  const onSessionTypeChange = (e) => {
    setSessionFlag(form.getFieldValue("sessionType"));
  };

  const onAuthChange = (value) => {
    if (value === 0) {
      setAuthFlag(0);
    } else {
      setAuthFlag(1);
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

    var hostType = 0,
      hostName = "",
      sessionId = "",
      loginUser = "",
      loginPassword = "",
      publicKey = "",
      privateKey = "",
      passPhrase = "",
      exec = "",
      prompt = "",
      charCode = "Unspecified",
      feedCode = 0,
      timeout = 0,
      stopCode = "",
      forceFlag = 0;

    switch (form.getFieldValue("sessionType")) {
      case 0:
        hostType = form.getFieldValue("rdoHost");
        hostName =
          hostType === 0
            ? form.getFieldValue("hostName")
            : form.getFieldValue("varName");
        loginUser = form.getFieldValue("username");
        if (authFlag === 0) {
          loginPassword = form.getFieldValue("password");
        } else {
          privateKey = form.getFieldValue("privateKey");
          publicKey = form.getFieldValue("publicKey");
          passPhrase = form.getFieldValue("passPhrase");
        }
        exec = form.getFieldValue("exec");
        prompt = form.getFieldValue("prompt");
        charCode = form.getFieldValue("charCode");
        feedCode = form.getFieldValue("feedCode");
        timeout = form.getFieldValue("timeout");
        stopCode = form.getFieldValue("stopCode");
        forceFlag = form.getFieldValue("forceFlag");
        break;
      case 1:
        hostType = form.getFieldValue("rdoHost");
        hostName =
          hostType === 0
            ? form.getFieldValue("hostName")
            : form.getFieldValue("varName");
        sessionId = form.getFieldValue("sessionId");
        loginUser = form.getFieldValue("username");
        if (authFlag === 0) {
          loginPassword = form.getFieldValue("password");
        } else {
          privateKey = form.getFieldValue("privateKey");
          publicKey = form.getFieldValue("publicKey");
          passPhrase = form.getFieldValue("passPhrase");
        }
        exec = form.getFieldValue("exec");
        prompt = form.getFieldValue("prompt");
        charCode = form.getFieldValue("charCode");
        feedCode = form.getFieldValue("feedCode");
        timeout = form.getFieldValue("timeout");
        stopCode = form.getFieldValue("stopCode");
        forceFlag = form.getFieldValue("forceFlag");
        break;
      case 2:
        sessionId = form.getFieldValue("sessionId");
        exec = form.getFieldValue("exec");
        prompt = form.getFieldValue("prompt");
        timeout = form.getFieldValue("timeout");
        stopCode = form.getFieldValue("stopCode");
        forceFlag = form.getFieldValue("forceFlag");
        break;
      case 3:
        sessionId = form.getFieldValue("sessionId");
        forceFlag = form.getFieldValue("forceFlag");
        break;
    }
    graph.model.setIconSetting(cell, {
      jobId: form.getFieldValue("jobId"),
      jobName: form.getFieldValue("jobName"),
      sessionFlag: form.getFieldValue("sessionType"),
      sessionId: sessionId,
      connectionMethod: 0,
      hostFlag: hostType,
      hostName: hostName,
      authMethod: authFlag,
      runMode: form.getFieldValue("interactFlag"),
      loginUser: loginUser,
      loginPassword: loginPassword,
      publicKey: publicKey,
      privateKey: privateKey,
      passPhrase: passPhrase,
      command: exec,
      promptString: prompt,
      characterCode: charCode,
      lineFeedCode: feedCode,
      timeout: timeout,
      stopCode: stopCode,
      forceFlag: forceFlag ? 1 : 0,
    });
    graph.model.setTooltipLabel(cell, resetAgentLessTooltip(cell, t));
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
            {`${t("txt-agentless-icon")} (${jobnetIconId ? jobnetIconId + "/" : ""
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
            <div className="card-container">
              <Tabs type="card">
                <TabPane tab={t("txt-config")} key="1">
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
                                return Promise.reject(
                                  new Error(t("err-id-format"))
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
                              if (
                                validateIsIdAlreadyExist(value, cell.id, graph)
                              ) {
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
                  <Row>
                    <Col span={6}>{t("txt-session")}</Col>
                    <Col span={18}>
                      <Form.Item
                        name="sessionType"
                        rules={[
                          {
                            required: true,
                            message: t("err-field-required", {
                              field: t("txt-session"),
                            }),
                          },
                        ]}
                      >
                        <Radio.Group onChange={onSessionTypeChange}>
                          <Radio value={0}>{t("txt-one-time")}</Radio>
                          <Radio value={1}>{t("txt-connect")}</Radio>
                          <Radio value={2}>{t("txt-ctn2")}</Radio>
                          <Radio value={3}>{t("txt-disconnect")}</Radio>
                        </Radio.Group>
                      </Form.Item>
                    </Col>
                  </Row>
                  <Row>
                    <Col span={6}>{t("txt-session-id")}</Col>
                    <Col span={18}>
                      <Form.Item
                        name="sessionId"
                        rules={[
                          {
                            required: sessionFlag !== 0,
                            message: t("err-field-required", {
                              field: t("txt-session-id"),
                            }),
                          },
                          () => ({
                            validator(_, value) {
                              if (getShiftJISByteLength(value) > 64) {
                                return Promise.reject(
                                  new Error(
                                    t("err-msg-exceed-byte", {
                                      field: t("txt-session-id"),
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
                              var regex = new RegExp(
                                REGEX_PATTERM.MATCH_HANKAKU_HYPHEN_UNDERBAR
                              );
                              if (!regex.test(value)) {
                                return Promise.reject(
                                  new Error(t("err-session-vali"))
                                );
                              }
                              return Promise.resolve();
                            },
                          }),
                        ]}
                      >
                        <Input disabled={sessionFlag === 0} />
                      </Form.Item>
                    </Col>
                  </Row>
                  <Row className="mt-top-3px">
                    <Col span={8}>
                      <Form.Item name="rdoHost">
                        <Radio.Group
                          disabled={sessionFlag === 2 || sessionFlag === 3}
                          onChange={hostFlagOnChange}
                        >
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
                            required:
                              hostFlag === 0 &&
                              (sessionFlag === 0 || sessionFlag === 1),
                            message: t("err-field-required-select", {
                              field: t("txt-host-name"),
                            }),
                          },
                        ]}
                      >
                        <Select
                          showSearch
                          disabled={
                            hostFlag !== 0 ||
                            sessionFlag === 2 ||
                            sessionFlag === 3
                          }
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
                            required:
                              hostFlag === 1 &&
                              (sessionFlag === 0 || sessionFlag === 1),
                            message: t("err-field-required", {
                              field: t("txt-var"),
                            }),
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
                              if (
                                !isHankakuStrAndUnderbarAndFirstNotNum(value)
                              ) {
                                return Promise.reject(
                                  new Error(
                                    t("err-msg-invalid-var", {
                                      field: t("txt-var"),
                                    })
                                  )
                                );
                              }
                              return Promise.resolve();
                            },
                          }),
                        ]}
                      >
                        <Input
                          disabled={
                            hostFlag !== 1 ||
                            sessionFlag === 2 ||
                            sessionFlag === 3
                          }
                          className="mt-5x"
                        />
                      </Form.Item>
                    </Col>
                  </Row>
                  <Divider
                    style={{ backgroundColor: "rgb(114 109 109 / 20%)" }}
                  />
                  {"SSH"}
                  <Row>
                    <Col span={11}>
                      <Form.Item name="authFlag" label={t("txt-auth")}>
                        <Select
                          onChange={onAuthChange}
                          disabled={sessionFlag === 2 || sessionFlag === 3}
                        >
                          <Option value={0}>{t("txt-pwd")}</Option>
                          <Option value={1}>{t("txt-pub-key")}</Option>
                        </Select>
                      </Form.Item>
                    </Col>
                    <Col offset={1} span={11}>
                      <Form.Item name="interactFlag" label={t("txt-mode")}>
                        <Select
                          disabled={sessionFlag === 2 || sessionFlag === 3}
                        >
                          <Option value={0}>{t("txt-interact")}</Option>
                          <Option value={1}>{t("txt-not-interact")}</Option>
                        </Select>
                      </Form.Item>
                    </Col>
                  </Row>
                  <Row className="mt-top-2px">
                    <Col span={6}>{t("lab-user-name")}</Col>
                    <Col span={18}>
                      <Form.Item
                        name="username"
                        rules={[
                          {
                            whitespace: true,
                            required: sessionFlag === 0 || sessionFlag === 1,
                            message: t("err-field-required", {
                              field: t("lab-user-name"),
                            }),
                          },
                          () => ({
                            validator(_, value) {
                              if (getShiftJISByteLength(value) > 256) {
                                return Promise.reject(
                                  new Error(
                                    t("err-msg-exceed-byte", {
                                      field: t("lab-user-name"),
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
                        <Input
                          disabled={sessionFlag === 2 || sessionFlag === 3}
                        />
                      </Form.Item>
                    </Col>
                  </Row>
                  <Row className="mt-top-2px">
                    <Col span={6}>{t("txt-pwd")}</Col>
                    <Col span={18}>
                      <Form.Item
                        name="password"
                        rules={[
                          {
                            whitespace: true,
                            required:
                              (sessionFlag === 0 || sessionFlag === 1) &&
                              authFlag === 0,
                            message: t("err-field-required", {
                              field: t("txt-pwd"),
                            }),
                          },
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
                        ]}
                      >
                        <Input.Password
                          disabled={
                            sessionFlag === 2 ||
                            sessionFlag === 3 ||
                            (authFlag === 1 &&
                              sessionFlag !== 2 &&
                              sessionFlag !== 3)
                          }
                          autoComplete="on"
                          visibilityToggle={!formDisable}
                        />
                      </Form.Item>
                    </Col>
                  </Row>
                  <Row className="mt-top-2px">
                    <Col span={6}>{t("txt-pub-key")}</Col>
                    <Col span={18}>
                      <Form.Item
                        name="publicKey"
                        rules={[
                          {
                            whitespace: true,
                            required:
                              (sessionFlag === 0 || sessionFlag === 1) &&
                              authFlag === 1,
                            message: t("err-field-required", {
                              field: t("txt-pub-key"),
                            }),
                          },
                          () => ({
                            validator(_, value) {
                              if (getShiftJISByteLength(value) > 2048) {
                                return Promise.reject(
                                  new Error(
                                    t("err-msg-exceed-byte", {
                                      field: t("txt-pub-key"),
                                      size: 2048,
                                    })
                                  )
                                );
                              }
                              return Promise.resolve();
                            },
                          }),
                          () => ({
                            validator(_, value) {
                              var regexPW = new RegExp(
                                REGEX_PATTERM.MATCH_ASCII
                              );
                              if (!regexPW.test(value)) {
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
                        ]}
                      >
                        <Input
                          disabled={
                            sessionFlag === 2 ||
                            sessionFlag === 3 ||
                            (authFlag === 0 &&
                              sessionFlag !== 2 &&
                              sessionFlag !== 3)
                          }
                        />
                      </Form.Item>
                    </Col>
                  </Row>
                  <Row className="mt-top-2px">
                    <Col span={6}>{t("txt-prv-key")}</Col>
                    <Col span={18}>
                      <Form.Item
                        name="privateKey"
                        rules={[
                          {
                            whitespace: true,
                            required:
                              (sessionFlag === 0 || sessionFlag === 1) &&
                              authFlag === 1,
                            message: t("err-field-required", {
                              field: t("txt-prv-key"),
                            }),
                          },
                          () => ({
                            validator(_, value) {
                              if (getShiftJISByteLength(value) > 2048) {
                                return Promise.reject(
                                  new Error(
                                    t("err-msg-exceed-byte", {
                                      field: t("txt-prv-key"),
                                      size: 2048,
                                    })
                                  )
                                );
                              }
                              return Promise.resolve();
                            },
                          }),
                          () => ({
                            validator(_, value) {
                              var regexPW = new RegExp(
                                REGEX_PATTERM.MATCH_ASCII
                              );
                              if (!regexPW.test(value)) {
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
                        ]}
                      >
                        <Input
                          disabled={
                            sessionFlag === 2 ||
                            sessionFlag === 3 ||
                            (authFlag === 0 &&
                              sessionFlag !== 2 &&
                              sessionFlag !== 3)
                          }
                        />
                      </Form.Item>
                    </Col>
                  </Row>
                  <Row className="mt-top-2px">
                    <Col span={6}>{t("txt-passpharse")}</Col>
                    <Col span={18}>
                      <Form.Item
                        name="passPhrase"
                        rules={[
                          () => ({
                            validator(_, value) {
                              if (getShiftJISByteLength(value) > 256) {
                                return Promise.reject(
                                  new Error(
                                    t("err-msg-exceed-byte", {
                                      field: t("txt-passpharse"),
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
                        <Input
                          disabled={
                            sessionFlag === 2 ||
                            sessionFlag === 3 ||
                            (authFlag === 0 &&
                              sessionFlag !== 2 &&
                              sessionFlag !== 3)
                          }
                        />
                      </Form.Item>
                    </Col>
                  </Row>
                </TabPane>
                <TabPane tab={t("txt-cmd")} key="2">
                  {t("txt-exec")}
                  <Form.Item
                    name="exec"
                    rules={[
                      () => ({
                        validator(_, value) {
                          if (getShiftJISByteLength(value) > 4000) {
                            return Promise.reject(
                              new Error(
                                t("err-msg-exceed-byte", {
                                  field: t("txt-cmd"),
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
                    <TextArea disabled={sessionFlag === 3} rows={8} />
                  </Form.Item>
                  {t("txt-prompt")}
                  <Form.Item
                    name="prompt"
                    rules={[
                      () => ({
                        validator(_, value) {
                          if (getShiftJISByteLength(value) > 256) {
                            return Promise.reject(
                              new Error(
                                t("err-msg-exceed-byte", {
                                  field: t("txt-prompt"),
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
                    <Input disabled={sessionFlag === 3} />
                  </Form.Item>
                  <Row className="mt-top-2px">
                    <Col span={11}>
                      <Form.Item name="charCode" label={t("txt-char-code")}>
                        <Select
                          disabled={sessionFlag === 2 || sessionFlag === 3}
                        >
                          <Option value="UNSPECIFIED">
                            {t("txt-unspecified")}
                          </Option>
                          <Option value="UTF-8">{"UTF-8"}</Option>
                          <Option value="SHIFT-JIS">{"SHIFT-JIS"}</Option>
                          <Option value="EUC-JP">{"EUC-JP"}</Option>
                        </Select>
                      </Form.Item>
                    </Col>
                    <Col offset={1} span={11}>
                      <Form.Item name="feedCode" label={t("txt-lfc")}>
                        <Select
                          disabled={sessionFlag === 2 || sessionFlag === 3}
                        >
                          <Option value={0}>{"LF"}</Option>
                          <Option value={1}>{"CR"}</Option>
                          <Option value={2}>{"CRLF"}</Option>
                        </Select>
                      </Form.Item>
                    </Col>
                  </Row>
                  <Row className="mt-top-2px">
                    <Col span={11}>
                      <Form.Item name="timeout" label={t("txt-timeout-min")}>
                        <InputNumber
                          disabled={sessionFlag === 3}
                          style={{ width: "100%" }}
                          min="0"
                          max="99999"
                        />
                      </Form.Item>
                    </Col>
                    <Col offset={1} span={11}>
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
                        <Input
                          disabled={sessionFlag === 3}
                          style={{ width: "100%" }}
                        />
                      </Form.Item>
                    </Col>
                  </Row>
                  <Row>
                    <Col span={24}>
                      <Form.Item name="forceFlag" valuePropName="checked">
                        <Checkbox disabled={formDisable}>
                          {t("btn-force-run")}
                        </Checkbox>
                      </Form.Item>
                    </Col>
                  </Row>
                </TabPane>
              </Tabs>
            </div>
          </Form>
        </div>
      </ResizableBox>
    </Modal>
  );
};

export default AgentLessIconDialog;
