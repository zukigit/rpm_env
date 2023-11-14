import React from "react";
import { Modal } from "antd";
import { useTranslation } from "react-i18next";
import {
  exportXmlAjaxCall,
  alertSuccess,
} from "../../components/dialogs/CommonDialog";
import "./Export.scss";
import moment from "moment-timezone";
const Export = ({ exportOpen, handleExport }) => {
  const { t } = useTranslation();

  function exportAll() {
    var data = {
      objectType: "Home",
      exportType: "All",
      timeZone: moment.tz("2022-09-24", moment.tz.guess()).format("Z"),
    };

    var response = exportXmlAjaxCall(data);
    if (response === "success") {
      handleExport();
      alertSuccess(t("title-info"), t("lab-export-success"));
    } else {
      handleExport();
    }
  }

  return (
    <Modal
      title={t("title-export")}
      visible={exportOpen}
      onOk={exportAll}
      width={450}
      onCancel={handleExport}
      centered={true}
      maskClosable={false}
      okText={t("btn-ok")}
      cancelText={t("btn-cancel")}
    >
      <div> {t("lab-export-info-all")} </div>
    </Modal>
  );
};
export default Export;
