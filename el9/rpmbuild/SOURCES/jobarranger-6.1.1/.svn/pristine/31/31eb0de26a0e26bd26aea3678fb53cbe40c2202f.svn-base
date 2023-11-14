import React from "react";
import { useTranslation } from "react-i18next";
import { APP_INFO } from "../../../constants";
import "./Footer.scss"

const Footer = () => {
  const { t } = useTranslation();
  return (
   <>
    <div className="copy-info">
      {t("lab-copy-right")}
      <a
        href="https://www.jobarranger.info/jaz/jaz_manualtop.html"
        target="_blank"
        rel="noopener noreferrer"
        style={{color: '#399efb'}}
      >
        {t("lab-zabbix")}
      </a>
    </div>
    <div className="ver-info">Version: {APP_INFO.VERSION}</div>
   </>
  );
};

export default Footer;
