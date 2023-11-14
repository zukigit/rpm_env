import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { Checkbox, Card } from "antd";
import OperationTableCard from "./OperationTableCard";
import { getAllOperationList } from "../../store/AllOperationListSlice";
import { getErrorOperationList } from "../../store/ErrorOperationListSlice";
import { getDuringOperationList } from "../../store/DuringOperationListSlice";
import DisplayExecuteModalList from "../../components/dialogs/displayExecuteModalList/DisplayExecuteModalList";
import UpdateScheduleDialog from "../../components/dialogs/updateScheduleDialog/UpdateScheduleDialog";
import "./JobExecutionManagement.scss";
import { SESSION_STORAGE } from "../../constants";

const JobExecutionManagement = ({ category = "jobnet", publicType = true }) => {
  const { t } = useTranslation();
  const [checkOptions, setCheckOptions] = useState(JSON.parse(sessionStorage.getItem(SESSION_STORAGE.JOB_EXEC_MANAGE_TABLE_VIEW)) ??[
    t("situation-list"),
    t("err-jobnet-list"),
    t("during-exec-list"),
  ]);
  const viewOptions = [
    t("situation-list"),
    t("err-jobnet-list"),
    t("during-exec-list"),
  ];

  const onChange = (checkedValues) => {
    sessionStorage.setItem(SESSION_STORAGE.JOB_EXEC_MANAGE_TABLE_VIEW,JSON.stringify(checkedValues))
    setCheckOptions(checkedValues);
  };

  const renderAllInfoTable = (checkOptions) => {
    if (checkOptions.includes(t("situation-list"))) {
      return (
        <>
          <OperationTableCard
            key={"all"}
            stateId={"allOperationList"}
            dispatchAction={getAllOperationList}
            tableType={t("title-op-info-job")}
            color={"#40a9ff"}
          />
          <UpdateScheduleDialog />
        </>
      );
    }
  };

  const renderErrorInfoTable = (checkOptions) => {
    if (checkOptions.includes(t("err-jobnet-list"))) {
      return (
        <OperationTableCard
          key={"error"}
          stateId={"errorOperationList"}
          dispatchAction={getErrorOperationList}
          tableType={t("title-op-err-job")}
          color={"#f00"}
        />
      );
    }
  };

  const renderDuringInfoTable = (checkOptions) => {
    if (checkOptions.includes(t("during-exec-list"))) {
      return (
        <OperationTableCard
          key={"warning"}
          stateId={"duringOperationList"}
          dispatchAction={getDuringOperationList}
          tableType={t("title-op-exe-job")}
          color={"#c4cf21"}
        />
      );
    }
  };

  return (
    <>
      <Card
        bordered={false}
        style={{
          height: 60,
        }}
      >
        <Checkbox.Group
          options={viewOptions}
          defaultValue={checkOptions}
          onChange={onChange}
        />
      </Card>
      {renderAllInfoTable(checkOptions)}
      {renderErrorInfoTable(checkOptions)}
      {renderDuringInfoTable(checkOptions)}
      <DisplayExecuteModalList />
    </>
  );
};

export default JobExecutionManagement;
