import http from "../common/http";

const calendarFormService = {
  initCalendar: (data = {}) => {
    return http
      .post(`/calendar/initCreate`, data)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },
  initCalendarEdit: (data = {}) => {
    return http
      .post(`/calendar/initEdit`, data)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },
  createCalendar: (calendarData) => {
    return http
      .post(`/calendar/save`, calendarData)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },
};

export default calendarFormService;
