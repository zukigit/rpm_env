import { useState, useEffect, useRef } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  Divider,
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
import {
  getHostGroup,
  getHostByZabbixApi,
  getItemByZabbixApi,
  getTriggerByZabbixApi,
  RESET_SELECT_HOST,
  RESET_SELECT_ITEM,
  RESET_SELECT_Trigger,
} from "../../../../store/JobnetInitDataSlice";
import "./ZabbixIconDialog.scss";
import {
  getExecIconSettingNextZIndex,
  removeExecOpenDialog,
} from "../../../../store/JobExecutionSlice";
import { INVALID_STRING, REGEX_PATTERM } from "../../../../constants";
import { getShiftJISByteLength } from "../../../../common/Util";
import {
  isImpossibleStr,
  validateIsIdAlreadyExist,
} from "../../../../views/jobnetForm/Validation";
import { resetZabbixTooltip } from "../../../../views/jobnetForm/ArrangeIconData";
const { Option } = Select;

const ZabbixIconDialog = ({
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
  const hostGroupList =
    useSelector((state) => state.jobnetInitData.initData.hostGroup) || [];
  const hostList =
    useSelector((state) => state.jobnetInitData.initData.selectHostList) || [];
  const itemList =
    useSelector((state) => state.jobnetInitData.initData.selectItemList) || [];
  const triggerList =
    useSelector((state) => state.jobnetInitData.initData.selectTriggerList) ||
    [];

  const [linkTarget, setLinkTarget] = useState(0);
  const [linkPermission, setLinkpermission] = useState(0);
  const [height, setHeight] = useState(400);
  const [width, setWidth] = useState(540);
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
    dispatch(getHostGroup());
    var linkTargetFlag = 0;
    if (cell.iconSetting.hasOwnProperty("linkTarget")) {
      linkTargetFlag = cell.iconSetting.linkTarget;
      setLinkTarget(linkTargetFlag);
    }

    if (cell.iconSetting.hasOwnProperty("groupId")) {
      dispatch(
        getHostByZabbixApi({ groupid: parseInt(cell.iconSetting.groupId) })
      );
    }

    if (cell.iconSetting.hasOwnProperty("hostId")) {
      if (cell.iconSetting.hostId) {
        if (linkTargetFlag === 2) {
          dispatch(getItemByZabbixApi({ hostid: cell.iconSetting.hostId }));
        } else if (linkTargetFlag === 3) {
          dispatch(getTriggerByZabbixApi({ hostid: cell.iconSetting.hostId }));
        }
      }
    }

    form.setFieldsValue({
      jobId: cell.jobId,
      jobName: cell.jobName || "",
      linkTarget: linkTargetFlag,
      linkMode: cell.iconSetting.hasOwnProperty("linkOperation")
        ? cell.iconSetting.linkOperation
        : "",
      hostGroup: cell.iconSetting.hasOwnProperty("groupId")
        ? cell.iconSetting.groupId
        : "",
      host:
        cell.iconSetting.hasOwnProperty("hostId") &&
          cell.iconSetting.hostId !== 0
          ? cell.iconSetting.hostId
          : "",
      item:
        cell.iconSetting.hasOwnProperty("itemId") &&
          cell.iconSetting.itemId !== 0
          ? cell.iconSetting.itemId
          : "",
      trigger:
        cell.iconSetting.hasOwnProperty("triggerId") &&
          cell.iconSetting.triggerId !== 0
          ? cell.iconSetting.triggerId
          : "",
    });
  }, [dispatch]);

  const onLinkTargetChange = (value) => {
    setLinkTarget(form.getFieldValue("linkTarget"));
    switch (form.getFieldValue("linkTarget")) {
      case 0:
        dispatch(RESET_SELECT_HOST());
        dispatch(RESET_SELECT_ITEM());
        dispatch(RESET_SELECT_Trigger());
        form.setFieldsValue({
          host: "",
          item: "",
          trigger: "",
        });
        break;
      case 1:
        dispatch(
          getHostByZabbixApi({ groupid: form.getFieldValue("hostGroup") })
        );
        dispatch(RESET_SELECT_ITEM());
        dispatch(RESET_SELECT_Trigger());
        form.setFieldsValue({
          item: "",
          trigger: "",
        });
        break;
      case 2:
        if (form.getFieldValue("host")) {
          dispatch(getItemByZabbixApi({ hostid: form.getFieldValue("host") }));
          form.setFieldsValue({
            trigger: "",
          });
        }
        break;
      case 3:
        if (form.getFieldValue("host")) {
          dispatch(
            getTriggerByZabbixApi({ hostid: form.getFieldValue("host") })
          );
          form.setFieldsValue({
            item: "",
          });
        }
        break;
    }
  };
  useEffect(() => {
    if (hostGroupList && hostGroupList.length > 0) {
      if (cell.iconSetting.hasOwnProperty("groupId")) {
        setLinkModePermission(cell.iconSetting.groupId);
      }
    } else {
      setLinkpermission(2);
    }
  }, [hostGroupList])
  // set link permission by selected host group.
  const setLinkModePermission = (hostGroupId) => {
    if (hostGroupId == 0) {
      setLinkpermission(3);
    }
    for (var hostGroup of hostGroupList) {
      if (hostGroup.groupid == hostGroupId) {
        setLinkpermission(hostGroup.permission);
        // if (hostGroup.permission == 2) {
        //   var linkMode = form.getFieldValue("linkMode");
        //   if (linkMode == 0 || linkMode == 1) {
        //   }
        // }
        break;
      }
    }
  };
  const onHostGroupChange = (value) => {
    setLinkModePermission(value);
    if (form.getFieldValue("linkTarget") !== 0) {
      dispatch(
        getHostByZabbixApi({ groupid: form.getFieldValue("hostGroup") })
      );
      form.setFieldsValue({
        host: "",
        item: "",
        trigger: "",
        linkMode: "",
      });
    }
  };

  const onHostChange = (value) => {

    if (form.getFieldValue("linkTarget") === 2) {
      dispatch(getItemByZabbixApi({ hostid: value }));
      form.setFieldsValue({
        item: "",
        trigger: "",
      });
    } else if (form.getFieldValue("linkTarget") === 3) {
      dispatch(getTriggerByZabbixApi({ hostid: value }));
      form.setFieldsValue({
        item: "",
        trigger: "",
      });
    }
  };

  const onItemChange = (value) => { };

  const onTriggerChange = (value) => { };

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

    let groupId, hostId, itemId, triggerId;

    switch (linkTarget) {
      case 0:
        groupId = parseInt(form.getFieldValue("hostGroup"));
        break;
      case 1:
        groupId = parseInt(form.getFieldValue("hostGroup"));
        hostId = parseInt(form.getFieldValue("host"));
        break;
      case 2:
        groupId = parseInt(form.getFieldValue("hostGroup"));
        hostId = parseInt(form.getFieldValue("host"));
        itemId = parseInt(form.getFieldValue("item"));
        break;
      case 3:
        groupId = parseInt(form.getFieldValue("hostGroup"));
        hostId = parseInt(form.getFieldValue("host"));
        triggerId = parseInt(form.getFieldValue("trigger"));
        break;
    }
    graph.model.setIconSetting(cell, {
      jobId: form.getFieldValue("jobId"),
      jobName: form.getFieldValue("jobName"),
      linkTarget,
      groupId,
      hostId,
      itemId,
      triggerId,
      linkOperation: form.getFieldValue("linkMode"),
    });
    graph.model.setTooltipLabel(cell, resetZabbixTooltip(cell, t));
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
            {`${t("txt-zabbix-link-icon")} (${jobnetIconId ? jobnetIconId + "/" : ""
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
            <Divider style={{ backgroundColor: "rgb(114 109 109 / 20%)" }} />
            <Row>
              <Col span={6}>{t("txt-link-trg")}</Col>
              <Col span={18}>
                <Form.Item
                  name="linkTarget"
                  rules={[
                    {
                      required: true,
                      message: t("err-field-required", {
                        field: t("txt-link-trg"),
                      }),
                    },
                  ]}
                >
                  <Radio.Group onChange={onLinkTargetChange}>
                    <Radio value={0}>{t("txt-host-gp")}</Radio>
                    <Radio value={1}>{t("txt-host-name")}</Radio>
                    <Radio value={2}>{t("txt-item")}</Radio>
                    <Radio value={3}>{t("txt-trigger")}</Radio>
                  </Radio.Group>
                </Form.Item>
              </Col>
            </Row>
            <Row className="mt-top-3px">
              <Col span={6}>{t("txt-host-gp-name")}</Col>
              <Col span={18}>
                <Form.Item
                  name="hostGroup"
                  rules={[
                    {
                      required: true,
                      message: t("err-field-required-select", {
                        field: t("txt-host-gp-name"),
                      }),
                    },
                  ]}
                >
                  <Select onChange={onHostGroupChange}>
                    <option key={0} value={0}>
                      All
                    </option>
                    {hostGroupList.map((item) => {
                      return (
                        <Option
                          key={item.groupid}
                          value={parseInt(item.groupid)}
                        >
                          {item.group_name}
                        </Option>
                      );
                    })}
                  </Select>
                </Form.Item>
              </Col>
            </Row>
            <Row className="mt-top-3px">
              <Col span={6}>{t("txt-host-name")}</Col>
              <Col span={18}>
                <Form.Item
                  name="host"
                  rules={[
                    ({ getFieldValue }) => ({
                      validator(_, value) {
                        if (getFieldValue("linkTarget") !== 0) {
                          if (!value) {
                            return Promise.reject(
                              new Error(
                                t("err-field-required-select", {
                                  field: t("txt-host-name"),
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
                  <Select showSearch onChange={onHostChange} disabled={linkTarget === 0}
                    filterOption={(input, option) =>
                      option.children.toLowerCase().indexOf(input.toLowerCase()) >= 0
                    }>
                    {hostList.map((item) => {
                      return (
                        <Option
                          key={item.groupid + item.hostid}
                          value={parseInt(item.hostid)}
                        >
                          {item.host}
                        </Option>
                      );
                    })}
                  </Select>
                </Form.Item>
              </Col>
            </Row>
            <Row className="mt-top-3px">
              <Col span={6}>{t("txt-item-name")}</Col>
              <Col span={18}>
                <Form.Item
                  name="item"
                  rules={[
                    ({ getFieldValue }) => ({
                      validator(_, value) {
                        if (getFieldValue("linkTarget") === 2) {
                          if (!value) {
                            return Promise.reject(
                              new Error(
                                t("err-field-required-select", {
                                  field: t("txt-item-name"),
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
                  <Select
                    onChange={onItemChange}
                    disabled={
                      linkTarget === 0 || linkTarget === 1 || linkTarget === 3
                    }
                  >
                    {itemList.map((item) => {
                      return (
                        <Option key={item.itemid} value={parseInt(item.itemid)}>
                          {item.item_name}
                        </Option>
                      );
                    })}
                  </Select>
                </Form.Item>
              </Col>
            </Row>
            <Row className="mt-top-3px">
              <Col span={6}>{t("txt-trigger-name")}</Col>
              <Col span={18}>
                <Form.Item
                  name="trigger"
                  rules={[
                    ({ getFieldValue }) => ({
                      validator(_, value) {
                        if (getFieldValue("linkTarget") === 3) {
                          if (!value) {
                            return Promise.reject(
                              new Error(
                                t("err-field-required-select", {
                                  field: t("txt-trigger-name"),
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
                  <Select
                    onChange={onTriggerChange}
                    disabled={
                      linkTarget === 0 || linkTarget === 1 || linkTarget === 2
                    }
                  >
                    {triggerList.map((item) => {
                      return (
                        <Option
                          key={item.triggerid}
                          value={parseInt(item.triggerid)}
                        >
                          {item.conditonalExpression}
                        </Option>
                      );
                    })}
                  </Select>
                </Form.Item>
              </Col>
            </Row>
            <Divider style={{ backgroundColor: "rgb(114 109 109 / 20%)" }} />
            <Row>
              <Col span={6}>{t("txt-link-mode")}</Col>
              <Col span={18}>
                <Form.Item
                  name="linkMode"
                  rules={[
                    {
                      required: true,
                      message: t("err-field-required", {
                        field: t("txt-link-trg"),
                      }),
                    },
                    ({ getFieldValue }) => ({
                      validator(_, value) {
                        if (getFieldValue("linkTarget") === 0) {
                          if (value === 2) {
                            return Promise.reject(
                              new Error(t("err-msg-zbx-status-2"))
                            );
                          }
                          if (value === 3) {
                            return Promise.reject(
                              new Error(t("err-msg-zbx-status-3"))
                            );
                          }
                        } else if (getFieldValue("linkTarget") === 1) {
                          if (value === 3) {
                            return Promise.reject(
                              new Error(t("err-msg-zbx-status-3"))
                            );
                          }
                        } else if (getFieldValue("linkTarget") === 3) {
                          if (value === 3) {
                            return Promise.reject(
                              new Error(t("err-msg-zbx-status-3"))
                            );
                          }
                        }
                        return Promise.resolve();
                      },
                    }),
                  ]}
                >
                  <Radio.Group>
                    <Radio value={0} disabled={linkPermission == 2}>{t("col-enabled")}</Radio>
                    <Radio value={1} disabled={linkPermission == 2}>{t("col-disable")}</Radio>
                    <Radio value={2} disabled={linkTarget === 0}>
                      {t("txt-get-sts")}
                    </Radio>
                    <Radio
                      value={3}
                      disabled={
                        linkTarget === 0 || linkTarget === 1 || linkTarget === 3
                      }
                    >
                      {t("txt-get-data")}
                    </Radio>
                  </Radio.Group>
                </Form.Item>
              </Col>
            </Row>
          </Form>
        </div>
      </ResizableBox>
    </Modal>
  );
};

export default ZabbixIconDialog;
