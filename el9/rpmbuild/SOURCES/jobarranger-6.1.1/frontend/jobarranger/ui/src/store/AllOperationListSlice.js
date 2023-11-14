import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { SERVICE_RESPONSE } from "../constants";
import jobExecutionManagementService from "../services/JobExecutionManagementService";

const initialState = {
  loading: false,
  data: [],
  selectedRow: [],
  selectedObject: [],
  hideRow: [],
  updateScheduleDialogVisible: false,
  intervelId: null
};

export const getAllOperationList = createAsyncThunk(
  "allOperationList/getAllOperationList",
  async (intervalId, {reject}) => {
    const response = await jobExecutionManagementService.getAllOperationList();
    if(response.type === SERVICE_RESPONSE.OK){
      return response.detail.data;
    }else{
      return reject();
    }
  }
);

export const allOperationListSlice = createSlice({
  name: "allOperationList",
  initialState,
  reducers: {
    SET_INFO: (state) => {
      state.loading = true;
    },
    setSelectedRowKeys: {
      reducer: (state, action) => {
        if (action.payload) {
          state.selectedRow = action.payload;
        }
      },
      prepare: (newSelect) => {
        return { payload: newSelect };
      },
    },
    setSelectedObject: {
      reducer: (state, action) => {
        if (action.payload) {
          state.selectedObject = action.payload;
        }
      },
      prepare: (newSelect) => {
        return { payload: newSelect };
      },
    },
    hideDataOnTable : (state, action) => {
      state.hideRow = state.hideRow.concat(state.selectedRow)
      let prepareData = state.data.filter(item => !state.selectedRow.includes(item.inner_jobnet_id));
      state.data = prepareData;
      state.selectedObject = [];
      state.selectedRow = [];
    },
    updateVisibleScheduleDialog : (state, action) => {
      state.updateScheduleDialogVisible = action.payload;
    },
    setIntervalId : (state, action) => {
      state.intervelId = action.payload
    } 
  },
  extraReducers(builder) {
    builder
      .addCase(getAllOperationList.pending, (state) => {
        state.loading = true;
      })
      .addCase(getAllOperationList.fulfilled, (state, action) => {
        let prepareData = action.payload.filter(item => !state.hideRow.includes(item.inner_jobnet_id));
        state.data = prepareData;
        state.loading = false;
      })
      .addCase(getAllOperationList.rejected, (state) => {
        state.loading = false;
        clearInterval(state.intervelId)
        state.intervelId = null;
      });
  },
});

export default allOperationListSlice.reducer;

export const {
  setSelectedRowKeys,
  setSelectedObject,
  hideDataOnTable,
  updateVisibleScheduleDialog,
  setIntervalId
} = allOperationListSlice.actions;
