import axios from "axios";
import { alertInfo } from "../components/dialogs/CommonDialog";
import moment from "moment";
import { setExpiredDialogVisible } from "../store/UserSlice";
import { setResponseData } from "../store/ResponseSLice";

const API = axios.create({
  //baseURL: window.__RUNTIME_CONFIG__.BASE_API_URL,
  baseURL:(()=>{
    let urlOrigin = window.location.origin;
    if (process.env.NODE_ENV === "development") {
      let portNo = window.location.port;
      if (portNo && portNo != "") {
        if (urlOrigin.includes(`:${portNo}`)) {
          urlOrigin = urlOrigin.replace(`:${portNo}`, "");
        }
      }
    }
    let result = `${urlOrigin}/jobarranger/api`;
    return result;
  })(),
  headers: {
    Accept: "application/json",
    "Content-Type": "application/json",
  },
});

export function setupInterceptor(store, relogin, errRedirect) {
  API.interceptors.request.use(
    (req) => {
      let data = {};
      const sessionId = store.getState().user.userInfo.sessionId;
      data.params = req.data;
      if (sessionId) {
        data.sessionId = sessionId;
      }
      data.date = moment().format("YYYYMMDDHHmmss");
      req.data = JSON.stringify(data);
      return req;
    },
    (err) => {
      throw err;
    }
  );
  API.interceptors.response.use(
    (res) => {
      if (res.data.type) {
        if (res.data.type === "text/txt" || res.data.type === "text/csv") {
          return res;
        } else if (
          res.data.detail.message == "Session terminated, re-login, please."
        ) {
          if (
            store.getState().user.userInfo.sessionId != undefined &&
            store.getState().user.expiredDialogVisible === false
          ) {
            store.dispatch(setExpiredDialogVisible(true));
            alertInfo(
              "",
              "Your login session is expired, re-login, please.",
              () => {
                relogin();
              }
            );
          }
        } else if (res.data.type == 404) {
          errRedirect("404");
        } else if (res.data.type == 500) {
          console.log("500 error response");
          console.log(res.data);
          store.dispatch(setResponseData(res.data));
          errRedirect("500");
        } else if (res.data.type == "config") {
          errRedirect("config");
        } else {
          return res;
        }
      }

      return res;
    },
    (err) => {
      if (err.code == "ERR_NETWORK") {
        errRedirect("networkErr");
      }else if(err.response.data){
          store.dispatch(setResponseData(err.response.data));
          errRedirect("500");
      }
      //throw err
    }
  );
  return null;
}

export default API;
