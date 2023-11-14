import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import filterFormService from "../services/FilterFormService";
import { createFilterFormObjectRequest } from "../factory/FormObjectFactory";
import { setObjectFormEditable } from "./ObjectListSlice";
import { SERVICE_RESPONSE } from "../constants";

const initialState = {
  data: {},
  formData: {},
  dateShift: {},
  calendarDate: {},
  responseData: {},
  isLoading: {},
  selectedYear: {},
};

// calendarDatate.dates = data;

// (state)=> state["filter"].calendarDate

export const initFilter = createAsyncThunk(
  "filter/initFilter",
  async (data) => {
    const response = await filterFormService.initFilter(data);
    if (response.type == SERVICE_RESPONSE.OK) {
      return response.detail["return-item"];
    }
    return [];
  }
);

export const initFilterEdit = createAsyncThunk(
  "filter/initFilterEdit",
  async (object) => {
    const response = await filterFormService.initFilterEdit(object);
    return response;
  }
);

export const createFilterObject = createAsyncThunk(
  "filter/createFilter",
  async (data) => {
    const response = await filterFormService.createFilter(data);
    return response;
  }
);

export const getCalendarDate = createAsyncThunk(
  "filter/getCalendarDate",
  async (data) => {
    const response = await filterFormService.getCalendarDate(data);
    return response;
  }
);

export const FilterSlice = createSlice({
  name: "filter",
  initialState,
  reducers: {
    setFilterData: {
      reducer: (state, action) => {
        if (action.payload) {
          state.data = action.payload;
        } else {
          state.data = [];
        }
      },
      prepare: (filterData) => {
        return { payload: filterData };
      },
    },
    setFilterFormData: {
      reducer: (state, action) => {
        if (action.payload) {
          state.formData = action.payload;
        } else {
          state.formData = {};
        }
      },
      prepare: (filterData) => {
        return { payload: filterData };
      },
    },
    setCalendarDate: {
      reducer: (state, action) => {
        if (action.payload) {
          state.calendarDate = action.payload;
        } else {
          state.calendarDate = {};
        }
      },
      prepare: (filterData) => {
        return { payload: filterData };
      },
    },
    cleanupFilterForm: {
      reducer: (state) => {
        state.data = {};
        state.formData = {};
        state.responseData = {};
        state.calendarDate = {};
        state.isLoading = {};
      },
    },
    setSelectedYear: (state, action) => {
      state.selectedYear = action.payload.selectedYear;
    },
  },
  extraReducers(builder) {
    builder
      .addCase(initFilter.pending, (state) => {
        state.isLoading = true;
      })
      .addCase(initFilter.fulfilled, (state, action) => {
        state.data = action.payload;
        state.isLoading = false;
      })
      .addCase(initFilter.rejected, (state) => {
        state.isLoading = false;
      })

      .addCase(initFilterEdit.pending, (state) => {
        state.isLoading = true;
      })
      .addCase(initFilterEdit.fulfilled, (state, action) => {
        //check for message type
        if (action.payload.type) {
          if (action.payload.type == SERVICE_RESPONSE.OK) {
            let editInit = action.payload.detail["return-item"];
            let filterData = createFilterFormObjectRequest(
              editInit.updateDate,
              editInit.filterId,
              editInit.filterId,
              editInit.filterName,
              editInit.userName,
              editInit.publicFlag,
              editInit.updateDate,
              editInit.desc,
              editInit.formType,
              editInit.createdDate,
              editInit.validFlag,
              editInit.editable,
              editInit.authority,
              editInit.baseDateFlag,
              editInit.designatedDay,
              editInit.shiftDay,
              editInit.baseCalendarId,
              editInit.calendarList,
              editInit.isLocked
            );
            state.data = filterData;
          } else {
            state.responseData = action.payload;
          }
        } else {
          state.responseData = {
            type: SERVICE_RESPONSE.NOT_ACCEPTABLE,
            data: {
              "message-detail": "Server Error Occurred",
            },
          };
        }
        state.isLoading = false;
      })
      .addCase(initFilterEdit.rejected, (state) => {
        state.isLoading = false;
      })

      .addCase(createFilterObject.pending, (state) => {})
      .addCase(createFilterObject.fulfilled, (state, action) => {
        state.responseData = action.payload;
      })
      .addCase(createFilterObject.rejected, (state) => {})

      .addCase(getCalendarDate.pending, (state) => {})
      .addCase(getCalendarDate.fulfilled, (state, action) => {
        if (action.payload.type) {
          if (action.payload.type == SERVICE_RESPONSE.OK) {
            let calDate = action.payload.detail["return-item"].calendarDate;
            state.calendarDate = calDate;
          } else {
            state.calendarDate = action.payload.data;
          }
        } else {
          state.responseData = {
            type: SERVICE_RESPONSE.NOT_ACCEPTABLE,
            data: {
              "message-detail": "Server Error Occurred",
            },
          };
        }
      })
      .addCase(getCalendarDate.rejected, (state) => {});
  },
});
export const {
  setSelectedYear,
  setFilterData,
  setFilterFormData,
  setCalendarDate,
  cleanupFilterForm,
} = FilterSlice.actions;
export default FilterSlice.reducer;
