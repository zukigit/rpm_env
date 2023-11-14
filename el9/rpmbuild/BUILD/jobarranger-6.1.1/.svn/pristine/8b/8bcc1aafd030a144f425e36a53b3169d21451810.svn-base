import React from "react";
import { Modal } from "antd";
import { t } from "i18next";
import { ExclamationOutlined } from "@ant-design/icons";
import ExportService from "../../services/ExportService";

export function exportXmlAjaxCall(data) {
  const before = Date.now();

  function dateToSring(dateTime, withMiliSec) {
    var year = dateTime.getFullYear();
    var month = ("0" + (dateTime.getMonth() + 1)).slice(-2);
    var date = ("0" + dateTime.getDate()).slice(-2);
    var hours = ("0" + dateTime.getHours()).slice(-2);
    var minutes = ("0" + dateTime.getMinutes()).slice(-2);
    var seconds = ("0" + dateTime.getSeconds()).slice(-2);
    var miliseconds = ("00" + dateTime.getMilliseconds()).slice(-3);

    return (
      year +
      month +
      date +
      hours +
      minutes +
      seconds +
      (withMiliSec ? miliseconds : "")
    );
  }

  function getCurDateTimeStr() {
    return dateToSring(new Date(), false);
  }

  function generateExportFileName(type) {
    var fileName = "Export_" + type + "_" + getCurDateTimeStr() + ".xml";
    return fileName;
  }

  ExportService.exportXML(data)
    .then((resultData) => {
      var filename = generateExportFileName(data["exportType"]);
      if (resultData instanceof Blob) {
        var a = document.createElement("a");
        var url = window.URL || window.webkitURL;
        var downloadURL = url.createObjectURL(resultData);
        a.href = downloadURL;
        a.download = filename;
        document.body.append(a);
        a.click();
        a.remove();
        url.revokeObjectURL(downloadURL);
        const after = Date.now();
      }
      return t("title-success");
    })
    .catch((error) => {
      return error.message;
    });
}

export function alertInfo(title, message, onOk = () => {}) {
  message = message ? message : "Notify.";
  title = title ? title : "JAZ - Info";
  Modal.info({
    title: title,
    content: message,
    centered: true,
    onOk: onOk,
    okText: t("btn-ok"),
  });
}

export function alertSuccess(title, message, ok) {
  title = title ? title : "JAZ - Info";
  message = message ? message : "Notify.";
  Modal.success({
    title: title,
    content: message,
    centered: true,
    onOk: ok,
    okText: t("btn-ok"),
  });
}

export function alertError(title, message, width = 416, ok = () => {}) {
  message = message ? message : "Notify.";
  title = title ? title : "JAZ - Info";
  Modal.error({
    title: title,
    content: message,
    centered: true,
    width: width,
    okText: t("btn-ok"),
    onOk: ok,
  });
}

export function alertExdetail(title, message, width = 416) {
  message = message ? message : "Notify.";
  title = title ? title : "JAZ - Info";
  Modal.info({
    title: title,
    content: message,
    centered: true,
    width: width,
    okText: t("btn-ok"),
  });
}

export function confirmDialog(title, message, ok, cancel) {
  message = message ? message : "Notify.";
  title = title ? title : "JAZ - Info";
  Modal.confirm({
    title: title,
    icon: <ExclamationOutlined />,
    content: message,
    okText: t("btn-ok"),
    cancelText: t("btn-cancel"),
    onOk: ok,
    onCancel: cancel,
    centered: true,
  });
}
