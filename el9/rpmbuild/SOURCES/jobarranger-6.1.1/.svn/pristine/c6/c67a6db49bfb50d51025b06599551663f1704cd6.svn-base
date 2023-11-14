import http from "../common/http";

const Auth = {
  login: (data) => {
    return http
      .post("/login", data)
      .then((res) => {
        return res.data;
      })
      .catch((err) => {
        console.log("Auth service err", err);
        throw err;
      });
  },

  logout: (data) => {
    return http
      .post("/logout", data)
      .then(({ data }) => {
        return data;
      })
      .catch((err) => {
        console.log("Auth service err", err);
        throw err;
      });
  },

  apiCheck: (data) => {
    return http
      .post("/apiCheck")
      .then((res) => {
        return res.data;
      })
      .catch((err) => {
        console.log("Auth service err", err);
        throw err;
      });
  },
};

export default Auth;
