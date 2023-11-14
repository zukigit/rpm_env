import React, { useEffect, useState } from "react";
import { Fragment } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { Button, Space, Col, Row, message } from "antd";
import VirtualScrollTable from "../../components/tables/virtualScrollTable/VirtualScrollTable";
import {
  updateObjectVersionValidation,
  getAllObjectVersion,
  deleteObjectVersion,
  setSelectedObject,
  setSelectedRowKeys,
  cleanupObjectListSlice,
} from "../../store/ObjectListSlice";
import "./ObjectVersion.scss";
import moment from "moment";
import momentTz from "moment-timezone";
import store from "../../store";
import {
  confirmDialog,
  alertInfo,
} from "../../components/dialogs/CommonDialog";
import { useDispatch, useSelector } from "react-redux";
import { exportXmlAjaxCall } from "../../components/dialogs/CommonDialog";
import { OBJECT_CATEGORY, FORM_TYPE } from "../../constants";
import ObjectVersionActionButtonGroup from "../../components/button/objectActionButtons/ObjectVersionActionButtonGroup";
import DisplayExecuteModalList from "../../components/dialogs/displayExecuteModalList/DisplayExecuteModalList";

const ObjectVersion = ({ category, publicType }) => {
  const { t } = useTranslation();
  const param = useParams();
  const objectId = param.objectId;
  let selectedObjectVersion = [];
  const dispatch = useDispatch();
  let action = "";

  useState(() => {
    dispatch(cleanupObjectListSlice());
  }, []);
  const DEFAULT_OBJECT_VERSION_TABLE_HEADER = [
    {
      title: t("col-obj-update"),
      dataIndex: "updatedDate",
      key: `${category}-${publicType ? "public" : "private"}-updatedDate`,
      align: "center",
      width: 135,
      ellipsis: true,
      searchable: true,
      showSorterTooltip: false,
      sorter: (a, b) =>
        moment(a.updatedDate).unix() - moment(b.updatedDate).unix(),
      sortDirections: ["descend", "ascend"],
    },
    {
      title: t("col-valid"),
      dataIndex: "action",
      key: `${category}-${publicType ? "public" : "private"}-action`,
      searchable: false,
      align: "left",
      width: 65,
      //width: 100,
      ellipsis: true,
      align: "center",
      showSorterTooltip: false,
      sorter: (a, b) => parseInt(a.validFlag) - parseInt(b.validFlag),
      sortDirections: ["descend", "ascend"],
      render: (_, record) => {
        return record.validFlag === "1" ? (
          <Button
            type="primary"
            size="small"
            onClick={(event) => {
              changeValidFlag(record);
            }}
          >
            O{/* {t("col-valid-flag-1")} */}
          </Button>
        ) : (
          <Button
            type="primary"
            size="small"
            onClick={(event) => {
              changeValidFlag(record);
            }}
          >
            X{/* {t("col-valid-flag-0")} */}
          </Button>
        );
      },
    },
    {
      title: t("lab-user-name"),
      dataIndex: "username",
      key: `${category}-${publicType ? "public" : "private"}-username`,
      align: "left",
      searchable: true,
      ellipsis: true,
      width: 120,
      showSorterTooltip: false,
      sorter: (a, b) => a.username.localeCompare(b.username),
      sortDirections: ["descend", "ascend"],
    },
    {
      title: t("col-obj-id"),
      dataIndex: "objectId",
      key: `${category}-${publicType ? "public" : "private"}-objectId`,
      ellipsis: true,
      searchable: true,
      showSorterTooltip: false,
      sorter: (a, b) => a.objectId.localeCompare(b.objectId),
      sortDirections: ["descend", "ascend"],
    },
    {
      title: t("col-obj-name"),
      dataIndex: "objectName",
      key: `${category}-${publicType ? "public" : "private"}-objectName`,
      ellipsis: true,
      searchable: true,
      showSorterTooltip: false,
      sorter: (a, b) => a.objectName.localeCompare(b.objectName, "fr"),
      sortDirections: ["descend", "ascend"],
    },
    {
      title: t("col-obj-des"),
      dataIndex: "desc",
      key: `${category}-${publicType ? "public" : "private"}-desc`,
      searchable: true,
      ellipsis: true,
      showSorterTooltip: false,
      sorter: (a, b) =>
        a.desc && b.desc ? a.desc.localeCompare(b.desc) : a.desc ? 1 : -1,
      sortDirections: ["descend", "ascend"],
    },
    {
      title: t("col-cre-date"),
      dataIndex: "createdDate",
      key: `${category}-${publicType ? "public" : "private"}-createdDate`,
      align: "center",
      ellipsis: true,
      searchable: true,
      showSorterTooltip: false,
      sorter: (a, b) =>
        moment(a.updatedDate).unix() - moment(b.updatedDate).unix(),
      sortDirections: ["descend", "ascend"],
    },
  ];

  const params = {
    publicType: publicType ? "public" : "private",
    category: category,
    objectId: objectId,
  };

  function prepareData(objectVersion) {
    let arr = [];
    if (objectVersion != "underfined") {
      for (let i = 0; i < objectVersion.length; i++) {
        arr.push({
          key: i,
          updatedDate: moment(
            objectVersion[i].update_date,
            "YYYYMMDDHHmmss"
          ).format("YYYY/MM/DD HH:mm:ss"),
          createdDate: moment(objectVersion[i].created_date).format(
            "YYYY/MM/DD HH:mm:ss"
          ),
          username: objectVersion[i].user_name,
          objectId: objectVersion[i][category + "_id"],
          objectName: objectVersion[i][category + "_name"],
          desc: objectVersion[i].memo,
          category: category,
          validFlag: objectVersion[i].valid_flag,
        });
      }
    }
    return arr;
  }

  const rowClassName = (record, index) => {
    if (record.validFlag === "1") {
      return "light-blue";
    }
    return null;
  };
  const setValidation = (actionType) => {
    let selectedObj = store.getState().objectList.selectedObject;
    action = actionType;
    if (selectedObj && selectedObj.length > 0) {
      if (action == "delete") {
        confirmDialog(
          t("title-confirm-delete"),
          t("confirm-delete"),
          okValidFunction,
          cancelValidFunction
        );
      } else if (action == "export") {
        confirmDialog(
          t("title-export"),
          t("lab-export-info-version"),
          okValidFunction,
          cancelValidFunction
        );
      } else if (action == "new_object") {
        newObjectClick();
      } else if (action == "new_version") {
        newVersionClick();
      } else {
        confirmDialog(
          t("title-msg-conf"),
          t("warn-msg-valid"),
          okValidFunction,
          cancelValidFunction
        );
      }
    }
  };

  const okValidFunction = () => {
    selectedObjectVersion = store.getState().objectList.selectedObject;
    let thisChange = "delete";
    switch (action) {
      case "export":
        thisChange = "export";
        break;
      case "delete":
        thisChange = "delete";
    }
    let changedObject = [];
    if (selectedObjectVersion) {
      selectedObjectVersion.map((object) => {
        let tmp_object = {
          objectId: object.objectId,
          updateDate: moment(object.updatedDate, "YYYY/MM/DD HH:mm:ss").format(
            "YYYYMMDDHHmmss"
          ),
          validState: object.validFlag,
        };
        changedObject = [...changedObject, tmp_object];
      });
    }
    let result = [];
    if (thisChange == "delete") {
      let updateJson = {
        datas: {
          data: [...changedObject],
          //actionType: thisChange,
          category: category,
        },
      };
      result = dispatch(deleteObjectVersion(updateJson));
    } else if (thisChange == "export") {
      let data = {
        objectType: category,
        exportType: "Version",
        data: [...changedObject],
        timeZone: momentTz.tz("2022-09-24", momentTz.tz.guess()).format("Z"),
      };
      result = exportXmlAjaxCall(data);
    }
    dispatch(setSelectedObject([]));
    dispatch(setSelectedRowKeys([]));
  };

  const cancelValidFunction = () => {
    dispatch(setSelectedObject([]));
    dispatch(setSelectedRowKeys([]));
  };

  const navigate = useNavigate();

  const newObjectClick = () => {
    let selectedObj = store.getState().objectList.selectedObject;
    if (selectedObj.length > 0) {
      if (selectedObj.length > 1) {
        alertInfo("", t("info-msg-select-one"));
      } else {
        navigate(
          `/${category}/${FORM_TYPE.NEW_OBJECT}/${
            publicType ? "public" : "private"
          }/${selectedObj[0].objectId}/${moment(
            selectedObj[0].updatedDate,
            "YYYY/MM/DD HH:mm:ss"
          ).format("YYYYMMDDHHmmss")}`
        );
      }
    }
  };

  const newVersionClick = () => {
    let selectedObj = store.getState().objectList.selectedObject;
    if (selectedObj.length > 0) {
      if (selectedObj.length > 1) {
        alertInfo("", t("info-msg-select-one"));
      } else {
        navigate(
          `/${category}/${FORM_TYPE.NEW_VERSION}/${
            publicType ? "public" : "private"
          }/${selectedObj[0].objectId}/${moment(
            selectedObj[0].updatedDate,
            "YYYY/MM/DD HH:mm:ss"
          ).format("YYYYMMDDHHmmss")}`
        );
      }
    }
  };

  const rowDoubleClickAction = (event, record, rowIndex) => {
    navigate(
      `/${record.category}/edit/${publicType ? "public" : "private"}/${
        record.objectId
      }/${moment(record.updatedDate, "YYYY/MM/DD HH:mm:ss").format(
        "YYYYMMDDHHmmss"
      )}`
    );
  };

  const changeValidFlag = (record) => {
    confirmDialog(
      t("title-msg-conf"),
      t("warn-msg-valid"),
      okEnable.bind(this, record),
      cancelEnable
    );
  };
  const okEnable = (record) => {
    let result = [];
    let tmp_object = {
      updatedDate: moment(record.updatedDate, "YYYY/MM/DD HH:mm:ss").format(
        "YYYYMMDDHHmmss"
      ),
      objectId: record.objectId,
      validFlag: record.validFlag,
      category: record.category,
    };
    result = dispatch(updateObjectVersionValidation(tmp_object));
  };
  const cancelEnable = () => {};
  const setDispatchAction = () => {};

  return (
    <Fragment>
      <ObjectVersionActionButtonGroup
        clickAction={setValidation}
        category={category}
        publicType={publicType}
      />
      <VirtualScrollTable
        stateId={"objectList"}
        dispatchAction={getAllObjectVersion}
        hasRowSelect={true}
        columnHeaders={DEFAULT_OBJECT_VERSION_TABLE_HEADER}
        prepareData={prepareData}
        params={params}
        rowClassName={rowClassName}
        onDoubleClickAction={rowDoubleClickAction}
        category={category}
        publicType={publicType}
        tableType={"objectVersion"}
      />
      <DisplayExecuteModalList />
    </Fragment>
  );
};
export default ObjectVersion;
