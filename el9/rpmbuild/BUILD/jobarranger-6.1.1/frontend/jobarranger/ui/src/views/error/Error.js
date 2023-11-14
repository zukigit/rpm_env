import React from 'react';
import { Button, Result } from 'antd';
import { useNavigate } from "react-router-dom";
import { t } from 'i18next';
import store from '../../store';

export function Err404() {
  const navigate = useNavigate();
  const back = () => {
    navigate(`/home`);
  };
  return (
    <Result
      status="404"
      title="404"
      subTitle={t('msg-error')}
      extra={<Button type="primary" onClick={back}>{t('btn-home')}</Button>}
    />
  );
};

export function Err500() {
  const sessionId = store.getState().user.userInfo.sessionId;
  const navigate = useNavigate();
  const back = () => {

    if (sessionId == undefined) {
      navigate(`/login`)
    } else {
      navigate(`/home`);
    }
  };
  const responseData = store.getState().responseData.responseData;
  console.log("500 Redirect");
  console.log(responseData);

  return (
    <Result
      status="500"
      title="500"
      //subTitle={t('500-err')}
      subTitle={
        responseData.detail ? (responseData.detail['message-detail'] ? t(responseData.detail['message-detail']) : t('500-err')) : t('500-err')
      }
      extra={<Button type="primary" onClick={back}>{sessionId == undefined ? t('title-login') : t('btn-home')}</Button>}
    />
  );
};

export function RandomError() {
  const navigate = useNavigate();
  const back = () => {
    navigate(`/home`);
  };

  return (
    <Result
      status="warning"
      title={t('other-err')}
      extra={<Button type="primary" onClick={back}>{t('btn-home')}</Button>}
    />
  );
};

export function NetworkError() {
  const navigate = useNavigate();
  const back = () => {
    navigate(`/home`);
  };

  return (
    <Result
      status="error"
      title={t('network-err')}
      extra={<Button type="primary" onClick={back}>{t('btn-home')}</Button>}
    />
  );
};

export default RandomError;