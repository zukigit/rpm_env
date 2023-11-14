import React from "react";
import { Button, Affix, Tooltip, Menu } from "antd";
import i18next, { t } from "i18next";
import { useDispatch, useSelector } from "react-redux";
import store from "../../store";
import {
  getAllOperationList,
  hideDataOnTable,
  setSelectedRowKeys,
  setSelectedObject,
  updateVisibleScheduleDialog,
} from "../../store/AllOperationListSlice";
import { alertError, alertInfo, confirmDialog } from "../dialogs/CommonDialog";
import JobExecutionService from "../../services/jobExecutionService";
import { LOAD_STATUS_TYPE, RESPONSE_OBJ_KEY, RUN_JOB_STATUS_TYPE, SERVICE_RESPONSE, START_PEND_FLAG } from "../../constants";

const JobExecutionAllTableContextMenu = ({ value, tableType, record }) => {
  const t = i18next.t;
  const dispatch = useDispatch();
  const detail = useSelector((state) => state.allOperationList.selectedObject);
  var hideMenuItemDisabled = true;
  var stopMenuItemDisabled = true;
  var delayMenuItemDisabled = true;
  var updateScheduleMenuItemDisabled = true;
  var holdMenuItemDisabled = true;
  var releaseMenuItemDisabled = true;
  var scheduleDeleteMenuItemDisabled = true;

  if (detail.length > 0) {
    if (
      parseInt(detail[0].intStatus) == RUN_JOB_STATUS_TYPE.NONE ||
      parseInt(detail[0].intStatus) == RUN_JOB_STATUS_TYPE.DURING
    ) {
      stopMenuItemDisabled = false;
    }

    if (
      parseInt(detail[0].loadStatus) == LOAD_STATUS_TYPE.DELAY &&
      parseInt(detail[0].intStatus) == RUN_JOB_STATUS_TYPE.DURING
    ) {
      delayMenuItemDisabled = false;
    }

    if (parseInt(detail[0].intStatus) == RUN_JOB_STATUS_TYPE.NONE) {
      updateScheduleMenuItemDisabled = false;
      holdMenuItemDisabled = false;
      if (parseInt(detail[0].startPendingFlag) == START_PEND_FLAG.NORMAL) {
        scheduleDeleteMenuItemDisabled = false;
      }
    } else {
      updateScheduleMenuItemDisabled = true;
      holdMenuItemDisabled = true;
    }

    if (parseInt(detail[0].startPendingFlag) == START_PEND_FLAG.PENDING) {
      if (parseInt(detail[0].intStatus) == RUN_JOB_STATUS_TYPE.NONE) {
        releaseMenuItemDisabled = false;
      }
      holdMenuItemDisabled = true;
    } else {
      releaseMenuItemDisabled = true;
    }
  }

  const jobExecManagementItems = [
    {
      id: "btnhide",
      label: t("btn-hide"),
      key: "item-1",
      onClick: () => {
        if (store.getState().allOperationList.selectedRow.length === 0) {
          alertError(t("title-error"), t("err-msg-no-select-data"));
        } else {
          store.dispatch(hideDataOnTable());
        }
      },
      disabled: hideMenuItemDisabled,
    },
    {
      id: "btnstop",
      label: t("btn-stop"),
      key: "item-2",
      onClick: () => {
        if (store.getState().allOperationList.selectedRow.length === 0) {
          alertError(t("title-error"), t("err-msg-no-select-data"));
        } else {
          JobExecutionService.stopAllJobnetSummary({
            innerJobnetId: store.getState().allOperationList.selectedRow[0],
            status:
              store.getState().allOperationList.selectedObject[0].intStatus,
          }).then((res) => {
            store.dispatch(setSelectedRowKeys([]))
            store.dispatch(setSelectedObject([]))
            if (res.type === SERVICE_RESPONSE.OK) {
              dispatch(getAllOperationList());
            } else {
              alertError(t("title-error"), res.error.message);
            }
          });
        }
      },
      disabled: stopMenuItemDisabled,
    },
    {
      id: "btndelay",
      label: t("btn-delay"),
      key: "item-3",
      onClick: () => {
        if (store.getState().allOperationList.selectedRow.length === 0) {
          alertError(t("title-error"), t("err-msg-no-select-data"));
        } else {
          JobExecutionService.delayJobnetSummary({
            innerJobnetId: store.getState().allOperationList.selectedRow[0],
            loadStatus:
              store.getState().allOperationList.selectedObject[0].loadStatus,
          }).then((res) => {
            if (res.type === SERVICE_RESPONSE.OK) {
              dispatch(getAllOperationList());
            } else {
              alertError(t("title-error"), res.error.message);
            }
          });
        }
      },
      disabled: delayMenuItemDisabled,
    },
    {
      id: 'btnupdschd',
      label: t("btn-upd-schd"),
      key: "item-4",
      onClick: () => {
        if (store.getState().allOperationList.selectedRow.length === 0) {
          alertError(t("title-error"), t("err-msg-no-select-data"));
        } else {
          dispatch(updateVisibleScheduleDialog(true));
        }
      },
      disabled: updateScheduleMenuItemDisabled,
    },
    {
      id: "btnhold",
      label: t("btn-hold"),
      key: "item-5",
      onClick: () => {
        if (store.getState().allOperationList.selectedRow.length === 0) {
          alertError(t("title-error"), t("err-msg-no-select-data"));
        } else {
          JobExecutionService.holdJobnetSummary({
            innerJobnetId: store.getState().allOperationList.selectedRow[0],
          }).then((res) => {
            store.dispatch(setSelectedRowKeys([]))
            store.dispatch(setSelectedObject([]))
            if (res.type === SERVICE_RESPONSE.OK) {
              dispatch(getAllOperationList());
            } else {
              alertError(t("title-error"), res.error.message);
            }
          });
        }
      },
      disabled: holdMenuItemDisabled,
    },
    {
      id: "btnrelease",
      label: t("btn-release"),
      key: "item-6",
      onClick: () => {
        if (store.getState().allOperationList.selectedRow.length === 0) {
          alertError(t("title-error"), t("err-msg-no-select-data"));
        } else {
          JobExecutionService.releaseJobnetSummary({
            innerJobnetId: store.getState().allOperationList.selectedRow[0],
          }).then((res) => {
            store.dispatch(setSelectedRowKeys([]))
            store.dispatch(setSelectedObject([]))
            if (res.type === SERVICE_RESPONSE.OK) {
              dispatch(getAllOperationList());
            } else {
              alertError(t("title-error"), res.error.message);
            }
          });
        }
      },
      disabled: releaseMenuItemDisabled,
    },
    {
      id: "btnschddel",
      label: t("btn-schd-del"),
      key: "item-7",
      onClick: () => {
        if (store.getState().allOperationList.selectedRow.length === 0) {
          alertError(t("title-error"), t("err-msg-no-select-data"));
        } else {
          if (store.getState().allOperationList.selectedObject[0].scheduleId === null) {
            alertError(t("title-error"), t("err-msg-no-schedule-id"));
            return;
          }
          JobExecutionService.checkScheduleValid({
            scheduleId:
              store.getState().allOperationList.selectedObject[0].scheduleId,
          }).then((res) => {
            if (res.type === SERVICE_RESPONSE.OK) {
              if (res.detail.message === SERVICE_RESPONSE.INVALID) {
                confirmDialog(
                  t("title-msg-conf"),
                  t("warn-msg-del"),
                  () => {
                    JobExecutionService.deleteSchedule({
                      innerJobnetId:
                        store.getState().allOperationList.selectedRow[0],
                      scheduleId:
                        store.getState().allOperationList.selectedObject[0]
                          .scheduleId,
                    }).then((res) => {
                      if (res.type === SERVICE_RESPONSE.OK) {
                        store.dispatch(setSelectedRowKeys([]))
                        store.dispatch(setSelectedObject([]))
                        dispatch(getAllOperationList());
                      } else {
                        if (res.type === SERVICE_RESPONSE.INCOMEPLETE) {
                          if (res.detail.hasOwnProperty(RESPONSE_OBJ_KEY.MESSAGE_CODE)) {
                            alertInfo(t("title-info"), t(res.detail[RESPONSE_OBJ_KEY.MESSAGE_CODE]));
                          } else {
                            alertError(
                              t("title-error"),
                              t("err-msg-schedule-enable", {
                                id: store.getState().allOperationList
                                  .selectedObject[0].scheduleId,
                              })
                            );
                          }

                        }
                      }
                    });
                  },
                  () => { }
                );
              } else {
                alertError(
                  t("title-error"),
                  t("err-msg-schedule-enable", {
                    id: store.getState().allOperationList.selectedObject[0]
                      .scheduleId,
                  })
                );
              }
            } else {
            }
          });
        }
      },
      disabled: scheduleDeleteMenuItemDisabled,
    },
  ];

  return <Menu items={jobExecManagementItems} />;
};
export default JobExecutionAllTableContextMenu;
