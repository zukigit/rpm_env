import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import JobnetService from "../services/JobnetService";
import {
  FORM_TYPE,
  FORM_ID,
  LOCAL_STORAGE,
  ICON_TYPE,
  SERVICE_RESPONSE,
} from "../constants";
import { createFormObject } from "../factory/FormObjectFactory";

const initialState = {
  loading: false,
  graphIndexId: 1,
  maxZindex: 0,
  formObjList: {
    1: {
      isMainForm: true,
      parentGraphId: null,
      loading: false,
      data: {},
      formData: {},
      responseData: {},
      modalState: {
        maxZindex: 0,
        modals: [],
      },
    },
  },
};

export const initCreateJobnet = createAsyncThunk(
  "jobnetForm/initCreateJobnet",
  async (data, { reject }) => {
    const response = await JobnetService.initCreateJobnet({ type: data.type });
    if (response.type === SERVICE_RESPONSE.OK) {
      return { ...response.detail.data, publicType: data.publicType };
    } else if (response.type === SERVICE_RESPONSE.INCOMEPLETE) {
      return response;
    } else {
      return reject();
    }
  }
);

export const initEditJobnet = createAsyncThunk(
  "jobnetForm/initEditJobnet",
  async (data, { reject }) => {
    const response = await JobnetService.initEditJobnet(data);
    if (response.type === SERVICE_RESPONSE.OK) {
      return response.detail.data;
    } else if (response.type === SERVICE_RESPONSE.INCOMEPLETE) {
      return response;
    } else {
      return reject();
    }
  }
);

const prepareFormData = (jobnetInfo) => {
  let formObject;
  let user = JSON.parse(localStorage.getItem(LOCAL_STORAGE.USER));
  if (jobnetInfo.type === FORM_TYPE.CREATE) {
    formObject = createFormObject(
      `${FORM_ID.JOBNET}_${jobnetInfo.lastid}`,
      jobnetInfo.publicType ? true : false,
      "0",
      "",
      "",
      user.userName,
      1,
      "",
      "",
      "0",
      "0",
      "",
      1, //editable
      1, //idEditable
      0 //isLocked
    );
  }
  if (
    jobnetInfo.type === FORM_TYPE.EDIT ||
    jobnetInfo.type === FORM_TYPE.NEW_VERSION
  ) {
    if (jobnetInfo.hasOwnProperty("detail")) {
      formObject = createFormObject(
        jobnetInfo.detail.jobnet_id,
        parseInt(jobnetInfo.detail.public_flag) ? true : false,
        jobnetInfo.detail.multiple_start_up,
        jobnetInfo.type === FORM_TYPE.EDIT ? jobnetInfo.detail.update_date : "",
        jobnetInfo.detail.jobnet_name,
        jobnetInfo.detail.user_name,
        jobnetInfo.authority,
        "",
        jobnetInfo.detail.memo,
        jobnetInfo.detail.jobnet_timeout,
        jobnetInfo.detail.timeout_run_type,
        null,
        jobnetInfo.editable,
        0,
        jobnetInfo.isLocked
      );
    }
  }
  if (jobnetInfo.type === FORM_TYPE.NEW_OBJECT) {
    if (jobnetInfo.hasOwnProperty("detail")) {
      formObject = createFormObject(
        `${FORM_ID.JOBNET}_${jobnetInfo.lastid}`,
        parseInt(jobnetInfo.detail.public_flag) ? true : false,
        jobnetInfo.detail.multiple_start_up,
        "",
        jobnetInfo.detail.jobnet_name,
        user.userName,
        jobnetInfo.authority,
        "",
        jobnetInfo.detail.memo,
        jobnetInfo.detail.jobnet_timeout,
        jobnetInfo.detail.timeout_run_type,
        "",
        jobnetInfo.editable, //editable
        1, //idEditable
        0 //isLocked
      );
    }
  }
  if (jobnetInfo.type === FORM_TYPE.SCHEDULE) {
    if (jobnetInfo.hasOwnProperty("detail")) {
      formObject = createFormObject(
        jobnetInfo.detail.jobnet_id,
        parseInt(jobnetInfo.detail.public_flag) ? true : false,
        jobnetInfo.detail.multiple_start_up,
        jobnetInfo.type === FORM_TYPE.EDIT || FORM_TYPE.SCHEDULE ? jobnetInfo.detail.update_date : "",
        jobnetInfo.detail.jobnet_name,
        jobnetInfo.detail.user_name,
        jobnetInfo.authority,
        "",
        jobnetInfo.detail.memo,
        jobnetInfo.detail.jobnet_timeout,
        jobnetInfo.detail.timeout_run_type,
        null,
        0,
        0,
        0
      );
    }
  }
  if (jobnetInfo.type === SERVICE_RESPONSE.INCOMEPLETE) {
    return jobnetInfo;
  }

  if (formObject !== undefined) {
    return formObject;
  }
};

export const JobnetFormSlice = createSlice({
  name: "jobnetForm",
  initialState,
  reducers: {
    setFormList: (state, action) => {
      state.formObjList[action.payload.key] = action.payload.value;
    },
    removeFormList: (state, action) => {
      let formObjectList = state.formObjList;
      delete formObjectList[action.payload];
      state.formObjList = formObjectList;
    },
    removeOpenDialog: (state, action) => {
      state.formObjList[action.payload.graphIndexId].modalState.modals =
        state.formObjList[action.payload.graphIndexId].modalState.modals.filter(
          (dialog) => {
            return dialog.id !== action.payload.id;
          }
        );
    },
    addOpenDialog: (state, action) => {
      state.formObjList[action.payload.graphIndexId].modalState.modals = [
        ...state.formObjList[action.payload.graphIndexId].modalState.modals,
        action.payload.modal,
      ];
    },
    clearData: (state, action) => {
      state.formObjList = {
        1: {
          isMainForm: true,
          parentGraphId: null,
          loading: false,
          data: {},
          formData: {},
          responseData: {},
          modalState: {
            maxZindex: 0,
            modals: [],
          },
        },
      };
    },
    increaseGraphId: (state, action) => {
      state.graphIndexId = action.payload;
    },
    setMaxZindex: (state, action) => {
      state.maxZindex = action.payload;
    },
    setModalZindex: (state, action) => {
      state.maxZindex = state.maxZindex + 1;
      state.formObjList[action.payload.graphIndexId].modalState.modals[
        action.payload.index
      ].zindex = state.maxZindex + 1;
    },
    addChildForm: (state, action) => {
      state.formObjList[action.payload.formGraphId] = {
        ...state.formObjList[action.payload.formGraphId],
        data: action.payload.form.data,
        formData: action.payload.form.formData,
      };
    },
  },
  extraReducers(builder) {
    builder
      .addCase(initCreateJobnet.pending, (state) => {
        state.formObjList["1"].loading = true;
      })
      .addCase(initCreateJobnet.fulfilled, (state, action) => {
        state.formObjList["1"].data = action.payload;
        state.formObjList["1"].formData = prepareFormData(action.payload);
        state.formObjList["1"].loading = false;
      })
      .addCase(initCreateJobnet.rejected, (state, action) => {
        state.formObjList["1"].loading = false;
      })
      .addCase(initEditJobnet.pending, (state) => {
        state.formObjList["1"].loading = true;
      })
      .addCase(initEditJobnet.fulfilled, (state, action) => {
        state.formObjList["1"].data = action.payload;
        state.formObjList["1"].formData = prepareFormData(action.payload);
        state.formObjList["1"].loading = false;
      })
      .addCase(initEditJobnet.rejected, (state) => {
        state.formObjList["1"].loading = false;
      });
  },
});

export const openIconSettingDialog =
  (graphIndexId, dialogObj) => (dispatch, getState) => {
    const rootState = getState();

    const isFound = rootState.jobnetForm.formObjList[
      graphIndexId
    ].modalState.modals.some((dialog) => {
      if (dialog.id === dialogObj.id) {
        return true;
      }
      return false;
    });

    if (!isFound) {
      let formGraphId = null;
      if (dialogObj.cellType === ICON_TYPE.JOBNET && !dialogObj.isJobnetSetting) {
        formGraphId = rootState.jobnetForm.graphIndexId + 1;
        dispatch(increaseGraphId(formGraphId));
        dispatch(
          setFormList({
            key: formGraphId,
            value: {
              isMainForm: false,
              parentGraphId: graphIndexId,
              loading: false,
              data: {},
              formData: {},
              responseData: {},
              modalState: {
                maxZindex: 0,
                modals: [],
              },
            },
          })
        );
      }

      dispatch(
        addOpenDialog({
          graphIndexId,
          modal: {
            ...dialogObj,
            zindex: rootState.jobnetForm.maxZindex + 1,
            formGraphId,
          },
        })
      );

      dispatch(setMaxZindex(rootState.jobnetForm.maxZindex + 1));
    }
  };

export const getNextZIndex =
  (dialogObj, graphIndexId) => (dispatch, getState) => {
    const modalState =
      getState().jobnetForm.formObjList[graphIndexId].modalState;
    if (modalState.maxZindex !== dialogObj.zindex) {
      const index = modalState.modals.findIndex((dialog) => {
        return dialog.id === dialogObj.id;
      });
      dispatch(setModalZindex({ index, graphIndexId }));
    }
  };

export default JobnetFormSlice.reducer;

export const {
  setFormList,
  removeFormList,
  removeOpenDialog,
  addOpenDialog,
  clearData,
  increaseGraphId,
  setMaxZindex,
  setModalZindex,
  addChildForm,
} = JobnetFormSlice.actions;
