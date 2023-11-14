import http from "../common/http";

const objectLockService = {
  getAllLockedObj: (data = {}) => {
    return http
      .post(`/getAllLockedObj`)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },
  deleteLockAsync: async (data = {}) => {
    const response = await http
      .post(`/objectLock/delete`, data)
      .then((response) => {
        return response;
      })
      .catch((err) => {
        throw err;
      });
  },
  deleteLock: (data = {}) => {
    return http
      .post(`/objectLock/delete`, data)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },
  lockObject: (objectId, date, category) => {
    return http
      .post(
        `/objectLock/check?id=${objectId}&&date=${date}&&category=${category}`
      )
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },
  heartbeat: (data) => {
    return http
      .post(`/objectLock/heartbeat`, data)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },
};
export default objectLockService;
