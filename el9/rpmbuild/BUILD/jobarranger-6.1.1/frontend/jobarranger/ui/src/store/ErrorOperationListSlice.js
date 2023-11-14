import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import { SERVICE_RESPONSE, SESSION_STORAGE } from '../constants'
import JobExecutionManagementService from '../services/JobExecutionManagementService'

let hideRowSessionStorage = []
try {
  const r = JSON.parse(sessionStorage.getItem(SESSION_STORAGE.ERROR_OPERATOION_INFO_HIDE_ROWS)) || [];
  if (r.length > 0) {
    hideRowSessionStorage = r
  }
} catch {}

const initialState = {
    loading: false,
    data: [],
    selectedRow: [],
    selectedObject: [],
    hideRow: hideRowSessionStorage,
    intervelId: null
  }

export const getErrorOperationList = createAsyncThunk('errorOperationList/getErrorOperationList', async (intervalId, {reject}) => {
  const response = await JobExecutionManagementService.getErrorOperationList()
  if(response.type === SERVICE_RESPONSE.OK){
    return response.detail.data;
  }else{
    return reject();
  }
})

export const errorOperationListSlice = createSlice({
  name: 'errorOperationList',
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
      let hideRow = state.hideRow.concat(state.selectedRow);
      state.hideRow = hideRow
      sessionStorage.setItem(SESSION_STORAGE.ERROR_OPERATOION_INFO_HIDE_ROWS,JSON.stringify(hideRow))
      let prepareData = state.data.filter(item => !state.selectedRow.includes(item.inner_jobnet_id));
      state.data = prepareData;
      state.selectedRow = [];
      state.selectedObject = [];
    },
    clearHideRow : (state) => {
      state.hideRow = []
    },
    setIntervalId : (state, action) => {
      state.intervelId = action.payload
    } 
  },
  extraReducers(builder) {
    builder
      .addCase(getErrorOperationList.pending, (state) => {
        state.loading = true
      })
      .addCase(getErrorOperationList.fulfilled, (state, action) => {
        let prepareData = action.payload.filter(item => !state.hideRow.includes(item.inner_jobnet_id));
        state.data = prepareData;
        state.loading = false;
      })
      .addCase(getErrorOperationList.rejected, (state) => {
        state.loading = false
      })
  }
})

export default errorOperationListSlice.reducer

export const {
  setSelectedRowKeys,
  setSelectedObject,
  hideDataOnTable,
  clearHideRow,
  setIntervalId
} = errorOperationListSlice.actions;
