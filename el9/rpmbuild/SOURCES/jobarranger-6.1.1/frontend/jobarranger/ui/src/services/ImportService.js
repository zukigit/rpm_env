import http from "../common/http";

const ImportService = {
  importXML: (data = {}) => {
    return http
      .post(`/importXML`, data)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        throw err;
      });
  },
};

export default ImportService;
