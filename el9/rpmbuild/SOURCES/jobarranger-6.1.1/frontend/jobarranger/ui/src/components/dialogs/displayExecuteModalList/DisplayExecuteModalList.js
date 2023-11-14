import { useSelector } from "react-redux";
import "../displayModalList/DisplayModalList.scss";
import JobExecutionDialog from "../jobExecutionDialog/JobExecutionDialog";

const DisplayExecuteModalList = () => {
  const executeJobData = useSelector(
    (state) => state.jobExecution.executeJobData
  );

  return (
    <>
      {executeJobData.map((dialog) => {
        return (
            <JobExecutionDialog key={dialog.id} id={dialog.id} cell={dialog}/>
        );
      })}
    </>
  );
};

export default DisplayExecuteModalList;
