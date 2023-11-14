import React from "react";
import { Form } from "antd";

const formItemLayout = {
  labelCol: {
    xs: {
      span: 25,
    },
    sm: {
      span: 6,
    },
  },
  wrapperCol: {
    xs: {
      span: 25,
    },
    sm: {
      span: 10,
    },
  },
};
function Summary({ form, onFinish, dbData, zbxData, logData }) {
  return (
    <Form
      {...formItemLayout}
      form={form}
      onFinish={onFinish}
      scrollToFirstError
    >
      <div>
        <h3 className="tabHead">Pre-installation summary:</h3>
        <p>
          Please check configuration parameters. If all is correct, press
          "Create" button, or "Previous" button to change configuration
          parameters.
        </p>
        <div id="wsErr" className="errDiv">
          <p id="wserrmsg" className="errorMsg"></p>
        </div>
        <div className="setupSummary">
          <div className="block">
            <label className="result"> Database type </label>
            <label id="rdbType" className="resultDisplay">
              {dbData.DBType}
            </label>
          </div>
          <div className="block">
            <label className="result"> Database host </label>
            <label id="rdbHost" className="resultDisplay">
              {dbData.DBHost}
            </label>
          </div>
          <div className="block">
            <label className="result"> Database Name </label>
            <label id="rdbName" className="resultDisplay">
              {dbData.DBName}
            </label>
          </div>
          <div className="block">
            <label className="result"> User </label>
            <label id="ruserName" className="resultDisplay">
              {dbData.DBUser}
            </label>
          </div>
          <div className="block">
            <label className="result"> Zabbix URL </label>
            <label id="rZabbix" className="resultDisplay">
              {zbxData.zabbixURL}
            </label>
          </div>
          <div className="block">
            <label className="result"> Application Log Path </label>
            <label id="rapplog" className="resultDisplay">
              {logData.appLog}
            </label>
          </div>
        </div>
      </div>
    </Form>
  );
}
export default Summary;
