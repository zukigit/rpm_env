import qs from "query-string";
import { useLocation, Navigate } from "react-router-dom";
import { useSelector } from "react-redux";
import { useEffect, useRef } from "react";
const PrivateRoute = ({ element: Component, meta = {}, ...rest }) => {
  
  //window title
  const defaultTitle = useRef(document.title);
  useEffect(() => {
    document.title = meta.title;
  }, [meta.title]);
  useEffect(
    () => () => {
      document.title = defaultTitle.current;
    },
    []
  );

  const { pathname, search } = useLocation();
  const isLogin = useSelector((state) => state.user.isLogin);
  const isLoginPage = pathname === "/" || pathname === "/login";

  if (pathname === "/setup") {
    return <Component {...rest} />;
  }

  if (isLoginPage && isLogin) {
    const redirectUrl = qs.parse(search).redirectUrl;
    const url = redirectUrl || "/home" + search;
    return <Navigate to={url} replace />;
  }

  if (meta.requiresAuth) {
    if (isLogin) {
      return <Component {...rest} />;
    } else {
      if (!isLoginPage) {
        return <Navigate to={`/?redirectUrl=${pathname}${search}`} replace />;
      }
    }
  }

  return <Component {...rest} />;
};

export default PrivateRoute;
