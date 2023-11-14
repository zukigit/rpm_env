import { Descriptions, Space, Divider } from "antd";
import { useTranslation } from "react-i18next";
import moment from "moment";
import {
  jobnetTimeoutTypeLabel,
  multipleStartUpLabel,
} from "../../../common/Jobnet";

const JobExecutionDescription = ({ id, info }) => {
  const { t } = useTranslation();

  return (
    <Descriptions
      size="small"
      labelStyle={{ width: "137px", padding: "5px 10px" }}
      className="exec-description"
      bordered
      // column={4}
      column={{ xxl: 4, xl: 3, lg: 3, md: 3, sm: 2, xs: 1 }}
    >
      <Descriptions.Item label={t("col-manage-id")}>{id}</Descriptions.Item>
      <Descriptions.Item label={t("lab-schedule-time")}>
        {info.scheduled_time && info.scheduled_time !== "0"
          ? moment(info.scheduled_time, "YYYYMMDDhhmmss").format(
              "YYYY/MM/DD HH:mm:ss"
            )
          : ""}
      </Descriptions.Item>
      <Descriptions.Item label={t("col-srt-time")}>
        {info.start_time && info.start_time !== "0" 
          ? moment(info.start_time, "YYYYMMDDhhmmss").format(
              "YYYY/MM/DD HH:mm:ss"
            )
          : ""}
      </Descriptions.Item>
      <Descriptions.Item label={t("col-end-time")}>
        {info.end_time && info.end_time !== "0" 
          ? moment(info.end_time, "YYYYMMDDhhmmss").format(
              "YYYY/MM/DD HH:mm:ss"
            )
          : ""}
      </Descriptions.Item>
      <Descriptions.Item label={t("label-jobnet-id")}>
        {info.jobnet_id || ""}
      </Descriptions.Item>
      <Descriptions.Item label={t("lab-multiple")}>
        {info.multiple_start_up
          ? multipleStartUpLabel(info.multiple_start_up)
          : ""}
      </Descriptions.Item>
      <Descriptions.Item label={t("lab-pub")}>
        {info.public_flag && info.public_flag === "1"
          ? t("sel-yes")
          : t("sel-no")}
      </Descriptions.Item>
      <Descriptions.Item label={t("lab-upd-date")}>
        {info.update_date
          ? moment(info.update_date, "YYYYMMDDhhmmss").format(
              "YYYY/MM/DD HH:mm:ss"
            )
          : ""}
      </Descriptions.Item>
      <Descriptions.Item label={t("label-jobnet-name")} span={2}>
        {info.jobnet_name || ""}
      </Descriptions.Item>
      <Descriptions.Item label={t("lab-user-name")}>
        {info.user_name || ""}
      </Descriptions.Item>
      <Descriptions.Item label={t("label-timeout-min")}>
        <Space split={<Divider type="vertical" />}>
          <>{info.jobnet_timeout || ""}</>
          <>
            {info.timeout_run_type
              ? jobnetTimeoutTypeLabel(info.timeout_run_type)
              : ""}
          </>
        </Space>
      </Descriptions.Item>
      <Descriptions.Item label={t("lab-description")}>
        {info.memo || ""}
      </Descriptions.Item>
    </Descriptions>
  );
};

export default JobExecutionDescription;
