import { configureStore } from '@reduxjs/toolkit'
import ObjectListReducer from './ObjectListSlice'
import AllOperationListReducer from './AllOperationListSlice'
import ErrorOperationListReducer from './ErrorOperationListSlice'
import DuringOperationListReducer from './DuringOperationListSlice'
import LockManagementListReducer from "./LockManagementSlice";
import JobnetFormReducer from './JobnetFormSlice'
import JobnetInitDataReducer from './JobnetInitDataSlice'
import JobExecutionResultReducer from './JobExecutionResultSlice'
import CalendarReducer from "./CalendarSlice";
import FilterReducer from "./FilterSlice";
import FormObjectReducer from "./FormObjectSlice";
import ScheduleFormReducer from './ScheduleFormSlice'
import UserReducer from './UserSlice'
import JobExecutionReducer from './JobExecutionSlice'
import ResponseReducer from './ResponseSLice'

const store = configureStore({
  reducer: {
    user: UserReducer,
    objectList: ObjectListReducer,
    allOperationList: AllOperationListReducer,
    errorOperationList: ErrorOperationListReducer,
    lockedObjList: LockManagementListReducer,
    duringOperationList: DuringOperationListReducer,
    jobnetForm: JobnetFormReducer,
    jobExecution: JobExecutionReducer,
    jobnetInitData: JobnetInitDataReducer,
    jobExecutionResultList: JobExecutionResultReducer,
    calendar:CalendarReducer,
    filter:FilterReducer,
    formObject:FormObjectReducer,
    schedule:ScheduleFormReducer,
    responseData:ResponseReducer,
  },
});

export default store;
