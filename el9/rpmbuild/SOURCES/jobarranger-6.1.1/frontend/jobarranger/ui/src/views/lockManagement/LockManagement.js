import React, { useState, useEffect } from "react";
import { Fragment } from "react";
import { useTranslation } from "react-i18next";
import { Button, Space, Col, Row, Modal } from "antd";
import VirtualScrollTable from "../../components/tables/lockManagementTable/LockManagementTable";
import {
  getAllLockedObj,
  setSelectedObject,
  setSelectedRowKeys,
  unlock,
} from "../../store/LockManagementSlice";
import { useDispatch } from "react-redux";
import "./LockManagement.scss";
import { useSelector } from "react-redux";
import moment from "moment";
import objectLockService from "../../services/objectLockService";
const LockManagement = () => {
  const selectedObject = useSelector(
    (state) => state["lockedObjList"].selectedObject
  );
  const selectedRowKeys = useSelector(
    (state) => state["lockedObjList"].selectedRow
  );
  const { t } = useTranslation();
  const dispatch = useDispatch();
  const [visible, setVisible] = useState(false);
  const userInfo = useSelector((state) => state["user"].userInfo);
  const [isRowSelected, setRowSelected] = useState(false);

  useEffect(() => {
    if (selectedRowKeys.length > 0) {
      setRowSelected(true);
    } else {
      setRowSelected(false);
    }
  }, [selectedRowKeys]);

  const DEFAULT_OBJECT_LIST_TABLE_HEADER = [
    {
      title: t("lab-user-name"),
      dataIndex: "username",
      key: "username",
      //width: 150,
      searchable: true,
      ellipsis: true,
      sorter: (a, b) => a.username.localeCompare(b.username),
      sortDirections: ["descend", "ascend"],
    },
    {
      title: t("col-obj-id"),
      dataIndex: "objectId",
      //width: 200,
      key: "objectId",
      searchable: true,
      ellipsis: true,
      sorter: (a, b) => a.objectId.localeCompare(b.objectId),
      sortDirections: ["descend", "ascend"],
    },
    {
      title: t("lab-object-type"),
      dataIndex: "objectType",
      //width: 250,
      key: "objectType",
      searchable: true,
      ellipsis: true,
      sorter: (a, b) => a.objectType.localeCompare(b.objectType),
      sortDirections: ["descend", "ascend"],
    },
    {
      title: t("txt-ip-address"),
      dataIndex: "ipAddress",
      //width: 250,
      key: "ipAddress",
      searchable: true,
      ellipsis: true,
      sorter: (a, b) =>
        (a.ipAddress).replace(/[^\w ]/g, '') && (b.ipAddress).replace(/[^\w ]/g, '')
          ? parseInt((a.ipAddress).replace(/[^\w ]/g, '')) - parseInt((b.ipAddress).replace(/[^\w ]/g, ''))
          : 0,
      sortDirections: ["descend", "ascend"],
    },

    {
      title: t("txt-last-active-time"),
      dataIndex: "lockDate",
      //width: 150,
      key: "lockDate",
      searchable: false,
      ellipsis: true,
      sorter: (a, b) =>
        moment(a.lockDate).unix() - moment(b.lockDate).unix(),
      sortDirections: ["descend", "ascend"],
    },
  ];

  function prepareData(lockedObjList) {
    let arr = [];
    let userName = userInfo.userName;
    let userType = userInfo.userType;

    for (let i = 0; i < lockedObjList.length; i++) {
      var objectType = "";
      var hasPermission = false;
      var objType = parseInt(lockedObjList[i]["object_type"]);
      switch (objType) {
        case 1:
          objectType = "CALENDAR";
          break;
        case 2:
          objectType = "FILTER";
          break;
        case 3:
          objectType = "SCHEDULE";
          break;
        case 4:
          objectType = "JOBNET";
          break;
      }
      if (
        userType >= 3 ||
        (userName == lockedObjList[i].username &&
          userType == lockedObjList[i].roleid)
      ) {
        hasPermission = true;
      }
      var lockDate = moment
        .utc(lockedObjList[i].last_active_time)
        .local()
        .format("YYYY/MM/DD HH:mm:ss");

      arr.push({
        key: lockedObjList[i].object_id,
        username: lockedObjList[i].username,
        objectId: lockedObjList[i].object_id,
        objectType: objectType,
        ipAddress: lockedObjList[i].attempt_ip,
        lockDate: lockDate,
        hasPermission: hasPermission,
      });
    }
    return arr;
  }
  const showModal = () => {
    if (selectedObject) {
      if (selectedObject.length > 0) {
        setVisible(true);
      }
    }
  };

  const hideModal = () => {
    dispatch(setSelectedObject([]));
    dispatch(setSelectedRowKeys([]));
    setVisible(false);
  };

  const unlockObject = async () => {
    const response = await objectLockService.deleteLock(selectedObject);
    // dispatch(unlock(selectedObject));
    hideModal();
    refresh();
  };

  const refresh = () => {
    dispatch(getAllLockedObj());
  };
  return (
    <Fragment>
      <Row className="mb-10x">
        <Col span={4}>
          <Space>
            <Button type="primary" size="medium" onClick={showModal} disabled={!isRowSelected ? true : false}>
              {t("btn-delete")}
            </Button>
            <Button type="primary" size="medium"
              onClick={() => {
                refresh();
              }}>
              {t("btn-refresh")}
            </Button>
            <Modal
              title={t("title-msg-confirm")}
              visible={visible}
              onOk={unlockObject}
              centered={true}
              onCancel={hideModal}
              okText={t("btn-ok")}
              cancelText={t("btn-cancel")}
            >
              <p>{t("warn-msg-del")}</p>
            </Modal>
          </Space>
        </Col>
      </Row>

      <VirtualScrollTable
        stateId={"lockedObjList"}
        dispatchAction={getAllLockedObj}
        hasRowSelect={true}
        columnHeaders={DEFAULT_OBJECT_LIST_TABLE_HEADER}
        prepareData={prepareData}
        params={null}
      />
    </Fragment>
  );
};
export default LockManagement;
