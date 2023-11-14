import React from "react";
import ReactDOM from "react-dom/client";
import { Provider } from "react-redux";
import { BrowserRouter as Router } from "react-router-dom";
import App from "./App";
import store from "./store";
import "./config/i18n/i18n";

const root = ReactDOM.createRoot(document.getElementById("root"));

const url_path = "/jobarranger"
root.render(
  <Provider store={store}>
    <Router basename={url_path}>
      <App />
    </Router>
  </Provider>
);
