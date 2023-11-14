import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import calendarFormService from "../services/calendarFormService";
import { createCalendarFormObjectRequest } from "../factory/FormObjectFactory";
import { SERVICE_RESPONSE } from "../constants";

const initialState = {
  data: {},
  formData: {},
  datasource: [],
  responseData: {},
  isLoading: false,
  isCalendarChanged: false,
  isInitRegistShow: false,
  isFileReadShow: false,
  dateBeforeInitLoad: [],
};

export const initCalendar = createAsyncThunk(
  "calendar/initCalendar",
  async (data) => {
    const response = await calendarFormService.initCalendar(data);
    if (response.type === SERVICE_RESPONSE.OK) {
      return response.detail["return-item"];
    }
    return [];
  }
);

export const initCalendarEdit = createAsyncThunk(
  "calendar/initCalendarEdit",
  async (object) => {
    const response = await calendarFormService.initCalendarEdit(object);
    return response;
  }
);

export const createCalendarObject = createAsyncThunk(
  "calendar/createCalendarObject",
  async (data) => {
    const response = await calendarFormService.createCalendar(data);
    return response;
  }
);

export const CalendarSlice = createSlice({
  name: "calendar",
  initialState,
  reducers: {
    setDateBeforeInitLoad: {
      reducer: (state, action) => {
        state.dateBeforeInitLoad = action.payload;
      },
      prepare: (initDate) => {
        return { payload: initDate };
      },
    },
    setInitRegistShow: {
      reducer: (state, action) => {
        state.isInitRegistShow = action.payload;
      },
      prepare: (initRegistShow) => {
        return { payload: initRegistShow };
      },
    },
    setDatasource: {
      reducer: (state, action) => {
        if (action.payload) {
          state.datasource = action.payload;
        }
      },
      prepare: (datasrc) => {
        return { payload: datasrc };
      },
    },
    setCalendarChanged: {
      reducer: (state, action) => {
        if (action.payload) {
          state.isCalendarChanged = action.payload;
        }
      },
      prepare: (isCalendarChanged) => {
        return { payload: isCalendarChanged };
      },
    },
    setFileReadShow: {
      reducer: (state, action) => {
        state.isFileReadShow = action.payload;
      },
      prepare: (FileReadShow) => {
        return { payload: FileReadShow };
      },
    },
    setCalendarData: {
      reducer: (state, action) => {
        if (action.payload) {
          state.data = action.payload;
        } else {
          state.data = [];
        }
      },
      prepare: (calendarData) => {
        return { payload: calendarData };
      },
    },
    setCalendarFormData: {
      reducer: (state, action) => {
        if (action.payload) {
          state.formData = action.payload;
        } else {
          state.formData = {};
        }
      },
      prepare: (calendarData) => {
        return { payload: calendarData };
      },
    },
    cleanupCalendarForm: {
      reducer: (state) => {
        state.data = {};
        state.formData = {};
        state.responseData = {};
        state.isCalendarChanged = false;
      },
    },
    setInitRegistData: (state, action) => {
      if (Object.keys(action.payload.dates).length !== 0) {
        let dates;
        if (state.data.dates) {
          dates = [...state.data.dates, action.payload.dates];
        } else {
          dates = [action.payload.dates];
        }
        state.data.dates = getUnique(dates);
      }
    },
    resetCalendarDates: (state) => {
      state.data.dates = [];
    },
    setCalendarDates: {
      reducer: (state, action) => {
        state.data.dates = action.payload;
      },
      prepare: (dates) => {
        return { payload: dates };
      },
    },
  },
  extraReducers(builder) {
    builder
      .addCase(initCalendar.pending, (state) => {
        state.isLoading = true;
      })
      .addCase(initCalendar.fulfilled, (state, action) => {
        state.data = action.payload;
        state.isLoading = false;
      })
      .addCase(initCalendar.rejected, (state) => {
        state.isLoading = false;
      })

      .addCase(initCalendarEdit.pending, (state) => {
        state.isLoading = true;
      })
      .addCase(initCalendarEdit.fulfilled, (state, action) => {
        //check for message type
        if (action.payload.type) {
          if (action.payload.type === SERVICE_RESPONSE.OK) {
            let editInit = action.payload.detail["return-item"];
            let calendarData = createCalendarFormObjectRequest(
              editInit.updateDate,
              editInit.calendarId,
              editInit.calendarId,
              editInit.calendarName,
              editInit.userName,
              editInit.publicFlag,
              editInit.updateDate,
              editInit.desc,
              editInit.formType,
              editInit.dates,
              editInit.createdDate,
              editInit.validFlag,
              editInit.lastday,
              editInit.editable,
              editInit.authority,
              editInit.notInitialize,
              editInit.isLocked
            );
            state.data = calendarData;
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
      .addCase(initCalendarEdit.rejected, (state) => {
        state.isLoading = false;
      })

      .addCase(createCalendarObject.pending, (state) => {
        state.isLoading = true;
      })
      .addCase(createCalendarObject.fulfilled, (state, action) => {
        state.responseData = action.payload;
        state.isLoading = false;
      })
      .addCase(createCalendarObject.rejected, (state) => {
        state.isLoading = false;
      });
  },
});

function getUnique(array) {
  var uniqueArray = [];
  for (let i = 0; i < array.length; i++) {
    if (uniqueArray.indexOf(array[i]) === -1) {
      uniqueArray.push(array[i]);
    }
  }

  return uniqueArray;
}
export const {
  setDateBeforeInitLoad,
  setFileReadShow,
  resetCalendarDates,
  setCalendarDates,
  setInitRegistData,
  setCalendarData,
  setCalendarFormData,
  setInitRegistShow,
  cleanupCalendarForm,
  setCalendarChanged,
  setDatasource,
} = CalendarSlice.actions;
export default CalendarSlice.reducer;
