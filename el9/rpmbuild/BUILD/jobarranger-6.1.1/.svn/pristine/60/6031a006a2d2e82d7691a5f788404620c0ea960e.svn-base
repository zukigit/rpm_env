import http from "../common/http";

const SetupService = {
  getInitial: () => {
    return http
      .post(`/setup/Initial`)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        console.log("Auth service err", err);
        throw err;
      });
  },
  getRequirement: () => {
    return http
      .post(`/setup/PreRequirement`)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        console.log("Auth service err", err);
        throw err;
      });
  },

  getDBConnection: (datas) => {
    return http
      .post(`/setup/dbConnection`, datas)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        console.log("Auth service err", err);
        throw err;
      });
  },

  getZbxConnection: (datas) => {
    return http
      .post(`/setup/zbxApiCheck`, datas)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        console.log("Auth service err", err);
        throw err;
      });
  },

  getLogPathCheck: (datas) => {
    return http
      .post(`/setup/logPathCheck`, datas)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        console.log("Auth service err", err);
        throw err;
      });
  },

  createConfig: (datas) => {
    return http
      .post(`/setup/configCreate`, datas)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        console.log("Auth service err", err);
        throw err;
      });
  },
};

export default SetupService;
