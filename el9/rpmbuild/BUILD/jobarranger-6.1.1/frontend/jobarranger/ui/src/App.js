import React from "react";
import "./App.scss";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { setupInterceptor } from "./common/http";
import { useDispatch } from "react-redux/es/exports";
import { removeUserInfo } from "./store/UserSlice";
import store from "./store";
import { MainRoutes } from "./router/Routes";

function App() {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const [mounted, setMounted] = React.useState(false);
  const { t } = useTranslation();

  const relogin = () => {
    dispatch(removeUserInfo());
    navigate("/login");
  };

  const errRedirect = (errCode) => {
    if (errCode == 404) {
      navigate("/404");
    }
    if (errCode == 500) {
      navigate("/500");
    }
    if (errCode == "error") {
      navigate("/error");
    }
    if (errCode == "config") {
      navigate("/setup");
    }
    if (errCode == "networkErr") {
      navigate("/networkErr");
    }
  };
  React.useEffect(() => {
    setupInterceptor(store, relogin, errRedirect);
    setMounted(true);
  }, []);

  return mounted ? <MainRoutes /> : null;
}

export default App;
