import http from "../common/http";

const generalSettingService = {
  getGeneralSetting: (data) => {
    return http
      .post("/generalSetting/getAll")
      .then((res) => {
        return res.data;
      })
      .catch((err) => {
        throw err;
      });
  },
  UpdateGeneralSetting: (data) => {
    return http
      .post("/generalSetting/update", data)
      .then((res) => {
        return res.data;
      })
      .catch((err) => {
        throw err;
      });
  },
};

export default generalSettingService;
