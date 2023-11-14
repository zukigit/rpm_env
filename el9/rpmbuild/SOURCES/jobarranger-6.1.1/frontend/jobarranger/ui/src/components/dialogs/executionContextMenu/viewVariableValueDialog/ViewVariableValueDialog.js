import { useState, useRef } from "react";
import { useDispatch } from "react-redux";
import {
  Form,
  Input,
  Button,
  Row,
  Col,
  Modal
} from "antd";
import { MinusOutlined, ExpandOutlined } from "@ant-design/icons";
import { ResizableBox } from "react-resizable";
import Draggable from "react-draggable";
import { useTranslation } from "react-i18next";
import { getExecIconSettingNextZIndex, removeExecOpenDialog } from "../../../../store/JobExecutionSlice";
import BasicTable from "../../../tables/basicTable/BasicTable";
import "./ViewVariableValueDialog.scss";

const ViewVariableValueDialog = ({ id, graph, visible = true, cell, graphIndexId, jobnetIconId, formDisable = false, innerJobnetId = null }) => {
  const [form] = Form.useForm();
  const { t } = useTranslation();
  const dispatch = useDispatch();
 
  const [height, setHeight] = useState(520);
  const [width, setWidth] = useState(800);
  const [disabled, setDisabled] = useState(false);
  const [isMinimize, setIsMinimize] = useState(false);

  const [bounds, setBounds] = useState({
    left: 0,
    top: 0,
    bottom: 0,
    right: 0,
  });

  const beforeDefaultColumns = [
    {
      title: t("txt-var-name"),
      dataIndex: "value_name",
      width: "30%",
      editable: true,
    },
    {
      title: t("txt-val"),
      dataIndex: "before_value",
      editable: true,
    }
  ];

  const afterDefaultColumns = [
    {
      title: t("txt-var-name"),
      dataIndex: "value_name",
      width: "30%",
      editable: true,
    },
    {
      title: t("txt-val"),
      dataIndex: "after_value",
      editable: true,
    }
  ];

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

  const onCancel = (e) => {
    dispatch(removeExecOpenDialog({id, innerJobnetId}));
  };

  return (
    <Modal
      title={<div
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
          dispatch(getExecIconSettingNextZIndex(cell, innerJobnetId))
        }}
        onMouseOver={() => {
          if (disabled) {
            setDisabled(false);
          }
        }}
        onMouseDown={() => {
          dispatch(getExecIconSettingNextZIndex(cell, innerJobnetId))
        }}
        onMouseOut={() => {
          setDisabled(true);
        }} // fix eslintjsx-a11y/mouse-events-have-key-events
        // https://github.com/jsx-eslint/eslint-plugin-jsx-a11y/blob/master/docs/rules/mouse-events-have-key-events.md
      >
        {`${t("title-view-var-value")} (Manage id : ${innerJobnetId})`}
      </div>
      <Button
        style={{
          float: "right",
          marginRight: "32px",
          border: "none",
          height: "10px",
          marginTop: "3px"
        }}
        onClick={onMinimize}
        shape="circle"
        icon={!isMinimize ? <MinusOutlined /> : <ExpandOutlined />}
      />
    </div>}
      visible={visible}
      onCancel={() => {
        onCancel();
      }}
      footer={[
        <Button
          key="cancel"
          onClick={() => {
            onCancel();
          }}
        >
          {t('btn-close')}
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
        className="icon-dialog-form view-variable-form"
        labelAlign="left"
        disabled={formDisable}
      >
        <Row>
          <Col span={2}>{t("lab-job-id")}{" : "}</Col>
          <Col span={5}>
            {cell.jobId}
          </Col>
          <Col span={3}>{t("col-job-name")}{" : "}</Col>
          <Col span={12}>
            {cell.jobName}
          </Col>
        </Row>
        {t("lab-before-var")}
        <BasicTable columnHeaders={beforeDefaultColumns} dataSource={cell.beforeVariables} tableHeight={'auto'} />
        <br/>
        {t("lab-after-var")}
        <BasicTable columnHeaders={afterDefaultColumns} dataSource={cell.afterVariables} tableHeight={'auto'} />
      </Form>
      </div>
      </ResizableBox>
    </Modal>
  );
};

export default ViewVariableValueDialog;
