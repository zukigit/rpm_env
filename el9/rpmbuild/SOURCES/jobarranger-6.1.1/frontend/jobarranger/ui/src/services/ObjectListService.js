import http from "../common/http";

const objectListService = {
  getAllObjectList: (data = {}) => {
    return http
      .post(`/objectList`, data)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },
  updateObjectListValidation: (data) => {
    return http
      .post(`/objectList/changeValidList`, data)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },
  deleteObjectListValidation: (data) => {
    return http
      .post(`/objectList/deleteList`, data)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },
};

export default objectListService;
