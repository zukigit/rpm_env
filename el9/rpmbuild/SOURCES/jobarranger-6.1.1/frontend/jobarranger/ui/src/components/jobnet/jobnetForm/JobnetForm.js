import React, { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useSelector, useDispatch } from "react-redux";
import { forwardRef, useImperativeHandle } from "react";
import { Spin, Modal, TreeSelect } from "antd";
import store from "../../../store";
import {
  OBJECT_CATEGORY,
  FORM_TYPE,
  GRAPH,
  JOB_ID_LIST,
  SERVICE_RESPONSE,
  CREATE_GRAPH,
} from "../../../constants";
import FormObject from "../../form/formObject/FormObject";
import ScheduleFormObject from "../../form/scheduleFormObject/ScheduleFormObject";
import {
  VertexOnDblclick,
  AddDataGraph,
  getTranslateObject,
  ClearGraph,
  addOverlap,
  getResourceTxt,
} from "../../../views/jobnetForm/InitGraph";
import {
  initCreateJobnet,
  initEditJobnet,
} from "../../../store/JobnetFormSlice";
import { clearData } from "../../../store/JobnetFormSlice";
import JobnetService from "../../../services/JobnetService";
import { validateGraphData } from "../../../views/jobnetForm/Validation";
import { alertError } from "../../dialogs/CommonDialog";

import { t } from "i18next";
import "ant-design-draggable-modal/dist/index.css";
import "./JobnetForm.scss";
import DisplayModalList from "../../dialogs/displayModalList/DisplayModalList";
import JobExecutionService from "../../../services/jobExecutionService";
import DisplayExecuteModalList from "../../dialogs/displayExecuteModalList/DisplayExecuteModalList";
import { openExecutionDialog } from "../../../store/JobExecutionSlice";
import objectLockService from "../../../services/objectLockService";
import { lockObject, setunLock } from "../../../store/ObjectListSlice";

const JobnetForm = forwardRef(
  (
    {
      formType,
      publicType,
      objectId,
      date,
      childCell = null,
      graphIndexId = 1,
      form = null,
      detectedGraphChange = () => { }
    },
    ref
  ) => {
    useImperativeHandle(ref, () => ({
      validateGraphData() {
        return validateGraphData(editorUi, t);
      }
    }));

    const navigate = useNavigate();
    const dispatch = useDispatch();
    const { TreeNode } = TreeSelect;
    const [formLoading, setFormLoading] = useState(false);
    const [iconSettingDisable, setIconSettingDisable] = useState(false);
    const [graph, setGraph] = useState();
    const [graphLoading, setGraphLoading] = useState(true);
    const [isInitGraphDataExist, setIsInitGraphDataExist] = useState(false);
    const [editorUi, setEditorUi] = useState();
    const [jobnetDropHandlerObj, setJobnetDropHandlerObj] = useState({});
    const [jobnetIconSelectModalVisible, setJobnetIconSelectModalVisible] =
      useState(false);
    const [selectJobnetIcon, setSelectJobnetIcon] = useState(undefined);
    const [jobnetOption, setJobnetOption] = useState({});
    const graphId = "geEditor";
    const objectLockObject = useRef({
      objectId: "",
      objectType: 0,
    });

    const onTreeSelectChange = (newValue) => {
      setSelectJobnetIcon(newValue);
    };

    const handleJobnetIconSelectOk = () => {
      let dataObj = JSON.parse(selectJobnetIcon);
      editorUi.editor.graph.createDropHandlerForJobnetIcon(
        jobnetDropHandlerObj.cells,
        true,
        true,
        jobnetDropHandlerObj.bounds,
        jobnetDropHandlerObj.graph,
        jobnetDropHandlerObj.target,
        jobnetDropHandlerObj.x,
        jobnetDropHandlerObj.y,
        jobnetDropHandlerObj.evt,
        dataObj.jobnet_id,
        dataObj.jobnet_id,
        dataObj.public_flag,
        dataObj.update_date,
        "<div>" +
        t("label-job-id") +
        dataObj.jobnet_id +
        "</div> <div>" +
        t("label-job-name") +
        dataObj.jobnet_id +
        "</div>"
      );
      setJobnetIconSelectModalVisible(false);
      setSelectJobnetIcon("");
    };

    const handleJobnetIconSelectCancel = () => {
      setJobnetIconSelectModalVisible(false);
    };

    const jobnetInfo = useSelector(
      (state) => state.jobnetForm.formObjList[graphIndexId].data
    );

    // useEffect(() => {
    //   if (jobnetFormData.editable == 0 || jobnetFormData.isLocked == 1) {
    //     dispatch(setObjectFormEditable(false));
    //   } else {
    //     dispatch(setObjectFormEditable(true));
    //     let object = {
    //       objectId: jobnetFormData.id,
    //       date: jobnetFormData.updateDate,
    //       category: "jobnet",
    //     };
    //     dispatch(lockObject(object));
    //   }
    // }, [jobnetFormData]);
    useEffect(() => {
      if (formType === FORM_TYPE.SCHEDULE && graph) {
        graph.setCellsMovable(false);
        graph.setCellsDeletable(false);
        graph.setCellsBendable(false);
        graph.setCellsEditable(false);
        graph.setConnectable(false);
        graph.setConnectableEdges(false);

        var sidebarContainer = document.querySelector(".geEditor .geSidebar");
        var sidebarOverlay = document.querySelector(
          ".geEditor .sidebar-overlay"
        );

        if (sidebarContainer) {
          addOverlap(sidebarContainer, sidebarOverlay, "sidebar-overlay");
        }

        var graphContainer = document.querySelector(".geDiagramContainer");
        var graphOverlay = document.querySelector(".graph-overlay");
        addOverlap(graphContainer, graphOverlay, "graph-overlay");
      }
    }, [graph]);

    const [isReloaded, reloaded] = useState(false);

    useEffect(() => {
      //unlock on reload
      if (formType != FORM_TYPE.SCHEDULE) {
        var localStoredValues = JSON.parse(
          sessionStorage.getItem("formIsRefreshed") || "[]"
        );
        if (
          "isLocked" in localStoredValues &&
          localStoredValues.isLocked == 0
        ) {
          let lockedObject = {
            objectId: localStoredValues.objectId,
            objectType: localStoredValues.objectType,
          };
          objectLockService.deleteLockAsync([lockedObject]).then((response) => {
            reloaded(true);
          });
          dispatch(setunLock());
        } else {
          reloaded(true);
        }
        sessionStorage.removeItem("formIsRefreshed");
      } else {
        reloaded(true);
      }
      //
    }, [initCreateJobnet, objectId]);
    useEffect(() => {
      if (formType === FORM_TYPE.CREATE) {
        if (isReloaded == true) {
          dispatch(
            initCreateJobnet({ type: formType, publicType: publicType ? 1 : 0 })
          );
        }
      } else if (
        formType === FORM_TYPE.EDIT ||
        formType === FORM_TYPE.NEW_OBJECT ||
        formType === FORM_TYPE.NEW_VERSION
      ) {
        if (objectId && date) {
          if (isReloaded == true) {
            dispatch(
              initEditJobnet({
                type: formType,
                id: objectId,
                date: date,
              })
            );
          }
        }
      } else if (formType === FORM_TYPE.SCHEDULE) {
        if (objectId && date) {
          if (isReloaded == true) {
            dispatch(
              initEditJobnet({
                type: FORM_TYPE.SCHEDULE, //edit
                id: objectId,
                date: date,
              })
            );
          }
        }
      }
    }, [isReloaded, objectId]);

    useEffect(() => {
      if (jobnetInfo.type === SERVICE_RESPONSE.INCOMEPLETE) {
        alertError(
          t("title-error"),
          `${jobnetInfo.detail["message-objectid"]
            ? t(jobnetInfo.detail["message-objectid"]) + " :"
            : ""
          }  ${t(
            jobnetInfo.detail["message-detail"]
              ? t(jobnetInfo.detail["message-detail"])
              : t("err-msg-fail")
          )}`
        );
        navigate(`/object-list/jobnet/${publicType ? "public" : "private"}/`);
      } else {
        if (typeof jobnetInfo === "object" && editorUi !== undefined) {
          if (formType === FORM_TYPE.CREATE) {
            setGraphLoading(false);
          }
          if (
            formType === FORM_TYPE.EDIT ||
            formType === FORM_TYPE.NEW_VERSION
          ) {
            if (
              jobnetInfo.hasOwnProperty("detail") &&
              isInitGraphDataExist === false
            ) {
              if (jobnetInfo.editable === 0 || jobnetInfo.isLocked == 1) {
                setIconSettingDisable(true);
                graph.setCellsMovable(false);
                graph.setCellsDeletable(false);
                graph.setCellsBendable(false);
                graph.setCellsEditable(false);
                graph.setConnectable(false);
                graph.setConnectableEdges(false);
                graph.setSwimlaneSelectionEnabled(false);

                var sidebarContainer = document.querySelector(
                  ".geEditor .geSidebar"
                );
                var sidebarOverlay = document.querySelector(
                  ".geEditor .sidebar-overlay"
                );

                if (sidebarContainer) {
                  addOverlap(
                    sidebarContainer,
                    sidebarOverlay,
                    "sidebar-overlay"
                  );
                }
              } else {
                //if [not locked/enabled/user_permission accepted], lock the jobnet object.
                let object = {
                  objectId: jobnetInfo.detail.jobnet_id,
                  date: jobnetInfo.detail.update_date,
                  category: "jobnet",
                };
                dispatch(lockObject(object));
              }
              AddDataGraph(jobnetInfo, graph, editorUi.editor);
              setGraphLoading(false);
              setIsInitGraphDataExist(true);
            }
          }
          if (formType === FORM_TYPE.NEW_OBJECT) {
            if (
              jobnetInfo.hasOwnProperty("detail") &&
              isInitGraphDataExist === false
            ) {
              AddDataGraph(jobnetInfo, graph, editorUi.editor);
              setGraphLoading(false);
              setIsInitGraphDataExist(true);
            }
          }
          if (formType === FORM_TYPE.SCHEDULE) {
            if (jobnetInfo.hasOwnProperty("detail")) {
              ClearGraph(graph);
              AddDataGraph(jobnetInfo, graph, editorUi.editor);
              setGraphLoading(false);
              setIsInitGraphDataExist(true);
            }
          }
        }
      }
    }, [jobnetInfo, formType]);
    useEffect(() => {
      return () => {
        //unlock object lock.

        let formObject = store.getState().jobnetForm.formObjList[graphIndexId];

        objectLockObject.current.objectId = formObject.formData.id;
        objectLockObject.current.objectType = "JOBNET";

        if (
          formObject.formData.editable == 1 &&
          formObject.formData.isLocked == 0
        ) {
          objectLockService.deleteLock([objectLockObject.current]);
          dispatch(setunLock());
        }
        dispatch(clearData());
      };
    }, []);

    useEffect(() => {
      return () => {
        if (editorUi !== undefined) {
          editorUi.editor.graph.tooltipHandler.hideTooltip();
          editorUi.destroy();
        }
      };
    }, [editorUi]);

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
                graphScreen: GRAPH.JOBNET_MANAGE_GRAPH,
                graphId: 1,
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
        editorUi.editor.graph.alertError = function (msg) {
          alertError(t("title-error"), msg);
        };
        editorUi.editor.graph.detectGraphChange = function () {
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
        editorUi.editor.graph.isFormDisable = function () {
          return iconSettingDisable;
        };
        editorUi.editor.graph.dblClick = function (evt, state) {
          VertexOnDblclick(evt, state, graph, t, dispatch, graphIndexId);
        };
        editorUi.editor.graph.jobnetIconDropHandler = function (dataObj) {
          setFormLoading(true);
          JobnetService.getJobnetOption({
            ignoreJobnetId: form.getFieldValue("id"),
          })
            .then((result) => {
              if (result.type === SERVICE_RESPONSE.OK) {
                setJobnetOption(result.detail.data);
                setJobnetDropHandlerObj(dataObj);
                setFormLoading(false);
                setJobnetIconSelectModalVisible(true);
              }
            })
            .catch((err) => {
              console.log(err);
            });
        };
      }
    }, [editorUi, VertexOnDblclick, t, iconSettingDisable]);

    return (
      <>
        <Spin size="large" spinning={formLoading}>
          {formType == FORM_TYPE.SCHEDULE ? (
            <ScheduleFormObject
              formId={OBJECT_CATEGORY.JOBNET}
              onFinishAction={() => { }}
              objectSlice="jobnetForm"
              isCalendar={false}
              formProperty={graphIndexId}
            />
          ) : (
            <FormObject
              formId={OBJECT_CATEGORY.JOBNET}
              objectSlice="jobnetForm"
              formProperty={graphIndexId}
            />
          )}
          <Spin
            size="large"
            spinning={formType === FORM_TYPE.SCHEDULE ? false : graphLoading}
          >
            <div className={graphId}></div>
          </Spin>
        </Spin>

        <DisplayModalList
          id={graphId}
          graph={graph}
          graphIndexId={graphIndexId}
          formDisable={iconSettingDisable}
        />
        <DisplayExecuteModalList />
        <Modal
          title={t("sel-jn")}
          width={"70%"}
          style={{ top: "35px" }}
          maskClosable={false}
          visible={jobnetIconSelectModalVisible}
          onOk={handleJobnetIconSelectOk}
          onCancel={handleJobnetIconSelectCancel}
          okText={t("btn-ok")}
          cancelText={t("btn-cancel")}
        >
          <TreeSelect
            showSearch
            treeLine={false}
            style={{
              width: "100%",
            }}
            value={selectJobnetIcon}
            dropdownStyle={{
              overflow: "auto",
            }}
            placeholder={t("lab-sel-day-plchold")}
            allowClear
            treeDefaultExpandAll
            onChange={onTreeSelectChange}
            listHeight={450}
          >
            {jobnetOption.hasOwnProperty("public") && (
              <TreeNode
                key="public"
                selectable={false}
                value="public"
                title={`${t("nav-public-jobnet")}`}
              >
                <TreeNode
                  key="public_header"
                  selectable={false}
                  disabled={true}
                  value="public_header"
                  title={
                    <div className="tree-combo-row">
                      <div className="w-100 border-top border-side font-bold combo-tree-item">
                        {t("col-obj-id")}
                      </div>
                      <div className="w-50 border-top border-right font-bold combo-tree-item">
                        {t("col-obj-valid")}
                      </div>
                      <div className="w-100 border-top border-right font-bold combo-tree-item">
                        {t("col-obj-name")}
                      </div>
                      <div className="w-100 border-top border-right font-bold combo-tree-item">
                        {t("col-obj-des")}
                      </div>
                    </div>
                  }
                />
                {jobnetOption.hasOwnProperty("public") &&
                  jobnetOption.public.map((item) => {
                    return (
                      <TreeNode
                        key={JSON.stringify(item)}
                        value={JSON.stringify(item)}
                        title={
                          <div className="tree-combo-row">
                            <div className="w-100 border-side combo-tree-item">
                              {item.jobnet_id}
                            </div>
                            <div className="w-50 border-right combo-tree-item">
                              {item.valid_flag == "1" ? "Yes" : "No"}
                            </div>
                            <div className="w-100 border-right combo-tree-item">
                              {item.jobnet_name}
                            </div>
                            <div className="w-100 border-right combo-tree-item">
                              {item.memo}
                            </div>
                          </div>
                        }
                      />
                    );
                  })}
              </TreeNode>
            )}
            {jobnetOption.hasOwnProperty("private") && (
              <TreeNode
                key="private"
                selectable={false}
                value="private"
                title={`${t("nav-private-jobnet")}`}
              >
                <TreeNode
                  key="private_header"
                  selectable={false}
                  disabled={true}
                  value="private_header"
                  title={
                    <div className="tree-combo-row">
                      <div className="w-100 border-top border-side font-bold combo-tree-item">
                        {t("col-obj-id")}
                      </div>
                      <div className="w-50 border-top border-right font-bold combo-tree-item">
                        {t("col-obj-valid")}
                      </div>
                      <div className="w-100 border-top border-right font-bold combo-tree-item">
                        {t("col-obj-name")}
                      </div>
                      <div className="w-100 border-top border-right font-bold combo-tree-item">
                        {t("col-obj-des")}
                      </div>
                    </div>
                  }
                />
                {jobnetOption.hasOwnProperty("private") &&
                  jobnetOption.private.map((item) => {
                    return (
                      <TreeNode
                        key={JSON.stringify(item)}
                        value={JSON.stringify(item)}
                        title={
                          <div className="tree-combo-row">
                            <div className="w-100 border-side combo-tree-item">
                              {item.jobnet_id}
                            </div>
                            <div className="w-50 border-right combo-tree-item">
                              {item.valid_flag == "1" ? "Yes" : "No"}
                            </div>
                            <div className="w-100 border-right combo-tree-item">
                              {item.jobnet_name}
                            </div>
                            <div className="w-100 border-right combo-tree-item">
                              {item.memo}
                            </div>
                          </div>
                        }
                      />
                    );
                  })}
              </TreeNode>
            )}
          </TreeSelect>
        </Modal>
      </>
    );
  }
);

export default JobnetForm;
