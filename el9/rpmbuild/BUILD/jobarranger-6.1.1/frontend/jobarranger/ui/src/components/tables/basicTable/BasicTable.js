import { Table } from "antd";
import React, { useMemo, useState } from "react";
import { Resizable } from "react-resizable";
import "./BasicTable.scss";

const ResizableTitle = (props) => {
  let resizing = false;
  const { onResize, width, onClick, ...restProps } = props;
  let tmp_width = 0;

  // if (!width) {
  //   return <th {...restProps} />;
  // }
  if (
    restProps.className.includes("ant-table-cell-scrollbar") ||
    restProps.className.includes("table-version-row") ||
    restProps.className.includes("table-return-row")
  ) {
    return <th {...restProps} />;
  }
  if (!width) {
    tmp_width = 100;
  } else {
    tmp_width = width;
  }
  return (
    <Resizable
      width={tmp_width}
      height={0}
      onResizeStart={() => (resizing = true)}
      onResizeStop={() => {
        setTimeout(() => {
          resizing = false;
        });
      }}
      handle={
        <span
          className="react-resizable-handle"
          onClick={(e) => {
            console.log(e);
          }}
        />
      }
      onResize={onResize}
      draggableOpts={{ enableUserSelectHack: false }}
    >
      <th
        onClick={(...args) => {
          if (!resizing && onClick) {
            onClick(...args);
          }
        }}
        {...restProps}
        style={{ ...restProps?.style, borderRight: "1px solid #eee" }}
      />
    </Resizable>
  );
};

const BasicTable = ({ dataSource, columnHeaders, tableHeight }) => {
  columnHeaders = columnHeaders.map((col, index) => ({
    ...col,
    onHeaderCell: (column) => ({
      width: column.width,
      onResize: handleResize(index),
    }),
  }));

  const [columns, setColumns] = useState(columnHeaders);

  const components = useMemo(() => {
    return {
      header: {
        cell: ResizableTitle,
      },
    };
  }, []);

  const handleResize =
    (index) =>
    (_, { size }) => {
      let tmp_width = 0;
      _.stopImmediatePropagation();
      _.stopPropagation();
      _.preventDefault();

      let temp = [...columns];

      if (!size.width || size.width <= 100) {
        tmp_width = 100;
      } else {
        tmp_width = size.width;
      }
      temp[index] = {
        ...temp[index],
        width: tmp_width,
      };
      setColumns(temp);
    };

  return (
    <div>
      <Table
        className="basic-table"
        bordered
        components={components}
        dataSource={dataSource}
        columns={columns}
        pagination={false}
        scroll={{ x: "100%", y: tableHeight }}
        size="small"
      />
    </div>
  );
};

export default BasicTable;
