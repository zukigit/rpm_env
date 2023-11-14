import React, { useState, useEffect } from "react";
import { Card, Switch, Space, Button, Tooltip } from "antd";
import { SyncOutlined } from "@ant-design/icons";
import { useDispatch, useSelector } from "react-redux";
import { useTranslation } from "react-i18next";
import moment from "moment";
import PaginationTable from "../../components/tables/paginationTable/PaginationTable";
import VirtualScrollTable from "../../components/tables/virtualScrollTable/VirtualScrollTable";
import {
  jobExecutionManagementMenuRender,
  jobExecutionManagementStatusRender,
} from "../../components/contextMenu/ContextMenu";
import { openExecutionDialog } from "../../store/JobExecutionSlice";
import {
  EXECUTION_MANAGEMENT,
  LOAD_STATUS_TYPE,
  RUN_JOB_STATUS_TYPE,
  START_PEND_FLAG,
} from "../../constants";
import "./JobExecutionManagement.scss";

const OperationTableCard = ({ stateId, dispatchAction, tableType, color }) => {
  const { t } = useTranslation();
  const dispatch = useDispatch();
  const [isListTable, setIsListTable] = useState(false);

  const DEFAULT_OBJECT_LIST_TABLE_HEADER = [
    {
      title: t("col-mgt-id"),
      dataIndex: "managementId",
      width: 150,
      searchable: true,
      ellipsis: true,
      sorter: (a, b) =>
        parseInt(
          BigInt(a.managementId) - BigInt(b.managementId)
        ) /* global BigInt */,
      sortDirections: ["descend", "ascend"],
      render: (text, record, index) => {
        return jobExecutionManagementMenuRender(text, record, tableType);
      },
    },
    {
      title: t("label-jobnet-id"),
      dataIndex: "jobnetId",
      width: 150,
      searchable: true,
      ellipsis: true,
      sorter: (a, b) => a.jobnetId.localeCompare(b.jobnetId),
      sortDirections: ["descend", "ascend"],
      render: (text, record, index) => {
        return jobExecutionManagementMenuRender(text, record, tableType);
      },
    },
    {
      title: t("col-status"),
      dataIndex: "status",
      width: 105,
      searchable: true,
      ellipsis: true,
      sorter: (a, b) => a.status.localeCompare(b.status),
      sortDirections: ["descend", "ascend"],
      render: (text, record, index) => {
        return jobExecutionManagementStatusRender(text, record, tableType);
      },
    },
    {
      title: t("label-jobnet-name"),
      dataIndex: "jobnetName",
      width: 130,
      searchable: true,
      ellipsis: true,
      sorter: (a, b) => a.jobnetName.localeCompare(b.jobnetName),
      sortDirections: ["descend", "ascend"],
      render: (text, record, index) => {
        return jobExecutionManagementMenuRender(text, record, tableType);
      },
    },
    {
      title: t("lab-job-id"),
      dataIndex: "jobId",
      width: 130,
      searchable: true,
      ellipsis: true,
      sorter: (a, b) => {
        if (a.jobId && b.jobId) {
          return a.jobId.localeCompare(b.jobId);
        } else {
          if (a.jobId) {
            return 1;
          } else {
            return -1;
          }
        }
      },
      sortDirections: ["descend", "ascend"],
      render: (text, record, index) => {
        return jobExecutionManagementMenuRender(text, record, tableType);
      },
    },
    {
      title: t("col-job-name"),
      dataIndex: "jobName",
      width: 110,
      searchable: true,
      ellipsis: true,
      sorter: (a, b) => {
        if (a.jobName && b.jobName) {
          return a.jobName.localeCompare(b.jobName);
        } else {
          if (a.jobName) {
            return 1;
          } else {
            return -1;
          }
        }
      },
      sortDirections: ["descend", "ascend"],
      render: (text, record, index) => {
        return jobExecutionManagementMenuRender(text, record, tableType);
      },
    },
    {
      title: t("col-schd-srt-time"),
      dataIndex: "scheduledTime",
      width: 180,
      searchable: true,
      ellipsis: true,
      sorter: (a, b) => {
        if (a.scheduledTime && b.scheduledTime) {
          return moment(a.scheduledTime).unix() - moment(b.scheduledTime).unix();
        } else {
          if (a.scheduledTime) {
            return 1;
          } else {
            return -1;
          }
        }
      },
      sortDirections: ["descend", "ascend"],
      render: (text, record, index) => {
        return jobExecutionManagementMenuRender(text, record, tableType);
      },
    },
    {
      title: t("col-srt-time"),
      dataIndex: "startTime",
      width: 126,
      searchable: true,
      ellipsis: true,
      sorter: (a, b) => {
        if (a.startTime && b.startTime) {
          return moment(a.startTime).unix() - moment(b.startTime).unix();
        } else {
          if (a.startTime) {
            return 1;
          } else {
            return -1;
          }
        }
      },
      sortDirections: ["descend", "ascend"],
      render: (text, record, index) => {
        return jobExecutionManagementMenuRender(text, record, tableType);
      },
    },
    {
      title: t("col-end-time"),
      dataIndex: "endTime",
      width: 126,
      searchable: true,
      ellipsis: true,
      sorter: (a, b) => {
        if (a.endTime && b.endTime) {
          return moment(a.endTime).unix() - moment(b.endTime).unix();
        } else {
          if (a.endTime) {
            return 1;
          } else {
            return -1;
          }
        }
      },
      sortDirections: ["descend", "ascend"],
      render: (text, record, index) => {
        return jobExecutionManagementMenuRender(text, record, tableType);
      },
    },
  ];

  document.addEventListener("keydown", function (event) {
    switch (tableType) {
      case t("title-op-info-job"):
        switch (event.keyCode) {
          case 72:
            if (document.getElementById('btnhide') != null) {
              document.getElementById('btnhide').click();
            }
            break;
          case 70:
            if (document.getElementById('btnstop') != null) {
              document.getElementById('btnstop').click();
            }
            break;
          case 82:
            if (document.getElementById('btndelay') != null) {
              document.getElementById('btndelay').click();
            }
            break;
          case 77:
            if (document.getElementById('btnupdschd') != null) {
              document.getElementById('btnupdschd').click();
            }
            break;
          case 83:
            if (document.getElementById('btnhold') != null) {
              document.getElementById('btnhold').click();
            }
            break;
          case 67:
            if (document.getElementById('btnrelease') != null) {
              document.getElementById('btnrelease').click();
            }
            break;
          default:
            break;
        }
        break;
      case t("title-op-err-job"):
        switch (event.keyCode) {
          case 72:
            if (document.getElementById('btnErrHide') != null) {
              document.getElementById('btnErrHide').click();
            }
            break;
          case 70:
            if (document.getElementById('btnErrStop') != null) {
              document.getElementById('btnErrStop').click();
            }
            break;
          case 82:
            if (document.getElementById('btnErrDelay') != null) {
              document.getElementById('btnErrDelay').click();
            }
            break;
          default:
            break;
        }
        break;
      case t("title-op-exe-job"):
        switch (event.keyCode) {
          case 72:
            if (document.getElementById('btnDurHide') != null) {
              document.getElementById('btnDurHide').click();
            }
            break;
          case 70:
            if (document.getElementById('btnDurStop') != null) {
              document.getElementById('btnDurStop').click();
            }
            break;
          case 82:
            if (document.getElementById('btnDurDelay') != null) {
              document.getElementById('btnDurDelay').click();
            }
            break;
          default:
            break;
        }
        break;
    }


  });

  const selectType =
    tableType === t("title-op-err-job") ? "multiple" : "single";
  const hideSelectAll = tableType === t("title-op-err-job") ? false : true;

  function prepareData(operationList) {
    let arr = [];
    for (let i = 0; i < operationList.length; i++) {
      arr.push({
        key: operationList[i].inner_jobnet_id,
        managementId: operationList[i].inner_jobnet_id,
        jobnetId: operationList[i].jobnet_id,
        status: getRunJobDisplayStatus(
          operationList[i].status,
          operationList[i].load_status,
          operationList[i].start_pending_flag
        ),
        intStatus: operationList[i].status,
        jobnetName: operationList[i].jobnet_name,
        jobId: operationList[i].running_job_id,
        jobName: operationList[i].running_job_name,
        scheduleId: operationList[i].schedule_id,
        scheduledTime:
          operationList[i].scheduled_time == "0" ||
            operationList[i].scheduled_time == null
            ? ""
            : moment(operationList[i].scheduled_time, "YYYYMMDDhhmmss").format(
              "YYYY/MM/DD HH:mm:ss"
            ),
        startTime:
          operationList[i].start_time == "0" ||
            operationList[i].start_time == null
            ? ""
            : moment(operationList[i].start_time, "YYYYMMDDhhmmss").format(
              "YYYY/MM/DD HH:mm:ss"
            ),
        endTime:
          operationList[i].end_time == "0" || operationList[i].end_time == null
            ? ""
            : moment(operationList[i].end_time, "YYYYMMDDhhmmss").format(
              "YYYY/MM/DD HH:mm:ss"
            ),
        jobStatus: operationList[i].job_status,
        loadStatus: operationList[i].load_status,
        startPendingFlag: operationList[i].start_pending_flag,
      });
    }
    return arr;
  }

  function getRunJobDisplayStatus(status, load_status, start_pending_flag) {
    let str;

    switch (parseInt(status)) {
      case RUN_JOB_STATUS_TYPE.NONE:
        // 「ステータス」が『未実行』の場合、表示優先順位は、「展開状況(load_status)」 ＜ 「起動保留フラグ(start_pending_flag)」
        str = t("btn-scheduled");
        if (parseInt(load_status) == LOAD_STATUS_TYPE.DELAY)
          str = t("btn-schedule-wait");
        if (parseInt(start_pending_flag) == START_PEND_FLAG.PENDING)
          str = t("btn-hold");
        break;
      case RUN_JOB_STATUS_TYPE.PREPARE:
        str = t("btn-scheduled");
        break;
      case RUN_JOB_STATUS_TYPE.DURING:
      case RUN_JOB_STATUS_TYPE.RUN_ERR:
        if (parseInt(load_status) == LOAD_STATUS_TYPE.DELAY) {
          // 「『ｽﾃｰﾀｽ(status) = 実行中(2)』and『展開状況(load_status) = 遅延起動(2)』」の場合、『遅延起動エラー』を設定
          str = t("err-delay");
        } else {
          str = t("btn-running");
        }
        break;
      case RUN_JOB_STATUS_TYPE.NORMAL:
        str = t("btn-done");
        if (parseInt(load_status) == LOAD_STATUS_TYPE.SKIP)
          str = t("err-skipped");
        break;
      case RUN_JOB_STATUS_TYPE.ABNORMAL:
        str = t("btn-done");
        if (parseInt(load_status) == LOAD_STATUS_TYPE.LOAD_ERR) {
          str = t("btn-load-err");
        }
        break;
      default:
        str = t("btn-scheduled");
        break;
    }
    return str;
  }

  const refresh = () => {
    dispatch(dispatchAction());
  };

  const onDoubleClickAction = (event, record, rowIndex) => {
    dispatch(openExecutionDialog(record.managementId));
  };

  const renderTable = (isListTable, onRow, stateId, dispatchAction) => {
    if (isListTable) {
      return (
        <VirtualScrollTable
          stateId={stateId}
          dispatchAction={dispatchAction}
          hasRowSelect={true}
          columnHeaders={DEFAULT_OBJECT_LIST_TABLE_HEADER}
          prepareData={prepareData}
          tableHeight={"30vh"}
          size={"small"}
          onRow={onRow}
          hideSelectAll={hideSelectAll}
          selectType={selectType}
          onDoubleClickAction={onDoubleClickAction}
          category={EXECUTION_MANAGEMENT}
          tableType={tableType}
          autoRefresh={true}
        />
      );

    } else {
      return (
        <PaginationTable
          stateId={stateId}
          dispatchAction={dispatchAction}
          hasRowSelect={true}
          columnHeaders={DEFAULT_OBJECT_LIST_TABLE_HEADER}
          prepareData={prepareData}
          tableHeight={"30vh"}
          size={"small"}
          onRow={onRow}
          hideSelectAll={hideSelectAll}
          selectType={selectType}
          onDoubleClickAction={onDoubleClickAction}
          category={EXECUTION_MANAGEMENT}
          tableType={tableType}
          autoRefresh={true}
        />
      );
    }
  };

  return (
    <Card
      className="job-exec-manage-card"
      headStyle={{ color }}
      title={tableType}
      bordered={false}
      extra={
        <Space>
          <Tooltip title="Refresh">
            <Button
              type="primary"
              onClick={() => {
                refresh();
              }}
              shape="circle"
              size="small"
              icon={<SyncOutlined />}
            />
          </Tooltip>
          <Switch
            unCheckedChildren={t("switch-lab-page")}
            checkedChildren={t("switch-lab-list")}
            checked={isListTable}
            onChange={setIsListTable}
          />
        </Space>
      }
    >
      {renderTable(isListTable, null, stateId, dispatchAction)}
    </Card>
  );
};

export default OperationTableCard;
