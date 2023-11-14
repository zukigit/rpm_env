import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import { SERVICE_RESPONSE } from '../constants'
import jobExecutionManagementService from '../services/JobExecutionManagementService'

const initialState = {
    loading: false,
    data: [],
    selectedRow: [],
    selectedObject: [],
    hideRow: [],
    intervelId: null
  }

export const getDuringOperationList = createAsyncThunk('duringOperationList/getDuringOperationList', async (intervalId, {reject}) => {
  const response = await jobExecutionManagementService.getDuringOperationList()
  if(response.type === SERVICE_RESPONSE.OK){
    return response.detail.data;
  }else{
    return reject();
  }
})

export const duringOperationListSlice = createSlice({
  name: 'duringOperationList',
  initialState,
  reducers: {
    SET_INFO: (state) => {
      state.loading = true
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
    setIntervalId : (state, action) => {
      state.intervelId = action.payload
    } 
  },
  extraReducers(builder) {
    builder
      .addCase(getDuringOperationList.pending, (state) => {
        state.loading = true
      })
      .addCase(getDuringOperationList.fulfilled, (state, action) => {
        let prepareData = action.payload.filter(item => !state.hideRow.includes(item.inner_jobnet_id));
        state.data = prepareData;
        state.loading = false;
      })
      .addCase(getDuringOperationList.rejected, (state) => {
        state.loading = false
      })
  }
})

export default duringOperationListSlice.reducer

export const {
  setSelectedRowKeys,
  setSelectedObject,
  hideDataOnTable,
  setIntervalId
} = duringOperationListSlice.actions;
