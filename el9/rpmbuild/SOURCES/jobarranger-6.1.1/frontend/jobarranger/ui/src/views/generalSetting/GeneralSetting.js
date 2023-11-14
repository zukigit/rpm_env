import React from "react";
import { useTranslation } from "react-i18next";
import generalSettingService from "../../services/generalSettingService";
import { useState, useEffect } from "react";
import { Form, Input, Select, Button, InputNumber } from "antd";
import "./GeneralSetting.scss";
import {
  alertInfo,
  confirmDialog,
} from "../../components/dialogs/CommonDialog";
import { RESPONSE_OBJ_KEY } from "../../constants";
const GeneralSetting = () => {
  const [form] = Form.useForm();
  const { t } = useTranslation();
  // const [dataChanged, setDataChanged] = useState(false);
  const [initialData, setInitialData] = useState({
    jobnetViewSpan: "",
    jobnetLoadSpan: "",
    jobnetKeepSpan: "",
    joblogKeepSpan: "",
    standardTime: "",
    notification: "",
    zabbixServerIPaddress: "",
    zabbixServerPortNumber: "",
    zabbixSenderCommand: "",
    messageDestinationServer: "",
    messageDestinationItemKey: "",
    retry: "",
    retryCount: "",
    retryInterval: "",
    heartbeatIntervalTime: "",
    objectLockExpiredTime: "",
    disabledEdit: true,
  });

  const [value, setValue] = useState({
    jobnetViewSpan: "",
    jobnetLoadSpan: "",
    jobnetKeepSpan: "",
    joblogKeepSpan: "",
    standardTime: "",
    notification: "",
    zabbixServerIPaddress: "",
    zabbixServerPortNumber: "",
    zabbixSenderCommand: "",
    messageDestinationServer: "",
    messageDestinationItemKey: "",
    retry: "",
    retryCount: "",
    retryInterval: "",
    heartbeatIntervalTime: "",
    objectLockExpiredTime: "",
    disabledEdit: true,
  });

  const setValueData = (data) => {
    value.jobnetViewSpan = data.jobnetViewSpan;
    value.jobnetLoadSpan = data.jobnetLoadSpan;
    value.jobnetKeepSpan = data.jobnetKeepSpan;
    value.joblogKeepSpan = data.joblogKeepSpan;
    value.standardTime = data.standardTime;
    value.notification = data.notification;
    value.zabbixServerIPaddress = data.zabbixServerIPaddress;
    value.zabbixServerPortNumber = data.zabbixServerPortNumber;
    value.zabbixSenderCommand = data.zabbixSenderCommand;
    value.messageDestinationServer = data.messageDestinationServer;
    value.messageDestinationItemKey = data.messageDestinationItemKey;
    value.messageDestinationItemKey = data.messageDestinationItemKey;
    value.retry = data.retry;
    value.retryCount = data.retryCount;
    value.retryInterval = data.retryInterval;
    value.heartbeatIntervalTime = data.heartbeatIntervalTime;
    value.objectLockExpiredTime = data.objectLockExpiredTime;
    value.disabledEdit = data.disabledEdit
      ? data.disabledEdit
      : value.disabledEdit;
  };

  const setFieldData = (data) => {
    form.setFieldsValue({
      jobnetViewSpan: data.jobnetViewSpan,
      jobnetLoadSpan: data.jobnetLoadSpan,
      jobnetKeepSpan: data.jobnetKeepSpan,
      joblogKeepSpan: data.joblogKeepSpan,
      standardTime: data.standardTime,
      notification: data.notification,
      zabbixServerIPaddress: data.zabbixServerIPaddress,
      zabbixServerPortNumber: data.zabbixServerPortNumber,
      zabbixSenderCommand: data.zabbixSenderCommand,
      messageDestinationServer: data.messageDestinationServer,
      messageDestinationItemKey: data.messageDestinationItemKey,
      retry: data.retry,
      retryCount: data.retryCount,
      retryInterval: data.retryInterval,
      heartbeatIntervalTime: data.heartbeatIntervalTime,
      objectLockExpiredTime: data.objectLockExpiredTime,
      disabledEdit: data.disabledEdit,
    });
  };

  useEffect(() => {
    generalSettingService.getGeneralSetting().then((data) => {

      setInitialData({
        jobnetViewSpan:
          data.detail[RESPONSE_OBJ_KEY.AJAX_MESSAGE_DATA].jobnetViewSpan,
        jobnetLoadSpan:
          data.detail[RESPONSE_OBJ_KEY.AJAX_MESSAGE_DATA].jobnetLoadSpan,
        jobnetKeepSpan:
          data.detail[RESPONSE_OBJ_KEY.AJAX_MESSAGE_DATA].jobnetKeepSpan,
        joblogKeepSpan:
          data.detail[RESPONSE_OBJ_KEY.AJAX_MESSAGE_DATA].joblogKeepSpan,
        standardTime:
          data.detail[RESPONSE_OBJ_KEY.AJAX_MESSAGE_DATA].standardTime,
        notification:
          data.detail[RESPONSE_OBJ_KEY.AJAX_MESSAGE_DATA].notification,
        zabbixServerIPaddress:
          data.detail[RESPONSE_OBJ_KEY.AJAX_MESSAGE_DATA]
            .zabbixServerIPaddress,
        zabbixServerPortNumber:
          data.detail[RESPONSE_OBJ_KEY.AJAX_MESSAGE_DATA]
            .zabbixServerPortNumber,
        zabbixSenderCommand:
          data.detail[RESPONSE_OBJ_KEY.AJAX_MESSAGE_DATA].zabbixSenderCommand,
        messageDestinationServer:
          data.detail[RESPONSE_OBJ_KEY.AJAX_MESSAGE_DATA]
            .messageDestinationServer,
        messageDestinationItemKey:
          data.detail[RESPONSE_OBJ_KEY.AJAX_MESSAGE_DATA]
            .messageDestinationItemKey,
        retry: data.detail[RESPONSE_OBJ_KEY.AJAX_MESSAGE_DATA].retry,
        retryCount:
          data.detail[RESPONSE_OBJ_KEY.AJAX_MESSAGE_DATA].retryCount,
        retryInterval:
          data.detail[RESPONSE_OBJ_KEY.AJAX_MESSAGE_DATA].retryInterval,
        heartbeatIntervalTime:
          data.detail[RESPONSE_OBJ_KEY.AJAX_MESSAGE_DATA]
            .heartbeatIntervalTime,
        objectLockExpiredTime:
          data.detail[RESPONSE_OBJ_KEY.AJAX_MESSAGE_DATA]
            .objectLockExpiredTime,
        disabledEdit:
          data.detail[RESPONSE_OBJ_KEY.AJAX_MESSAGE_DATA].disabledEdit,
      });
      setValueData(data.detail[RESPONSE_OBJ_KEY.AJAX_MESSAGE_DATA]);
      setFieldData(data.detail[RESPONSE_OBJ_KEY.AJAX_MESSAGE_DATA]);
      if (
        JSON.parse(
          data.detail[RESPONSE_OBJ_KEY.AJAX_MESSAGE_DATA].disabledEdit
        ) == true
      ) {
        alertInfo("", t("info-msg-read-only"));
      }
    });
  }, []);

  const reDisplayOk = () => {
    setFieldData(value);
  };

  const UpdateOk = () => {
    let updateData = {
      value,
    };
    generalSettingService.UpdateGeneralSetting(updateData).then((data) => {
      if (data.type == "Incomplete") {
        alertInfo(t("warn-title-update"), t("db-lock"));
      } else {

        setValueData(data.detail[RESPONSE_OBJ_KEY.AJAX_MESSAGE_DATA]);
      }
    });
  };

  function cancel() {
    return false;
  }

  const onFinish = (values) => {
    if (dataChanged(values)) {
      setValueData(values);
      confirmDialog(
        t("warn-title-update"),
        t("warn-mess-update"),
        UpdateOk,
        cancel
      );
    }
  };

  const reDisplay = () => {
    if (dataChanged(form.getFieldsValue())) {
      confirmDialog(
        t("warn-mess-title"),
        t("warn-mess-redisplay"),
        reDisplayOk,
        cancel
      );
    }
  };

  const dataChanged = (values) => {
    if (value.jobnetViewSpan != values.jobnetViewSpan) {
      return true;
    }
    if (value.jobnetLoadSpan != values.jobnetLoadSpan) {
      return true;
    }
    if (value.jobnetKeepSpan != values.jobnetKeepSpan) {
      return true;
    }
    if (value.joblogKeepSpan != values.joblogKeepSpan) {
      return true;
    }
    if (value.standardTime != values.standardTime) {
      return true;
    }
    if (value.notification != values.notification) {
      return true;
    }
    if (value.zabbixServerIPaddress != values.zabbixServerIPaddress) {
      return true;
    }
    if (value.zabbixServerPortNumber != values.zabbixServerPortNumber) {
      return true;
    }
    if (value.zabbixSenderCommand != values.zabbixSenderCommand) {
      return true;
    }
    if (value.messageDestinationServer != values.messageDestinationServer) {
      return true;
    }
    if (value.messageDestinationItemKey != values.messageDestinationItemKey) {
      return true;
    }
    if (value.retry != values.retry) {
      return true;
    }
    if (value.retryCount != values.retryCount) {
      return true;
    }
    if (value.retryInterval != values.retryInterval) {
      return true;
    }
    if (value.heartbeatIntervalTime != values.heartbeatIntervalTime) {
      return true;
    }
    if (value.objectLockExpiredTime != values.objectLockExpiredTime) {
      return true;
    }
    return false;
  };

  return (
    <div>
      <Form
        id="create-form"
        labelCol={{ xs: 10, sm: 10, md: 8, lg: 8, xl: 5 }}
        form={form}
        name="control-hooks"
        onFinish={onFinish}
        disabled={JSON.parse(value.disabledEdit)}
      >
        <fieldset>
          <legend>{t("lab-lasys-sett")} :</legend>
          <Form.Item name="standardTime" label={t("lab-job-arr-std-time")}>
            <Select style={{ width: "300px" }}>
              <Select.Option value="0">{t("sel-loc-time")}</Select.Option>
              <Select.Option value="1">{t("sel-ser-time")}</Select.Option>
            </Select>
          </Form.Item>
          <Form.Item
            name="jobnetViewSpan"
            label={t("lab-jn-vi-sp")}
            rules={[{ required: true }]}
          >
            <InputNumber min={1} max={1059127200} style={{ width: "300px" }} />
          </Form.Item>
          <Form.Item
            name="jobnetLoadSpan"
            label={t("lab-jn-lo-sp")}
            rules={[{ required: true }]}
          >
            <InputNumber min={1} max={2147483647} style={{ width: "300px" }} />
          </Form.Item>
          <Form.Item
            name="jobnetKeepSpan"
            label={t("lab-jn-ke-sp")}
            rules={[{ required: true }]}
          >
            <InputNumber min={1} max={2147483647} style={{ width: "300px" }} />
          </Form.Item>
          <Form.Item
            name="joblogKeepSpan"
            label={t("lab-jl-ke-sp")}
            rules={[{ required: true }]}
          >
            <InputNumber min={1} max={2147483647} style={{ width: "300px" }} />
          </Form.Item>
        </fieldset>
        <fieldset>
          <legend>{t("lab-zab-noti-sett")} :</legend>
          <Form.Item name="notification" label={t("lab-zab-noti")}>
            <Select style={{ width: "300px" }}>
              <Select.Option value="0">{t("sel-no")}</Select.Option>
              <Select.Option value="1">{t("sel-yes")}</Select.Option>
            </Select>
          </Form.Item>
          <Form.Item
            name="zabbixServerIPaddress"
            label={t("lab-zab-ser-ip-add")}
            rules={[
              { required: true },
              { pattern: /^([0-9]{1,3}\.){3}[0-9]{1,3}$/, message: t("err-msg-general-setting-invalid-ip") }
            ]}
          >
            <Input style={{ width: "300px" }} />
          </Form.Item>
          <Form.Item
            name="zabbixServerPortNumber"
            label={t("lab-zab-ser-po-num")}
            rules={[{ required: true }]}
          >
            <InputNumber min={0} max={65535} style={{ width: "300px" }} />
          </Form.Item>
          <Form.Item
            name="zabbixSenderCommand"
            label={t("lab-zab-sen-com")}
            rules={[{ required: true }]}
          >
            <Input style={{ width: "300px" }} />
          </Form.Item>
          <Form.Item
            name="messageDestinationServer"
            label={t("lab-mes-des-ser")}
            rules={[{ required: true }]}
          >
            <Input style={{ width: "300px" }} />
          </Form.Item>
          <Form.Item
            name="messageDestinationItemKey"
            label={t("lab-mes-des-item-key")}
            rules={[{ required: true }]}
          >
            <Input style={{ width: "300px" }} />
          </Form.Item>
          <Form.Item name="retry" label={t("lab-retry")}>
            <Select style={{ width: "300px" }}>
              <Select.Option value="0">{t("sel-no-retry")}</Select.Option>
              <Select.Option value="1">{t("sel-retry")}</Select.Option>
            </Select>
          </Form.Item>
          <Form.Item
            name="retryCount"
            label={t("lab-retry-count")}
            rules={[{ required: true }]}
          >
            <InputNumber min={0} max={2147483647} style={{ width: "300px" }} />
          </Form.Item>
          <Form.Item
            name="retryInterval"
            label={t("lab-retry-inter")}
            rules={[{ required: true }]}
          >
            <InputNumber min={1} max={2147483647} style={{ width: "300px" }} />
          </Form.Item>
        </fieldset>
        <fieldset>
          <legend>{t("lab-web-sett")} :</legend>
          <Form.Item
            name="heartbeatIntervalTime"
            label={t("lab-heartbeat-interval")}
            rules={[{ required: true }]}
          >
            <InputNumber min={1} style={{ width: "300px" }} />
          </Form.Item>
          <Form.Item
            name="objectLockExpiredTime"
            label={t("lab-objlock-expired-time")}
            rules={[{ required: true }]}
          >
            <InputNumber min={1} style={{ width: "300px" }} />
          </Form.Item>
        </fieldset>
        <div style={{ height: "60px" }}></div>
        <br className="small" />
        <span style={{ fontWeight: "bold" }}>{t("lab-remark1")} </span>
        <br className="small" />
        <span style={{ fontWeight: "bold" }}>{t("lab-remark2")} </span>
        <br className="small" />
        <span style={{ fontWeight: "bold" }}>{t("lab-remark3")} </span>
        <br className="small" />
        <div style={{ float: "right" }}>
          <Form.Item>
            <Button onClick={reDisplay}>{t("btn-redispaly")}</Button>
            <Button type="primary" htmlType="submit">{t("btn-update")}</Button>
          </Form.Item>
        </div>
      </Form>
    </div >
  );
};
export default GeneralSetting;
