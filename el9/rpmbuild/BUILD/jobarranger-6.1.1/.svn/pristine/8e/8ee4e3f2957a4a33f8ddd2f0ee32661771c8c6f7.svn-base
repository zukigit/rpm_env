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
import { useTranslation } from "react-i18next";
import { checkRowForRender } from "../../contextMenu/ContextMenu";
import {
  setSelectedRowKeys,
  setSelectedObject,
} from "../../../store/ObjectListSlice";
import {
  setSelectedRowKeys as setSelectedRowKeysAllTable,
  setSelectedObject as setSelectedObjectAllTable,
  setIntervalId as setIntervalIdAllTable,
} from "../../../store/AllOperationListSlice";
import {
  setSelectedRowKeys as setSelectedRowKeysDuringTable,
  setSelectedObject as setSelectedObjectDuringTable,
  setIntervalId as setIntervalIdDuringTable,
} from "../../../store/DuringOperationListSlice";
import {
  setSelectedRowKeys as setSelectedRowKeysErrorTable,
  setSelectedObject as setSelectedObjectErrorTable,
  setIntervalId as setIntervalIdErrorTable,
} from "../../../store/ErrorOperationListSlice";
import "./PaginationTable.scss";
import i18next from "i18next";
import { EXECUTION_MANAGEMENT } from "../../../constants";

const ResizableTitle = (props) => {
  const { onResize, width, onClick, ...restProps } = props;
  let tmp_width = 0;
  let ui_width = 200;
  const resizing = useRef(false);
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
          if (headers[i].innerText === restProps.title) {
            ui_width = headers[i].offsetWidth;
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
          className="pgtable-react-resizable-handle"
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
          if (!resizing.current && onmousedown) {
            onmousedown(...args);
          }
        }}
        onMouseMove={(...args) => {
          if (document.selection) {
            document.selection.empty();
          } else {
            window.getSelection().removeAllRanges();
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

function PaginationTable({
  stateId,
  dispatchAction,
  hasRowSelect = false,
  columnHeaders,
  getTableData,
  prepareData,
  params = null,
  tableHeight = "76vh",
  size = "middle",
  onRow = null,
  hideSelectAll = false,
  selectType = "multiple",
  onDoubleClickAction = (event, record, rowIndex) => { },
  category = null,
  tableType = null,
  autoRefresh = false,
}) {
  const { t } = useTranslation();
  const selectedRowKeys = useSelector((state) => state[stateId].selectedRow);
  const selectedRowObject = useSelector((state) => state[stateId].selectedObject);
  const [searchText, setSearchText] = useState("");
  const [searchedColumn, setSearchedColumn] = useState("");
  const [data, setData] = useState([]);
  const [rowCount, setRowCount] = useState(0);
  const searchInput = useRef(null);

  const dispatch = useDispatch();
  const objectList = useSelector((state) => state[stateId].data);
  const isLoading = useSelector((state) => state[stateId].loading);

  const current_column = useRef([...columnHeaders]);

  useEffect(() => {
    if (!autoRefresh) {
      if (params != null) {
        dispatch(dispatchAction(params));
      } else {
        dispatch(dispatchAction());
      }
    } else {
      const intervalId = setInterval(() => {
        dispatch(dispatchAction(params));
      }, 2000);
      if (tableType === t("title-op-info-job")) {
        dispatch(setIntervalIdAllTable(intervalId));
      } else if (tableType === t("title-op-err-job")) {
        dispatch(setIntervalIdErrorTable(intervalId));
      } else if (tableType === t("title-op-exe-job")) {
        dispatch(setIntervalIdDuringTable(intervalId));
      }

      return () => {
        clearInterval(intervalId);
      };
    }
  }, [params]);

  useEffect(() => {
    setData(prepareData(objectList));
  }, [objectList, prepareData, setData]);

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
          placeholder={`Search ${dataIndex}`}
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
              clearFilters && handleReset(clearFilters, confirm, dataIndex)
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
    filterIcon: (filtered) => (
      <SearchOutlined
        style={{
          color: filtered ? "#1890ff" : undefined,
        }}
      />
    ),
    onFilter: (value, record) =>
      record[dataIndex]
        ? value
          ? record[dataIndex]
            .toString()
            .toLowerCase()
            .includes(value.toLowerCase())
          : true
        : false,
    onFilterDropdownVisibleChange: (visible) => {
      if (visible) {
        setTimeout(() => searchInput.current?.select(), 100);
      }
    },
    render: (text, col) => {
      if (searchedColumn === dataIndex) {
        return (
          <Highlighter
            highlightStyle={{
              backgroundColor: "#ffc069",
              padding: 0,
            }}
            searchWords={[searchText]}
            autoEscape
            textToHighlight={text ? text.toString() : ""}
          />
        );
      } else {
        return checkRowForRender(category, text, col, dataIndex, tableType);
      }
    },
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

  useEffect(() => {
    window.addEventListener("resize", browserResize);
  }, []);

  const browserResize = (e) => {
    var pathArray = window.location.pathname.split("/");
    let tmp_columns = [];

    for (let i = 0; i < columns.length; i++) {
      if (
        !current_column.current[i].width ||
        pathArray[2] === "job-execution-management"
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

  const components = useMemo(() => {
    return {
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
                if (headers[i].innerText === temp[ind].title) {
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

  const onSelectChange = (newSelectedRowKeys, selectedRows) => {
    const t = i18next.t;
    let selectedRowKeyByType = newSelectedRowKeys;
    let selectRowsObjectByType = selectedRows;
    if (selectType === "single") {
      if (selectedRowKeys.length > 0) {
        selectedRowKeyByType = selectedRowKeyByType.filter(
          (el) => selectedRowKeys[0] !== el
        );
        selectRowsObjectByType = selectRowsObjectByType.filter(
          (el) => selectedRowKeys[0] !== el.key
        );
      }
    }

    if (category === EXECUTION_MANAGEMENT) {
      if (tableType === t("title-op-info-job")) {
        dispatch(setSelectedObjectAllTable(selectRowsObjectByType));
        dispatch(setSelectedRowKeysAllTable(selectedRowKeyByType));
      } else if (tableType === t("title-op-err-job")) {
        dispatch(setSelectedObjectErrorTable(selectRowsObjectByType));
        dispatch(setSelectedRowKeysErrorTable(selectedRowKeyByType));
      } else if (tableType === t("title-op-exe-job")) {
        dispatch(setSelectedObjectDuringTable(selectRowsObjectByType));
        dispatch(setSelectedRowKeysDuringTable(selectedRowKeyByType));
      }
    } else {
      dispatch(setSelectedObject(selectRowsObjectByType));
      dispatch(setSelectedRowKeys(selectedRowKeyByType));
    }
  };

  const handleSearch = (selectedKeys, confirm, dataIndex) => {
    confirm();
    setSearchText(selectedKeys[0]);
    setSearchedColumn(dataIndex);
  };

  const handleReset = (clearFilters, confirm, dataIndex) => {
    clearFilters();
    handleSearch([], confirm, dataIndex);
  };

  const rowSelection = hasRowSelect
    ? {
      hideSelectAll,
      selectedRowKeys,
      onChange: onSelectChange,
    }
    : undefined;

  const onRowAction = (record, rowIndex) => {
    return {
      onDoubleClick: (event) => {
        onDoubleClickAction(event, record, rowIndex);
      },
      onContextMenu: (event) => {
        let selectedRow = [];
        let selectedRowKey = [];
        selectedRow.push(record);
        selectedRowKey.push(record.key);
        if (tableType === t("title-op-info-job")) {
          dispatch(setSelectedObjectAllTable(selectedRow));
          dispatch(setSelectedRowKeysAllTable(selectedRowKey));
        } else if (tableType === t("title-op-err-job")) {
          for (let i = 0; i < selectedRowObject.length; i++) {
            if (selectedRowObject[i].key != record.key) {
              selectedRow.push(selectedRowObject[i]);
              selectedRowKey.push(selectedRowKeys[i]);
            }
          }
          dispatch(setSelectedObjectErrorTable(selectedRow));
          dispatch(setSelectedRowKeysErrorTable(selectedRowKey));
        } else if (tableType === t("title-op-exe-job")) {
          dispatch(setSelectedObjectDuringTable(selectedRow));
          dispatch(setSelectedRowKeysDuringTable(selectedRowKey));
        }
        document.addEventListener(`click`, function onClickOutside() {
          if (tableType === t("title-op-info-job")) {
            dispatch(setSelectedRowKeysAllTable([]));
            dispatch(setSelectedObjectAllTable([]));
          } else if (tableType === t("title-op-err-job")) {
            dispatch(setSelectedRowKeysErrorTable([]));
            dispatch(setSelectedObjectErrorTable([]));
          } else if (tableType === t("title-op-exe-job")) {
            dispatch(setSelectedRowKeysDuringTable([]));
            dispatch(setSelectedObjectDuringTable([]));
          }

          document.removeEventListener(`click`, onClickOutside)
        })

      },
    };
  };

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

  useEffect(() => {
    return () => {
      window.removeEventListener("resize", browserResize);
    };
  }, []);

  const tableFilterChanged = (pagination, filters, sorter, extra) => {
    if (extra.action == "filter") {
      setRowCount(extra.currentDataSource.length);
    }
  };
  useEffect(() => {
    if (!searchText) {
      setRowCount(data.length);
    }
  }, [data]);
  return (
    <Table
      className="pagination-table"
      bordered
      loading={autoRefresh ? false : isLoading}
      rowSelection={rowSelection}
      components={components}
      columns={columns}
      dataSource={data}
      scroll={{ x: "100%", y: tableHeight }}
      sticky
      size={size}
      onRow={onRowAction}
      onChange={tableFilterChanged}
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
                  {selectedRowKeys ? displaySelectedInfo(selectedRowKeys) : ""}{" "}
                </span>
              </div>
            </Space>
          </span>
        );
      }}
    />
  );
}

export default PaginationTable;
