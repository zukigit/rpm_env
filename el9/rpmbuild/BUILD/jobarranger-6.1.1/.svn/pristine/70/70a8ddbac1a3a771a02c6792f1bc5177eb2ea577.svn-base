import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import ScheduleFormService from "../services/ScheduleFormService";
import { SERVICE_RESPONSE } from "../constants";

const initialState = {
  loading: false,
  data: [],
  formData: {},
  objectIdList: [],
  responseData: {},
  calendarData: [],
  jobnetData: [],
  calObjInfo: {},
  jobnetObjInfo: {},
  calendarObjLists: [],
  jobnetObjLists: [],
  // boottimeSelectedRow: [],
  // jobnetSelectedRow: [],
  boottimeSelectedObject: [],
  JobnetSelectedObject: [],
  showBoottime: false,
  chkTime: true,
};

//get auto generate schedule id
export const initSchedule = createAsyncThunk(
  "schedule/initCreate",
  async (data) => {
    const response = await ScheduleFormService.initSchedule(data);
    return response.detail.data;
  }
);

//get schedule form data for edit
export const initScheduleEdit = createAsyncThunk(
  "schedule/initEdit",
  async (data) => {
    const response = await ScheduleFormService.initScheduleEdit(data);
    return response;
  }
);

//get calendar id lists
export const getCalendarIdList = createAsyncThunk(
  "boottime/getCalFltIDList",
  async (data) => {
    const response = await ScheduleFormService.getCalFltIDList();
    return response.detail.data;
  }
);

//get jobnet id lists
export const getJobnetIdList = createAsyncThunk(
  "boottime/getJobnetIDList",
  async (data) => {
    const response = await ScheduleFormService.getJobnetIDList();
    return response.detail.data;
  }
);

//get calendar or filter object info
export const getCalFilObj = createAsyncThunk(
  "boottime/registration",
  async (data) => {
    const response = await ScheduleFormService.getCalFilObj(data);
    return response.detail.data;
  }
);

//get jobnet object info
export const getJobnetObj = createAsyncThunk(
  "boottime/jobnet",
  async (data) => {
    const response = await ScheduleFormService.getJobnetObj(data);
    return response.detail.data;
  }
);

export const saveScheduleObj = createAsyncThunk(
  "/schedule/save",
  async (data) => {
    const response = await ScheduleFormService.saveScheduleObj(data);
    return response;
  }
);

//bind calendar lists
function bindCalLists(calLists) {
  let arr = [];
  if (calLists) {
    for (let i = 0; i < calLists.length; i++) {
      arr.push({
        key: i,
        id: calLists[i].id,
        type: calLists[i].type,
        createdDate: calLists[i].createdDate,
        updateDate: calLists[i].updateDate,
        validFlag: calLists[i].validFlag,
        publicFlag: calLists[i].publicFlag,
        userName: calLists[i].userName,
        name: calLists[i].name,
        boottime: calLists[i].boottime,
        oldBoottime: calLists[i].oldBoottime,
      });
    }
  }
  return arr;
}

//bind jobnet list
function bindJobnetLists(jobnetLists) {
  let arr = [];
  if (jobnetLists) {
    for (let i = 0; i < jobnetLists.length; i++) {
      arr.push({
        key: i,
        jobnetId: jobnetLists[i].jobnetId,
        updateDate: jobnetLists[i].updateDate,
        createdDate: jobnetLists[i].createdDate,
        validFlag: jobnetLists[i].validFlag,
        publicFlag: jobnetLists[i].publicFlag,
        multipleStartUp: jobnetLists[i].multipleStartUp,
        //validFlag: jobnetLists[i].valid_flag,
        userName: jobnetLists[i].userName,
        jobnetName: jobnetLists[i].jobnetName,
        memo: jobnetLists[i].memo,
        jobnetTimeout: jobnetLists[i].jobnetTimeout,
        timeoutRunType: jobnetLists[i].timeoutRunType,
      });
    }
  }
  return arr;
}
export const ScheduleSlice = createSlice({
  name: "schedule",
  initialState,
  reducers: {
    setScheduleData: {
      reducer: (state, action) => {
        if (action.payload) {
          state.data = action.payload;
        } else {
          state.data = [];
        }
      },
      prepare: (scheduleData) => {
        return { payload: scheduleData };
      },
    },

    //schedule form data
    setScheduleFormData: {
      reducer: (state, action) => {
        if (action.payload) {
          state.formData = action.payload;
        } else {
          state.formData = [];
        }
      },
      prepare: (formData) => {
        return { payload: formData };
      },
    },

    //store caledar data
    setCalendarData: {
      reducer: (state, action) => {
        if (action.payload) {
          state.calendarData = action.payload;
        } else {
          state.calendarData = [];
        }
      },
      prepare: (calendarData) => {
        return { payload: calendarData };
      },
    },

    //store jobnet data
    setJobnetData: {
      reducer: (state, action) => {
        if (action.payload) {
          state.jobnetData = action.payload;
        } else {
          state.jobnetData = [];
        }
      },
      prepare: (jobnetData) => {
        return { payload: jobnetData };
      },
    },

    //store calendar object lists
    setCalendarObjectLists: {
      reducer: (state, action) => {
        if (action.payload) {
          state.calendarObjLists = action.payload;
        } else {
          state.calendarObjLists = [];
        }
      },
      prepare: (calendarObjLists) => {
        return { payload: calendarObjLists };
      },
    },

    //store jobnet object lists
    setJobnetObjectLists: {
      reducer: (state, action) => {
        if (action.payload) {
          state.jobnetObjLists = action.payload;
        } else {
          state.jobnetObjLists = [];
        }
      },
      prepare: (jobnetObjLists) => {
        return { payload: jobnetObjLists };
      },
    },

    //set boottime flag
    setBoottimeFlag: {
      reducer: (state, action) => {
        if (action.payload) {
          state.showBoottime = action.payload;
        }
      },
      prepare: (newSelect) => {
        return { payload: newSelect };
      },
    },

    //set boottime flag
    setChkTime: {
      reducer: (state, action) => {
        if (action.payload) {
          state.chkTime = action.payload;
        }
      },
      prepare: (newSelect) => {
        return { payload: newSelect };
      },
    },
  },
  extraReducers(builder) {
    builder
      .addCase(initSchedule.pending, (state) => {
        state.loading = true;
      })
      .addCase(initSchedule.fulfilled, (state, action) => {
        state.formData = action.payload;
        state.calendarObjLists = bindCalLists(action.payload.calDetailLists); //call function

        state.jobnetObjLists = bindJobnetLists(
          action.payload.jobnetDetailLists
        );
        state.loading = false;
      })
      .addCase(initSchedule.rejected, (state) => {
        state.loading = false;
      })

      //get schedule object for edit
      .addCase(initScheduleEdit.pending, (state) => {
        state.loading = true;
      })
      .addCase(initScheduleEdit.fulfilled, (state, action) => {
        //check for message type
        if (action.payload.type) {
          if (action.payload.type == SERVICE_RESPONSE.OK) {
            state.formData = action.payload.detail["return-item"];
            state.calendarObjLists = bindCalLists(
              // action.payload.calDetailLists
              action.payload.detail["return-item"].calDetailLists
            );
            state.jobnetObjLists = bindJobnetLists(
              // action.payload.jobnetDetailLists
              action.payload.detail["return-item"].jobnetDetailLists
            );
            state.loading = false;
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
        // state.formData = action.payload;
        // state.calendarObjLists = bindCalLists(action.payload.calDetailLists); //call function
        // state.jobnetObjLists = bindJobnetLists(
        //   action.payload.jobnetDetailLists
        // );
        // state.loading = false;
      })
      .addCase(initScheduleEdit.rejected, (state) => {
        state.loading = false;
      })

      //get calendar id list
      .addCase(getCalendarIdList.pending, (state) => {
        state.loading = true;
      })
      .addCase(getCalendarIdList.fulfilled, (state, action) => {
        state.objectIdList = action.payload;
        state.loading = false;
      })
      .addCase(getCalendarIdList.rejected, (state) => {
        state.loading = false;
      })

      //get jobnet id list
      .addCase(getJobnetIdList.pending, (state) => {
        state.loading = true;
      })
      .addCase(getJobnetIdList.fulfilled, (state, action) => {
        state.objectIdList = action.payload;
        state.loading = false;
      })
      .addCase(getJobnetIdList.rejected, (state) => {
        state.loading = false;
      })

      //get cal or fil info
      .addCase(getCalFilObj.pending, (state) => {
        state.loading = true;
      })
      .addCase(getCalFilObj.fulfilled, (state, action) => {
        //chk boottime is same or not
        if (action.payload !== 0) {
          for (var i = 0; i < action.payload.length; i++) {
            state.calendarObjLists = [
              ...state.calendarObjLists,
              action.payload[i],
            ];
          }
        }
        state.calendarObjLists = bindCalLists(state.calendarObjLists);
        state.loading = false;
      })
      .addCase(getCalFilObj.rejected, (state) => {
        state.loading = false;
      })

      //get jobnet info
      .addCase(getJobnetObj.pending, (state) => {
        state.loading = true;
      })
      .addCase(getJobnetObj.fulfilled, (state, action) => {
        state.jobnetObjInfo = action.payload; // boottime
        //chk already exist in lists

        if (action.payload !== 0) {
          state.jobnetObjLists = [...state.jobnetObjLists, state.jobnetObjInfo];
        }
        state.jobnetObjLists = bindJobnetLists(state.jobnetObjLists);
        state.loading = false;
      })
      .addCase(getJobnetObj.rejected, (state) => {
        state.loading = false;
      })

      //get jobnet info
      .addCase(saveScheduleObj.pending, (state) => {
        state.loading = true;
      })
      .addCase(saveScheduleObj.fulfilled, (state, action) => {
        state.responseData = action.payload;
        state.loading = false;
      })
      .addCase(saveScheduleObj.rejected, (state) => {
        state.loading = false;
      });
  },
});
export const {
  setScheduleData,
  setScheduleFormData,
  setCalendarData,
  setJobnetData,
  setCalendarObjectLists,
  setJobnetObjectLists,
  setBoottimeSelectedObject,
  setJobnetSelectedObject,
  setBoottimeSelectedRowKeys,
  setJobnetSelectedRowKeys,
  setBoottimeFlag,
  setChkTime,
} = ScheduleSlice.actions;
export default ScheduleSlice.reducer;
