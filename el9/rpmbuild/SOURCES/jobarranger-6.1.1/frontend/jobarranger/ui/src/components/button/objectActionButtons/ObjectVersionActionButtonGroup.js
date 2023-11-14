import { Button, Col, Row, Space, message } from "antd";
import { t } from "i18next";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate, useParams } from "react-router-dom";
import {
  OBJECT_CATEGORY,
  RESPONSE_OBJ_KEY,
  RUN_TYPE,
  SERVICE_RESPONSE,
  USER_TYPE,
} from "../../../constants";
import JobExecutionService from "../../../services/jobExecutionService";
import store from "../../../store";
import { openExecutionDialog } from "../../../store/JobExecutionSlice";
import ObjectList from "../../../views/objectList/ObjectList";
import { alertError, confirmDialog } from "../../dialogs/CommonDialog";

const ObjectVersionActionButtonGroup = ({
  clickAction,
  category,
  publicType,
}) => {
  const userInfo = useSelector((state) => state["user"].userInfo);
  const param = useParams();
  const objectId = param.objectId;
  const navigate = useNavigate();
  const [isRowSelected, setRowSelected] = useState(false);
  const [isSelectedOne, setSelectedOne] = useState(false);
  const selectedObjectRow = useSelector(
    (state) => state["objectList"].selectedRow
  );
  //jobarranger/object-list/calendar/public
  const back = () => {
    navigate(`/object-list/${category}/${publicType ? "public" : "private"}`);
  };
  useEffect(() => {
    if (selectedObjectRow.length > 0) {
      setRowSelected(true);
      if (selectedObjectRow.length === 1) {
        setSelectedOne(true);
      } else {
        setSelectedOne(false);
      }
    } else {
      setRowSelected(false);
      setSelectedOne(false);
    }
  }, [selectedObjectRow]);

  const executeAction = (runType) => {
    JobExecutionService.checkValid({ id: objectId })
      .then((res) => {
        if (res.type === SERVICE_RESPONSE.OK) {
          if (res.detail.message === SERVICE_RESPONSE.VALID) {
            const onOk = () => {
              const hideMessage = message.loading(
                t("during-jobnet-execute"),
                0
              );
              JobExecutionService.run({ id: objectId, runType: runType })
                .then((res) => {
                  hideMessage();
                  if (res.type == SERVICE_RESPONSE.INCOMEPLETE) {
                    if (
                      res.detail.hasOwnProperty(RESPONSE_OBJ_KEY.MESSAGE_CODE)
                    ) {
                      alertError(
                        t("title-error"),
                        t(res.detail[RESPONSE_OBJ_KEY.MESSAGE_CODE])
                      );
                    } else {
                      alertError(t("title-error"), res.detail.message);
                    }
                  } else {
                    store.dispatch(
                      openExecutionDialog(res.result.innerJobnetId)
                    );
                  }
                })
                .catch((err) => {
                  alertError(t("title-error"), err.message);
                });
            };

            const onCancel = () => {};
            confirmDialog(
              t("title-msg-confirm"),
              t("lab-jobnet-run"),
              onOk,
              onCancel
            );
          } else if (res.detail.message === SERVICE_RESPONSE.INVALID) {
            alertError(t("title-error"), t("err-msg-cannot-exec"));
          }
        }
      })
      .catch((err) => {});
  };

  return (
    <Row className="mb-10x">
      <Col span={6}>
        <Space>
          <Button type="primary" onClick={back}>
            {t("btn-back")}
          </Button>
          <Button
            type="primary"
            disabled={
              !isSelectedOne ||
              userInfo.userType === USER_TYPE.USER_TYPE_GENERAL
                ? true
                : false
            }
            onClick={() => {
              clickAction("new_object");
            }}
          >
            {t("btn-new-obj")}
          </Button>
          <Button
            type="primary"
            disabled={
              !isSelectedOne ||
              userInfo.userType === USER_TYPE.USER_TYPE_GENERAL
                ? true
                : false
            }
            onClick={() => {
              clickAction("new_version");
            }}
          >
            {t("btn-new-ver")}
          </Button>
          <Button
            type="primary"
            disabled={
              !isRowSelected ||
              userInfo.userType === USER_TYPE.USER_TYPE_GENERAL
                ? true
                : false
            }
            onClick={() => {
              clickAction("delete");
            }}
          >
            {t("btn-delete")}
          </Button>
          {category == OBJECT_CATEGORY.JOBNET ? (
            <>
              <Button
                type="primary"
                onClick={() => {
                  executeAction(RUN_TYPE.IMMEDIATE_RUN);
                }}
              >
                {t("btn-run")}
              </Button>
              <Button
                type="primary"
                onClick={() => {
                  executeAction(RUN_TYPE.IMMEDIATE_RUN_HOLD);
                }}
              >
                {t("btn-run-hold")}
              </Button>
              <Button
                type="primary"
                onClick={() => {
                  executeAction(RUN_TYPE.TEST_RUN);
                }}
              >
                {t("btn-test-run")}
              </Button>
            </>
          ) : (
            false
          )}
          <Button
            type="primary"
            disabled={!isRowSelected}
            onClick={() => {
              clickAction("export");
            }}
          >
            {t("btn-export")}
          </Button>
        </Space>
      </Col>
    </Row>
  );
};
export default ObjectVersionActionButtonGroup;
