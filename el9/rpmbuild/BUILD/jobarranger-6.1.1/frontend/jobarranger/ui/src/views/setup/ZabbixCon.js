import React from "react";
import { Form, Input } from "antd";

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

function ZabbixCon({ form, onFinish }) {
  return (
    <Form {...formItemLayout} form={form} onFinish={onFinish}>
      <h3 className="tabHead">Zabbix connection:</h3>
      <p>Please enter Zabbix URL for zabbix connection.</p>
      <div id="zbxErr" className="errDiv">
        <p id="zbxerrmsg" className="errorMsg"></p>
      </div>

      <Form.Item
        name="zabbixURL"
        label="Zabbix URL"
        rules={[
          {
            required: true,
            message: "Please input zabbix URL!",
          },
        ]}
      >
        <Input />
      </Form.Item>
    </Form>
  );
}
export default ZabbixCon;
