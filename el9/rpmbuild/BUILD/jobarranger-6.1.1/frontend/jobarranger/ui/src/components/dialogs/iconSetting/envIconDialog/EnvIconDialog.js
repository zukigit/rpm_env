import { useState, useEffect, useRef } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Popconfirm, Form, Input, Button, Row, Col, Modal } from "antd";
import { MinusOutlined, ExpandOutlined } from "@ant-design/icons";
import { ResizableBox } from "react-resizable";
import Draggable from "react-draggable";
import { removeOpenDialog } from "../../../../store/JobnetFormSlice";
import { getNextZIndex } from "../../../../store/JobnetFormSlice";
import { useTranslation } from "react-i18next";
import EditableTable from "../../../tables/editableTable/EditableTable";
import "./EnvIconDialog.scss";
import {
  getExecIconSettingNextZIndex,
  removeExecOpenDialog,
} from "../../../../store/JobExecutionSlice";
import { getShiftJISByteLength } from "../../../../common/Util";
import { INVALID_STRING, REGEX_PATTERM } from "../../../../constants";
import {
  isHankakuStrAndUnderbarAndFirstNotNum,
  isImpossibleStr,
  validateIsIdAlreadyExist,
} from "../../../../views/jobnetForm/Validation";
import { alertError } from "../../CommonDialog";
import { resetJobConVarIconTooltip } from "../../../../views/jobnetForm/ArrangeIconData";

const EnvIconDialog = ({
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

  const [height, setHeight] = useState(270);
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
  const [jobVariable, setJobVariable] = useState([]);
  const [count, setCount] = useState(0);

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
    });
  }, [dispatch]);

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

  const onFinish = (values) => {
    if (jobVariable.length === 0) {
      alertError(
        t("title-error"),
        t("err-field-required", { field: t("label-job-con-var") })
      );
      return;
    }
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
      valueJob: jobVariable,
    });
    graph.model.setTooltipLabel(cell, resetJobConVarIconTooltip(cell, t));
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
            {`${t("txt-job-ctr-icon")} (${
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
            {t("label-job-con-var")}
            <EditableTable
              defaultColumns={defaultColumns}
              dataSource={jobVariable}
              handleSave={handleSave}
              tableHeight={"120px"}
            />
            <Row className="mt-3x">
              <Col span={9}>
                <Form.Item name="jobVarName">
                  <Input
                    placeholder={t("txt-var-name")}
                    maxLength={128}
                    type="text"
                  />
                </Form.Item>
              </Col>
              <Col offset={1} span={9}>
                <Form.Item name="jobVarValue">
                  <Input placeholder={t("txt-val")} type="text" />
                </Form.Item>
              </Col>
              <Col offset={1} span={4}>
                <Form.Item>
                  <Button size={"small"} maxLength={4000} onClick={handleAdd}>
                    {t("btn-add")}
                  </Button>
                </Form.Item>
              </Col>
            </Row>
          </Form>
        </div>
      </ResizableBox>
    </Modal>
  );
};

export default EnvIconDialog;
