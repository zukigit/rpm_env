import { useState, useEffect, useRef } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  Divider,
  Select,
  Checkbox,
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
import { getAvailableHosts } from "../../../../store/JobnetInitDataSlice";
import { useTranslation } from "react-i18next";
import "./FileTransferIconDialog.scss";
import {
  getExecIconSettingNextZIndex,
  removeExecOpenDialog,
} from "../../../../store/JobExecutionSlice";
import {
  isHankakuStrAndUnderbarAndFirstNotNum,
  isImpossibleStr,
  validateIsIdAlreadyExist,
} from "../../../../views/jobnetForm/Validation";
import { INVALID_STRING, REGEX_PATTERM } from "../../../../constants";
import { getShiftJISByteLength } from "../../../../common/Util";
import { resetFileTransferTooltip } from "../../../../views/jobnetForm/ArrangeIconData";

const { Option } = Select;

const FileTransferIconDialog = ({
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
  const [sourceHostFlag, setSourceHostFlag] = useState(0);
  const [destinationHostFlag, setDestinationHostFlag] = useState(0);

  const hostList =
    useSelector((state) => state.jobnetInitData.initData.host) || [];

  const [height, setHeight] = useState(420);
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

    var sourceHostFlag = 0,
      destinationHostFlag = 0;
    if (cell.iconSetting.hasOwnProperty("fromHostFlag")) {
      sourceHostFlag = cell.iconSetting.fromHostFlag;
      setSourceHostFlag(sourceHostFlag);
    }
    if (cell.iconSetting.hasOwnProperty("toHostFlag")) {
      destinationHostFlag = cell.iconSetting.toHostFlag;
      setDestinationHostFlag(destinationHostFlag);
    }

    form.setFieldsValue({
      jobId: cell.jobId,
      jobName: cell.jobName || "",
      rdoSource: sourceHostFlag,
      sourceHostName:
        sourceHostFlag === 0 ? cell.iconSetting.fromHostName || "" : "",
      sourceVarName:
        sourceHostFlag === 1 ? cell.iconSetting.fromHostName || "" : "",
      rdoDestination: destinationHostFlag,
      destinationHostName:
        destinationHostFlag === 0 ? cell.iconSetting.toHostName || "" : "",
      destinationVarName:
        destinationHostFlag === 1 ? cell.iconSetting.toHostName || "" : "",
      sourceDirectory: cell.iconSetting.hasOwnProperty("fromDirectory")
        ? cell.iconSetting.fromDirectory
        : "",
      destinationDirectory: cell.iconSetting.hasOwnProperty("toDirectory")
        ? cell.iconSetting.toDirectory
        : "",
      sourceFile: cell.iconSetting.hasOwnProperty("fromFileName")
        ? cell.iconSetting.fromFileName
        : "",
      overwriteFlag: cell.iconSetting.hasOwnProperty("overwriteFlag")
        ? cell.iconSetting.overwriteFlag
        : false,
      forceFlag: cell.iconSetting.hasOwnProperty("forceFlag")
        ? cell.iconSetting.forceFlag
        : false,
    });
  }, [dispatch]);

  const sourceHostFlagOnChange = (e) => {
    if (e.target.value === 0) {
      setSourceHostFlag(0);
    } else {
      setSourceHostFlag(1);
    }
  };

  const destinationHostFlagOnChange = (e) => {
    if (e.target.value === 0) {
      setDestinationHostFlag(0);
    } else {
      setDestinationHostFlag(1);
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
      fromHostFlag: form.getFieldValue("rdoSource"),
      toHostFlag: form.getFieldValue("rdoDestination"),
      fromHostName:
        form.getFieldValue("rdoSource") === 0
          ? form.getFieldValue("sourceHostName")
          : form.getFieldValue("sourceVarName"),
      toHostName:
        form.getFieldValue("rdoDestination") === 0
          ? form.getFieldValue("destinationHostName")
          : form.getFieldValue("destinationVarName"),
      overwriteFlag: form.getFieldValue("overwriteFlag") ? 1 : 0,
      forceFlag: form.getFieldValue("forceFlag") ? 1 : 0,
      fromDirectory: form.getFieldValue("sourceDirectory"),
      toDirectory: form.getFieldValue("destinationDirectory"),
      fromFileName: form.getFieldValue("sourceFile"),
    });
    graph.model.setTooltipLabel(cell, resetFileTransferTooltip(cell, t));
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
            {`${t("txt-transfer-icon")} (${jobnetIconId ? jobnetIconId + "/" : ""
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
              <Col span={8}>
                <Form.Item name="rdoSource">
                  <Radio.Group onChange={sourceHostFlagOnChange}>
                    <Radio value={0}>{t("txt-src-host-name")}</Radio>
                    <Radio value={1} className="mt-top-2px">
                      {t("txt-src-var")}
                    </Radio>
                  </Radio.Group>
                </Form.Item>
              </Col>
              <Col span={16}>
                <Form.Item
                  name="sourceHostName"
                  rules={[
                    {
                      required: sourceHostFlag === 0,
                      message: t("err-field-required-select", {
                        field: t("txt-src-host-name"),
                      }),
                    },
                  ]}
                >
                  <Select
                    showSearch
                    disabled={sourceHostFlag !== 0}
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
                  name="sourceVarName"
                  rules={[
                    {
                      whitespace: true,
                      required: sourceHostFlag === 1,
                      message: t("err-field-required", {
                        field: t("txt-src-var"),
                      }),
                    },
                    () => ({
                      validator(_, value) {
                        if (getShiftJISByteLength(value) > 128) {
                          return Promise.reject(
                            new Error(
                              t("err-msg-exceed-byte", {
                                field: t("txt-src-var"),
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
                              t("err-msg-invalid-var", {
                                field: t("txt-src-var"),
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
                    maxLength={128}
                    disabled={sourceHostFlag !== 1}
                    className="mt-top-3px"
                  />
                </Form.Item>
              </Col>
            </Row>
            <Row className="mt-top-3px">
              <Col span={8}>{t("txt-src-dir")}</Col>
              <Col span={16}>
                <Form.Item
                  name="sourceDirectory"
                  rules={[
                    {
                      whitespace: true,
                      required: true,
                      message: t("err-field-required", {
                        field: t("txt-src-dir"),
                      }),
                    },
                    () => ({
                      validator(_, value) {
                        if (getShiftJISByteLength(value) > 1024) {
                          return Promise.reject(
                            new Error(
                              t("err-msg-exceed-byte", {
                                field: t("txt-src-dir"),
                                size: 1024,
                              })
                            )
                          );
                        }
                        return Promise.resolve();
                      },
                    }),
                  ]}
                >
                  <Input maxLength={1024} />
                </Form.Item>
              </Col>
            </Row>
            <Row>
              <Col span={8}>{t("txt-src-file-name")}</Col>
              <Col span={16}>
                <Form.Item
                  name="sourceFile"
                  rules={[
                    {
                      whitespace: true,
                      required: true,
                      message: t("err-field-required", {
                        field: t("txt-src-file-name"),
                      }),
                    },
                    () => ({
                      validator(_, value) {
                        if (getShiftJISByteLength(value) > 1024) {
                          return Promise.reject(
                            new Error(
                              t("err-msg-exceed-byte", {
                                field: t("txt-src-file-name"),
                                size: 1024,
                              })
                            )
                          );
                        }
                        return Promise.resolve();
                      },
                    }),
                  ]}
                >
                  <Input maxLength={1024} />
                </Form.Item>
              </Col>
            </Row>
            <Divider />
            <Row>
              <Col span={8}>
                <Form.Item name="rdoDestination">
                  <Radio.Group onChange={destinationHostFlagOnChange}>
                    <Radio value={0}>{t("txt-dest-host-name")}</Radio>
                    <Radio value={1} className="mt-top-3px">
                      {t("txt-dest-var")}
                    </Radio>
                  </Radio.Group>
                </Form.Item>
              </Col>
              <Col span={16}>
                <Form.Item
                  name="destinationHostName"
                  rules={[
                    {
                      required: destinationHostFlag === 0,
                      message: t("err-field-required-select", {

                        field: t("txt-dest-host-name"),
                      }),
                    },
                  ]}
                >
                  <Select
                    showSearch
                    disabled={destinationHostFlag !== 0}
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
                  name="destinationVarName"
                  rules={[
                    {
                      required: destinationHostFlag === 1,
                      message: t("err-field-required", {
                        field: t("txt-dest-var"),
                      }),
                    },
                    () => ({
                      validator(_, value) {
                        if (getShiftJISByteLength(value) > 128) {
                          return Promise.reject(
                            new Error(
                              t("err-msg-exceed-byte", {
                                field: t("txt-dest-var"),
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
                              t("err-msg-invalid-var", {
                                field: t("txt-dest-var"),
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
                    disabled={destinationHostFlag !== 1}
                    className="mt-top-3px"
                  />
                </Form.Item>
              </Col>
            </Row>
            <Row>
              <Col span={8}>{t("txt-dest-dir")}</Col>
              <Col span={16}>
                <Form.Item
                  name="destinationDirectory"
                  rules={[
                    {
                      whitespace: true,
                      required: true,
                      message: t("err-field-required", {
                        field: t("txt-dest-dir"),
                      }),
                    },
                    () => ({
                      validator(_, value) {
                        if (getShiftJISByteLength(value) > 1024) {
                          return Promise.reject(
                            new Error(
                              t("err-msg-exceed-byte", {
                                field: t("txt-dest-dir"),
                                size: 1024,
                              })
                            )
                          );
                        }
                        return Promise.resolve();
                      },
                    }),
                  ]}
                >
                  <Input maxLength={1024} />
                </Form.Item>
              </Col>
            </Row>
            <Row>
              <Col span={10}>
                <Form.Item name="overwriteFlag" valuePropName="checked">
                  <Checkbox disabled={formDisable}>
                    {t("chk-overwrite")}
                  </Checkbox>
                </Form.Item>
              </Col>
              <Col span={14}>
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

export default FileTransferIconDialog;
