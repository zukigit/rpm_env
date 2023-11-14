import { Button, Input, Space, Table, Divider } from "antd";
import { SearchOutlined } from "@ant-design/icons";
import React, {
  useMemo,
  useCallback,
  useRef,
  useState,
  useEffect,
} from "react";
import { useSelector, useDispatch } from "react-redux";
import Highlighter from "react-highlight-words";
import { Resizable } from "react-resizable";
import { VList } from "virtuallist-antd";
import { useTranslation } from "react-i18next";
import { jobExecutionManagementMenuRender } from "../../contextMenu/ContextMenu";
import "./LockManagementTable.scss";

import {
  setSelectedRowKeys,
  setSelectedObject,
} from "../../../store/LockManagementSlice";
import { alertError, alertSuccess } from "../../dialogs/CommonDialog";
import { useResolvedPath } from "react-router-dom";
import { param } from "jquery";
const ResizableTitle = (props) => {
  const resizing = useRef(false);
  const hold = useRef(false);
  const { onResize, width, onClick, ...restProps } = props;
  let tmp_width = 0;
  let ui_width = 200;
  if (
    restProps.className.includes("ant-table-selection-column") ||
    restProps.className.includes("ant-table-cell-scrollbar") ||
    restProps.className.includes("table-version-row") ||
    restProps.className.includes("table-return-row") ||
    restProps.className.includes("col-hidden-offset")

    // ||
    // restProps.className.includes("extra-col-end")
    //restProps.className.includes("table-valid-row")
  ) {
    return <th {...restProps} />;
  }
  if (!width) {
    if (document.getElementsByClassName("ant-table-thead")[0]) {
      var headers =
        document.getElementsByClassName("ant-table-thead")[0].children[0]
          .children;
      if (headers) {
        for (var i = 0; i < headers.length; i++) {
          if (headers[i].innerText == restProps.title) {
            if (headers[i].offsetWidth) {
              ui_width = headers[i].offsetWidth;
            }
          }
        }
      }
    }
    tmp_width = ui_width;
  } else {
    tmp_width = width;
  }
  return (
    <Resizable
      width={tmp_width}
      height={0}
      onResizeStart={() => {
        resizing.current = true;
      }}
      onResizeStop={() => {
        setTimeout(() => {
          resizing.current = false;
        });
      }}
      handle={
        <span
          className="lock-table-react-resizable-handle"
          onClick={(e) => {
            e.stopPropagation();
          }}
        />
      }
      onResize={onResize}
      draggableOpts={{ enableUserSelectHack: false }}
    >
      <th
        onClick={(...args) => {
          if (!resizing.current && onClick) {
            onClick(...args);
          }
        }}
        onMouseDown={(...args) => {
          hold.current = true;
          if (!resizing.current && onmousedown) {
            onmousedown(...args);
          }
        }}
        onMouseUp={(...args) => {
          hold.current = false;
        }}
        onMouseMove={(...args) => {
          if (hold.current) {
            if (document.selection) {
              document.selection.empty();
            } else {
              window.getSelection().removeAllRanges();
            }
          }
          if (!resizing.current && onmousemove) {
            onmousemove(...args);
          }
        }}
        {...restProps}
        style={{ ...restProps?.style, borderRight: "1px solid #eee" }}
      />
    </Resizable>
  );
};
function VirtualScrollTable({
  dispatchAction,
  hasRowSelect = false,
  columnHeaders,
  prepareData,
  params = null,
  tableHeight = "76vh",
  hideSelectAll = false,
  selectType = "multiple",
  hasContextMenu = false,
}) {
  const [searchText, setSearchText] = useState("");
  const [searchedColumn, setSearchedColumn] = useState("");
  const [data, setData] = useState([]);
  const searchInput = useRef(null);
  const [rowCount, setRowCount] = useState(0);

  const current_column = useRef([...columnHeaders]);
  const searchInputs = useRef([]);
  const dispatch = useDispatch();
  const objectList = useSelector((state) => state["lockedObjList"].data);
  const isLoading = useSelector((state) => state["lockedObjList"].loading);
  const selectedRowKeys = useSelector(
    (state) => state["lockedObjList"].selectedRow
  );
  const unlockResult =
    useSelector((state) => state["lockedObjList"].unlockResult) ?? [];
  const disabledAlert = useRef(0);
  const [t] = useTranslation();
  useEffect(() => {
    if (params) {
      dispatch(dispatchAction(params));
    } else {
      dispatch(dispatchAction(params));
    }
  }, [params]);

  useEffect(() => {
    if (objectList) {
      setData(prepareData(objectList));
    }
  }, [objectList]);

  useEffect(() => {

    var isSearched = false;
    for (var column of columns) {
      if (
        searchInputs.current[column.dataIndex] &&
        searchInputs.current[column.dataIndex].length > 0
      ) {
        isSearched = true;
        break;
      }
    }
    if (!isSearched) {
      setRowCount(objectList.length);
    }

  }, [objectList]);

  const tableFilterChanged = (pagination, filters, sorter, extra) => {
    if (extra.action == "filter") {
      setRowCount(extra.currentDataSource.length);
    }
  };

  useEffect(() => {
    if (disabledAlert.current < 1) {
      disabledAlert.current++;
      return;
    }
    if (unlockResult) {
      if (unlockResult == "Success") {
        dispatch(setSelectedRowKeys([]));
        dispatch(setSelectedObject([]));
        dispatch(dispatchAction(params));
        alertSuccess(t("title-success"), t("label-success"), handleOk);
      } else {
        alertError(t("title-error"), t("err-msg-fail"));
      }
    }
  }, [unlockResult]);

  const handleOk = () => {
    return false;
  };
  const getColumnSearchProps = (dataIndex) => ({
    filterDropdown: ({
      setSelectedKeys,
      selectedKeys,
      confirm,
      clearFilters,
    }) => (
      <div
        style={{
          padding: 8,
        }}
      >
        <Input
          ref={searchInput}
          placeholder={t("txt-search", {
            dataIndex: dataIndex,
          })}
          value={selectedKeys[0]}
          onChange={(e) =>
            setSelectedKeys(e.target.value ? [e.target.value] : [])
          }
          onPressEnter={() => handleSearch(selectedKeys, confirm, dataIndex)}
          style={{
            marginBottom: 8,
            display: "block",
          }}
        />
        <Space>
          <Button
            type="primary"
            onClick={() => handleSearch(selectedKeys, confirm, dataIndex)}
            icon={<SearchOutlined />}
            size="small"
            style={{
              width: 90,
            }}
          >
            Search
          </Button>
          <Button
            onClick={() =>
              clearFilters &&
              handleReset(clearFilters, setSelectedKeys, confirm, dataIndex)
            }
            size="small"
            style={{
              width: 90,
            }}
          >
            Reset
          </Button>
        </Space>
      </div>
    ),
    filterIcon: (filtered) => <SearchOutlined />,
    onFilter: (value, record) =>
      record[dataIndex].toString().toLowerCase().includes(value.toLowerCase()),
    onFilterDropdownVisibleChange: (visible) => {
      if (visible) {
        setTimeout(() => searchInput.current?.select(), 100);
      }
    },
    render: (text) =>
      searchedColumn === dataIndex ? (
        <Highlighter
          highlightStyle={{
            backgroundColor: "#ffc069",
            padding: 0,
          }}
          searchWords={[searchText]}
          autoEscape
          textToHighlight={text ? text.toString() : ""}
        />
      ) : hasContextMenu ? (
        jobExecutionManagementMenuRender(text)
      ) : (
        text
      ),
  });

  columnHeaders = columnHeaders.map((col, index) => ({
    ...col,
    ...(col.searchable && getColumnSearchProps(col.dataIndex)),
    onHeaderCell: (column) => ({
      width: column.width,
      onResize: handleResize(index),
    }),
  }));

  const [columns, setColumns] = useState(columnHeaders);

  const components = useMemo(() => {
    return {
      ...VList({
        height: "40vh",
      }),
      header: {
        cell: ResizableTitle,
      },
    };
  }, []);

  const handleResize = useCallback((index) => {
    return (e, { size }) => {
      setColumns((pre) => {
        var tmp_width = 0;
        let temp = [...pre];
        //if width changed is less than  25px, do not resize.
        if (size.width < 25) {
          return temp;
        }
        //add initial width for all header if not default is set.
        for (let ind = 0; ind < temp.length; ind++) {
          if (!temp[ind].width) {
            var headers =
              document.getElementsByClassName("ant-table-thead")[0].children[0]
                .children;
            if (headers) {
              for (var i = 0; i < headers.length; i++) {
                if (headers[i].innerText == temp[ind].title) {
                  tmp_width = headers[i].offsetWidth;
                }
              }
            }
            temp[ind] = {
              ...temp[ind],
              width: tmp_width,
            };
          }
        }
        if (temp.length == index + 1) {
          return temp;
        }
        if (temp[index].width > size.width) {
          var tmpWidthNext =
            temp[index + 1].width + (temp[index].width - size.width);
          temp[index + 1] = {
            ...temp[index + 1],
            width: tmpWidthNext,
          };
        } else {
          //if new size is not 25px greater than current size, do not change.
          if (temp[index + 1].width <= size.width - temp[index].width + 25) {
            return temp;
          }
          var tmpWidthNext =
            temp[index + 1].width - (size.width - temp[index].width);
          if (tmpWidthNext < 25) {
            return temp;
          }
          temp[index + 1] = {
            ...temp[index + 1],
            width: tmpWidthNext,
          };
        }
        // if (index + 2 == temp.length && temp[index].width) {

        // }
        temp[index] = {
          ...temp[index],
          width: size.width,
        };

        return temp;
      });
    };
  }, []);
  const onSelectChange = (newSelectedRowKeys) => {
    let selectedRowKeyByType = [];
    if (selectType == "multiple") {
      selectedRowKeyByType = newSelectedRowKeys;
    } else {
      if (selectedRowKeys.length === 0) {
        selectedRowKeyByType = [newSelectedRowKeys[0]];
      } else {
        selectedRowKeyByType = [
          newSelectedRowKeys[newSelectedRowKeys.length - 1],
        ];
      }
    }

    let resultList = [];
    let dataList = data;
    resultList = selectedRowKeyByType.map((rowKey) => {
      return dataList.find((dataItem) => {
        if (dataItem.key == rowKey) {
          return dataItem;
        }
      });
    });
    dispatch(setSelectedObject(resultList));
    dispatch(setSelectedRowKeys(selectedRowKeyByType));
  };

  const handleSearch = (selectedKeys, confirm, dataIndex) => {
    confirm();
    setSearchText(selectedKeys[0]);
    setSearchedColumn(dataIndex);
  };

  const handleReset = (clearFilters, setSelectedKeys, confirm, dataIndex) => {
    clearFilters();
    setSearchText("");
    handleSearch([], confirm, dataIndex);
  };

  const rowSelection = hasRowSelect
    ? {
      hideSelectAll,
      selectedRowKeys,
      onChange: onSelectChange,
      getCheckboxProps: (record) => ({
        disabled: !record.hasPermission, // Column configuration not to be checked
      }),
    }
    : undefined;

  const displaySelectedInfo = (selected) => {
    if (selected) {
      if (selected.length > 0) {
        return (
          <>
            <strong>&nbsp;&nbsp;|&nbsp;&nbsp;</strong>
            {selected.length === 1 ? (
              <>
                <strong>1</strong> {t("txt-item-select")}
              </>
            ) : (
              <>
                <strong>{selected.length}</strong> {t("txt-items-select")}
              </>
            )}
          </>
        );
      }
    }
    return "";
  };

  const browserResize = (e) => {
    var pathArray = window.location.pathname.split("/");
    let tmp_columns = [];
    for (let i = 0; i < columns.length; i++) {
      if (
        !current_column.current[i].width ||
        pathArray[2] == "job-execution-management"
      ) {
        let tmp_element = columns[i];
        delete tmp_element.width;
        tmp_columns.push(tmp_element);
      } else {
        tmp_columns.push(columns[i]);
      }
    }
    setColumns(tmp_columns);
  };
  useEffect(() => {
    window.addEventListener("resize", browserResize);
  }, []);

  return (
    <>
      <Table
        bordered
        loading={isLoading}
        rowSelection={rowSelection}
        components={components}
        columns={columns}
        dataSource={data}
        scroll={{ x: "100%", y: tableHeight }}
        pagination={false}
        sticky
        size="middle"
        onChange={tableFilterChanged}
        rowClassName={(record) => !record.hasPermission && "disabled-row"}
        footer={() => {
          const display = rowCount > 0 ? "show" : "hide";
          return (
            <span className={display}>
              <Space size={"middle"} split={<Divider type="vertical" />}>
                <div>
                  <span>
                    {t("txt-show")} <strong>{rowCount}</strong>{" "}
                    {rowCount === 1 ? t("txt-entry") : t("txt-entries")}
                  </span>
                  <span>
                    {selectedRowKeys
                      ? displaySelectedInfo(selectedRowKeys)
                      : ""}
                  </span>
                </div>
              </Space>
            </span>
          );
        }}
      />
    </>
  );
}

export default VirtualScrollTable;
