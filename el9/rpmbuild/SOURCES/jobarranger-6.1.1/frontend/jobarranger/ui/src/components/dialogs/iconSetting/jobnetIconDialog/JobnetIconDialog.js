import { useState, useEffect, useRef } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Form, Spin, Button, Row, Col, Modal } from "antd";
import { MinusOutlined, ExpandOutlined } from "@ant-design/icons";
import { ResizableBox } from "react-resizable";
import Draggable from "react-draggable";
import { alertSuccess, alertError, confirmDialog } from "../../CommonDialog";
import {
  removeOpenDialog,
  addChildForm,
  removeFormList,
} from "../../../../store/JobnetFormSlice";
import { getNextZIndex } from "../../../../store/JobnetFormSlice";
import { useTranslation } from "react-i18next";
import {
  FORM_ID,
  GRAPH,
  FORM_TYPE,
  OBJECT_CATEGORY,
  SERVICE_RESPONSE,
  JOB_ID_LIST,
  CREATE_GRAPH,
} from "../../../../constants";
import FormObject from "../../../form/formObject/FormObject";
import JobnetService from "../../../../services/JobnetService";
import { createFormObject } from "../../../../factory/FormObjectFactory";
import {
  AddDataGraph,
  addOverlap,
  getResourceTxt,
  getTranslateObject,
  VertexOnDblclick,
} from "../../../../views/jobnetForm/InitGraph";
import DisplayModalList from "../../displayModalList/DisplayModalList";
import { validateGraphData } from "../../../../views/jobnetForm/Validation";
import { lockObject, setunLock } from "../../../../store/ObjectListSlice";
import objectLockService from "../../../../services/objectLockService";
import store from "../../../../store";
import { resetDefaultTooltip } from "../../../../views/jobnetForm/ArrangeIconData";
import JobExecutionService from "../../../../services/jobExecutionService";
import { openExecutionDialog } from "../../../../store/JobExecutionSlice";
import DisplayExecuteModalList from "../../displayExecuteModalList/DisplayExecuteModalList";

const JobnetIconDialog = ({
  id,
  parentGraph,
  visible = true,
  cell,
  graphIndexId,
  formGraphId,
  jobnetIconId,
}) => {
  const [form] = Form.useForm();
  const { t } = useTranslation();
  const dispatch = useDispatch();

  const childFormState = useSelector(
    (state) => state.jobnetForm.formObjList[formGraphId]
  );
  const [isJobnetFormDataExist, setIsJobnetFormDataExist] = useState(false);
  const [formDisable, setFormDisable] = useState(false);

  const [height, setHeight] = useState(500);
  const [width, setWidth] = useState(900);
  const [isBlocked, setIsBlocked] = useState(false);
  const [graphDataChange, setGraphDataChange] = useState(false);
  const [disabled, setDisabled] = useState(false);
  const [isMinimize, setIsMinimize] = useState(false);
  const [graph, setGraph] = useState();
  const [loading, setLoading] = useState(false);
  const [graphLoading, setGraphLoading] = useState(true);
  const [isInitGraphDataExist, setIsInitGraphDataExist] = useState(false);
  const [editorUi, setEditorUi] = useState();
  const graphId =
    formGraphId === null
      ? "geJobnetEditor" + graphIndexId
      : "geJobnetEditor" + formGraphId;

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

  const detectedGraphChange = () => {
    setGraphDataChange(true);
  }

  useEffect(() => {
    if (childFormState.formData.hasOwnProperty("id")) {
      setIsJobnetFormDataExist(true);
    }
  }, [childFormState]);

  useEffect(() => {
    if (!isInitGraphDataExist && editorUi !== undefined) {
      JobnetService.initJobnetIcon({
        id: cell.iconSetting.linkJobnetId,
      })
        .then((result) => {
          if (result.type === SERVICE_RESPONSE.OK) {
            let data = result.detail.data;
            dispatch(
              addChildForm({
                formGraphId: formGraphId,
                form: {
                  data: data,
                  formData: createFormObject(
                    data.detail.jobnet_id,
                    data.detail.public_flag === '1' ? true : false,
                    data.detail.multiple_start_up,
                    data.type === FORM_TYPE.EDIT ? data.detail.update_date : "",
                    data.detail.jobnet_name,
                    data.detail.user_name,
                    data.authority,
                    "",
                    data.detail.memo,
                    data.detail.jobnet_timeout,
                    data.detail.timeout_run_type,
                    null,
                    data.editable,
                    0,
                    data.isLocked
                  ),
                },
              })
            );
            if (data.editable === 0 || data.isLocked === 1) {
              setFormDisable(true);
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

              if(sidebarContainer){
                addOverlap(sidebarContainer, sidebarOverlay, "sidebar-overlay")
              }
            } else {
              //if [not locked/enabled/user_permission accepted], lock the jobnet object.
              let object = {
                objectId: data.detail.jobnet_id,
                date: data.detail.update_date,
                category: "jobnet",
              };

              dispatch(lockObject(object));
            }
            AddDataGraph(data, graph, editorUi.editor);
            setGraphLoading(false);
            setIsInitGraphDataExist(true);
          }else if (result.type === SERVICE_RESPONSE.INCOMEPLETE) {
            if(result.detail.message === SERVICE_RESPONSE.ITEM_NOT_FOUND){
              alertError(t("title-error"), t("res-not-exist"));
            }
            dispatch(removeOpenDialog({ id, graphIndexId }));
            dispatch(removeFormList(formGraphId));
          }
        })
        .catch((err) => { });
    }
  }, [graph, editorUi, isInitGraphDataExist]);

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
      
      // Adds required resources (disables loading of fallback properties, this can only
      // be used if we know that all keys are defined in the language specific file)
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
              graphScreen: GRAPH.JOBNET_ICON_GRAPH,
              graphId: graphId,
              iconTypeAndIdMap: new Map(),
              jobnetIdListObj: JSON.parse(JSON.stringify(JOB_ID_LIST)),
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
        VertexOnDblclick(evt, state, graph, t, dispatch, formGraphId);
      };
      editorUi.editor.graph.isFormDisable = function () {
        return formDisable;
      };
      editorUi.editor.graph.detectGraphChange = function  () {
        detectedGraphChange();
      }
      editorUi.editor.graph.singleJobRun = function (
        jobId,
        iconSetting,
        methodFlag
      ) {
        JobExecutionService.singleJobRun({
          jobnetId: form.getFieldValue("id"),
          iconSetting,
          methodFlag,
        })
          .then((res) => {
            if (res.type === SERVICE_RESPONSE.INCOMEPLETE) {
              alertError(t("title-error"), res.detail.message);
            } else {
              dispatch(openExecutionDialog(res.detail.data.innerJobnetId));
            }
          })
          .catch((err) => {
            alertError(t("title-error"), err.message);
          });
      };
    }
  }, [editorUi, VertexOnDblclick, formDisable, t]);
  //temp unlock. Migrate to clear data later.
  useEffect(() => {
    return () => {
      unlockObject();
    };
  }, []);
  const unlockObject = () => {
    //unlock object lock.

    let formObject = store.getState().jobnetForm.formObjList[formGraphId];
    if (formObject) {
      let objectLockObject = {
        objectId: formObject.formData.id,
        objectType: "JOBNET",
      };

      if (
        formObject.formData.editable == 1 &&
        formObject.formData.isLocked == 0
      ) {
        objectLockService.deleteLock([objectLockObject]);
        dispatch(setunLock());
      }
    }
  };

  const onFormChange = (changedValues, allValues) => {
    if (!isBlocked) {
      setIsBlocked(true);
    }
  }

  const onCancelAction = () => {
    unlockObject();
    dispatch(removeOpenDialog({ id, graphIndexId }));
    dispatch(removeFormList(formGraphId));
    editorUi.editor.graph.tooltipHandler.hideTooltip();
  }

  const onCancel = (e) => {
    if(isBlocked || graphDataChange){
      confirmDialog(
        t("title-msg-confirm"),
        t("warn-mess-redisplay"),
        onCancelAction,
        () => {}
      );
    }else{
      onCancelAction();
    }
    
  };

  const onFinish = () => {
    setLoading(true);
    form
      .validateFields()
      .then((values) => {
        let result = validateGraphData(editorUi, t);
        if (!result[0]) {
          setLoading(false);
          return;
        }

        let data = {
          jobnetId: form.getFieldValue("id"),
          multiple: form.getFieldValue("multiple"),
          public: form.getFieldValue("isPublic") ? 1 : 0,
          jobnetName: form.getFieldValue("name"),
          description: form.getFieldValue("description") || "",
          timeoutSec: parseInt(form.getFieldValue("timeout")),
          timeoutType: parseInt(form.getFieldValue("timeoutType")),
          icon: result[1],
          flow: result[2],
          userName: form.getFieldValue("userName"),
          type: FORM_TYPE.EDIT,
          updateDate: childFormState.data.detail.update_date,
        };

        JobnetService.saveJobnet(data).then((res) => {
          setLoading(false)
          if (res.type === SERVICE_RESPONSE.OK) {
            alertSuccess(t("title-success"), t("label-success"));
            let cell = parentGraph.getModel().getCell(id);
            var parent = editorUi.editor.graph.getDefaultParent();
            parentGraph.model.setJobName(cell, form.getFieldValue("name"));
            parentGraph.model.setTooltipLabel(
              cell,
              resetDefaultTooltip(cell, t)
            );
            let label = `<div style="height: 15px; width: 80px; overflow:hidden; white-space: nowrap; text-overflow: ellipsis;">${cell.jobId}</div>`;
            if (form.getFieldValue("name")) {
              label += `<div style="height:15px; width: 80px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis;">${form.getFieldValue(
                "name"
              )}</div>`;
            }
            parentGraph.model.setValue(cell, label);
            parentGraph.model.setIconSetting(cell, {
              ...cell.iconSetting,
              publicType: res.detail.data.publicFlag,
              updatedDate: res.detail.data.updateDate,
            });
            //unlock
            unlockObject();
            dispatch(removeOpenDialog({ id, graphIndexId }));
            dispatch(removeFormList(formGraphId));
          } else {
            alertError("", res.detail.message);
          }
        });
      })
      .catch((errorInfo) => { 
        setLoading(false)
      });
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
              dispatch(getNextZIndex(cell, graphIndexId));
            }}
            onMouseOver={() => {
              if (disabled) {
                setDisabled(false);
              }
            }}
            onMouseDown={() => {
              dispatch(getNextZIndex(cell, graphIndexId));
            }}
            onMouseOut={() => {
              setDisabled(true);
            }} // fix eslintjsx-a11y/mouse-events-have-key-events
          // https://github.com/jsx-eslint/eslint-plugin-jsx-a11y/blob/master/docs/rules/mouse-events-have-key-events.md
          >
            {`${t("txt-jobnet-icon")} (${jobnetIconId ? jobnetIconId + "/" : ""
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
          type="primary"
          onClick={onFinish}
          disabled={formDisable}
          loading={loading}
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
          {isJobnetFormDataExist && (
            <Form form={form} onChange={onFormChange}>
              <FormObject
                formId={OBJECT_CATEGORY.JOBNET}
                onFinishAction={() => { }}
                objectSlice="jobnetForm"
                formProperty={formGraphId}
              />
            </Form>
          )}

          <Spin size="large" spinning={graphLoading}>
            <div className={graphId}></div>
          </Spin>
          <DisplayModalList
            id={graphId}
            graph={graph}
            graphIndexId={formGraphId}
            jobnetIconId={cell.jobId}
            formDisable={formDisable}
          />
          {/* <DisplayExecuteModalList /> */}
        </div>
      </ResizableBox>
    </Modal>
  );
};

export default JobnetIconDialog;
