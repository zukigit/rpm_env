import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import objectListService from "../services/ObjectListService";
import objectLockService from "../services/objectLockService";
import objectVersionService from "../services/ObjectVersionService";

const initialState = {
  loading: false,
  isLocked: [],
  data: [],
  selectedRow: [],
  selectedObject: [],
  updateValidationResult: [],
  isObjectFormEditable: true,
};

export const getAllObjectList = createAsyncThunk(
  "objectList/getAllObjectList",
  async (data) => {
    const response = await objectListService.getAllObjectList(data);
    return response.detail;
  }
);

export const updateObjectListValidation = createAsyncThunk(
  "objectList/updateObjectListValidation",
  async (data) => {
    const response = await objectListService.updateObjectListValidation(data);
    return response;
  }
);
export const lockObject = createAsyncThunk(
  "objectList/lockObject",
  async (object) => {
    let calendarId = object.objectId;
    let date = object.date;
    let category = object.category;
    const response = await objectLockService.lockObject(
      calendarId,
      date,
      category
    );
    return response.data;
  }
);
export const deleteObjectList = createAsyncThunk(
  "objectList/deleteObjectList",
  async (data) => {
    const response = await objectListService.deleteObjectListValidation(data);
    return response;
  }
);

export const getAllObjectVersion = createAsyncThunk(
  "objectList/getAllObjectVersion",
  async (data) => {
    const response = await objectVersionService.getAllObjectVersion(data);
    if (response.type == "Ok" && response.detail.datas) {
      if (response.detail.datas.length <= 0) {
        return null;
      } else {
        return response.detail.datas;
      }
    } else {
      return null;
    }
  }
);

export const deleteObjectVersion = createAsyncThunk(
  "objectList/deleteObjectVersion",
  async (data) => {
    const response = await objectVersionService.deleteObjectVersion(data);
    return response;
  }
);

export const updateObjectVersionValidation = createAsyncThunk(
  "objectList/updateObjectVersionValidation",
  async (data) => {
    const response = await objectVersionService.updateObjectVersionValidation(
      data
    );
    return response;
  }
);
export const objectListSlice = createSlice({
  name: "objectList",
  initialState,
  reducers: {
    SET_INFO: (state) => {
      state.loading = true;
    },
    setunLock: (state) => {
      state.isLocked = [];
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
    setObjectFormEditable: {
      reducer: (state, action) => {
        if (action.payload == false) {
          state.isObjectFormEditable = action.payload;
        } else {
          state.isObjectFormEditable = true;
        }
      },
      prepare: (isEditable) => {
        return { payload: isEditable };
      },
    },
    cleanupObjectListSlice: (state) => {
      state.selectedRow = [];
      state.selectedObject = [];
      //state.data = [];
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
      .addCase(getAllObjectList.pending, (state) => {
        state.loading = true;
      })
      .addCase(getAllObjectList.fulfilled, (state, action) => {
        state.data = action.payload;
        state.loading = false;
      })
      .addCase(getAllObjectList.rejected, (state) => {
        state.loading = false;
      })
      .addCase(updateObjectListValidation.pending, (state) => {
        state.loading = true;
      })
      .addCase(updateObjectListValidation.fulfilled, (state, action) => {
        state.updateValidationResult = action.payload;
        state.loading = false;
      })
      .addCase(updateObjectListValidation.rejected, (state) => {
        state.loading = false;
      })

      .addCase(deleteObjectList.pending, (state) => {
        state.loading = true;
      })
      .addCase(deleteObjectList.fulfilled, (state, action) => {
        state.updateValidationResult = action.payload;
        state.loading = false;
      })
      .addCase(deleteObjectList.rejected, (state) => {
        state.loading = false;
      })
      .addCase(getAllObjectVersion.pending, (state) => {
        state.loading = true;
      })
      .addCase(getAllObjectVersion.fulfilled, (state, action) => {
        state.data = action.payload;
        state.loading = false;
      })
      .addCase(getAllObjectVersion.rejected, (state) => {
        state.loading = false;
      })
      .addCase(deleteObjectVersion.pending, (state) => {
        state.loading = true;
      })
      .addCase(deleteObjectVersion.fulfilled, (state, action) => {
        state.updateValidationResult = action.payload;
        state.loading = false;
      })
      .addCase(deleteObjectVersion.rejected, (state) => {
        state.loading = false;
      })
      .addCase(lockObject.pending, (state) => {
        state.loading = true;
      })
      .addCase(lockObject.fulfilled, (state, action) => {
        state.isLocked = action.payload;
        state.loading = false;
      })
      .addCase(lockObject.rejected, (state) => {
        state.loading = false;
      })
      .addCase(updateObjectVersionValidation.pending, (state) => {
        state.loading = true;
      })
      .addCase(updateObjectVersionValidation.fulfilled, (state, action) => {
        state.updateValidationResult = action.payload;
        state.loading = false;
      })
      .addCase(updateObjectVersionValidation.rejected, (state) => {
        state.loading = false;
      });
  },
});
export const {
  setSelectedRowKeys,
  setSelectedObject,
  setInitReload,
  setObjectFormEditable,
  cleanupObjectListSlice,
  setunLock,
} = objectListSlice.actions;
export default objectListSlice.reducer;
