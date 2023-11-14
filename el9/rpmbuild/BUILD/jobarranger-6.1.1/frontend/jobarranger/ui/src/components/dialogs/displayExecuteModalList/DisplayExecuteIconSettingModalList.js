import { useSelector, useDispatch } from "react-redux";
import "../displayModalList/DisplayModalList.scss";
import JobIconDialog from "../iconSetting/jobIconDialog/JobIconDialog";
import StartIconDialog from "../iconSetting/startIconDialog/StartIconDialog";
import ConditionalStartIconDialog from "../iconSetting/conditionalStartIconDialog/ConditionalStartIconDialog";
import ConditionalEndIconDialog from "../iconSetting/conditionalEndIconDialog/ConditionalEndIconDialog";
import ParallelStartIconDialog from "../iconSetting/parallelStartIconDialog/ParallelStartIconDialog";
import ParallelEndIconDialog from "../iconSetting/parallelEndIconDialog/ParallelEndIconDialog";
import EnvIconDialog from "../iconSetting/envIconDialog/EnvIconDialog";
import ExtendedJobIconDialog from "../iconSetting/extendedJobIcon/ExtendedJobIconDialog";
import EndIconDialog from "../iconSetting/endIconDialog/EndIconDialog";
import CalculationIconDialog from "../iconSetting/calculationIconDialog/CalculationIconDialog";
import LoopIconDialog from "../iconSetting/loopIconDialog/LoopIconDialog";
import TaskIconDialog from "../iconSetting/taskIconDialog/TaskIconDialog";
import InfoIconDialog from "../iconSetting/infoIconDialog/InfoIconDialog";
import FileTransferIconDialog from "../iconSetting/fileTransferIconDialog/FileTransferIconDialog";
import FileWaitIconDialog from "../iconSetting/fileWaitIconDialog/FileWaitIconDialog";
import RebootIconDialog from "../iconSetting/rebootIconDialog/RebootIconDialog";
import ReleaseIconDialog from "../iconSetting/releaseIconDialog/ReleaseIconDialog";
import ZabbixIconDialog from "../iconSetting/zabbixIconDialog/ZabbixIconDialog";
import AgentLessIconDialog from "../iconSetting/agentLessIconDialog/AgentLessIconDialog";
import JobnetIconDialog from "../iconSetting/jobnetIconDialog/JobnetIconDialog";
import JobExecutionDialog from "../jobExecutionDialog/JobExecutionDialog";
import { ICON_TYPE } from "../../../constants";
import ViewVariableValueDialog from "../executionContextMenu/viewVariableValueDialog/ViewVariableValueDialog";
import VariableValueChangeDialog from "../executionContextMenu/variableValueChangeDialog/VariableValueChangeDialog";
import JobnetSettingDialog from "../iconSetting/jobnetIconDialog/JobnetSettingDialog";

const DisplayExecuteIconSettingModalList = ({ id, graph, innerJobnetId, graphIndexId }) => {
    
    const modals = useSelector(state => state.jobExecution.executeJobData.find(job => job.id === innerJobnetId).modalState.modals);

  return (
    <>
      {modals.map((dialog) => {
        switch (dialog.cellType) {
          case ICON_TYPE.JOB:
            return (
              <JobIconDialog key={id+dialog.id} graph={graph} id={dialog.id} cell={dialog} formDisable={true} innerJobnetId={innerJobnetId} />
          );
          case ICON_TYPE.START:
            return (
              <StartIconDialog key={id+dialog.id} graph={graph} id={dialog.id} cell={dialog} formDisable={true} innerJobnetId={innerJobnetId} />
          );
          case ICON_TYPE.CONDITIONAL_START:
            return (
              <ConditionalStartIconDialog key={id+dialog.id} graph={graph} id={dialog.id} cell={dialog} formDisable={true} innerJobnetId={innerJobnetId} />
          );
          case ICON_TYPE.CONDITIONAL_END:
            return (
              <ConditionalEndIconDialog key={id+dialog.id} graph={graph} id={dialog.id} cell={dialog} formDisable={true} innerJobnetId={innerJobnetId} />
          );
          case ICON_TYPE.PARALLEL_START:
            return (
              <ParallelStartIconDialog key={id+dialog.id} graph={graph} id={dialog.id} cell={dialog} formDisable={true} innerJobnetId={innerJobnetId} />
          );
          case ICON_TYPE.PARALLEL_END:
            return (
              <ParallelEndIconDialog key={id+dialog.id} graph={graph} id={dialog.id} cell={dialog} formDisable={true} innerJobnetId={innerJobnetId} />
          );
          case ICON_TYPE.JOB_CONTROL_VARIABLE:
            return (
              <EnvIconDialog key={id+dialog.id} graph={graph} id={dialog.id} cell={dialog} formDisable={true} innerJobnetId={innerJobnetId} />
          );
          case ICON_TYPE.EXTENDED_JOB:
            return (
              <ExtendedJobIconDialog key={id+dialog.id} graph={graph} id={dialog.id} cell={dialog} formDisable={true} innerJobnetId={innerJobnetId} />
          );
          case ICON_TYPE.END:
            return (
              <EndIconDialog key={id+dialog.id} graph={graph} id={dialog.id} cell={dialog} formDisable={true} innerJobnetId={innerJobnetId} />
          );
          case ICON_TYPE.CALCULATION:
            return (
              <CalculationIconDialog key={id+dialog.id} graph={graph} id={dialog.id} cell={dialog} formDisable={true} innerJobnetId={innerJobnetId} />
          );
          case ICON_TYPE.LOOP:
            return (
              <LoopIconDialog key={id+dialog.id} graph={graph} id={dialog.id} cell={dialog} formDisable={true} innerJobnetId={innerJobnetId} />
          );
          case ICON_TYPE.TASK:
            return (
              <TaskIconDialog key={id+dialog.id} graph={graph} id={dialog.id} cell={dialog} formDisable={true} innerJobnetId={innerJobnetId} />
          );
          case ICON_TYPE.INFO:
            return (
              <InfoIconDialog key={id+dialog.id} graph={graph} id={dialog.id} cell={dialog} formDisable={true} innerJobnetId={innerJobnetId} />
          );
          case ICON_TYPE.FILE_COPY:
            return (
              <FileTransferIconDialog key={id+dialog.id} graph={graph} id={dialog.id} cell={dialog} formDisable={true} innerJobnetId={innerJobnetId} />
          );
          case ICON_TYPE.FILE_WAIT:
            return (
              <FileWaitIconDialog key={id+dialog.id} graph={graph} id={dialog.id} cell={dialog} formDisable={true} innerJobnetId={innerJobnetId} />
          );
          case ICON_TYPE.REBOOT:
            return (
              <RebootIconDialog key={id+dialog.id} graph={graph} id={dialog.id} cell={dialog} formDisable={true} innerJobnetId={innerJobnetId} />
          );
          case ICON_TYPE.RELEASE:
            return (
              <ReleaseIconDialog key={id+dialog.id} graph={graph} id={dialog.id} cell={dialog} formDisable={true} innerJobnetId={innerJobnetId} />
          );
          case ICON_TYPE.ZABBIX:
            return (
              <ZabbixIconDialog key={id+dialog.id} graph={graph} id={dialog.id} cell={dialog} formDisable={true} innerJobnetId={innerJobnetId} />
          );
          case ICON_TYPE.AGENT_LESS:
            return (
              <AgentLessIconDialog key={id+dialog.id} graph={graph} id={dialog.id} cell={dialog} formDisable={true} innerJobnetId={innerJobnetId} />
          );
          case ICON_TYPE.VIEW_VAR_VALUE:
            return (
              <ViewVariableValueDialog key={id+dialog.id} graph={graph} id={dialog.id} cell={dialog} innerJobnetId={innerJobnetId} />
          );
          case ICON_TYPE.VAR_VALUE_CHANGE:
            return (
              <VariableValueChangeDialog key={id+dialog.id} graph={graph} id={dialog.id} cell={dialog} innerJobnetId={innerJobnetId} />
          );
        }
      })}
    </>
  );
};

export default DisplayExecuteIconSettingModalList;
