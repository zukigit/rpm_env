import React, { useEffect, useState } from "react";
import { Table, Form, Input, Tag } from "antd";
import SetupService from "../../services/SetupService";

function Require({ form, onFinish }) {
  const fixedColumns = [
    {
      // title: 'Name',
      dataIndex: "name",
      fixed: true,
      width: 250,
    },
    {
      title: "Current",
      dataIndex: "current",
      width: 100,
    },
    {
      title: "Required",
      dataIndex: "require",
      width: 100,
    },
    {
      key: "tags",
      dataIndex: "status",
      width: 50,
      render: (_, { tags }) => (
        <>
          {tags.map((tag) => {
            let color = tag.length > 5 ? "geekblue" : "green";

            if (tag === "Fail") {
              color = "volcano";
            } else if (tag === "Warning") {
              color = "yellow";
            }

            return (
              <Tag color={color} key={tag}>
                {tag.toUpperCase()}
              </Tag>
            );
          })}
        </>
      ),
    },
  ];

  const [fixedData, setFixedData] = useState([]);

  useEffect(() => {
    SetupService.getRequirement().then((response) => {
      let result = response.detail.data;
      let chk = result.check;
      let arr = [];
      form.setFieldsValue({ pass: result.finalresult });
      if (result.finalresult > 1) {
        document.getElementById("reqerrmsg").innerHTML = result.returnMsg;
        // document.getElementById("reqErr").classList.remove("errHide");
        document.getElementById("reqErr").classList.add("divScroll");
        document.getElementById("reqErr").style.borderColor = "red";
      } else {
        document.getElementById("reqerrmsg").innerHTML = "Current data meet pre-requisites.";
        document.getElementById("reqErr").style.borderColor = "#fafafa";
      }
      for (let i = 0; i < chk.length; i += 1) {
        arr.push({
          key: i,
          name: chk[i].name,
          current: chk[i].current,
          require: chk[i].required,
          tags:
            chk[i].result === 1
              ? ["OK"]
              : chk[i].result === 2
                ? ["Warning"]
                : ["Fail"],
        });
      }
      setFixedData(arr);
    });
  }, [SetupService, setFixedData]);

  return (
    <Form form={form} onFinish={onFinish}>
      <Form.Item label="pass" name="pass" hidden={true}>
        <Input />
      </Form.Item>
      <div className="inner_div">
        <h3 className="tabHead">Pre-requisites:</h3>
        <input type="hidden" id="reqResult"></input>
        <div id="reqErr" className="errDivReq ">
          <p id="reqerrmsg" className="errorMsg" style={{ height: '20px' }}></p>
        </div>
        <Table
          columns={fixedColumns}
          dataSource={fixedData}
          pagination={false}
          scroll={{
            x: 200,
            y: 300,
          }}
          bordered
          summary={() => (
            <Table.Summary fixed>
              <Table.Summary.Row></Table.Summary.Row>
            </Table.Summary>
          )}
        />
      </div>
    </Form>
  );
}
export default Require;
