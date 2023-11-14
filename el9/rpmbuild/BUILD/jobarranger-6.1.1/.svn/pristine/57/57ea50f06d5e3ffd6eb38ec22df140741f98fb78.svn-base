import { Button, Col, Row, Space } from "antd";
import { t } from "i18next";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { USER_TYPE } from "../../../constants";
import store from "../../../store";
import ObjectList from "../../../views/objectList/ObjectList";

const ObjectListActionButtonGroup = ({ clickAction, category, publicType }) => {
  const userInfo = useSelector((state) => state["user"].userInfo);
  const navigate = useNavigate();
  const [isRowSelected, setRowSelected] = useState(false);
  const selectedObjectRow = useSelector(
    (state) => state["objectList"].selectedRow
  );
  const navigateFormHandler = () => {
    navigate(`/${category}/create/${publicType ? "public" : "private"}`);
  };
  useEffect(() => {
    if (selectedObjectRow.length > 0) {
      setRowSelected(true);
    } else {
      setRowSelected(false);
    }
  }, [selectedObjectRow]);
  return (
    <Row className="mb-10x">
      <Col span={4}>
        <Space>
          <Button
            disabled={
              !isRowSelected ||
              userInfo.userType === USER_TYPE.USER_TYPE_GENERAL
                ? true
                : false
            }
            type="primary"
            onClick={() => {
              clickAction("delete");
            }}
          >
            {t("btn-delete")}
          </Button>
          <Button
            disabled={
              !isRowSelected ||
              userInfo.userType === USER_TYPE.USER_TYPE_GENERAL
                ? true
                : false
            }
            type="primary"
            onClick={() => {
              clickAction("enable");
            }}
          >
            {t("btn-enable")}
          </Button>
          <Button
            disabled={
              !isRowSelected ||
              userInfo.userType === USER_TYPE.USER_TYPE_GENERAL
                ? true
                : false
            }
            type="primary"
            onClick={() => {
              clickAction("disable");
            }}
          >
            {t("btn-disable")}
          </Button>
          <Button
            disabled={!isRowSelected}
            type="primary"
            onClick={() => {
              clickAction("export");
            }}
          >
            {t("btn-export")}
          </Button>
          <Button
            type="primary"
            disabled={userInfo.userType === 1 ? true : false}
            onClick={navigateFormHandler}
          >
            {t("txt-create-" + category)}
          </Button>
        </Space>
      </Col>
    </Row>
  );
};
export default ObjectListActionButtonGroup;
