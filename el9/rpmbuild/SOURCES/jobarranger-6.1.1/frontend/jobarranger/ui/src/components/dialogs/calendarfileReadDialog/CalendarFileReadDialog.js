import { Col, Row, Modal } from "antd";
import { useDispatch, useSelector } from "react-redux";
import React, { useEffect, useRef, useState } from "react";
import { t } from "i18next";
import Draggable from "react-draggable";
import {
  resetCalendarDates,
  setFileReadShow,
  setInitRegistData,
} from "../../../store/CalendarSlice";
import { Select } from "antd";
import { Button, Upload, Input } from "antd";
import { alertError } from "../CommonDialog";
import moment from "moment";
const { Option } = Select;

const dummyRequest = ({ file, onSuccess }) => {
  setTimeout(() => {
    onSuccess("ok");
  }, 0);
};
var dateFormat = "YYYY/MM/DD";

const CalendarFileReadDialog = () => {
  const dispatch = useDispatch();
  const [disabled, setDisabled] = useState(false);
  const draggleRef = useRef(null);
  const [bounds, setBounds] = useState({
    left: 0,
    top: 0,
    bottom: 0,
    right: 0,
  });
  const [selectedFile, setSelectedFile] = useState();
  const [dates, setDates] = useState([]);
  const [fileName, setFileName] = useState();
  const [fileToValidate, setFileToValidate] = useState([]);
  const onChange = (info) => {
    if (info.fileList.length > 1) {
      setDates([]);
    }
    setFileName(info.file.name);
    let reader = new FileReader();

    reader.readAsText(info.file.originFileObj);
    reader.onload = (e) => {
      setFileToValidate(e.target.result.split("\r\n"));
    };
  };

  const isFileReadShowSelect = useSelector(
    (state) => state["calendar"].isFileReadShow
  );

  const handleCancel = (e) => {
    dispatch(setFileReadShow(false));
  };

  const handleOk = (e) => {
    if (!fileName) {
      alertError(t("title-error"), t("err-msg-input-file"));
      return false;
    }
    var fileExtension = fileName.replace(/^.*\./, "");
    if (fileExtension != "txt") {
      // dispatch(setFileReadShow(false));
      alertError(t("title-error"), t("lab-txt-file"));

      return false;
    }
    for (var i = 0; i < fileToValidate.length; i++) {
      if (fileToValidate[i] != "") {
        if (!moment(fileToValidate[i], dateFormat, true).isValid()) {
          // dispatch(setFileReadShow(false));
          alertError(t("title-error"), t("err-file-format"));
          return false;
        }
        var dbFormat = "YYYYMMDD";
        var responseDate = moment(fileToValidate[i], dateFormat).format(dbFormat);
        dates.push(responseDate);
      }
    }
    dispatch(setFileReadShow(false));
    dispatch(resetCalendarDates());
    dates.forEach((element) => {
      dispatch(setInitRegistData({ dates: element }));
    });
  };

  const onStart = (_event, uiData) => {
    const { clientWidth, clientHeight } = window.document.documentElement;
    const targetRect = draggleRef.current?.getBoundingClientRect();

    if (!targetRect) {
      return;
    }

    setBounds({
      left: -targetRect.left + uiData.x,
      right: clientWidth - (targetRect.right - uiData.x),
      top: -targetRect.top + uiData.y,
      bottom: clientHeight - (targetRect.bottom - uiData.y),
    });
  };

  const handleChange = (value) => {
    dateFormat = value;
  };

  return (
    <>
      <Modal
        maskClosable={false}
        afterClose={() => {
          setDates([]);
          setFileName();
          dateFormat = "YYYY/MM/DD";
        }}
        destroyOnClose={true}
        width={600}
        title={
          <div
            style={{
              width: "100%",
              cursor: "move",
            }}
            onMouseOver={() => {
              if (disabled) {
                setDisabled(false);
              }
            }}
            onMouseOut={() => {
              setDisabled(true);
            }}
            onFocus={() => { }}
            onBlur={() => { }} // end
          >
            {t("title-file-read")}
          </div>
        }
        visible={isFileReadShowSelect}
        onOk={handleOk}
        onCancel={handleCancel}
        okText={t("btn-reading")}
        cancelText={t("btn-cancel")}
        modalRender={(modal) => (
          <Draggable
            disabled={disabled}
            bounds={bounds}
            onStart={(event, uiData) => onStart(event, uiData)}
          >
            <div ref={draggleRef}>{modal}</div>
          </Draggable>
        )}
      >
        {t("lab-import-file-read")}
        <br />
        <Upload
          showUploadList={false}
          fileList={selectedFile}
          customRequest={dummyRequest}
          onChange={onChange}
        >
          <Row>
            <Col>
              <Button className="file-read">{t("btn-ref")}</Button>
            </Col>
            <Col>
              <Input
                value={fileName}
                placeholder="No file referenced"
                disabled
              />
            </Col>
          </Row>
        </Upload>

        <br />
        {t("lab-format")}
        <br />
        <Select
          defaultValue="YYYY/MM/DD"
          style={{ width: "50%" }}
          onChange={handleChange}
        >
          <Option value="YYYY/MM/DD">YYYY/MM/DD</Option>
          <Option value="YYYY-MM-DD">YYYY-MM-DD</Option>
          <Option value="MM/DD/YYYY">MM/DD/YYYY</Option>
          <Option value="YYYYMMDD">YYYYMMDD</Option>
          <Option value="MMDDYYYY">MMDDYYYY</Option>
        </Select>
      </Modal>
    </>
  );
};

export default CalendarFileReadDialog;
