import React from "react";
import { Button, Affix, Tooltip, Menu } from "antd";
import i18next, { t } from "i18next";
import { useDispatch, useSelector } from "react-redux";
import store from "../../store";
import { getErrorOperationList, hideDataOnTable, setSelectedObject, setSelectedRowKeys } from "../../store/ErrorOperationListSlice";
import { alertError } from "../dialogs/CommonDialog";
import JobExecutionService from "../../services/jobExecutionService";
import { LOAD_STATUS_TYPE, RUN_JOB_STATUS_TYPE, SERVICE_RESPONSE } from "../../constants";

const JobExecutionErrorTableContextMenu = ({ value, tableType, record }) => {
  const t = i18next.t;
  const dispatch = useDispatch();
  const selectedObject = useSelector(
    (state) => state.errorOperationList.selectedObject
  );
  var hideMenuItemErrDisabled = false;
  var stopMenuItemErrDisabled = true;
  var delayMenuItemErrDisabled = true;

  if (selectedObject.length > 0) {
    var statusFlag = 0;
    var detail = null;
    for (var i = 0; i < selectedObject.length; i++) {
      detail = selectedObject[i];
      if (
        parseInt(detail.intStatus) == RUN_JOB_STATUS_TYPE.DURING &&
        (statusFlag == 0 || statusFlag == 1)
      ) {
        stopMenuItemErrDisabled = false;
        statusFlag = 1;
      }

      if (parseInt(detail.loadStatus) == LOAD_STATUS_TYPE.DELAY && selectedObject.length == 1) {
        delayMenuItemErrDisabled = false;
        statusFlag = 1;
      }
    }
  }

  const jobExecManagementItems = [
    {
      id: "btnErrHide",
      label: t("btn-hide"),
      key: "item-1",
      onClick: () => {
        if (store.getState().errorOperationList.selectedRow.length === 0) {
          alertError("", t("err-msg-no-select-data"));
        } else {
          store.dispatch(hideDataOnTable());
        }
      },
      disabled: hideMenuItemErrDisabled,
    },
    {
      id: "btnErrStop",
      label: t("btn-stop"),
      key: "item-2",
      onClick: () => {
        if (store.getState().errorOperationList.selectedRow.length === 0) {
          alertError("", t("err-msg-no-select-data"));
        } else {
          JobExecutionService.stopErrorJobnetSummary({
            innerJobnetIdList: store.getState().errorOperationList.selectedRow
          }).then((res) => {
            store.dispatch(setSelectedRowKeys([]))
            store.dispatch(setSelectedObject([]))
            if (res.type === SERVICE_RESPONSE.OK) {
              dispatch(getErrorOperationList());
            } else {
              alertError("", res.error.message);
            }
          });
        }
      },
      disabled: stopMenuItemErrDisabled,
    },
    {
      id: "btnErrDelay",
      label: t("btn-delay"),
      key: "item-3",
      onClick: () => {
        if (store.getState().errorOperationList.selectedRow.length === 0) {
          alertError("", t("err-msg-no-select-data"));
        } else {
          JobExecutionService.delayJobnetSummary({
            innerJobnetId: store.getState().errorOperationList.selectedRow[0],
            loadStatus: store.getState().errorOperationList.selectedObject[0].loadStatus,
          }).then((res) => {
            store.dispatch(setSelectedRowKeys([]))
            store.dispatch(setSelectedObject([]))
            if (res.type === SERVICE_RESPONSE.OK) {
              dispatch(getErrorOperationList());
            } else {
              alertError("", res.error.message);
            }
          });
        }
      },
      disabled: delayMenuItemErrDisabled,
    },
  ];

  return <Menu items={jobExecManagementItems} />;
};
export default JobExecutionErrorTableContextMenu;
