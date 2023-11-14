import React, { useEffect, useState } from "react";
import "./ScheduleFormObject.scss";
import { Input, Col, Row, Form, Radio } from "antd";
import { useTranslation } from "react-i18next";
import { OBJECT_CATEGORY } from "../../../constants";
import { useSelector } from "react-redux";
import moment from "moment";

const FormObject = ({
  formId,
  objectSlice,
  isCalendar = true,
  formProperty = null,
}) => {
  const form = useSelector((state) => {
    if (objectSlice === "jobnetForm") {
      return state.jobnetForm.formObjList[formProperty].formData;
    } else {
      return state[objectSlice].formData;
    }
  }); //calendar // jobnet/// jobnet.child {.formObject}
  const { t } = useTranslation();
  const objectForm = Form.useFormInstance();

  const [value, setValue] = useState(1);

  const onChange = (e) => {
    setValue(e.target.value);
  };

  useEffect(() => {
    if (form) {
      objectForm.setFieldsValue({
        sch_id: form.id,
        sch_name: form.name,
        sch_isPublic: form.isPublic,
        sch_updateDate: form.updateDate,
        sch_lastWorkingDay: form.lastWorkingDay,
        sch_userName: form.userName,
        sch_description: form.description,
      });
    }
  }, [form]);
  var radioFlag = true;
  useEffect(() => {
    if (isCalendar) {
      radioFlag = value === 1 ? true : false;

      document.getElementById("starttime").disabled = radioFlag;
      document.getElementById("endtime").disabled = radioFlag;
      document.getElementById("interval").disabled = radioFlag;

      document.getElementById("timevalue").disabled = !radioFlag;
      document.getElementById("timevalue").value = "";

      document.getElementById("starttime").value = "";
      document.getElementById("endtime").value = "";
      document.getElementById("interval").value = "";
    }
  }, [value]);

  const multipleInput = (formId) => {
    if (formId === OBJECT_CATEGORY.JOBNET) {
      return (
        <Col xs={24} sm={24} md={9} lg={10} xl={5}>
          <Form.Item
            labelAlign="left"
            labelCol={{ xs: 10, sm: 6, md: 7, lg: 7, xl: 7 }}
            label={t("lab-multiple")}
            initialValue="0"
          >
            <label>
              {form.multiple === 0 || form.multiple === "0"
                ? t("sel-yes")
                : form.multiple === 1 || form.multiple === "1"
                  ? t("sel-skip")
                  : form.multiple === 2 || form.multiple === "2"
                    ? t("sel-waiting")
                    : ""}
            </label>
          </Form.Item>
        </Col>
      );
    }
    return null;
  };

  const timoutInput = (formId) => {
    if (formId === OBJECT_CATEGORY.JOBNET) {
      return (
        <>
          <Col xs={16} sm={16} md={8} lg={6} xl={5}>
            <Form.Item
              labelAlign="center"
              label={t("lab-timeout")}
              initialValue={"0"}
            >
              <label>{form.timeout}</label>
            </Form.Item>
          </Col>
          <Form.Item>
            <label>
              {form.timeoutType === 0 || form.timeoutType === "0"
                ? t("sel-warning")
                : form.timeoutType === 1 || form.timeoutType === "1"
                  ? t("sel-jn-stop")
                  : ""}
            </label>
          </Form.Item>
        </>
      );
    }
    return null;
  };
  return (
    <>
      <div id="schObjInfo">
        <Form
          id={`${formId}-header-form`}
          name={`${formId}-header-form`}
          form={objectForm}
        >
          <Row className="row" gutter={{ xs: 8, sm: 16, md: 24, lg: 32 }}>
            <Col xs={19} sm={20} md={16} lg={10} xl={8}>
              <Form.Item
                labelAlign="left"
                labelCol={{ xs: 10, sm: 8, md: 7, lg: 7, xl: 9 }}
                label={t("label-" + formId + "-id") + " : "}
              >
                <label>{form.id}</label>
              </Form.Item>
            </Col>
            <Col
              xs={5}
              sm={4}
              md={8}
              lg={3}
              xl={2}
              style={{ paddingTop: "5px" }}
            >
              <Form.Item labelAlign="left" label={t("lab-pub")}>
                {form.isPublic && <label>O</label>}
              </Form.Item>
            </Col>
            <Col
              xs={12}
              sm={12}
              md={6}
              lg={5}
              xl={4}
              style={{ paddingTop: "5px" }}
            >
              <label>{t("lab-authority2")}</label>
            </Col>
            <Col
              xs={12}
              sm={12}
              md={9}
              lg={6}
              xl={5}
              style={{ paddingTop: "5px" }}
            >
              <label>{t("lab-upd-date")} :</label>{" "}
              <label>
                {form.updateDate !== "" && form.updateDate !== null
                  ? moment(form.updateDate, "YYYYMMDDHHmmss").format(
                    "YYYY/MM/DD HH:mm:ss"
                  )
                  : ""}
              </label>
            </Col>
            {multipleInput(formId)}
          </Row>

          <Row className="row" gutter={{ xs: 8, sm: 16, md: 24, lg: 32 }}>
            <Col xs={24} sm={24} md={16} lg={12} xl={10}>
              <Row></Row>
              <Form.Item
                labelAlign="left"
                labelCol={{ xs: 10, sm: 6, md: 7, lg: 6, xl: 7 }}
                label={t("label-" + formId + "-name") + " : "}
              >
                <label>{form.name}</label>
              </Form.Item>
            </Col>
            <Col xs={24} sm={24} md={8} lg={6} xl={4}>
              <label id="usrName">{t("lab-user-name")} :</label>{" "}
              <label>{form.userName}</label>
            </Col>
            <Col xs={24} sm={24} md={8} lg={6} xl={4}>
              {(formId === OBJECT_CATEGORY.CALENDAR ||
                formId === OBJECT_CATEGORY.FILTER) &&
                !isCalendar ? (
                <>
                  <label id="usrName">
                    {t("lab-lastday")} {":"}
                  </label>{" "}
                  <label>
                    {form.lastWorkingDay !== "" && form.lastWorkingDay !== null
                      ? moment(form.lastWorkingDay, "YYYYMMDD").format(
                        "YYYY/MM/DD"
                      )
                      : ""}
                  </label>
                </>
              ) : (
                ""
              )}
            </Col>
          </Row>

          <Row className="row" gutter={{ xs: 8, sm: 16, md: 24, lg: 32 }}>
            <Col xs={24} sm={24} md={16} lg={12} xl={10}>
              <Row></Row>
              <Form.Item
                labelAlign="left"
                labelCol={{ xs: 10, sm: 6, md: 7, lg: 6, xl: 7 }}
                label={t("lab-description")}
              >
                <label>{form.description}</label>
              </Form.Item>
            </Col>
            {timoutInput(formId)}
          </Row>
        </Form>
      </div>

      {(formId === OBJECT_CATEGORY.CALENDAR ||
        formId === OBJECT_CATEGORY.FILTER) &&
        isCalendar && (
          <>
            <br className="small" />
            <div>
              <Radio.Group
                style={{ display: "inline-flex" }}
                onChange={onChange}
                value={value}
              >
                <Radio id="time" name="sch_time" className="left-30" value={1}>
                  {t("lab-time")} : <span className="error">*</span>
                </Radio>
                <Form.Item name="time" rules={[{ pattern: /^(\d{2}):[0-5][0-9]$/, message: t("err-msg-schedule-invalid-time") }]}>
                  <Input
                    style={{ textAlign: "center" }}
                    type="text"
                    id="timevalue"
                    name="sch_timevalue"
                    className="timeInput"
                    placeholder="HH:MM"
                    maxLength="5"
                  />
                </Form.Item>
                <Radio
                  id="cycle"
                  value="2"
                  className="left-30"
                  name="sch_cycle"
                >
                  {t("lab-cycle")} : <span className="error">*</span>
                </Radio>
                <Form.Item name="starttime" rules={[{ pattern: /^(\d{2}):[0-5][0-9]$/, message: t("err-msg-schedule-invalid-time") }]}>
                  <Input
                    type="text"
                    id="starttime"
                    className="timeInput"
                    name="sch_cycle_start"
                    placeholder="HH:MM"
                    maxLength="5"
                  />
                </Form.Item>

                <label>~</label>
                <Form.Item name="endtime" rules={[{ pattern: /^(\d{2}):[0-5][0-9]$/, message: t("err-msg-schedule-invalid-time") }]}>
                  <Input
                    type="text"
                    id="endtime"
                    className="timeInput"
                    placeholder="HH:MM"
                    name="sch_cycle_end"
                    maxLength="5"
                  />
                </Form.Item>
                <Form.Item name="interval">
                  <Input
                    type="text"
                    id="interval"
                    className="minuteInput"
                    maxLength={3}
                  />
                </Form.Item>
                <div>
                  <label> {t("lab-minute")} </label>
                </div>
              </Radio.Group>

              <label className="left-70">
                {t("lab-lastday")} :
                {form.lastWorkingDay !== null && form.lastWorkingDay !== ""
                  ? moment(form.lastWorkingDay, "YYYYMMDD").format("YYYY/MM/DD")
                  : ""}
              </label>
            </div>
            <br className="small" />
            <span id="validationError" className="grid"></span>
          </>
        )}
    </>
  );
};

export default FormObject;
