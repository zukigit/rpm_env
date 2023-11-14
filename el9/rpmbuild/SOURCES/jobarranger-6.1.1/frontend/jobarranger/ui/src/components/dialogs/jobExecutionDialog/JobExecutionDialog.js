import { useState, useEffect, useRef } from "react";
import { useDispatch } from "react-redux";
import { Form, Spin, Button, Modal } from "antd";
import { MinusOutlined, ExpandOutlined } from "@ant-design/icons";
import { ResizableBox } from "react-resizable";
import Draggable from "react-draggable";
import { useTranslation } from "react-i18next";
import {
  CREATE_GRAPH,
  GRAPH,
  ICON_TYPE,
  SERVICE_RESPONSE,
} from "../../../constants";
import {
  addOverlap,
  displayVertexAndFlow,
  ExecVertexOnDblclick,
  getResourceTxt,
  getTranslateObject,
  updateVertex,
} from "../../../views/jobnetForm/InitGraph";
import {
  getExecJobNextZIndex,
  openExecIconSettingDialog,
  removeExecOpenDialog,
  removeExecOpenDialogForChild,
  removeExecuteJob,
} from "../../../store/JobExecutionSlice";
import JobExecutionService from "../../../services/jobExecutionService";
import DisplayExecuteIconSettingModalList from "../displayExecuteModalList/DisplayExecuteIconSettingModalList";
import JobExecutionHeader from "./JobExecutionHeader";

const JobExecutionDialog = ({ id, cell, visible = true }) => {
  const [form] = Form.useForm();
  const { t } = useTranslation();
  const dispatch = useDispatch();

  const [height, setHeight] = useState(500);
  const [width, setWidth] = useState(975);
  const [disabled, setDisabled] = useState(false);
  const [isMinimize, setIsMinimize] = useState(false);
  const [graph, setGraph] = useState();
  const [graphLoading, setGraphLoading] = useState(false);
  const [executionInfo, setExecutionInfo] = useState(false);
  const [isInitGraph, setIsInitGraph] = useState(true);
  const [editorUi, setEditorUi] = useState();
  const graphId = "geExecEditor" + id;

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

  const prepareExecuteInfo = (data) => {
    var execInfo = data.runJobnet;
    var jobnet_timeout,
      timeout_run_type = null;
    if (data.runJobSummary != null) {
      jobnet_timeout = data.runJobSummary.jobnet_timeout;
      timeout_run_type = data.runJobSummary.timeout_run_type;
    }
    execInfo = {
      ...execInfo,
      jobnet_timeout,
      timeout_run_type,
    };
    setExecutionInfo(execInfo);
  };

  useEffect(() => {
    document.body.style.overflow = "visible";
    if (graph) {
      graph.setCellsMovable(false);
      graph.setCellsDeletable(false);
      graph.setCellsBendable(false);
      graph.setCellsEditable(false);
      graph.setConnectable(false);
      graph.setConnectableEdges(false);
      graph.setSwimlaneSelectionEnabled(false);

      var sidebarContainer = document.querySelector(
        "." + graphId + " .geSidebar"
      );
      var sidebarOverlay = document.querySelector(
        "." + graphId + " .sidebar-overlay"
      );

      if (sidebarContainer) {
        addOverlap(sidebarContainer, sidebarOverlay, "sidebar-overlay")
      }
    }

    JobExecutionService.getRunJobnetData({ id })
      .then((res) => {
        if (res.type === SERVICE_RESPONSE.OK) {
          prepareExecuteInfo(res.detail.data);
          if (isInitGraph) {
            displayVertexAndFlow(editorUi.editor, res.detail.data);
            setIsInitGraph(false);
          }
        }
      })
      .catch((err) => { });
  }, [graph]);

  useEffect(() => {
    if (!isInitGraph) {
      const intervalId = setInterval(() => {
        JobExecutionService.getRunJobnetData({ id })
          .then((res) => {
            if (res.type === SERVICE_RESPONSE.OK) {
              prepareExecuteInfo(res.detail.data);
              updateVertex(editorUi.editor, res.detail.data);
            }
          })
          .catch((err) => { });
      }, 1500);

      return () => {
        clearInterval(intervalId);
      };
    }
  }, [isInitGraph]);

  useEffect(() => {
    if (document.querySelector("." + graphId).children.length === 0) {
      var urlParams = (function (url) {
        var result = new Object();
        var idx = url.lastIndexOf("?");

        if (idx > 0) {
          var params = url.substring(idx + 1).split("&");

          for (var i = 0; i < params.length; i++) {
            idx = params[i].indexOf("=");

            if (idx > 0) {
              result[params[i].substring(0, idx)] = params[i].substring(
                idx + 1
              );
            }
          }
        }

        return result;
      })(window.location.href);

      // Default resources are included in grapheditor resources

      var xmlFile = CREATE_GRAPH.XML_FILE;

      var resourceTxt = getResourceTxt();

      window.mxResources.loadDefaultBundle = false;
      var bundle =
        window.mxResources.getDefaultBundle(
          window.RESOURCE_BASE,
          window.mxLanguage
        ) ||
        window.mxResources.getSpecialBundle(
          window.RESOURCE_BASE,
          window.mxLanguage
        );

      var parser = new DOMParser();
      var xmlDoc = parser.parseFromString(xmlFile, "text/xml");
      window.mxUtils.getAll(
        [],
        function (xhr) {
          // Adds bundle text to resources
          window.mxResources.parse(resourceTxt);

          // Configures the default graph theme
          var themes = new Object();
          themes["default"] = xmlDoc.documentElement;

          const editorUi = new window.EditorUi(
            new window.Editor(urlParams["chrome"] == "0", themes),
            document.querySelector("div." + graphId),
            undefined,
            {
              graphScreen: GRAPH.JOBNET_EXEC_GRAPH,
            }
          );
          setEditorUi(editorUi);
          setGraph(editorUi.editor.graph);
        },
        function () {
          document.body.innerHTML =
            '<center style="margin-top:10%;">Sry cannot generate graph</center>';
        }
      );
    }
  }, []);

  useEffect(() => {
    if (editorUi !== undefined) {
      editorUi.editor.graph.getTranslateObject = function () {
        return getTranslateObject();
      };
      editorUi.editor.graph.dblClick = function (evt, state) {
        ExecVertexOnDblclick(evt, state, graph, t, dispatch, cell.id);
      };
      editorUi.editor.graph.setSkipJob = function (
        innerJobnetId,
        innerJobId,
        runStatus
      ) {
        JobExecutionService.setSkipJob({ innerJobnetId, innerJobId, runStatus })
          .then((res) => { })
          .catch((err) => console.log(err));
      };
      editorUi.editor.graph.setHoldJob = function (innerJobnetId, innerJobId) {
        JobExecutionService.setHoldJob({ innerJobnetId, innerJobId })
          .then((res) => { })
          .catch((err) => console.log(err));
      };
      editorUi.editor.graph.setNormalJob = function (
        innerJobnetId,
        innerJobId
      ) {
        JobExecutionService.setNormalJob({ innerJobnetId, innerJobId })
          .then((res) => { })
          .catch((err) => console.log(err));
      };
      editorUi.editor.graph.setRerun = function (innerJobnetId, innerJobId) {
        JobExecutionService.setRerun({ innerJobnetId, innerJobId })
          .then((res) => { })
          .catch((err) => console.log(err));
      };
      editorUi.editor.graph.setForceStop = function (
        innerJobnetId,
        innerJobId
      ) {
        JobExecutionService.setForceStop({ innerJobnetId, innerJobId })
          .then((res) => { })
          .catch((err) => console.log(err));
      };
      editorUi.editor.graph.variableValueChange = function (
        innerJobnetId,
        innerJobId,
        cellItem
      ) {
        dispatch(
          openExecIconSettingDialog(
            {
              id: cellItem[0].id + "-var-change",
              cellType: ICON_TYPE.VAR_VALUE_CHANGE,
              jobId: cellItem[0].jobId,
              jobName: cellItem[0].jobName,
              iconSetting: cellItem[0].iconSetting,
              beforeVariables: cellItem[0].beforeVariables,
              innerJobnetId,
              innerJobId,
            },
            cell.id
          )
        );
      };
      editorUi.editor.graph.viewVariableValue = function (cellItem) {
        dispatch(
          openExecIconSettingDialog(
            {
              id: cellItem[0].id + "-var-view",
              cellType: ICON_TYPE.VIEW_VAR_VALUE,
              jobId: cellItem[0].jobId,
              jobName: cellItem[0].jobName,
              iconSetting: cellItem[0].iconSetting,
              beforeVariables: cellItem[0].beforeVariables,
              afterVariables: cellItem[0].afterVariables,
            },
            cell.id
          )
        );
      };
      return () => {
        editorUi.destroy();
      }; 
    }
  }, [editorUi, t]);

  const onCancel = (e) => {
    if (cell.parentId !== null) {
      dispatch(
        removeExecOpenDialogForChild({ id: cell.childId, innerJobnetId: cell.parentId })
      );
    }
    dispatch(removeExecuteJob(id));
    editorUi.editor.graph.tooltipHandler.hideTooltip();
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
              dispatch(getExecJobNextZIndex(cell));
            }}
            onMouseOver={() => {
              if (disabled) {
                setDisabled(false);
              }
            }}
            onMouseDown={() => {
              dispatch(getExecJobNextZIndex(cell));
            }}
            onMouseOut={() => {
              setDisabled(true);
            }} // fix eslintjsx-a11y/mouse-events-have-key-events
          // https://github.com/jsx-eslint/eslint-plugin-jsx-a11y/blob/master/docs/rules/mouse-events-have-key-events.md
          >
            {`${t("title-job-detail")} (${t("col-manage-id")} : ${cell.id})`}
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
      footer={null}
      wrapClassName="ant-multiple-modal-wrap"
      className={`execute-dialog ${isMinimize ? 'hide-body-modal' : ''}`}
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
      style={{ top: 10 }}
    >
      <ResizableBox
        width={width}
        height={height}
        maxConstraints={[window.innerWidth - 100, window.innerHeight - 180]}
        // resizeHandles={["sw", "nw", "se", "ne"]}
        minConstraints={[200, 60]}
        onResize={onResize}
        handleSize={[80, 80]}
      >
        <div
          className="dialog-wrapper"
          style={{ width: width, height: height, overflow: "auto" }}
          onMouseOver={() => {
            setDisabled(true);
          }}
        >
          {/* <JobExecutionDescription id={id} info={executionInfo} /> */}
          <JobExecutionHeader id={id} info={executionInfo} />
          <Spin size="large" spinning={graphLoading}>
            <div className={graphId}></div>
          </Spin>
          <DisplayExecuteIconSettingModalList
            id={graphId}
            graph={graph}
            innerJobnetId={cell.id}
          />
        </div>
      </ResizableBox>
    </Modal>
  );
};

export default JobExecutionDialog;
