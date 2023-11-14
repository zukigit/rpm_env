import React, { useEffect, useState } from "react";
import "./home.scss";
import AuthService from "../../services/Auth";
import { useTranslation } from "react-i18next";
import { CaretRightOutlined } from "@ant-design/icons";
import { Link } from "react-router-dom";
import ImportDialog from "../import/ImportDialog";
import Logout from "../logout/Logout";
import Export from "../export/Export";

const Home = () => {
  const { t } = useTranslation();
  const [status, setStatus] = useState(false);
  const [show, setShow] = useState(false);
  const [exportOpen, setExportOpen] = useState(false);
  const [fileUpload, setFileUpload] = useState("");
  function handleExport() {
    setExportOpen(!exportOpen);
  }
  function onCancel() {
    setShow(!show);
  }
  function onHandle() {
    setStatus(!status);
  }

  useEffect(() => {
    AuthService.apiCheck().then((result) => {
      //console.log("config updated");
    });
  }, []);

  return (
    <div className="content-wrapper">
      <div className="bottom-10">
        <table className="hometbl">
          <tr className="hometbl tr td">
            <td rowSpan="9">
              <CaretRightOutlined />
            </td>
            <td colSpan="4">{t("menu-object-management")}</td>
            <td>{t("label-function")}</td>
          </tr>
          <tr className="hometbl tr td">
            <td></td>
            <td rowSpan="4">
              <label> {t("menu-cal")} </label>
            </td>
            <td>
              <CaretRightOutlined />
            </td>
            <td>
              <Link to="/object-list/calendar/public" tabIndex={1}>
                {t("menu-pub-cal")}
              </Link>
            </td>
            <td rowSpan="8">
              {/* {t("txt-home-cal-fun")} */}
              <li>{t("txt-home-create-obj")}</li>
              <li>{t("txt-home-list-obj")}</li>
              <div style={{ marginLeft: "15px" }}>
                {t("txt-home-edit")} <br />
                {t("txt-home-delete")}
                <br />
                {t("txt-home-enable-disable")}
                <br />
                {t("txt-home-export")}
              </div>
              <br />
            </td>
          </tr>
          <tr className="hometbl tr td">
            <td></td>
            <td>
              <CaretRightOutlined />
            </td>
            <td>
              <Link to="/object-list/calendar/private" tabIndex={2}>
                {t("menu-prv-cal")}
              </Link>
            </td>
          </tr>
          <tr className="hometbl tr td">
            <td></td>
            <td>
              <CaretRightOutlined />
            </td>
            <td>
              <Link to="/object-list/filter/public" tabIndex={3}>
                {t("menu-pub-flt")}
              </Link>
            </td>
          </tr>
          <tr className="hometbl tr td">
            <td></td>
            <td>
              <CaretRightOutlined />
            </td>
            <td>
              <Link to="/object-list/filter/private" tabIndex={4}>
                {t("menu-prv-flt")}
              </Link>
            </td>
          </tr>
          <tr className="hometbl tr td">
            <td></td>
            <td rowSpan="2">
              <label>{t("menu-schd")}</label>
            </td>
            <td>
              <CaretRightOutlined />
            </td>
            <td>
              <Link to="/object-list/schedule/public" tabIndex={5}>
                {t("menu-pub-schd")}
              </Link>
            </td>
          </tr>
          <tr className="hometbl tr td">
            <td></td>
            <td>
              <CaretRightOutlined />
            </td>
            <td>
              <Link to="/object-list/schedule/private" tabIndex={6}>
                {t("menu-prv-schd")}
              </Link>
            </td>
          </tr>
          <tr className="hometbl tr td">
            <td></td>
            <td rowSpan="2">
              <label> {t("menu-jobnet")} </label>
            </td>
            <td>
              <CaretRightOutlined />
            </td>
            <td>
              <Link to="/object-list/jobnet/public" tabIndex={7}>
                {t("menu-pub-jobnet")}
              </Link>
            </td>
          </tr>
          <tr className="hometbl tr td">
            <td></td>
            <td>
              <CaretRightOutlined />
            </td>
            <td>
              <Link to="/object-list/jobnet/private" tabIndex={8}>
                {t("menu-prv-jobnet")}
              </Link>
            </td>
          </tr>

          <tr className="hometbl tr td">
            <td rowSpan="3">
              <CaretRightOutlined />
            </td>
            <td rowSpan="3" colSpan="4">
              <Link to="/job-execution-management" tabIndex={9}>
                {t("menu-job-exe-mgt")}
              </Link>
            </td>
            <td colSpan="2">
              <label>{t("txt-home-job-exe-fun1")}</label>
            </td>
          </tr>
          <tr className="hometbl tr td">
            <td>
              <label>{t("txt-home-job-exe-fun2")}</label>
            </td>
          </tr>
          <tr className="hometbl tr td">
            <td>
              <label>{t("txt-home-job-exe-fun3")}</label>
            </td>
          </tr>

          <tr className="hometbl tr td">
            <td>
              <CaretRightOutlined />
            </td>
            <td colSpan="4">
              <Link to="/job-execution-result" tabIndex={10}>
                {t("menu-job-exe-result")}
              </Link>
            </td>
            <td>
              <label>{t("txt-home-exe-result-fun")}</label>
            </td>
          </tr>

          <tr className="hometbl tr td">
            <td rowSpan="2">
              <CaretRightOutlined />
            </td>
            <td rowSpan="2" colSpan="4">
              <Link to="/general-setting" tabIndex={11}>
                {t("menu-gen-setting")}
              </Link>
            </td>
            <td>
              <label>{t("txt-home-gen-fun1")}</label>
            </td>
          </tr>
          <tr className="hometbl tr td">
            <td>
              <label>{t("txt-home-gen-fun2")}</label>
            </td>
          </tr>

          <tr className="hometbl tr td">
            <td>
              <CaretRightOutlined />
            </td>
            <td colSpan="4">
              <Link to="/lock-management" tabIndex={12}>
                {t("menu-lock-management")}
              </Link>
            </td>
            <td>
              <label>{t("txt-home-lock-mgt-fun")}</label>
            </td>
          </tr>

          <tr className="hometbl tr td">
            <td rowSpan="3">
              <CaretRightOutlined />
            </td>
            <td colSpan="3">
              <label>{t("menu-export-import")}</label>
            </td>
          </tr>
          <tr className="hometbl tr td">
            <td colSpan="2"></td>
            <td>
              <CaretRightOutlined />
            </td>
            <td>
              <Link to="#" tabIndex={13} onClick={handleExport}>
                {t("txt-exp-all")}
              </Link>
            </td>
            <Export exportOpen={exportOpen} handleExport={handleExport} />
            <td>
              <label>{t("txt-home-export-fun")}</label>
            </td>
          </tr>
          <tr className="hometbl tr td">
            <td colSpan="2"></td>
            <td>
              <CaretRightOutlined />
            </td>
            <td>
              <Link
                to="#"
                tabIndex={14}
                onClick={() => {
                  setFileUpload("HomeFileUpload");
                  setStatus(true);
                }}
              >
                {t("btn-import")}
              </Link>
            </td>
            <td>
              <label>{t("txt-home-import-fun")}</label>
            </td>
          </tr>

          <tr className="hometbl tr td">
            <td>
              <CaretRightOutlined />
            </td>
            <td colSpan="3">
              <Link
                to="#"
                tabIndex={15}
                onClick={() => {
                  setShow(true);
                }}
              >
                {t("btn-logout")}
              </Link>
              <Logout isOpen={show} onCancel={onCancel} />
            </td>
          </tr>
        </table>
      </div>
      <ImportDialog isOpen={status} onHandle={onHandle} upload={fileUpload} />
    </div>
  );
};

export default Home;
