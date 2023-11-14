import React, { useEffect, useState } from "react";

import Flatpickr from "react-flatpickr";
import { Button, Form, Collapse, Col, Row, Select, Input, Modal } from "antd";
import { useTranslation } from "react-i18next";
import "./jobExecutionResult.scss";
import "flatpickr/dist/flatpickr.css";
import { useDispatch } from "react-redux";
import JobExecutionResultService from "../../services/JobExecutionResultService";
import VirtualScrollTable from "../../components/tables/virtualScrollTable/VirtualScrollTable";
import {
  getJobExecutionResult,
  cleanupResultListSlice,
} from "../../store/JobExecutionResultSlice";
import { confirmDialog } from "../../components/dialogs/CommonDialog";
import moment from "moment";
import store from "../../store";
import { alertExdetail } from "../../components/dialogs/CommonDialog";

const { Panel } = Collapse;
const { Option } = Select;

function dateToString(dateTime) {
  const year = dateTime.getFullYear();
  const month = ("0" + (dateTime.getMonth() + 1)).slice(-2);
  const date = ("0" + dateTime.getDate()).slice(-2);
  const hours = ("0" + dateTime.getHours()).slice(-2);
  const minutes = ("0" + dateTime.getMinutes()).slice(-2);
  const seconds = ("0" + dateTime.getSeconds()).slice(-2);
  const miliseconds = ("00" + dateTime.getMilliseconds()).slice(-3);
  return year + month + date + hours + minutes + seconds + miliseconds;
}

function getFormatDateString(str) {
  const strDate = str.toString();
  return (
    strDate.substring(0, 4) +
    "/" +
    strDate.substring(4, 6) +
    "/" +
    strDate.substring(6, 8) +
    " " +
    strDate.substring(8, 10) +
    ":" +
    strDate.substring(10, 12) +
    ":" +
    strDate.substring(12, 14)
  );
}

const JobExecutionResult = () => {
  let action = "";
  const dispatch = useDispatch();
  const [userData, setUserData] = useState();
  const searchData = Object();
  const showDetail = (output, error) => {
    alertExdetail(
      t("title-job-exe-res-detail-popup"),
      relatedDataErrMsg(output, error),
      600
    );
  };

  const relatedMessage = (data, header) => {
    return (
      <div>
        <strong style={{ margin: "10px" }}>{t(header)}...</strong>
        <br />
        <textarea
          name="stdOut"
          id="stdOut"
          cols="60"
          rows="8"
          readOnly
          className="resize-none"
        >
          {data}
        </textarea>
      </div>
    );
  };

  const relatedDataErrMsg = (output, error) => {
    return (
      <>
        {relatedMessage(output, "STD_OUT")}
        {relatedMessage(error, "STD_ERR")}
      </>
    );
  };

  useEffect(() => {
    JobExecutionResultService.getAllUserAlias().then((result) => {
      const children = [];
      children.push(<Option value="">{t("lab-all")}</Option>);
      for (let i = 0; i < result.data.length; i++) {
        children.push(
          <Option value={result.data[i].username}>
            {result.data[i].username}
          </Option>
        );
      }
      setUserData(children);
    });
  }, [JobExecutionResultService, setUserData]);

  const { t } = useTranslation();
  const today = new Date();

  const DateOptions = {
    enableTime: true,
    time_24hr: true,
  };

  const [form] = Form.useForm();
  const handleSubmit = (values) => {
    let searchData = {
      fromDateTime: dateToString(new Date(values.fromDateTime)),
      toDateTime: dateToString(new Date(values.toDateTime)),
      jobnetId: values.jobnetId,
      jobId: values.jobId,
      manageId: values.manageId,
      userName: values.userName,
      userType: "3",
      lang: "en_us",
    };
    if (action === "search") {
      dispatch(getJobExecutionResult(searchData));
    } else if (action === "export") {
      if (store.getState().jobExecutionResultList.data.length > 0) {
        confirmDialog(
          t("title-export"),
          t("lab-export-info-csv"),
          okCsvExport.bind(this, searchData),
          cancelCsvExport
        );
      }
    }
  };

  const cancelCsvExport = () => {};

  const okCsvExport = (searchData) => {
    JobExecutionResultService.getExportCsv(searchData).then((resultData) => {
      const filename = generateExecResultFileName("JobExecutionResult");
      if (resultData instanceof Blob) {
        const a = document.createElement("a");
        const url = window.URL || window.webkitURL;
        const downloadURL = url.createObjectURL(resultData);
        a.href = downloadURL;
        a.download = filename;
        document.body.append(a);
        a.click();
        a.remove();
        url.revokeObjectURL(downloadURL);
      }
    });
  };

  function prepareData(dataList) {
    let arr = [];
    for (let i = 0; i < dataList.length; i++) {
      arr.push({
        key: i,
        output: dataList[i].std_out,
        error: dataList[i].std_err,
        runDate: getFormatDateString(dataList[i].log_date),
        manageId: dataList[i].inner_jobnet_main_id,
        jobnetId: dataList[i].jobnet_id,
        jobId: dataList[i].job_id,
        status: dataList[i].message,
        jnName: dataList[i].jobnet_name,
        jobName: dataList[i].job_name,
        userName: dataList[i].user_name,
        updDate: getFormatDateString(dataList[i].update_date),
        rtnValue: dataList[i].return_code,
      });
    }
    return arr;
  }

  function refreshPage() {
    form.setFieldsValue({
      fromDateTime:
        today.getFullYear() +
        "/" +
        (today.getMonth() + 1) +
        "/" +
        today.getDate() +
        " 00:00",
      toDateTime:
        today.getFullYear() +
        "/" +
        (today.getMonth() + 1) +
        "/" +
        today.getDate() +
        " 23:59",
      jobnetId: "",
      jobId: "",
      manageId: "",
      userName: "",
    });
    dispatch(cleanupResultListSlice([]));
    // window.location.reload(false);
  }

  function FromDayToDayChanges(from, to) {
    let day = 0;
    let month = 0;
    let year = 0;
    let minute = 0;
    let hour = 0;

    var FromDate = new Date();
    let ToDate = new Date();

    const tempFrom = from.split(",");
    if (tempFrom[0].includes("-")) {
      const fromArray = tempFrom[0].split("-");
      if (fromArray[1] === "d") {
        day = parseInt(fromArray[0]);
        FromDate.setDate(FromDate.getDate() - day);
      } else if (fromArray[1] === "M") {
        month = parseInt(fromArray[0]);
        FromDate.setMonth(FromDate.getMonth() - month);
      } else if (fromArray[1] === "y") {
        year = parseInt(fromArray[0]);
        FromDate.setFullYear(FromDate.getFullYear() - year);
      } else if (fromArray[1] === "m") {
        minute = parseInt(fromArray[0]);
        FromDate.setMinutes(FromDate.getMinutes() - minute);
      } else if (fromArray[1] === "h") {
        hour = parseInt(fromArray[0]);
        FromDate.setHours(FromDate.getHours() - hour);
      }
    }
    if (tempFrom[1].includes("/")) {
      const fromArray = tempFrom[1].split("/");
      if (fromArray[1] === "d") {
      } else if (fromArray[1] === "w") {
        const day = FromDate.getDay();
        const diff = FromDate.getDate() - day + (day === 0 ? -6 : 0);
        FromDate.setDate(diff);
      } else if (fromArray[1] === "M") {
        FromDate = new Date(FromDate.getFullYear(), FromDate.getMonth(), 1);
      } else if (fromArray[1] === "y") {
        FromDate = new Date(FromDate.getFullYear(), 0, 1);
      }
      FromDate.setHours(0, 0, 0, 0);
    }

    const tempTo = to.split(",");

    if (tempTo[0].includes("-")) {
      const toArray = tempTo[0].split("-");
      if (toArray[1] === "d") {
        day = parseInt(toArray[0]);
        ToDate.setDate(ToDate.getDate() - day);
      } else if (toArray[1] === "M") {
        month = parseInt(toArray[0]);
        ToDate.setMonth(ToDate.getMonth() - month);
      } else if (toArray[1] === "y") {
        year = parseInt(toArray[0]);
        ToDate.setFullYear(ToDate.getFullYear() - year);
      } else if (toArray[1] === "m") {
        minute = parseInt(toArray[0]);
        ToDate.setMinutes(ToDate.getMinutes() - minute);
      } else if (toArray[1] === "h") {
        hour = parseInt(toArray[0]);
        ToDate.setHours(ToDate.getHours() - hour);
      }
    }

    if (tempTo[1].includes("/")) {
      const toArray = tempTo[1].split("/");
      if (toArray[1] === "d") {
      } else if (toArray[1] === "w") {
        const day = ToDate.getDay();
        const diff = ToDate.getDate() - day + (day === 0 ? -6 : 6);
        ToDate.setDate(diff);
      } else if (toArray[1] === "M") {
        ToDate = new Date(ToDate.getFullYear(), ToDate.getMonth() + 1, 0);
      } else if (toArray[1] === "y") {
        ToDate = new Date(ToDate.getFullYear(), 11, 31);
      }
      ToDate.setHours(23, 59, 59, 999);
    }

    form.setFieldsValue({
      fromDateTime:
        FromDate.getFullYear() +
        "/" +
        (FromDate.getMonth() + 1) +
        "/" +
        FromDate.getDate() +
        " " +
        FromDate.getHours() +
        ":" +
        FromDate.getMinutes(),
      toDateTime:
        ToDate.getFullYear() +
        "/" +
        (ToDate.getMonth() + 1) +
        "/" +
        ToDate.getDate() +
        " " +
        ToDate.getHours() +
        ":" +
        ToDate.getMinutes(),
    });
  }

  const DATE_RANGE = [
    ["5-m,", "now,", t("lab-time-range-5min")],
    ["15-m,", "now,", t("lab-time-range-15min")],
    ["30-m,", "now,", t("lab-time-range-30min")],
    ["1-h,", "now,", t("lab-time-range-1hr")],
    ["3-h,", "now,", t("lab-time-range-3hr")],
    ["6-h,", "now,", t("lab-time-range-6hr")],
    ["12-h,", "now,", t("lab-time-range-12hr")],
    ["24-h,", "now,", t("lab-time-range-1d")],
    ["2-d,", "now,", t("lab-time-range-2d")],
    ["7-d,", "now,", t("lab-time-range-7d")],
    ["30-d,", "now,", t("lab-time-range-30d")],
    ["3-M,", "now,", t("lab-time-range-3m")],
    ["6-M,", "now,", t("lab-time-range-6m")],
    ["1-y,", "now,", t("lab-time-range-1y")],
    ["2-y,", "now,", t("lab-time-range-2y")],
    ["now,/d", "now,/d", t("lab-time-range-tdy")],
    ["now,/d", "now,", t("lab-time-range-tdy-now")],
    ["now,/w", "now,/w", t("lab-time-range-wek")],
    ["now,/w", "now,", t("lab-time-range-wek-now")],
    ["now,/M", "now,/M", t("lab-time-range-m")],
    ["now,/M", "now,", t("lab-time-range-m-now")],
    ["now,/y", "now,/y", t("lab-time-range-y")],
    ["now,/y", "now,", t("lab-time-range-y-now")],
    ["1-d,/d", "1-d,/d", t("lab-time-range-yst")],
    ["2-d,/d", "2-d,/d", t("lab-time-range-bef-yst")],
    ["7-d,/d", "7-d,/d", t("lab-time-range-cur-week")],
    ["7-d,/w", "7-d,/w", t("lab-time-range-pev-week")],
    ["1-M,/M", "1-M,/M", t("lab-time-range-pev-m")],
    ["1-y,/y", "1-y,/y", t("lab-time-range-pev-y")],
  ];

  const DateRangeButton1 = [];
  const DateRangeButton2 = [];
  const DateRangeButton3 = [];
  const DateRangeButton4 = [];
  const DateRangeButton5 = [];
  for (let i = 0; i < DATE_RANGE.length; i++) {
    if (DateRangeButton1.length < 6) {
      DateRangeButton1.push(
        timeRangeButton(DATE_RANGE[i][0], DATE_RANGE[i][1], DATE_RANGE[i][2])
      );
    } else if (DateRangeButton2.length < 6) {
      DateRangeButton2.push(
        timeRangeButton(DATE_RANGE[i][0], DATE_RANGE[i][1], DATE_RANGE[i][2])
      );
    } else if (DateRangeButton3.length < 6) {
      DateRangeButton3.push(
        timeRangeButton(DATE_RANGE[i][0], DATE_RANGE[i][1], DATE_RANGE[i][2])
      );
    } else if (DateRangeButton4.length < 6) {
      DateRangeButton4.push(
        timeRangeButton(DATE_RANGE[i][0], DATE_RANGE[i][1], DATE_RANGE[i][2])
      );
    } else if (DateRangeButton5.length < 6) {
      DateRangeButton5.push(
        timeRangeButton(DATE_RANGE[i][0], DATE_RANGE[i][1], DATE_RANGE[i][2])
      );
    }
  }

  function timeRangeButton(from, to, display) {
    return (
      <li>
        <Button className="link" onClick={() => FromDayToDayChanges(from, to)}>
          {display}
        </Button>
      </li>
    );
  }

  const DEFAULT_JOB_EXEC_RESULT_TABLE_HEADER = [
    {
      title: t("lab-detail"),
      dataIndex: "detail",
      searchable: false,
      sortDirections: ["descend", "ascend"],
      width: 90,
      showSorterTooltip: false,
      ellipsis: true,
      render: (_, record) => {
        return record.output !== null || record.error !== null ? (
          <Button
            type="primary"
            size="small"
            onClick={showDetail.bind(this, record.output, record.error)}
          >
            {t("lab-detail")}
          </Button>
        ) : (
          ""
        );
      },
    },
    {
      title: t("col-run-date"),
      dataIndex: "runDate",
      searchable: false,
      ellipsis: true,
      showSorterTooltip: false,
      sorter: (a, b) => moment(a.runDate).unix() - moment(b.runDate).unix(),
      sortDirections: ["descend", "ascend"],
    },
    {
      title: t("lab-mag-id"),
      dataIndex: "manageId",
      searchable: false,
      ellipsis: true,
      showSorterTooltip: false,
      sorter: (a, b) => parseInt(BigInt(a.manageId) - BigInt(b.manageId)),/* global BigInt */
      sortDirections: ["descend", "ascend"],
    },
    {
      title: t("label-jobnet-id"),
      dataIndex: "jobnetId",
      searchable: false,
      ellipsis: true,
      showSorterTooltip: false,
      sorter: (a, b) => a.jobnetId.localeCompare(b.jobnetId),
      sortDirections: ["descend", "ascend"],
    },
    {
      title: t("lab-job-id"),
      dataIndex: "jobId",
      searchable: false,
      ellipsis: true,
      showSorterTooltip: false,
      sorter: (a, b) =>
        a.jobId && b.jobId ? a.jobId.localeCompare(b.jobId) : a.jobId ? 1 : -1,
      sortDirections: ["descend", "ascend"],
    },
    {
      title: t("col-status"),
      dataIndex: "status",
      searchable: false,
      ellipsis: true,
      showSorterTooltip: false,
      sorter: (a, b) =>
        a.status && b.status
          ? a.status.localeCompare(b.status)
          : a.status
          ? 1
          : -1,
      sortDirections: ["descend", "ascend"],
    },
    {
      title: t("label-jobnet-name"),
      dataIndex: "jnName",
      searchable: false,
      ellipsis: true,
      showSorterTooltip: false,
      sorter: (a, b) => a.jnName.localeCompare(b.jnName),
      sortDirections: ["descend", "ascend"],
    },
    {
      title: t("job-name"),
      dataIndex: "jobName",
      searchable: false,
      ellipsis: true,
      showSorterTooltip: false,
      sorter: (a, b) =>
        a.jobName && b.jobName
          ? a.jobName.localeCompare(b.jobName)
          : a.jobName
          ? 1
          : -1,
      sortDirections: ["descend", "ascend"],
    },
    {
      title: t("lab-user-name"),
      dataIndex: "userName",
      searchable: false,
      ellipsis: true,
      showSorterTooltip: false,
      sorter: (a, b) => a.userName.localeCompare(b.userName),
      sortDirections: ["descend", "ascend"],
    },
    {
      title: t("lab-upd-date"),
      dataIndex: "updDate",
      searchable: false,
      ellipsis: true,
      showSorterTooltip: false,
      sorter: (a, b) => moment(a.updDate).unix() - moment(b.updDate).unix(),
      sortDirections: ["descend", "ascend"],
    },
    {
      title: t("col-return-value"),
      dataIndex: "rtnValue",
      className: "table-return-row",
      searchable: false,
      ellipsis: true,
      showSorterTooltip: false,
      // sorter: (a, b) => a.rtnValue.length - b.rtnValue.length,
      // sortDirections: ["descend", "ascend"],
    },
  ];

  const generateExecResultFileName = function (type) {
    const fileName =
      "Export_" + type + "_" + dateToString(new Date(), false) + ".csv";

    return fileName;
  };

  return (
    <div className="main-panel">
      <div className="content-wrapper">
        <Collapse defaultActiveKey={["1"]} expandIconPosition={"end"}>
          <Panel header={t("lab-sea-ter")} key="1">
            <Form
              onFinish={handleSubmit}
              form={form}
              layout="inline"
              fields={[
                {
                  name: ["userName"],
                  value: "",
                },
              ]}
              initialValues={{
                fromDateTime:
                  today.getFullYear() +
                  "/" +
                  (today.getMonth() + 1) +
                  "/" +
                  today.getDate() +
                  " 00:00",
                toDateTime:
                  today.getFullYear() +
                  "/" +
                  (today.getMonth() + 1) +
                  "/" +
                  today.getDate() +
                  " 23:59",
              }}
            >
              <Row type="flex" justify="space-between" gutter={16}>
                <Col>
                  <Form.Item
                    name="fromDateTime"
                    label={t("lab-from")}
                    rules={[
                      {
                        required: true,
                        message: "Please input From Date!",
                      },
                    ]}
                  >
                    <Flatpickr className="datepicker" options={DateOptions} />
                  </Form.Item>
                </Col>
                <Col>
                  <Form.Item
                    name="toDateTime"
                    label={t("lab-to")}
                    rules={[
                      {
                        required: true,
                        message: "Please input To Date!",
                      },
                    ]}
                  >
                    <Flatpickr className="datepicker" options={DateOptions} />
                  </Form.Item>
                </Col>
                <Col>
                  <div className="cell">
                    <ul className="timeList">{DateRangeButton1}</ul>
                  </div>
                  <div className="cell">
                    <ul className="timeList">{DateRangeButton2}</ul>
                  </div>
                  <div className="cell">
                    <ul className="timeList">{DateRangeButton3}</ul>
                  </div>
                  <div className="cell">
                    <ul className="timeList">{DateRangeButton4}</ul>
                  </div>
                  <div className="cell">
                    <ul className="timeList">{DateRangeButton5}</ul>
                  </div>
                </Col>
              </Row>
              <Row type="flex" justify="space-between" gutter={16}>
                <Col>
                  <Form.Item name="jobnetId" label={t("label-jobnet-id")}>
                    <Input />
                  </Form.Item>
                </Col>
                <Col>
                  <Form.Item name="jobId" label={t("lab-job-id")}>
                    <Input />
                  </Form.Item>
                </Col>
                <Col>
                  <Form.Item name="manageId" label={t("lab-mag-id")}>
                    <Input />
                  </Form.Item>
                </Col>
                <Col>
                  <Form.Item name="userName" label={t("lab-user-name")}>
                    <Select
                      style={{
                        width: 120,
                      }}
                      defaultValue=""
                    >
                      {userData}
                    </Select>
                  </Form.Item>
                </Col>
                <Col>
                  <Form.Item>
                    <Button
                      type="primary"
                      onClick={() => {
                        action = "search";
                        form.submit();
                      }}
                    >
                      {t("btn-search")}
                    </Button>
                    <Button type="primary" onClick={refreshPage}>
                      {t("btn-reset")}
                    </Button>
                    <Button
                      type="primary"
                      onClick={() => {
                        action = "export";
                        form.submit();
                      }}
                    >
                      {t("btn-csv")}
                    </Button>
                  </Form.Item>
                </Col>
              </Row>
            </Form>
          </Panel>
        </Collapse>
        <VirtualScrollTable
          stateId={"jobExecutionResultList"}
          dispatchAction={null}
          hasRowSelect={false}
          columnHeaders={DEFAULT_JOB_EXEC_RESULT_TABLE_HEADER}
          prepareData={prepareData}
          params={searchData}
          rowClassName={null}
          onDoubleClickAction={null}
          category={null}
        />

        <div
          id="detail_dialog"
          title={t("title-job-exe-res-detail-popup")}
          className="display-none"
        >
          <br></br>
          <span className="lang-lab-stdout-capital">
            {" "}
            {t("lab-stdout-capital")} :
          </span>
          <textarea
            name="stdOut"
            id="stdOut"
            cols="90"
            rows="8"
            readOnly
            className="resize-none"
          ></textarea>
          <br></br>
          <br></br>
          <span className="lang-lab-stderr-capital">
            {" "}
            {t("lab-stderr-capital")} :
          </span>
          <textarea
            name="stdErr"
            id="stdErr"
            cols="90"
            rows="8"
            readOnly
            className="resize-none"
          ></textarea>
        </div>
        <div id="csv_dialog" title={t("title-export")} className="display-none">
          <br></br>
          <br></br>
          <span>{t("lab-export-info-csv")}</span>
          <br></br>
          <br></br>
        </div>
      </div>
    </div>
  );
};

export default JobExecutionResult;
