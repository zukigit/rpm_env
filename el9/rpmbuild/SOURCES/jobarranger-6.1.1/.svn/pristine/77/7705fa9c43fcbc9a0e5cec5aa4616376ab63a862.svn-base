import React, { useEffect, useState } from "react";
import { Form } from "antd";
import SetupService from "../../services/SetupService";

function Welcome({ form, onFinish }) {
  const [response, setResponse] = useState([]);
  useEffect(() => {
    SetupService.getInitial().then((response) => {
    });
  }, [SetupService, response]);
  return (
    <Form form={form} onFinish={onFinish}>
      <div className="title">
        <p>
          <span>Welcome to</span>
          <br></br>Job Arranger Setup
        </p>
      </div>
    </Form>
  );
}

export default Welcome;
