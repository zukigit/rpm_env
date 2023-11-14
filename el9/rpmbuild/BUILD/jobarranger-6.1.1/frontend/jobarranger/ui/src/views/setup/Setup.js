import React, { useState } from "react";
import "antd/dist/antd.css";
import "./Setup.scss";
import { Button, Spin, Steps, Form } from "antd";
import SetupService from "../../services/SetupService";

import Welcome from "./Welcome";
import Require from "./Require";
import DBconnection from "./DBconnection";
import ZabbixCon from "./ZabbixCon";
import Loglocation from "./LogLocation";
import Summary from "./Summary";
import Final from "./Final";
import { useNavigate } from "react-router-dom";

const { Step } = Steps;
const steps = [
  {
    title: "WELCOME",
  },
  {
    title: "Pre-requisites",
  },
  {
    title: "Database connection",
  },
  {
    title: "Zabbix Connection",
  },
  {
    title: "Log Location",
  },
  {
    title: "Summary",
  },
  {
    title: "Finish",
  },
];

const Setup = () => {
  const navigate = useNavigate();
  let comp = "";
  const [current, setCurrent] = useState(0);
  const [formDataDBTmp, setFormDataDBTmp] = useState();
  const [formDataZabbix, setFormZabbix] = useState();
  const [formDataLog, setFormLog] = useState();
  const [isLoading, setIsloading] = useState(false);

  const prev = () => {
    setIsloading(false);
    setCurrent(current - 1);
  };

  const [form] = Form.useForm();

  const handleFinish = (values) => {
    if (current === 1) {
      if (values.pass === 1) {
        setCurrent(current + 1);
      }
    } else if (current === 2) {
      setIsloading(true);
      document.getElementById("dberrmsg").innerHTML = "";
      document.getElementById("dbErr").classList.add("errHide");
      setFormDataDBTmp(values);
      //Check DB connection
      SetupService.getDBConnection(values).then((result) => {
        if (result.detail.message === "success") {
          setCurrent(current + 1);
        } else {
          document.getElementById("dberrmsg").innerHTML =
            "Database cannot be connected";
          document.getElementById("dbErr").style.borderColor = "red";
          // document.getElementById("dbErr").classList.remove("errHide");
        }
        setIsloading(false);
      });
    } else if (current === 3) {
      setIsloading(true);
      document.getElementById("zbxerrmsg").innerHTML = "";
      document.getElementById("zbxErr").classList.add("errHide");
      //   const formDataZbx = new FormData();
      //   formDataZbx.append("zabbixURL", values["zabbixURL"]);
      setFormZabbix(values);
      SetupService.getZbxConnection(values).then((result) => {
        if (result.detail.message === "version") {
          document.getElementById("zbxerrmsg").innerHTML =
            "Zabbix version incompatible";
          document.getElementById("zbxErr").style.borderColor = "red";
          // document.getElementById("zbxErr").classList.remove("errHide");
        } else if (result.detail.message !== "zbxnotfound") {
          setCurrent(current + 1);
        } else {
          document.getElementById("zbxerrmsg").innerHTML =
            "Zabbix cannot be connected";
          document.getElementById("zbxErr").style.borderColor = "red";
          // document.getElementById("zbxErr").classList.remove("errHide");
        }
        setIsloading(false);
      });
    } else if (current === 4) {
      setIsloading(true);
      document.getElementById("logerrmsg").innerHTML = "";
      document.getElementById("logErr").classList.add("errHide");
      setFormLog(values);
      var fullLogFile = {
        appLog: "",
      };
      // Linux
      if (values["appLog"].slice(-1) == "/") {
        fullLogFile.appLog = values["appLog"] + "" + values["logfile"];
      } else {
        fullLogFile.appLog = values["appLog"] + "/" + values["logfile"];
      }
      // if (values["appLog"].slice(-1) == "\\") {
      //   fullLogFile.appLog = values["appLog"] + "" + values["logfile"];
      // } else {
      //   fullLogFile.appLog = values["appLog"] + "\\" + values["logfile"];
      // }
      SetupService.getLogPathCheck(fullLogFile).then((result) => {
        let response = result.detail.data;
        if (response.result === "fail") {
          document.getElementById("logerrmsg").innerHTML =
            response.resultMessage;
          document.getElementById("logErr").style.borderColor = "red";
          // document.getElementById("logErr").classList.remove("errHide");
        } else {
          setCurrent(current + 1);
        }
        setIsloading(false);
      });
    } else if (current === 5) {
      setIsloading(true);
      document.getElementById("wserrmsg").innerHTML = "";
      document.getElementById("wsErr").classList.add("errHide");
      const formDataCreate = formDataDBTmp;
      //   for (const key in formDataDBTmp) {
      //     formDataCreate.append(key, formDataDBTmp[key]);
      //   }
      //   formDataCreate.append("zabbixURL", formDataZabbix["zabbixURL"]);
      //   formDataCreate.append("appLog", formDataLog["appLog"]);
      formDataCreate.zabbixURL = formDataZabbix["zabbixURL"];
      if (formDataLog["appLog"].slice(-1) == "/") {
        formDataCreate.appLog =
          formDataLog["appLog"] + "" + formDataLog["logfile"];
      } else {
        formDataCreate.appLog =
          formDataLog["appLog"] + "/" + formDataLog["logfile"];
      }
      let root = window.location.href;
      //   root = root.substring(0, root.length - 6);
      formDataCreate.HostName = root.substring(0, root.length - 6);

      SetupService.createConfig(formDataCreate).then((result) => {
        let response = result.detail.data;
        if (response.result === "fail") {
          document.getElementById("wserrmsg").innerHTML =
            response.resultMessage;
          document.getElementById("wsErr").style.borderColor = "red";
          // document.getElementById("wsErr").classList.remove("errHide");
        } else {
          setCurrent(current + 1);
        }
        setIsloading(false);
      });
    } else {
      setCurrent(current + 1);
    }
  };

  function handleKeyUp(event) {
    // Enter
    if (event.keyCode === 13) {
      form.submit();
    }
  }

  return (
    <>
      <div className="full">
        <div className="parent">
          <div className="setupBody">
            <div className="leftContent">
              <Steps Steps progressDot direction="vertical" current={current}>
                {steps.map((item) => (
                  <Step key={item.title} title={item.title} />
                ))}
              </Steps>
            </div>

            <div className="rightContent">
              <Spin size="large" spinning={isLoading}>
                <div className="steps-content">
                  {(() => {
                    switch (current) {
                      case 0:
                        comp = (
                          <Welcome
                            form={form}
                            onFinish={handleFinish}
                            onKeyUp={handleKeyUp}
                          />
                        );
                        break;
                      case 1:
                        comp = (
                          <Require
                            form={form}
                            onFinish={handleFinish}
                            onKeyUp={handleKeyUp}
                          />
                        );
                        break;
                      case 2:
                        comp = (
                          <DBconnection
                            form={form}
                            onFinish={handleFinish}
                            onKeyUp={handleKeyUp}
                          />
                        );
                        break;
                      case 3:
                        comp = (
                          <ZabbixCon
                            form={form}
                            onFinish={handleFinish}
                            onKeyUp={handleKeyUp}
                          />
                        );
                        break;
                      case 4:
                        comp = (
                          <Loglocation
                            form={form}
                            onFinish={handleFinish}
                            onKeyUp={handleKeyUp}
                          />
                        );
                        break;
                      case 5:
                        comp = (
                          <Summary
                            form={form}
                            dbData={formDataDBTmp}
                            zbxData={formDataZabbix}
                            logData={formDataLog}
                            onKeyUp={handleKeyUp}
                            onFinish={handleFinish}
                          />
                        );
                        break;
                      default:
                        comp = <Final />;
                        break;
                    }
                  })()}
                  {comp}
                </div>
              </Spin>
              <div className="steps-action">
                {current > 0 && (
                  <Button
                    style={{
                      margin: "0 8px",
                    }}
                    onClick={() => prev()}
                  >
                    Previous
                  </Button>
                )}
                {current === steps.length - 1 && (
                  <Button type="primary" onClick={() => navigate("/login")}>
                    Done
                  </Button>
                )}
                {current === steps.length - 2 && (
                  <Button onClick={() => form.submit()}>Create</Button>
                )}
                {current < steps.length - 2 && (
                  <Button onClick={() => form.submit()}>Next</Button>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default Setup;
