import React from "react";
import { Form, Input, Select } from "antd";

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

function DBconnection({ form, onFinish }) {
  const { Option } = Select;
  return (
    <Form
      {...formItemLayout}
      form={form}
      onFinish={onFinish}
      scrollToFirstError
    >
      <h3 className="tabHead">Configure DB connection:</h3>
      <p>
        Please create database manually, and set the configuration parameters
        for connection to this database. Press "Next step" button when done.
      </p>
      <div id="dbErr" className="errDiv">
        <p id="dberrmsg" className="errorMsg"></p>
      </div>
      <Form.Item
        name="DBType"
        label="Database Type"
        rules={[
          {
            required: true,
            message: "Please select Database Type!",
          },
        ]}
      >
        <Select id="dbType" placeholder="select your Database Type">
          <Option value="mysql">MySQL</Option>
          <Option value="pgsql">Postgres</Option>
        </Select>
      </Form.Item>

      <Form.Item
        name="DBHost"
        label="Database Host"
        rules={[
          {
            required: true,
            message: "Please input Database Host!",
          },
        ]}
      >
        <Input />
      </Form.Item>

      <Form.Item
        name="DBName"
        label="Database Name"
        rules={[
          {
            required: true,
            message: "Please input Database Name!",
          },
        ]}
      >
        <Input />
      </Form.Item>

      <Form.Item
        name="DBUser"
        label="User Name"
        rules={[
          {
            required: true,
            message: "Please input user name!",
          },
        ]}
      >
        <Input />
      </Form.Item>
      <Form.Item
        name="DBPass"
        label="Password"
        rules={[
          {
            required: true,
            message: "Please input your password!",
          },
        ]}
      >
        <Input.Password />
      </Form.Item>
    </Form>
  );
}
export default DBconnection;
