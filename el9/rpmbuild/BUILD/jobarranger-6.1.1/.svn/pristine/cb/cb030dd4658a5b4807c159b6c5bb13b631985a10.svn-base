import React from "react";
import { Menu, Dropdown, Tag, message } from "antd";
import i18next from "i18next";
import {
  EXECUTION_MANAGEMENT,
  ICON_TYPE,
  OBJECT_CATEGORY,
  RESPONSE_OBJ_KEY,
  RUN_TYPE,
  SERVICE_RESPONSE,
} from "../../constants";
import JobExecutionService from "../../services/jobExecutionService";
import { alertError, alertInfo, confirmDialog } from "../dialogs/CommonDialog";
import store from "../../store";
import { openExecutionDialog } from "../../store/JobExecutionSlice";
import JobExecutionAllTableContextMenu from "./JobExecutionAllTableContextMenu";
import JobExecutionErrorTableContextMenu from "./JobExecutionErrorTableContextMenu";
import JobExecutionDuringTableContextMenu from "./JobExecutionDuringTableContextMenu";

const executeAction = (data, runType, t) => {
  let selectedObj = store.getState().objectList.selectedObject;
  if (selectedObj.length > 1) {
    alertError(t("title-error"), t("err-msg-shortcut-mulit-selected"));
    return;
  }
  JobExecutionService.checkValid({ id: selectedObj[0].objectId })
    .then((res) => {
      if (res.type === SERVICE_RESPONSE.OK) {
        if (res.detail.message === SERVICE_RESPONSE.VALID) {
          const onOk = () => {
            const hideMessage = message.loading(t("during-jobnet-execute"), 0);
            JobExecutionService.run({ id: selectedObj[0].objectId, runType: runType })
              .then((res) => {
                hideMessage();
                if (res.type == SERVICE_RESPONSE.INCOMEPLETE) {
                  if (res.detail.hasOwnProperty(RESPONSE_OBJ_KEY.MESSAGE_CODE)) {
                    alertError(t("title-error"), t(res.detail[RESPONSE_OBJ_KEY.MESSAGE_CODE]));
                  } else {
                    alertError(t("title-error"), res.detail.message);
                  }

                } else {
                  store.dispatch(openExecutionDialog(res.result.innerJobnetId));
                }
              })
              .catch((err) => {
                alertError(t("title-error"), err.message);
              });
          };

          const onCancel = () => { };
          confirmDialog(
            t("title-msg-confirm"),
            t("lab-jobnet-run"),
            onOk,
            onCancel
          );
        } else if (res.detail.message === SERVICE_RESPONSE.INVALID) {
          alertError(t("title-error"), t("err-msg-cannot-exec"));
        }
      }
    })
    .catch((err) => { });
};



const jobnetObjectMenu = (value, col) => {

  const t = i18next.t;
  const runMenuItems = [
    {
      id: "btnObjImrun",
      label: t("col-obj-im-run"),
      key: "item-1",
      onClick: (value) => {
        executeAction(col, RUN_TYPE.IMMEDIATE_RUN, t);
      },
    },
    {
      id: "btnObjImrunHold",
      label: t("col-obj-im-run-hold"),
      key: "item-2",
      onClick: (value) => {
        executeAction(col, RUN_TYPE.IMMEDIATE_RUN_HOLD, t);
      },
    },
    {
      id: "btnObjTestRun",
      label: t("col-obj-test-run"),
      key: "item-3",
      onClick: (value) => {
        executeAction(col, RUN_TYPE.TEST_RUN, t);
      },
    },
  ];

  return <Menu items={runMenuItems} />;
};

function getRunJobStatusColor(
  status,
  job_status,
  load_status,
  start_pending_flag
) {
  let color = "#7FFFD4";
  switch (parseInt(status)) {
    case 0:
      switch (parseInt(start_pending_flag)) {
        case 1:
          color = "#4169E1";
          break;
      }
      break;
    case 1:
      break;
    case 2:
    case 6:
      switch (parseInt(job_status)) {
        case 0:
          color = "#d8d818";
          break;
        case 1:
          color = "#FFA500";
          break;
        case 2:
          color = "#FF0000";
          break;
      }
      break;
    case 3:
      switch (parseInt(job_status)) {
        case 0:
          if (parseInt(load_status) != 3) {
            color = "#afe214";
          } else {
            color = "#808080";
          }
          break;
        case 1:
          color = "#FFA500";
          break;
        case 2:
          color = "#FF0000";
          break;
      }
      break;
    case 4:
    case 5:
      color = "#FF0000";
      break;
  }
  return color;
}

export const checkRowForRender = (
  category,
  text,
  col,
  dataIndex,
  tableType
) => {
  const t = i18next.t;
  if (category === OBJECT_CATEGORY.JOBNET && tableType !== "objectVersion") {
    return jobnetObjectRender(text, col);
  } else if (category === EXECUTION_MANAGEMENT) {
    if (dataIndex === "status") {
      return jobExecutionManagementStatusRender(text, col, tableType, t);
    } else {
      return jobExecutionManagementMenuRender(text, col, tableType, t);
    }
  } else {
    return <>{text}</>;
  }
};

export const jobExecutionManagementMenuRender = (
  value,
  record,
  tableType,
) => {
  const t = i18next.t;
  let overlay = "";
  if (tableType === t("title-op-info-job")) {
    overlay = (
      <JobExecutionAllTableContextMenu
        value={value}
        record={record}
        tableType={tableType}
      />
    );
  } else if (tableType === t("title-op-err-job")) {
    overlay = (
      <JobExecutionErrorTableContextMenu
        value={value}
        record={record}
        tableType={tableType}
      />
    );
  } else {
    overlay = (
      <JobExecutionDuringTableContextMenu
        value={value}
        record={record}
        tableType={tableType}
      />
    );
  }
  return (
    <Dropdown overlay={overlay} trigger={[`contextMenu`]}>
      <div>{value}</div>
    </Dropdown>
  );
};

export const jobExecutionManagementStatusRender = (
  value,
  record,
  tableType
) => {
  const t = i18next.t;
  let overlay = "";
  if (tableType === t("title-op-info-job")) {
    overlay = (
      <JobExecutionAllTableContextMenu
        value={value}
        record={record}
        tableType={tableType}
      />
    );
  } else if (tableType === t("title-op-err-job")) {
    overlay = (
      <JobExecutionErrorTableContextMenu
        value={value}
        record={record}
        tableType={tableType}
      />
    );
  } else {
    overlay = (
      <JobExecutionDuringTableContextMenu
        value={value}
        record={record}
        tableType={tableType}
      />
    );
  }
  return (
    <Dropdown overlay={overlay} trigger={[`contextMenu`]}>
      <Tag
        color={getRunJobStatusColor(
          record.intStatus,
          record.jobStatus,
          record.loadStatus,
          record.startPendingFlag
        )}
      >
        {value}
      </Tag>
    </Dropdown>
  );
};

export const jobnetObjectRender = (value, col) => {
  return (
    <Dropdown overlay={jobnetObjectMenu(value, col)} trigger={[`contextMenu`]}>
      <div style={{ height: "27px" }}>{value}</div>
    </Dropdown>
  );
};
