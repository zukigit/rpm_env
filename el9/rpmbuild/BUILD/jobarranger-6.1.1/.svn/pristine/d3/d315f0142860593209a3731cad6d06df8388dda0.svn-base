import { useSelector, useDispatch } from "react-redux";
import { ICON_TYPE } from "../../../constants";
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
import "./DisplayModalList.scss";
import JobnetIconDialog from "../iconSetting/jobnetIconDialog/JobnetIconDialog";
import JobExecutionDialog from "../jobExecutionDialog/JobExecutionDialog";
import JobnetSettingDialog from "../iconSetting/jobnetIconDialog/JobnetSettingDialog";

const DisplayModalList = ({ id, graph, graphIndexId, jobnetIconId = null, formDisable = false }) => {
    
    const modals = useSelector(state => state.jobnetForm.formObjList[graphIndexId].modalState.modals);

  return (
    <>
      {modals.map((dialog) => {
        switch (dialog.cellType) {
          case ICON_TYPE.JOB:
            return (
              <JobIconDialog key={dialog.key} graphIndexId={graphIndexId} graph={graph} id={dialog.id} cell={dialog} formDisable={formDisable} jobnetIconId={jobnetIconId} />
          );
          case ICON_TYPE.START:
            return (
              <StartIconDialog key={dialog.key} graphIndexId={graphIndexId} graph={graph} id={dialog.id} cell={dialog} formDisable={formDisable} jobnetIconId={jobnetIconId} />
          );
          case ICON_TYPE.CONDITIONAL_START:
            return (
              <ConditionalStartIconDialog key={dialog.key} graphIndexId={graphIndexId} graph={graph} id={dialog.id} cell={dialog} formDisable={formDisable} jobnetIconId={jobnetIconId} />
          );
          case ICON_TYPE.CONDITIONAL_END:
            return (
              <ConditionalEndIconDialog key={dialog.key} graphIndexId={graphIndexId} graph={graph} id={dialog.id} cell={dialog} formDisable={formDisable} jobnetIconId={jobnetIconId} />
          );
          case ICON_TYPE.PARALLEL_START:
            return (
              <ParallelStartIconDialog key={dialog.key} graphIndexId={graphIndexId} graph={graph} id={dialog.id} cell={dialog} formDisable={formDisable} jobnetIconId={jobnetIconId} />
          );
          case ICON_TYPE.PARALLEL_END:
            return (
              <ParallelEndIconDialog key={dialog.key} graphIndexId={graphIndexId} graph={graph} id={dialog.id} cell={dialog} formDisable={formDisable} jobnetIconId={jobnetIconId} />
          );
          case ICON_TYPE.JOB_CONTROL_VARIABLE:
            return (
              <EnvIconDialog key={dialog.key} graphIndexId={graphIndexId} graph={graph} id={dialog.id} cell={dialog} formDisable={formDisable} jobnetIconId={jobnetIconId} />
          );
          case ICON_TYPE.EXTENDED_JOB:
            return (
              <ExtendedJobIconDialog key={dialog.key} graphIndexId={graphIndexId} graph={graph} id={dialog.id} cell={dialog} formDisable={formDisable} jobnetIconId={jobnetIconId} />
          );
          case ICON_TYPE.END:
            return (
              <EndIconDialog key={dialog.key} graphIndexId={graphIndexId} graph={graph} id={dialog.id} cell={dialog} formDisable={formDisable} jobnetIconId={jobnetIconId} />
          );
          case ICON_TYPE.CALCULATION:
            return (
              <CalculationIconDialog key={dialog.key} graphIndexId={graphIndexId} graph={graph} id={dialog.id} cell={dialog} formDisable={formDisable} jobnetIconId={jobnetIconId} />
          );
          case ICON_TYPE.LOOP:
            return (
              <LoopIconDialog key={dialog.key} graphIndexId={graphIndexId} graph={graph} id={dialog.id} cell={dialog} formDisable={formDisable} jobnetIconId={jobnetIconId} />
          );
          case ICON_TYPE.TASK:
            return (
              <TaskIconDialog key={dialog.key} graphIndexId={graphIndexId} graph={graph} id={dialog.id} cell={dialog} formDisable={formDisable} jobnetIconId={jobnetIconId} />
          );
          case ICON_TYPE.INFO:
            return (
              <InfoIconDialog key={dialog.key} graphIndexId={graphIndexId} graph={graph} id={dialog.id} cell={dialog} formDisable={formDisable} jobnetIconId={jobnetIconId} />
          );
          case ICON_TYPE.FILE_COPY:
            return (
              <FileTransferIconDialog key={dialog.key} graphIndexId={graphIndexId} graph={graph} id={dialog.id} cell={dialog} formDisable={formDisable} jobnetIconId={jobnetIconId} />
          );
          case ICON_TYPE.FILE_WAIT:
            return (
              <FileWaitIconDialog key={dialog.key} graphIndexId={graphIndexId} graph={graph} id={dialog.id} cell={dialog} formDisable={formDisable} jobnetIconId={jobnetIconId} />
          );
          case ICON_TYPE.REBOOT:
            return (
              <RebootIconDialog key={dialog.key} graphIndexId={graphIndexId} graph={graph} id={dialog.id} cell={dialog} formDisable={formDisable} jobnetIconId={jobnetIconId} />
          );
          case ICON_TYPE.RELEASE:
            return (
              <ReleaseIconDialog key={dialog.key} graphIndexId={graphIndexId} graph={graph} id={dialog.id} cell={dialog} formDisable={formDisable} jobnetIconId={jobnetIconId} />
          );
          case ICON_TYPE.ZABBIX:
            return (
              <ZabbixIconDialog key={dialog.key} graphIndexId={graphIndexId} graph={graph} id={dialog.id} cell={dialog} formDisable={false} jobnetIconId={jobnetIconId} />
          );
          case ICON_TYPE.AGENT_LESS:
            return (
              <AgentLessIconDialog key={dialog.key} graphIndexId={graphIndexId} graph={graph} id={dialog.id} cell={dialog} formDisable={formDisable} jobnetIconId={jobnetIconId} />
          );
          case ICON_TYPE.JOBNET:
            if(dialog.isJobnetSetting){
              return (
                <JobnetSettingDialog key={dialog.key} graphIndexId={graphIndexId} graph={graph} id={dialog.id} cell={dialog} formDisable={formDisable} jobnetIconId={jobnetIconId} />
              );
            }else{
              return (
                <JobnetIconDialog key={dialog.key} graphIndexId={graphIndexId} formGraphId={dialog.formGraphId} parentGraph={graph} id={dialog.id} cell={dialog} jobnetIconId={jobnetIconId}/>
                );
            }
          case ICON_TYPE.EXECUTE_JOB:
            return (
              <JobExecutionDialog key={dialog.key} id={dialog.id} cell={dialog} graphIndexId={graphIndexId}/>
          );
        }
      })}
    </>
  );
};

export default DisplayModalList;
