import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { ICON_TYPE } from "../constants";

const initialState = {
  loading: false,
  maxZindex: 5,
  executeJobData: [],
};

export const jobExecutionSlice = createSlice({
  name: "jobExecution",
  initialState,
  reducers: {
    addExecuteJob: (state, action) => {
      state.executeJobData = [...state.executeJobData, action.payload];
    },
    removeExecuteJob: (state, action) => {
      state.executeJobData = state.executeJobData.filter((dialog) => {
        return dialog.id !== action.payload;
      });
    },
    removeAllExecuteJob: (state, action) => {
      state.executeJobData = []
    },
    setMaxZindex: (state, action) => {
      state.maxZindex = action.payload;
    },
    setExecModalZindex: (state, action) => {
      state.maxZindex = state.maxZindex + 1;
      state.executeJobData[action.payload.executeJobIndex].modalState.modals[
        action.payload.index
      ].zindex = state.maxZindex + 1;
    },
    setExecuteJobModalZindex: (state, action) => {
      state.maxZindex = state.maxZindex + 1;
      state.executeJobData[action.payload].zindex = state.maxZindex + 1;
    },
    addExecOpenDialog: (state, action) => {
      state.executeJobData[action.payload.index].modalState.modals = [
        ...state.executeJobData[action.payload.index].modalState.modals,
        action.payload.modal,
      ];
    },
    removeExecOpenDialog: (state, action) => {
      const index = state.executeJobData.findIndex((dialog) => {
        return dialog.id === action.payload.innerJobnetId;
      });
      state.executeJobData[index].modalState.modals = state.executeJobData[
        index
      ].modalState.modals.filter((dialog) => {
        return dialog.id !== action.payload.id;
      });
    },
  },
});

export const openExecutionDialog = (id) => (dispatch, getState) => {
  const rootState = getState().jobExecution;

  const isFound = rootState.executeJobData.some((dialog) => {
    if (dialog.id === id) {
      return true;
    }
    return false;
  });

  if(!isFound){
    dispatch(
      addExecuteJob({
        id,
        modalState: {
          modals: [],
        },
        zindex: rootState.maxZindex + 1,
        childId: null,
        parentId: null,
      })
    );
    dispatch(setMaxZindex(rootState.maxZindex + 1));
  }
  
};

export const getExecJobNextZIndex = (dialogObj) => (dispatch, getState) => {
  const rootState = getState().jobExecution;
  if (rootState.maxZindex !== dialogObj.zindex) {
    const index = rootState.executeJobData.findIndex((dialog) => {
      return dialog.id === dialogObj.id;
    });
    dispatch(setExecuteJobModalZindex(index));
  }
};

export const removeExecOpenDialogForChild = (dialogObj) => (dispatch, getState) => {
  const rootState = getState().jobExecution;
  const isFound = rootState.executeJobData.some((dialog) => {
    if (dialog.id === dialogObj.innerJobnetId) {
      return true;
    }
    return false;
  });
  if(isFound){
    dispatch(removeExecOpenDialog(dialogObj));
  }
};

export const openExecIconSettingDialog =
  (dialogObj, innerJobnetId) => (dispatch, getState) => {
    const rootState = getState().jobExecution;

    const index = rootState.executeJobData.findIndex((dialog) => {
      return dialog.id === innerJobnetId;
    });

    const isFound = rootState.executeJobData[index].modalState.modals.some(
      (dialog) => {
        if (dialog.id === dialogObj.id) {
          return true;
        }
        return false;
      }
    );

    if (!isFound) {
      if (dialogObj.cellType === ICON_TYPE.JOBNET) {
        dispatch(
          addExecuteJob({
            id: dialogObj.innerJobnetId,
            modalState: {
              modals: [],
            },
            zindex: rootState.maxZindex + 1,
            childId: dialogObj.id,
            parentId: innerJobnetId,
          })
        );
      }
      dispatch(
        addExecOpenDialog({
          index,
          modal: {
            ...dialogObj,
            zindex: rootState.maxZindex + 1,
          },
        })
      );
      dispatch(setMaxZindex(rootState.maxZindex + 1));
    }
  };

export const getExecIconSettingNextZIndex =
  (dialogObj, innerJobnetId) => (dispatch, getState) => {
    const executeJobData = getState().jobExecution.executeJobData;
    const executeJobIndex = executeJobData.findIndex((dialog) => {
      return dialog.id === innerJobnetId;
    });
    if (executeJobData[executeJobIndex].modalState.maxZindex !== dialogObj.zindex) {
      const index = executeJobData[executeJobIndex].modalState.modals.findIndex((dialog) => {
        return dialog.id === dialogObj.id;
      });
      dispatch(setExecModalZindex({ index, executeJobIndex }));
    }
  };

export default jobExecutionSlice.reducer;

export const {
  addExecuteJob,
  removeExecuteJob,
  removeAllExecuteJob,
  setMaxZindex,
  setExecModalZindex,
  setExecuteJobModalZindex,
  addExecOpenDialog,
  removeExecOpenDialog,
} = jobExecutionSlice.actions;
