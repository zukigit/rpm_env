import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import jobExecutionResultService from "../services/JobExecutionResultService";

const initialState = {
  loading: false,
  data: [],
};

export const getJobExecutionResult = createAsyncThunk(
  "jobExecutionResult/getJobExecutionResult",
  async (data) => {
    const response = await jobExecutionResultService.getJobExecutionResult(
      data
    );
    return response.data;
  }
);

export const jobExecutionResultSlice = createSlice({
  name: "jobExecutionResult",
  initialState,
  reducers: {
    SET_INFO: (state) => {
      state.loading = true;
    },
    cleanupResultListSlice: (state) => {
      state.data = [];
    },
  },

  extraReducers(builder) {
    builder
      .addCase(getJobExecutionResult.pending, (state) => {
        state.loading = true;
      })
      .addCase(getJobExecutionResult.fulfilled, (state, action) => {
        state.data = action.payload;
        state.loading = false;
      })
      .addCase(getJobExecutionResult.rejected, (state) => {
        state.loading = false;
      });
  },
});

export const { cleanupResultListSlice } = jobExecutionResultSlice.actions;
export default jobExecutionResultSlice.reducer;
