import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import objectLockService from "../services/objectLockService";

const initialState = {
  loading: false,
  data: [],

  selectedRow: [],
  selectedObject: [],

  unlockResult: [],
};

export const getAllLockedObj = createAsyncThunk(
  "/getAllLockedObj",
  async (data) => {
    const response = await objectLockService.getAllLockedObj(data);
    return response.detail.data;
  }
);

export const unlock = createAsyncThunk("/objectLock/delete", async (data) => {
  const response = await objectLockService.deleteLock(data);
  return response.detail.message;
});

export const LockManagementSlice = createSlice({
  name: "lockedObjList",
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
  },

  extraReducers(builder) {
    builder
      .addCase(getAllLockedObj.pending, (state) => {
        state.loading = true;
      })
      .addCase(getAllLockedObj.fulfilled, (state, action) => {
        state.data = action.payload;
        //        state.unlockResult = [];
        state.loading = false;
      })
      .addCase(getAllLockedObj.rejected, (state) => {
        state.loading = false;
      })

      .addCase(unlock.pending, (state) => {
        state.loading = true;
      })
      .addCase(unlock.fulfilled, (state, action) => {
        state.unlockResult = action.payload;
        state.loading = false;
      })
      .addCase(unlock.rejected, (state) => {
        state.loading = false;
      });
  },
});
export const { setSelectedRowKeys, setSelectedObject } =
  LockManagementSlice.actions;
export default LockManagementSlice.reducer;
