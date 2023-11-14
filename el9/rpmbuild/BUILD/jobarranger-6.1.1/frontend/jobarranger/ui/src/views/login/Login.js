import React, { Fragment, useState, useEffect } from "react";
import { Form, Input, Button } from "antd";
import { useNavigate, useSearchParams } from "react-router-dom";
import AuthService from "../../services/Auth";
import { setUserInfo } from "../../store/UserSlice";
import { useDispatch } from "react-redux";
import "./Login.scss";
import { useTranslation } from "react-i18next";
import { alertError } from "../../components/dialogs/CommonDialog";
import { SERVICE_RESPONSE } from "../../constants";

const Login = () => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [searchParams, setSearchParams] = useSearchParams();
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const { t, i18n } = useTranslation();

  useEffect(() => {
    const param = searchParams.get("redirectUrl");

    if (param) {
      searchParams.delete("redirectUrl");
      setSearchParams(searchParams);
    }
  }, []);

  const onFinish = (values) => {
    form
      .validateFields()
      .then((values) => {
        setLoading(true);
        AuthService.login({
          username: values.username,
          password: values.password,
        })
          .then((res) => {
            if (res.type === SERVICE_RESPONSE.INCOMEPLETE) {
              alertError(t("title-error"), res.detail.message);
              setLoading(false);
              return;
            }
            setLoading(false);
            dispatch(setUserInfo(res.detail.data));
            navigate("/home");

            i18n.changeLanguage(res.detail.data.language.toLowerCase());
          })
          .catch(() => {
            setLoading(false);
          });
      })
      .catch((err) => {});
  };

  useEffect(() => {
    AuthService.apiCheck().then((result) => {
      //console.log("config updated");
    });
  }, []);

  return (
    <Fragment>
    
      <div className="login-form">
        <div className="login-title">
          <span>JOB ARRANGER FOR ZABBIX</span>
        </div>
        <Form
          form={form}
          initialValues={{ remember: true }}
          onFinish={onFinish}
          autoComplete="off"
          onKeyPress={(e) => {
            if (e.key === "Enter") {
              onFinish();
            }
          }}
        >
          <Form.Item
            name="username"
            rules={[{ required: true, message: "Please input your username!" }]}
          >
            <Input
              className="mb-10x"
              placeholder="Username"
              tabIndex={1}
              autoFocus
            />
          </Form.Item>

          <Form.Item
            name="password"
            rules={[{ required: true, message: "Please input your password!" }]}
          >
            <Input.Password
              className="mb-20x"
              placeholder="Password"
              tabIndex={2}
            />
          </Form.Item>

          <Button
            type="primary"
            loading={loading}
            onClick={onFinish}
            block
            tabIndex={3}
          >
            Submit
          </Button>
        </Form>
      </div>
    </Fragment>
  );
};

export default Login;
