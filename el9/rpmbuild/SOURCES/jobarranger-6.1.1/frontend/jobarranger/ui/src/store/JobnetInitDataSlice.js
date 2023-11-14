import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { SERVICE_RESPONSE } from "../constants";
import JobnetService from "../services/JobnetService";

const initialState = {
  loading: false,
  initData: {
    host: [],
    defineValueJobCon: [],
    defineExtendedJob: [],
    allJobnetList: [],
    allCalendarList: [],
    hostGroup: [],
    selectHostList: [],
    selectItemList: [],
    selectTriggerList: [],
    jobnetOption: {},
  },
};

export const getAvailableHosts = createAsyncThunk(
  "jobnetInitData/getAvailableHosts",
  async (data, { reject }) => {
    const response = await JobnetService.getAvailableHosts();
    if (response.type === SERVICE_RESPONSE.OK) {
      return response.detail.data;
    } else {
      return reject();
    }
  }
);

export const getDefineValueJobCon = createAsyncThunk(
  "jobnetInitData/getDefineValueJobCon",
  async (data, { reject }) => {
    const response = await JobnetService.getDefineValueJobCon();
    if (response.type === SERVICE_RESPONSE.OK) {
      return response.detail.data;
    } else {
      return reject();
    }
  }
);

export const getDefineExtendedJob = createAsyncThunk(
  "jobnetInitData/getDefineExtendedJob",
  async (data, { reject }) => {
    const response = await JobnetService.getDefineExtendedJob();
    if (response.type === SERVICE_RESPONSE.OK) {
      return response.detail.data;
    } else {
      return reject();
    }
  }
);

export const getAllJobnet = createAsyncThunk(
  "jobnetInitData/getAllJobnet",
  async (data, { reject }) => {
    const response = await JobnetService.getAllJobnet();
    if (response.type === SERVICE_RESPONSE.OK) {
      return response.detail.data;
    } else {
      return reject();
    }
  }
);

export const getAllCalendar = createAsyncThunk(
  "jobnetInitData/getAllCalendar",
  async (data, { reject }) => {
    const response = await JobnetService.getAllCalendar();
    if (response.type === SERVICE_RESPONSE.OK) {
      return response.detail.data;
    } else {
      return reject();
    }
  }
);

export const getHostGroup = createAsyncThunk(
  "jobnetInitData/getHostGroup",
  async (data, { reject }) => {
    const response = await JobnetService.getHostGroup();
    if (response.type === SERVICE_RESPONSE.OK) {
      return response.detail.data;
    } else {
      return reject();
    }
  }
);

export const getHostByZabbixApi = createAsyncThunk(
  "jobnetInitData/getHostByZabbixApi",
  async (data, { reject }) => {
    const response = await JobnetService.getHostByZabbixApi(data);
    if (response.type === SERVICE_RESPONSE.OK) {
      return response.detail.data;
    } else {
      return reject();
    }
  }
);

export const getItemByZabbixApi = createAsyncThunk(
  "jobnetInitData/getItemByZabbixApi",
  async (data, { reject }) => {
    const response = await JobnetService.getItemByZabbixApi(data);
    if (response.type === SERVICE_RESPONSE.OK) {
      return response.detail.data;
    } else {
      return reject();
    }
  }
);

export const getTriggerByZabbixApi = createAsyncThunk(
  "jobnetInitData/getTriggerByZabbixApi",
  async (data, { reject }) => {
    const response = await JobnetService.getTriggerByZabbixApi(data);
    if (response.type === SERVICE_RESPONSE.OK) {
      return response.detail.data;
    } else {
      return reject();
    }
  }
);

export const getJobnetOption = createAsyncThunk(
  "jobnetInitData/getJobnetOption",
  async (data, { reject }) => {
    const response = await JobnetService.getJobnetOption(data);
    if (response.type === SERVICE_RESPONSE.OK) {
      return response.detail.data;
    } else {
      return reject();
    }
  }
);

export const JobnetInitDataSlice = createSlice({
  name: "jobnetInitData",
  initialState,
  reducers: {
    RESET_SELECT_HOST: (state, action) => {
      state.initData.selectHostList = [];
    },
    RESET_SELECT_ITEM: (state, action) => {
      state.initData.selectItemList = [];
    },
    RESET_SELECT_Trigger: (state, action) => {
      state.initData.selectTriggerList = [];
    },
  },
  extraReducers(builder) {
    builder
      .addCase(getAvailableHosts.pending, (state) => {
        state.loading = true;
      })
      .addCase(getAvailableHosts.fulfilled, (state, action) => {
        state.initData.host = action.payload;
        state.loading = false;
      })
      .addCase(getAvailableHosts.rejected, (state) => {
        state.loading = false;
      })
      .addCase(getDefineValueJobCon.pending, (state) => {
        state.loading = true;
      })
      .addCase(getDefineValueJobCon.fulfilled, (state, action) => {
        state.initData.defineValueJobCon = action.payload;
        state.loading = false;
      })
      .addCase(getDefineValueJobCon.rejected, (state) => {
        state.loading = false;
      })
      .addCase(getDefineExtendedJob.pending, (state) => {
        state.loading = true;
      })
      .addCase(getDefineExtendedJob.fulfilled, (state, action) => {
        state.initData.defineExtendedJob = action.payload;
        state.loading = false;
      })
      .addCase(getDefineExtendedJob.rejected, (state) => {
        state.loading = false;
      })
      .addCase(getAllJobnet.pending, (state) => {
        state.loading = true;
      })
      .addCase(getAllJobnet.fulfilled, (state, action) => {
        state.initData.allJobnetList = action.payload;
        state.loading = false;
      })
      .addCase(getAllJobnet.rejected, (state) => {
        state.loading = false;
      })
      .addCase(getAllCalendar.pending, (state) => {
        state.loading = true;
      })
      .addCase(getAllCalendar.fulfilled, (state, action) => {
        state.initData.allCalendarList = action.payload;
        state.loading = false;
      })
      .addCase(getAllCalendar.rejected, (state) => {
        state.loading = false;
      })
      .addCase(getHostGroup.pending, (state) => {
        state.loading = true;
      })
      .addCase(getHostGroup.fulfilled, (state, action) => {
        state.initData.hostGroup = action.payload;
        state.loading = false;
      })
      .addCase(getHostGroup.rejected, (state) => {
        state.loading = false;
      })
      .addCase(getHostByZabbixApi.pending, (state) => {
        state.loading = true;
      })
      .addCase(getHostByZabbixApi.fulfilled, (state, action) => {
        state.initData.selectHostList = action.payload;
        state.loading = false;
      })
      .addCase(getHostByZabbixApi.rejected, (state) => {
        state.loading = false;
      })
      .addCase(getItemByZabbixApi.pending, (state) => {
        state.loading = true;
      })
      .addCase(getItemByZabbixApi.fulfilled, (state, action) => {
        state.initData.selectItemList = action.payload;
        state.loading = false;
      })
      .addCase(getItemByZabbixApi.rejected, (state) => {
        state.loading = false;
      })
      .addCase(getTriggerByZabbixApi.pending, (state) => {
        state.loading = true;
      })
      .addCase(getTriggerByZabbixApi.fulfilled, (state, action) => {
        state.initData.selectTriggerList = action.payload;
        state.loading = false;
      })
      .addCase(getTriggerByZabbixApi.rejected, (state) => {
        state.loading = false;
      })
      .addCase(getJobnetOption.pending, (state) => {
        state.loading = true;
      })
      .addCase(getJobnetOption.fulfilled, (state, action) => {
        state.initData.jobnetOption = action.payload;
        state.loading = false;
      })
      .addCase(getJobnetOption.rejected, (state) => {
        state.loading = false;
      });
  },
});

export default JobnetInitDataSlice.reducer;

export const { RESET_SELECT_HOST, RESET_SELECT_ITEM, RESET_SELECT_Trigger } =
  JobnetInitDataSlice.actions;
