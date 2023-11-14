import { Modal } from "antd";
import React from "react";
import { useTranslation } from "react-i18next";
import { useDispatch } from "react-redux";
import AuthService from "../../services/Auth";
import { removeUserInfo } from "../../store/UserSlice";
import { useNavigate } from "react-router-dom";
import { SERVICE_RESPONSE } from "../../constants";
import { clearHideRow } from "../../store/ErrorOperationListSlice";
import { removeAllExecuteJob } from "../../store/JobExecutionSlice";

const Logout = ({ isOpen, onCancel }) => {
  const { t } = useTranslation();
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const clearSessionAndLogout = () => {
    dispatch(removeUserInfo());
    dispatch(clearHideRow())
    dispatch(removeAllExecuteJob())
    navigate("/login", { replace: true });
  }

  const onOkClick = () => {
    AuthService.logout({})
      .then((res) => {
        if (res.type === SERVICE_RESPONSE.OK) {
          clearSessionAndLogout()
        }else if(res.type === SERVICE_RESPONSE.INCOMEPLETE && res.detail.message === SERVICE_RESPONSE.NO_PERMISSION_TO_CALL){
          clearSessionAndLogout()
        }
      })
      .catch(() => {});
  }

  return (
    <Modal
      visible={isOpen}
      title={t("title-msg-confirm")}
      onOk={onOkClick}
      width={450}
      onCancel={onCancel}
      centered={true}
      okText={t('btn-ok')}
      cancelText={t('btn-cancel')}
      maskClosable={false}
    >
      <div>{t("logout-confirm")}</div>
    </Modal>
  );
};

export default Logout;
