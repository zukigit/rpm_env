import React from "react";
import { Button, Affix, Tooltip, Menu } from "antd";
import i18next, { t } from "i18next";
import { useDispatch, useSelector } from "react-redux";
import store from "../../store";
import { getDuringOperationList, hideDataOnTable, setSelectedObject, setSelectedRowKeys } from "../../store/DuringOperationListSlice";
import { alertError } from "../dialogs/CommonDialog";
import JobExecutionService from "../../services/jobExecutionService";
import { LOAD_STATUS_TYPE, SERVICE_RESPONSE } from "../../constants";

const JobExecutionDuringTableContextMenu = ({ value, tableType, record }) => {
  const t = i18next.t;
  const dispatch = useDispatch();
  const selectedItem = useSelector((state) => state.duringOperationList.selectedObject);
  var hideMenuItemRunningDisabled = true;
  var stopMenuItemRunningDisabled = true;
  var delayMenuItemRunningDisabled = true;

  if (selectedItem.length > 0) {
    var detail = selectedItem[0];

    stopMenuItemRunningDisabled = false;
    if (parseInt(detail.loadStatus) == LOAD_STATUS_TYPE.DELAY) {
      delayMenuItemRunningDisabled = false;
    }
  }

  const jobExecManagementItems = [
    {
      id: "btnDurHide",
      label: t("btn-hide"),
      key: "item-1",
      onClick: () => {
        if (store.getState().duringOperationList.selectedRow.length === 0) {
          alertError("", t("err-msg-no-select-data"));
        } else {
          store.dispatch(hideDataOnTable());
        }
      },
      disabled: hideMenuItemRunningDisabled,
    },
    {
      id: "btnDurStop",
      label: t("btn-stop"),
      key: "item-2",
      onClick: () => {
        if (store.getState().duringOperationList.selectedRow.length === 0) {
          alertError("", t("err-msg-no-select-data"));
        } else {
          JobExecutionService.stopDuringJobnetSummary({
            innerJobnetId: store.getState().duringOperationList.selectedRow[0],
          }).then((res) => {
            store.dispatch(setSelectedRowKeys([]))
            store.dispatch(setSelectedObject([]))
            if (res.type === SERVICE_RESPONSE.OK) {
              dispatch(getDuringOperationList());
            } else {
              alertError("", res.error.message);
            }
          });
        }
      },
      disabled: stopMenuItemRunningDisabled
    },
    {
      id: "btnDurDelay",
      label: t("btn-delay"),
      key: "item-3",
      onClick: () => {
        if (store.getState().duringOperationList.selectedRow.length === 0) {
          alertError("", t("err-msg-no-select-data"));
        } else {
          JobExecutionService.delayJobnetSummary({
            innerJobnetId: store.getState().duringOperationList.selectedRow[0],
            loadStatus: store.getState().duringOperationList.selectedObject[0].loadStatus,
          }).then((res) => {
            store.dispatch(setSelectedRowKeys([]))
            store.dispatch(setSelectedObject([]))
            if (res.type === SERVICE_RESPONSE.OK) {
              dispatch(getDuringOperationList());
            } else {
              alertError("", res.error.message);
            }
          });
        }
      },
      disabled: delayMenuItemRunningDisabled
    },
  ];

  return <Menu items={jobExecManagementItems} />;
};
export default JobExecutionDuringTableContextMenu;
