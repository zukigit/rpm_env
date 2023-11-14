import http from "../common/http";

const objectVersionService = {
  getAllObjectVersion: (data = {}) => {
    return http
      .post(
        `/objectVersion/${data.category}?type=${data.publicType}&id=${data.objectId}`,
        data
      )
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },
  deleteObjectVersion: (data) => {
    return http
      .post(`/objectVersion/${data.datas.category}/deleteVersion`, data.datas)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },
  updateObjectVersionValidation: (data) => {
    return http
      .post(`/objectVersion/${data.category}/changeValidVersion`, data)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },
};

export default objectVersionService;
