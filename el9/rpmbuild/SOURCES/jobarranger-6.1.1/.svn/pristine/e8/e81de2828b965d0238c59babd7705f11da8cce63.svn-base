import { Fragment, useEffect, useRef } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { Button, Space, Col, Row, Dropdown, Menu, message, Spin } from "antd";
import VirtualScrollTable from "../../components/tables/virtualScrollTable/VirtualScrollTable";
import ObjectListActionButtonGroup from "../../components/button/objectActionButtons/ObjectListActionButtonGroup";
import { jobnetObjectRender } from "../../components/contextMenu/ContextMenu";
import {
  getAllObjectList,
  updateObjectListValidation,
  setSelectedRowKeys,
  setSelectedObject,
  deleteObjectList,
  cleanupObjectListSlice,
  checkLock,
} from "../../store/ObjectListSlice";
import { OBJECT_CATEGORY, SERVICE_RESPONSE, ICON_TYPE } from "../../constants";
import {
  confirmDialog,
  alertError,
  exportXmlAjaxCall,
} from "../../components/dialogs/CommonDialog";
import { useDispatch, useSelector } from "react-redux";
import "./ObjectList.scss";
import store from "../../store";
import moment from "moment";
import momentTz from "moment-timezone";
import DisplayExecuteModalList from "../../components/dialogs/displayExecuteModalList/DisplayExecuteModalList";

const ObjectList = ({ category, publicType }) => {
  const Item = Menu.Item;
  const { t } = useTranslation();
  const dispatch = useDispatch();
  let action = "";
  let selectedObjectList = [];

  // useEffect(() => {
  //   if (doubleClickedObject.current.clicked) {
  //     let isError = true;
  //     if (isObjectLocked) {
  //       if (isObjectLocked.AJAX_MESSAGE_TYPE === "AJAX_MESSAGE_SUCCESS") {
  //         let objectId = isObjectLocked.AJAX_MESSAGE_OBJECTID;
  //         if (objectId === doubleClickedObject.current.objectId) {
  //           let date = doubleClickedObject.current.updateDate;
  //           isError = false;
  //           doubleClickedObject.current.objectId = "";
  //           doubleClickedObject.current.updateDate = "";
  //           doubleClickedObject.current.clicked = false;
  //         }
  //       } else {
  //         isError = false;
  //         doubleClickedObject.current.objectId = "";
  //         doubleClickedObject.current.updateDate = "";
  //         doubleClickedObject.current.clicked = false;
  //         alertError(
  //           t("sel-warning"),
  //           `${isObjectLocked.AJAX_MESSAGE_OBJECTID} : ${t(
  //             isObjectLocked.AJAX_MESSAGE_DETAIL
  //           )}`
  //         );
  //       }
  //     }
  //     if (isError) {
  //       doubleClickedObject.current.objectId = "";
  //       doubleClickedObject.current.updateDate = "";
  //       doubleClickedObject.current.clicked = false;
  //       alertError(t("btn-load-err"), `${t("err-msg-fail")}`);
  //     }
  //   }
  // }, [isObjectLocked]);
  useEffect(() => {
    dispatch(cleanupObjectListSlice([]));
  }, [publicType, category]);

  const handleMenuClick = (e) => {
    message.info("Click on menu item.");
  };
  const navigate = useNavigate();
  const navigateEditHandler = (objectId, date) => {
    navigate(
      `/${category}/edit/${publicType ? "public" : "private"
      }/${objectId}/${moment(date, "YYYY/MM/DD HH:mm:ss").format(
        "YYYYMMDDHHmmss"
      )}`
    );
  };

  const okValidFunction = () => {
    selectedObjectList = store.getState().objectList.selectedObject;
    let thisChange = "enable";

    switch (action) {
      case "enable":
        thisChange = "enable";
        break;
      case "disable":
        thisChange = "disable";
        break;
      case "delete":
        thisChange = "delete";
        break;
      case "export":
        thisChange = "export";
    }
    let changedObject = [];
    if (selectedObjectList) {
      selectedObjectList.map((object) => {
        let tmp_updated_date = moment(
          object.updatedDate,
          "YYYY/MM/DD HH:mm:ss"
        ).format("YYYYMMDDHHmmss");
        let tmp_object = {
          id: object.objectId,
          update: tmp_updated_date,
          validState: object.validFlag,
        };
        changedObject = [...changedObject, tmp_object];
      });
    }
    let updateJson = {
      datas: {
        selectedRows: [...changedObject],
        actionType: thisChange,
        category: category,
      },
    };
    let result = [];
    if (thisChange == "delete") {
      result = dispatch(deleteObjectList(updateJson));
    } else if (action == "export") {
      let data = {
        objectType: category,
        exportType: "Object",
        data: [...changedObject],
        timeZone: momentTz.tz("2022-09-24", momentTz.tz.guess()).format("Z"),
      };
      result = exportXmlAjaxCall(data);
    } else {
      result = dispatch(updateObjectListValidation(updateJson));
    }
  };
  const cancelValidFunction = () => {
    dispatch(setSelectedObject([]));
    dispatch(setSelectedRowKeys([]));
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
          t("lab-export-info-object"),
          okValidFunction,
          cancelValidFunction
        );
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
  const listBtnClick = (record) => {
    navigate(
      `/object-version/${record.category}/${publicType ? "public" : "private"
      }/${record.objectId}`
    );
  };

  const DEFAULT_OBJECT_LIST_TABLE_HEADER = [
    {
      title: t("col-obj-update"),
      dataIndex: "updatedDate",
      key: `${category}-${publicType ? "public" : "private"}-updatedDate`,
      className: "objlist-updated-date",
      width: 135,
      //colSpan: 3,
      align: "center",
      searchable: true,
      ellipsis: true,
      sorter: (a, b) =>
        moment(a.updatedDate).unix() - moment(b.updatedDate).unix(),
      //sortDirections: ["descend", "ascend"],
      showSorterTooltip: false,
      render: (text, record, index) => {
        return category === OBJECT_CATEGORY.JOBNET ? (
          jobnetObjectRender(text, record)
        ) : (
          <>{text}</>
        );
      },
    },
    {
      title: t("col-valid"),
      dataIndex: "validFlag",
      key: `${category}-${publicType ? "public" : "private"}-validflag`,
      className: "table-valid-row",
      width: 65,
      align: "center",
      ellipsis: true,

      sorter: (a, b) => parseInt(a.validFlag) - parseInt(b.validFlag),
      //sortDirections: ["descend", "ascend"],
      showSorterTooltip: false,
      render: (text, record, index) => {
        return category === OBJECT_CATEGORY.JOBNET ? (
          <>{jobnetObjectRender(parseInt(text) === 1 ? "O" : " ", record)}</>
        ) : (
          <>{parseInt(text) === 1 ? "O" : ""}</>
        );
      },
    },
    {
      title: t("lab-user-name"),
      dataIndex: "username",
      key: `${category}-${publicType ? "public" : "private"}-username`,
      width: 120,
      //colSpan: 3,
      align: "left",
      ellipsis: true,
      searchable: true,
      sorter: (a, b) => a.username.localeCompare(b.username),
      //sortDirections: ["descend", "ascend"],
      showSorterTooltip: false,
      render: (text, record, index) => {
        return category === OBJECT_CATEGORY.JOBNET ? (
          jobnetObjectRender
        ) : (
          <>{text}</>
        );
      },
    },
    {
      title: t("col-obj-id"),
      dataIndex: "objectId",
      key: `${category}-${publicType ? "public" : "private"}-objectId`,
      searchable: true,
      //width: 280,
      //colSpan: 3,
      ellipsis: true,
      sorter: (a, b) => a.objectId.localeCompare(b.objectId),
      //sortDirections: ["descend", "ascend"],
      showSorterTooltip: false,
      render: (text, record, index) => {
        return category === OBJECT_CATEGORY.JOBNET ? (
          jobnetObjectRender
        ) : (
          <>{text}</>
        );
      },
    },
    {
      title: t("col-obj-name"),
      dataIndex: "objectName",
      key: `${category}-${publicType ? "public" : "private"}-objectName`,
      searchable: true,
      //align: "center",
      showSorterTooltip: false,
      ellipsis: true,
      //width: 320,
      sorter: (a, b) => a.objectName.localeCompare(b.objectName),
      //sortDirections: ["descend", "ascend"],
      render: (text, record, index) => {
        return category === OBJECT_CATEGORY.JOBNET ? (
          jobnetObjectRender
        ) : (
          <>{text}</>
        );
      },
    },
    {
      title: t("col-obj-des"),
      dataIndex: "desc",
      key: `${category}-${publicType ? "public" : "private"}-desc`,
      searchable: true,
      //align: "center",
      ellipsis: true,
      //width: 320,
      sorter: (a, b) =>
        a.desc && b.desc ? a.desc.localeCompare(b.desc) : a.desc ? 1 : -1,
      sortDirections: ["descend", "ascend"],
      showSorterTooltip: false,
      render: (text, record, index) => {
        return category === OBJECT_CATEGORY.JOBNET ? (
          jobnetObjectRender
        ) : (
          <>{text}</>
        );
      },
    },
    {
      title: t("col-version"),
      dataIndex: "version",
      className: "table-version-row",
      key: `${category}-${publicType ? "public" : "private"}-version`,
      align: "left",
      ellipsis: true,
      //fixed: "right",
      width: 87,
      searchable: false,
      showSorterTooltip: false,
      render: (_, record) => {
        return (
          <Button
            type="primary"
            size="small"
            onClick={(event) => {
              listBtnClick(record);
            }}
          >
            {t("col-obj-dtl")}
          </Button>
        );
      },
    },
    // {
    //   className: "extra-col-end",
    // },
  ];

  const shortcutHandler = (event) => {
    let selectedObj = store.getState().objectList.selectedObject;
    if (selectedObj.length > 1 && (event.keyCode == 82 || event.keyCode == 87 || event.keyCode == 84) && category === OBJECT_CATEGORY.JOBNET) {
      alertError(t("title-error"), t("err-msg-shortcut-mulit-selected"));
      return;
    }
    if (selectedObj.length < 1 && (event.keyCode == 82 || event.keyCode == 87 || event.keyCode == 84) && category === OBJECT_CATEGORY.JOBNET) {
      return;
    }
    switch (event.keyCode) {
      case 82:
        if (document.getElementById("btnObjImrun") != null) {
          document.getElementById("btnObjImrun").click();
        }
        return;
      case 87:
        if (document.getElementById("btnObjImrunHold") != null) {
          document.getElementById("btnObjImrunHold").click();
        }
        return;
      case 84:
        if (document.getElementById("btnObjTestRun") != null) {
          document.getElementById("btnObjTestRun").click();
        }
        break;
      default:
        return;
    }
  };

  useEffect(() => {
    window.addEventListener("keydown", shortcutHandler)
    return () => {
      window.removeEventListener("keydown", shortcutHandler);
    };
  }, [])


  const params = {
    publicType: publicType ? "public" : "private",
    category,
  };

  function prepareData(objectList) {
    let arr = [];
    let updatedDate = "";
    for (let i = 0; i < objectList.length; i++) {
      updatedDate = moment(objectList[i].update_date, "YYYYMMDDHHmmss").format(
        "YYYY/MM/DD HH:mm:ss"
      );
      arr.push({
        key: i,
        updatedDate: updatedDate,
        username: objectList[i].user_name,
        objectId: objectList[i][category + "_id"],
        objectName: objectList[i][category + "_name"],
        desc: objectList[i].memo,
        category: category,
        validFlag: objectList[i].valid_flag,
      });
    }
    return arr;
  }

  const rowClassName = (record, index) => {
    if (record.validFlag === "1") {
      return "light-blue";
    }
    return null;
  };
  const rowDoubleClickAction = (event, record, rowIndex) => {
    let updateDate = moment(record.updatedDate, "YYYY/MM/DD HH:mm:ss").format(
      "YYYYMMDDHHmmss"
    );
    switch (category) {
      case OBJECT_CATEGORY.CALENDAR:
        navigateEditHandler(record.objectId, updateDate);
        break;
      case OBJECT_CATEGORY.FILTER:
        navigateEditHandler(record.objectId, updateDate);
        break;
      case OBJECT_CATEGORY.JOBNET:
        navigateEditHandler(record.objectId, updateDate);
        break;
      case OBJECT_CATEGORY.SCHEDULE:
        navigateEditHandler(record.objectId, updateDate);
        break;
      default:
        alertError(t("title-error"), t("err-websocket-error"));
    }
  };
  return (
    <>
      <ObjectListActionButtonGroup
        clickAction={setValidation}
        category={category}
        publicType={publicType}
      />
      <VirtualScrollTable
        stateId={"objectList"}
        dispatchAction={getAllObjectList}
        hasRowSelect={true}
        columnHeaders={DEFAULT_OBJECT_LIST_TABLE_HEADER}
        prepareData={prepareData}
        params={params}
        onDoubleClickAction={rowDoubleClickAction}
        rowClassName={rowClassName}
        category={category}
        publicType={publicType}
      />
      <DisplayExecuteModalList />
    </>
  );
};
export default ObjectList;
