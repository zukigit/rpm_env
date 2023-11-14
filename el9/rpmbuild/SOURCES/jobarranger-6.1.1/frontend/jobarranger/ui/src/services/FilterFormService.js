import http from "../common/http";

const FilterFormService = {
  initFilter: (data = {}) => {
    return http
      .post(`/filter/initCreate`, data)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },
  initFilterEdit: (data = {}) => {
    return http
      .post(`/filter/initEdit`, data)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },
  getCalendarDate: (calendarId) => {
    return http
      .post(`/filter/getCalendarDate`, calendarId)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        console.log("Auth service err", err);
        throw err;
      });
  },
  createFilter: (filterData) => {
    return http
      .post(`/filter/save`, filterData)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        console.log("Auth service err", err);
        throw err;
      });
  },
};

export default FilterFormService;
