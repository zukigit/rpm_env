import React from "react";
import { Radio, Form, Input } from "antd";

import { useState } from "react";

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

function WebsocketCon() {
  const [form] = Form.useForm();
  const [value, setValue] = useState(1);

  const onChange = (e) => {
    setValue(e.target.value);
  };

  return (
    <div>
      <h3 className="tabHead">WebSocket connection:</h3>
      <p>Please enter WebSocket.</p>
      <div id="wsErr" className="errDiv errHide">
        <p id="wserrmsg" className="errorMsg"></p>
      </div>

      <Form
        {...formItemLayout}
        form={form}
        name="register"
        // onFinish={onFinish}
        scrollToFirstError
      >
        <Form.Item
          name="webSocketUrl"
          label="Websocket URL"
          rules={[
            {
              required: true,
              message: "Please input Websocket URL!",
            },
          ]}
        >
          <Input />
        </Form.Item>

        <Form.Item name="radio-group" label="Web socket type">
          <Radio.Group onChange={onChange} value={value}>
            <Radio value={1}>WS</Radio>
            <Radio value={2}>WSS</Radio>
          </Radio.Group>
        </Form.Item>
      </Form>
    </div>
  );
}
export default WebsocketCon;
