import { Space, Divider } from "antd";
import { useTranslation } from "react-i18next";
import moment from "moment";
import {
  jobnetTimeoutTypeLabel,
  multipleStartUpLabel,
} from "../../../common/Jobnet";
import "./JobExecutionHeader.scss";

const JobExecutionHeader = ({ id, info }) => {
  const { t } = useTranslation();

  return (
    <>
      <div className="exec-description">
        <div className="desc-item" style={{ width: 230 }}>
          <div className="header-label" style={{ width: "34%" }}>
            {t("col-manage-id")}
          </div>
          <div className="header-value" style={{ width: "66%" }}>
            {id}
          </div>
        </div>

        <div className="desc-item" style={{ width: 240 }}>
          <div className="header-label" style={{ width: "43%" }}>
            {t("lab-schedule-time")}
          </div>
          <div className="header-value" style={{ width: "57%" }}>
            {info.scheduled_time && info.scheduled_time !== "0"
              ? moment(info.scheduled_time, "YYYYMMDDhhmmss").format(
                  "YYYY/MM/DD HH:mm:ss"
                )
              : ""}
          </div>
        </div>
        <div className="desc-item" style={{ width: 240 }}>
          <div className="header-label" style={{ width: "43%" }}>
            {t("col-srt-time")}
          </div>
          <div className="header-value" style={{ width: "57%" }}>
            {info.start_time && info.start_time !== "0"
              ? moment(info.start_time, "YYYYMMDDhhmmss").format(
                  "YYYY/MM/DD HH:mm:ss"
                )
              : ""}
          </div>
        </div>

        <div className="desc-item" style={{ width: 240 }}>
          <div className="header-label" style={{ width: "43%" }}>
            {t("col-end-time")}
          </div>
          <div className="header-value" style={{ width: "57%" }}>
            {info.end_time && info.end_time !== "0"
              ? moment(info.end_time, "YYYYMMDDhhmmss").format(
                  "YYYY/MM/DD HH:mm:ss"
                )
              : ""}
          </div>
        </div>
        <div className="desc-item" style={{ width: 410 }}>
          <div className="header-label" style={{ width: "25%" }}>
            {t("label-jobnet-id")}
          </div>
          <div className="header-value" style={{ width: "75%" }}>
            {info.jobnet_id || ""}
          </div>
        </div>
        <div className="desc-item" style={{ width: 150 }}>
          <div className="header-label" style={{ width: "60%" }}>
            {t("lab-multiple")}
          </div>
          <div className="header-value" style={{ width: "40%" }}>
            {info.multiple_start_up
              ? multipleStartUpLabel(info.multiple_start_up)
              : ""}
          </div>
        </div>
        <div className="desc-item" style={{ width: 150 }}>
          <div className="header-label" style={{ width: "60%" }}>
            {t("lab-pub")}
          </div>
          <div className="header-value" style={{ width: "40%" }}>
            {info.public_flag && info.public_flag === "1"
              ? t("sel-yes")
              : t("sel-no")}
          </div>
        </div>
        <div className="desc-item" style={{ width: 240 }}>
          <div className="header-label" style={{ width: "43%" }}>
            {t("lab-upd-date")}
          </div>
          <div className="header-value" style={{ width: "57%" }}>
            {info.update_date
              ? moment(info.update_date, "YYYYMMDDhhmmss").format(
                  "YYYY/MM/DD HH:mm:ss"
                )
              : ""}
          </div>
        </div>
        <div className="desc-item" style={{ width: 713 }}>
          <div className="header-label" style={{ width: "17%" }}>
            {t("label-jobnet-name")}
          </div>
          <div className="header-value" style={{ width: "83%" }}>
            {info.jobnet_name || ""}
          </div>
        </div>
        <div className="desc-item" style={{ width: 240 }}>
          <div className="header-label" style={{ width: "40%" }}>
            {t("lab-user-name")}
          </div>
          <div className="header-value" style={{ width: "60%" }}>
            {info.user_name || ""}
          </div>
        </div>
        <div className="desc-item" style={{ width: 620 }}>
          <div className="header-label" style={{ width: "15%" }}>
            {t("lab-description")}
          </div>
          <div className="header-value" style={{ width: "85%" }}>
            {info.memo || ""}
          </div>
        </div>
        <div className="desc-item" style={{ width: 330 }}>
          <div className="header-label" style={{ width: "50%" }}>
            {t("label-timeout-min")}
          </div>
          <div className="header-value" style={{ width: "50%" }}>
            <Space split={<Divider type="vertical" />}>
              <>{info.jobnet_timeout || ""}</>
              <>
                {info.timeout_run_type
                  ? jobnetTimeoutTypeLabel(info.timeout_run_type)
                  : ""}
              </>
            </Space>
          </div>
        </div>
      </div>
    </>
  );
};

export default JobExecutionHeader;
