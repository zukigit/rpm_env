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

function LogLocation({ form, onFinish }) {
  return (
    <Form
      {...formItemLayout}
      form={form}
      onFinish={onFinish}
      scrollToFirstError
    >
      <h3 className="tabHead">Log Location:</h3>
      <p>Please enter log location.</p>
      <div id="logErr" className="errDiv">
        <p id="logerrmsg" className="errorMsg"></p>
      </div>
      <Form.Item
        name="appLog"
        label="Log file directory"
        rules={[
          {
            required: true,
            message: "Please input Log file directory",
          },
        ]}
      >
        <Input />
      </Form.Item>
      <Form.Item
        name="logfile"
        label="Log file name"
        rules={[
          {
            required: true,
            message: "Please input Log filename",
          },
        ]}
      >
        <Input />
      </Form.Item>
    </Form>
  );
}
export default LogLocation;
