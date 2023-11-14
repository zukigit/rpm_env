import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import jobExecutionManagementService from '../services/JobExecutionManagementService'
import { isPlainObject } from 'lodash'
import {LOCAL_STORAGE, SESSION_STORAGE} from "../constants/index";

let localUser
try {
  const r = JSON.parse(localStorage.getItem(LOCAL_STORAGE.USER))
  if (isPlainObject(r)) {
    localUser = r
  }
} catch {}

const initialState = {
    isLogin: !!localUser,
    userInfo: localUser || {},
    expiredDialogVisible: false
  }

export const userSlice = createSlice({
  name: 'user',
  initialState,
  reducers: {
    setUserInfo: (state, action) => {
      const userInfo = action.payload
      if (userInfo.sessionId) {
        state.isLogin = true
        localStorage.setItem(LOCAL_STORAGE.USER, JSON.stringify(userInfo))
      }
      userInfo["heartbeatIntervalTime"] = userInfo.heartbeatIntervalTime ? parseInt(userInfo.heartbeatIntervalTime) : 30000;
      state.userInfo = userInfo
      state.expiredDialogVisible = false
    },
    removeUserInfo: (state) => {
      state.isLogin = false
      state.userInfo = {}
      sessionStorage.removeItem(SESSION_STORAGE.JOB_EXEC_MANAGE_TABLE_VIEW)
      sessionStorage.removeItem(SESSION_STORAGE.ERROR_OPERATOION_INFO_HIDE_ROWS)
      sessionStorage.removeItem(SESSION_STORAGE.IS_SIDEBAR_COLLAPSE)
      localStorage.removeItem(LOCAL_STORAGE.USER)
    },
    setExpiredDialogVisible: (state, action) => {
      state.expiredDialogVisible = action.payload
    }
  }
})

export const { setUserInfo, removeUserInfo, setExpiredDialogVisible } = userSlice.actions

export default userSlice.reducer
