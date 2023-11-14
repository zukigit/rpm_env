import http from "../common/http";

const ScheduleFormService = {
  initSchedule: (data = {}) => {
    return http
      .post(`/schedule/initCreate`, data)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },

  //get schedule form data for edit
  initScheduleEdit: (data = {}) => {
    return http
      .post(`schedule/initEdit`, data)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },

  //get calendar id list
  getCalFltIDList: (data = {}) => {
    return http
      .post(`/boottime/getCalFltIDList`, data)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },

  //get jobnet id list
  getJobnetIDList: (data = {}) => {
    return http
      .post(`/boottime/getJobnetIDList`, data)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },

  //get cal or fil object
  getCalFilObj: (data) => {
    return http
      .post(`/boottime/registration`, data)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },

  //get jobnet obj
  getJobnetObj: (data = {}) => {
    return http
      .post(`/boottime/jobnet`, data)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },

  //save schedule object info
  saveScheduleObj: (data = {}) => {
    return http
      .post(`/schedule/save`, data)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },
};

export default ScheduleFormService;
