import { useState, useEffect, useRef } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Select, Form, Input, Button, Row, Col, Modal } from "antd";
import { MinusOutlined, ExpandOutlined } from "@ant-design/icons";
import { ResizableBox } from "react-resizable";
import Draggable from "react-draggable";
import { removeOpenDialog } from "../../../../store/JobnetFormSlice";
import { getNextZIndex } from "../../../../store/JobnetFormSlice";
import { getAllCalendar } from "../../../../store/JobnetInitDataSlice";
import { useTranslation } from "react-i18next";
import "./InfoIconDialog.scss";
import {
  getExecIconSettingNextZIndex,
  removeExecOpenDialog,
} from "../../../../store/JobExecutionSlice";
import {
  INVALID_STRING,
  REGEX_PATTERM,
  SERVICE_RESPONSE,
} from "../../../../constants";
import { getShiftJISByteLength } from "../../../../common/Util";
import {
  checkIdOnSelfJobNetwork,
  isImpossibleStr,
  validateIsIdAlreadyExist,
} from "../../../../views/jobnetForm/Validation";
import { alertError } from "../../CommonDialog";
import JobnetService from "../../../../services/JobnetService";
import { resetInfoIconTooltip } from "../../../../views/jobnetForm/ArrangeIconData";
const { Option } = Select;

const InfoIconDialog = ({
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
  const [infoCalendarName, setInfoCalendarName] = useState("");
  const [infoFlag, setInfoFlag] = useState(0);

  const allCalendarList =
    useSelector((state) => state.jobnetInitData.initData.allCalendarList) || [];

  const [height, setHeight] = useState(190);
  const [width, setWidth] = useState(560);
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
    dispatch(getAllCalendar());
    if (cell.iconSetting.hasOwnProperty("infoFlag")) {
      setInfoFlag(cell.iconSetting.infoFlag);
      if (cell.iconSetting.infoFlag === 0) {
        setInfoCalendarName("");
      } else {
        if (cell.iconSetting.hasOwnProperty("getCalendarName")) {
          setInfoCalendarName(cell.iconSetting.getCalendarName);
        } else {
          setInfoCalendarName("");
        }
      }
    }
    form.setFieldsValue({
      jobId: cell.jobId,
      jobName: cell.jobName || "",
      infoFlag: cell.iconSetting.hasOwnProperty("infoFlag")
        ? cell.iconSetting.infoFlag
        : 0,
      infoJobId: cell.iconSetting.hasOwnProperty("getJobId")
        ? cell.iconSetting.getJobId
        : "",
      infoCalendarId: cell.iconSetting.hasOwnProperty("getCalendarId")
        ? cell.iconSetting.getCalendarId
        : "",
    });
  }, [dispatch]);

  const onChangeCalendarId = (value) => {
    JobnetService.getValidOrLatestCalendar({ id: value }).then((res) => {
      if (res.type === SERVICE_RESPONSE.OK) {
        setInfoCalendarName(res.detail.data.calendar_name);
      }
    });
  };

  const onInfoFlagChange = (value) => {
    setInfoFlag(value);
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

    var infoJobId = form.getFieldValue("infoJobId");
    if (infoJobId && form.getFieldValue("infoFlag") === 0) {
      if (!infoJobId.includes("/")) {
        if (checkIdOnSelfJobNetwork(infoJobId, graph)[1] === false) {
          alertError(t("title-error"), t("err-msg-not-self-job-network"));
        } else {
          saveSetting(cell);
        }
      } else {
        var infoJob = infoJobId.split("/");
        var [jobnetIcon, result] = checkIdOnSelfJobNetwork(infoJob[0], graph);
        if (result === false) {
          alertError(t("title-error"), t("err-msg-not-self-job-network"));
        }

        var linkJobnetId = jobnetIcon.iconSetting.linkJobnetId;

        infoJob.shift();
        JobnetService.checkJob({
          jobnetId: linkJobnetId,
          jobs: infoJob,
        }).then((res) => {
          if (res.type === SERVICE_RESPONSE.OK) {
            if (res.detail.message === SERVICE_RESPONSE.RECORD_NOT_EXIST) {
              alertError(t("title-error"), t("err-msg-not-self-job-network"));
            } else {
              saveSetting(cell);
            }
          }
        });
      }
    } else {
      saveSetting(cell);
    }
  };

  const saveSetting = (cell) => {
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
      infoFlag: infoFlag,
      getJobId: infoFlag === 0 ? form.getFieldValue("infoJobId") : null,
      getCalendarId:
        infoFlag === 3 ? form.getFieldValue("infoCalendarId") : null,
      getCalendarName: infoFlag === 3 ? infoCalendarName : null,
    });
    graph.model.setTooltipLabel(cell, resetInfoIconTooltip(cell, t));
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
            {`${t("txt-info-icon")} (${jobnetIconId ? jobnetIconId + "/" : ""}${
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
              <Col span={6}>{t("txt-info-clfi")}</Col>
              <Col span={18}>
                <Form.Item
                  name="infoFlag"
                  rules={[
                    {
                      required: true,
                      message: t("err-field-required", {
                        field: t("txt-info-clfi"),
                      }),
                    },
                  ]}
                >
                  <Select onChange={onInfoFlagChange}>
                    <Option value={0}>{t("txt-job-sts")}</Option>
                    <Option value={3}>{t("txt-run-day")}</Option>
                  </Select>
                </Form.Item>
              </Col>
            </Row>
            <Row className="mt-top-2px">
              <Col span={6}>{t("lab-job-id")}</Col>
              <Col span={18}>
                <Form.Item
                  name="infoJobId"
                  rules={[
                    {
                      whitespace: true,
                      required: infoFlag === 0,
                      message: t("err-field-required", {
                        field: t("lab-job-id"),
                      }),
                    },
                    () => ({
                      validator(_, value) {
                        if (getShiftJISByteLength(value) > 1024) {
                          return Promise.reject(
                            new Error(
                              t("err-msg-exceed-byte", {
                                field: t("lab-job-id"),
                                size: 1024,
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
                          REGEX_PATTERM.MATCH_HANKAKU_HYPHEN_UNDERBAR_SLASH
                        );
                        if (!regex.test(value)) {
                          return Promise.reject(
                            new Error(
                              t("err-msg-job-info", { field: t("lab-job-id") })
                            )
                          );
                        }
                        return Promise.resolve();
                      },
                    }),
                  ]}
                >
                  <Input disabled={infoFlag !== 0} />
                </Form.Item>
              </Col>
            </Row>
            <Row className="mt-top-2px">
              <Col span={6}>{t("lab-cal-id")}</Col>
              <Col span={18}>
                <Form.Item
                  name="infoCalendarId"
                  rules={[
                    {
                      required: infoFlag === 3,
                      message: t("err-field-required-select", {
                        field: t("lab-cal-id"),
                      }),
                    },
                  ]}
                >
                  <Select
                    onChange={onChangeCalendarId}
                    disabled={infoFlag !== 3}
                  >
                    {allCalendarList.map((item) => {
                      return (
                        <Option key={item.calendar_id} value={item.calendar_id}>
                          {item.calendar_id}
                        </Option>
                      );
                    })}
                  </Select>
                </Form.Item>
              </Col>
            </Row>
            <Row className="mt-top-2px">
              <Col span={6}>{t("lab-cal-name")}</Col>
              <Col span={18}>{infoCalendarName}</Col>
            </Row>
          </Form>
        </div>
      </ResizableBox>
    </Modal>
  );
};

export default InfoIconDialog;
