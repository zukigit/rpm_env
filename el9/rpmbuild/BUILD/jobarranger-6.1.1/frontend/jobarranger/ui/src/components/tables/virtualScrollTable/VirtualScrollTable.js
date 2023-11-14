import { Button, Input, Space, Divider, Table } from "antd";
import { SearchOutlined } from "@ant-design/icons";
import moment from "moment";
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
import { alertError, alertSuccess } from "../../dialogs/CommonDialog";
import {
  checkRowForRender,
  jobExecutionManagementMenuRender,
  jobnetObjectRender,
} from "../../contextMenu/ContextMenu";
import "./VirtualScrollTable.scss";
import {
  setSelectedRowKeys,
  setSelectedObject,
} from "../../../store/ObjectListSlice";
import {
  setSelectedRowKeys as setSelectedRowKeysAllTable,
  setSelectedObject as setSelectedObjectAllTable,
} from "../../../store/AllOperationListSlice";
import {
  setSelectedRowKeys as setSelectedRowKeysDuringTable,
  setSelectedObject as setSelectedObjectDuringTable,
} from "../../../store/DuringOperationListSlice";
import {
  setSelectedRowKeys as setSelectedRowKeysErrorTable,
  setSelectedObject as setSelectedObjectErrorTable,
} from "../../../store/ErrorOperationListSlice";
import {
  EXECUTION_MANAGEMENT,
  OBJECT_CATEGORY,
  SERVICE_RESPONSE,
} from "../../../constants";
import { Content } from "antd/lib/layout/layout";
import i18next from "i18next";
import { useNavigate } from "react-router-dom";
import store from "../../../store";
const ResizableTitle = (props) => {
  const resizing = useRef(false);
  const hold = useRef(false);
  const { onResize, width, onClick, ...restProps } = props;
  let tmp_width = 0;
  let ui_width = 200;

  // if (!width) {
  //   return <th {...restProps} />;
  // }
  if (
    restProps.className.includes("ant-table-selection-column") ||
    restProps.className.includes("ant-table-cell-scrollbar") ||
    restProps.className.includes("table-version-row") ||
    restProps.className.includes("table-last-row") ||
    restProps.className.includes("table-return-row") ||
    restProps.className.includes("col-hidden-offset")

    // ||
    // restProps.className.includes("extra-col-end")
    //restProps.className.includes("table-valid-row")
  ) {
    return <th {...restProps} />;
  }
  //add initial width for all header if not default is set.
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
        restProps.className.includes("table-version-row") ? (
          <></>
        ) : (
          <span className="vtb-react-resizable-handle" onClick={(e) => { }} />
        )
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
        style={{ ...restProps?.style, borderRight: "3px solid #eee" }}
      />
    </Resizable>
  );
};

function VirtualScrollTable({
  stateId,
  dispatchAction,
  hasRowSelect = false,
  columnHeaders,
  prepareData,
  params = null,
  tableHeight = "76vh",
  size = "middle",
  onRow = null,
  hideSelectAll = false,
  selectType = "multiple",
  hasContextMenu = false,
  onDoubleClickAction,
  rowClassName = {},
  category,
  publicType = true,
  tableType = null,
  autoRefresh = false,
  // cleanupDispatchAction,
}) {
  const { t } = useTranslation();
  const [data, setData] = useState([]);
  const [rowCount, setRowCount] = useState(0);
  const searchInput = useRef(null);
  const disabledAlert = useRef(0);
  const disabledEmpty = useRef(0);
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const columnFilterItem = useRef([]);
  const initState = useRef(true);
  const objectList = useSelector((state) => state[stateId].data);
  const isLoading = useSelector((state) => state[stateId].loading);
  const selectedRowKeys = useSelector((state) => state[stateId].selectedRow);
  const selectedRowObject = useSelector((state) => state[stateId].selectedObject);
  const selectedObjectList = store.getState().objectList.selectedObject;
  const selectedObjectListKey = store.getState().objectList.selectedRow;
  //test
  const searchInputs = useRef([]);
  const [searchInputText, setSearchInputText] = useState([]);
  const updateValidationResult = useSelector(
    (state) => state[stateId].updateValidationResult
  );
  const current_column = useRef([...columnHeaders]);
  const sortInfo = useRef({});

  const dateFormatter = (date) => {
    if (!date) {
      return "";
    }
    return moment(date, "YYYYMMDDHHmmss").format("YYYY/MM/DD HH:mm:ss");
  };
  const relatedMessage = (data, id, header) => {
    return (
      <div>
        <strong style={{ margin: "10px" }}>{t(header)}...</strong>
        <br />
        <table>
          <tr>
            <th>Object Id</th>
            <th>Updated Date</th>
          </tr>
          {data.map((item) => {
            return (
              <tr>
                <td>{item[id]}</td>
                <td>{dateFormatter(item.update_date)}</td>
              </tr>
            );
          })}
        </table>
      </div>
    );
  };
  const relatedDataErrMsg = (relatedData) => {
    if (relatedData) {
      //relatedData.map(data=>{});
      return (
        <>
          <span>
            {t("err-msg-related")} <strong>{relatedData.objectId}</strong>
            <br />
            <br />
            {t("rel-error")}
            <br />
          </span>
          <div className="related-data-box">
            {relatedData.calendarData
              ? relatedData.calendarData.length > 0
                ? relatedMessage(
                  relatedData.calendarData,
                  "calendar_id",
                  "menu-cal"
                )
                : ""
              : ""}
            {relatedData.filterData
              ? relatedData.filterData.length > 0
                ? relatedMessage(
                  relatedData.filterData,
                  "filter_id",
                  "menu-flt"
                )
                : ""
              : ""}

            {relatedData.scheduleData
              ? relatedData.scheduleData.length > 0
                ? relatedMessage(
                  relatedData.scheduleData,
                  "schedule_id",
                  "menu-schd"
                )
                : ""
              : ""}

            {relatedData.jobnetData
              ? relatedData.jobnetData.length > 0
                ? relatedMessage(
                  relatedData.jobnetData,
                  "jobnet_id",
                  "menu-jobnet"
                )
                : ""
              : ""}
          </div>
        </>
      );
    }
  };
  const getColumnSearchProps = (dataIndex) => ({
    filterDropdown: ({
      setSelectedKeys,
      //selectedKeys,
      confirm,
      clearFilters,
    }) => {
      // if (selectedKeys[0] && selectedKeys[0].length > 0) {
      //   let tmpColumnFilterItem = columnFilterItem.current;
      //   tmpColumnFilterItem[dataIndex] = {
      //     setSelectedKeys: setSelectedKeys,
      //     selectedKeys: selectedKeys,
      //     confirm: confirm,
      //     clearFilters: clearFilters,
      //   };
      //   columnFilterItem.current = tmpColumnFilterItem;
      // }
      let tmp_columnSelectedKeys = searchInputs.current;
      if (tmp_columnSelectedKeys[dataIndex]) {
        let selectedKeys = tmp_columnSelectedKeys[dataIndex];
        //tmp_columnSelectedKeys[col.dataIndex]=[];
        if (selectedKeys[0] && selectedKeys[0].length > 0) {
          let tmpColumnFilterItem = columnFilterItem.current;
          tmpColumnFilterItem[
            `${category}-${publicType ? "public" : "private"}-${dataIndex}`
          ] = {
            setSelectedKeys: setSelectedKeys,
            // selectedKeys: selectedKeys,
            confirm: confirm,
            clearFilters: clearFilters,
          };
          columnFilterItem.current = tmpColumnFilterItem;
        }
      }
      return (
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
            //useState here.
            value={
              searchInputText[dataIndex] ? searchInputText[dataIndex][0] : ""
            }
            onChange={
              (e) => {
                let tmp_searchInputText = searchInputText;
                tmp_searchInputText[dataIndex] = e.target.value
                  ? [e.target.value]
                  : [];
                setSearchInputText([...tmp_searchInputText]);
              }
              //setSelectedKeys(e.target.value ? [e.target.value] : [])
            }
            onPressEnter={() =>
              handleSearch(setSelectedKeys, confirm, dataIndex)
            }
            style={{
              marginBottom: 8,
              display: "block",
            }}
          />
          <Space>
            <Button
              type="primary"
              onClick={() => handleSearch(setSelectedKeys, confirm, dataIndex)}
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
      );
    },
    onFilterDropdownVisibleChange: (visible) => {
      if (visible) {
        setTimeout(() => searchInput.current?.select(), 100);
      }
    },
    filterIcon: (filtered) => (
      <SearchOutlined
      // style={{
      //   color: filtered ? "#1890ff" : undefined,
      // }}
      />
    ),
    onFilter: (value, record) => {
      if (initState.current) {
        return true;
      } else {
        return record[dataIndex]
          ? searchInputs.current[dataIndex]
            ? record[dataIndex]
              .toString()
              .toLowerCase()
              .includes(
                searchInputs.current[dataIndex].toString().toLowerCase()
              )
            : true
          : false;
      }
    },
    render: (text, col) =>
      checkRowForRender(category, text, col, dataIndex, tableType),
  });
  const headerClickSortHandler = (headerColumn) => {
    // setSortInfo({
    // });
    let sortingInfo = {
      order: "ascend",
      columnKey: headerColumn.title,
    };
    //previous sort title is same.
    if (
      sortInfo.current.columnKey &&
      sortInfo.current.columnKey == headerColumn.title
    ) {
      switch (sortInfo.current.order) {
        case "ascend":
          sortingInfo.order = "descend";
          break;
        case "decend":
          sortingInfo.order = null;
          break;
        case null:
          sortingInfo.order = "ascend";
          break;
        default:
          sortingInfo.order = null;
          break;
      }
    }
    sortInfo.current = sortingInfo;
    let tmp_columns = columns.map((col) => ({
      ...col,
      sortOrder: getSortOrder(col),
    }));
    setColumns(tmp_columns);
  };
  const getSortOrder = (col) => {
    if (!col.sorter) {
      return null;
    }
    if (sortInfo.current) {
      if (!sortInfo.current.columnKey) {
        return null;
      }
    } else {
      return null;
    }
    return sortInfo.current.columnKey === col.title
      ? sortInfo.current.order
      : null;
  };

  columnHeaders = columnHeaders.map((col, index) => {
    return {
      ...col,
      ...(col.searchable && getColumnSearchProps(col.dataIndex, true)),
      sortOrder: getSortOrder(col),
      onHeaderCell: (column) => ({
        width: column.width,
        onResize: handleResize(index),
        onClick: () => {
          headerClickSortHandler(col);
        },
      }),
    };
  });
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
  const components = useMemo(() => {
    return {
      ...VList({
        height: "40vh",
        resetTopWhenDataChange: false,
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

  const onSelectChange = (newSelectedRowKeys, selectedRows) => {
    const t = i18next.t;
    let selectedRowKeyByType = newSelectedRowKeys;
    let selectRowsObjectByType = selectedRows;
    if (selectType == "single") {
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

  const handleSearch = (setSelectedKeys, confirm, dataIndex) => {
    initState.current = false;
    let tmp_columnSelectedKeys = searchInputs.current;
    tmp_columnSelectedKeys[dataIndex] = searchInputText[dataIndex];
    searchInputs.current = tmp_columnSelectedKeys;
    let selectedKeys = tmp_columnSelectedKeys[dataIndex];
    setSelectedKeys(selectedKeys);
    confirm();
  };

  const handleReset = (clearFilters, setSelectedKeys, confirm, dataIndex) => {
    initState.current = false;
    clearFilters();
    searchInputs.current[dataIndex] = [];
    let tmp_searchInputText = searchInputText;
    tmp_searchInputText[dataIndex] = [];
    setSearchInputText([...tmp_searchInputText]);
    handleSearch(setSelectedKeys, confirm, dataIndex);
  };

  const rowSelection = hasRowSelect
    ? {
      hideSelectAll,
      selectedRowKeys,
      columnWidth: "50px",
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
        if(stateId == "objectList"){
          for (let i = 0; i < selectedObjectList.length; i++) {
            if (selectedObjectList[i].key != record.key) {
              selectedRow.push(selectedObjectList[i]);
              selectedRowKey.push(selectedObjectListKey[i]);
            }
          }
          dispatch(setSelectedObject(selectedRow));
          dispatch(setSelectedRowKeys(selectedRowKey));
        }
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
  const clearSearchSort = () => {
    if (category != EXECUTION_MANAGEMENT) {
      sortInfo.current = {};
      // setSearchText([]);
      // setSearchedColumn("");
      if (columns) {
        let editColumn = columnHeaders;
        editColumn = editColumn.map((col, index) => {
          return {
            ...col,
            sortOrder: getSortOrder(col),
          };
        });
        setColumns(editColumn);
      }
    }
  };
  //Render only first time. Capture windows resize
  useEffect(() => {
    return () => {
      window.removeEventListener("resize", browserResize);
    };
  }, []);
  useEffect(() => {
    let tmp_column = columns;
    if (!autoRefresh) {
      if (stateId != "jobExecutionResultList") {
        if (params) {
          dispatch(dispatchAction(params));
        } else {
          dispatch(dispatchAction(params));
        }
      }
    } else {
      const intervalId = setInterval(() => {
        dispatch(dispatchAction(params));
      }, 2000);

      return () => {
        clearInterval(intervalId);
      };
    }
  }, [params]);
  useEffect(() => {
    //if (category !== OBJECT_CATEGORY.JOBNET) {
    if (disabledAlert.current < 1) {
      //if(disabledAlert.current<2){
      disabledAlert.current++;
      return;
    }
    if (updateValidationResult) {
      if (updateValidationResult.type == SERVICE_RESPONSE.OK) {
        dispatch(setSelectedRowKeys([]));
        dispatch(setSelectedObject([]));
        dispatch(dispatchAction(params));
        alertSuccess(
          t(updateValidationResult.detail["message-detail"]),
          t("label-success"),
          () => { }
        );
      } else if (
        updateValidationResult.detail["message-detail"] ==
        "err-msg-no-boottime-jobnet"
      ) {
        alertError(t("title-error"), t("err-msg-no-boottime-jobnet"));
      } else if (
        updateValidationResult.detail["message-detail"] == "rel-error"
      ) {
        alertError(
          t("title-rel-error"),
          relatedDataErrMsg(updateValidationResult.detail["return-item"]),
          600
        );
        dispatch(setSelectedRowKeys([]));
        dispatch(setSelectedObject([]));
      } else if (
        updateValidationResult.detail["message-detail"] == "err-flow"
      ) {
        console.log("error")
        alertError(
          t("title-err"),
          `${updateValidationResult.detail["message-objectid"]} : ${t(
            "err-flow-001"
          )}${updateValidationResult.detail["return-item"][0]}${t(
            "err-flow-002"
          )}`,
          600
        );
        dispatch(setSelectedRowKeys([]));
        dispatch(setSelectedObject([]));
      } else {
        alertError(
          t("sel-warning"),
          `${updateValidationResult.detail["message-objectid"]
            ? updateValidationResult.detail["message-objectid"] + " :"
            : ""
          } ${updateValidationResult.detail["message-detail"]
            ? t(updateValidationResult.detail["message-detail"])
            : t("err-msg-fail")
          }`
        );
        dispatch(setSelectedRowKeys([]));
        dispatch(setSelectedObject([]));
      }
    }
    // }
  }, [updateValidationResult]);

  useEffect(() => {
    if (objectList) {
      setData(prepareData(objectList));
    } else {
      if (disabledEmpty.current > 0) {
        //alertError(t("sel-warning"), t("res-not-exist"));
        navigate(
          `/object-list/${category}/${publicType ? "public" : "private"}/`
        );
      }
    }
    if (disabledEmpty.current < 1) {
      disabledEmpty.current++;
    }
  }, [objectList]);
  useEffect(() => {
    if (category != EXECUTION_MANAGEMENT) {
      setRowCount(data.length);
      clearSearchSort();
      setSearchInputText([]);
      if (columns) {
        initState.current = true;
        for (var colItem of columns) {
          let currentCol = columnFilterItem.current;
          if (
            currentCol &&
            currentCol[
            `${category}-${publicType ? "public" : "private"}-${colItem.dataIndex
            }`
            ]
          ) {
            let col =
              currentCol[
              `${category}-${publicType ? "public" : "private"}-${colItem.dataIndex
              }`
              ];
            searchInputs.current[colItem.dataIndex] = [];
            col.clearFilters();
            handleSearch(col.setSelectedKeys, col.confirm, colItem.dataIndex);
          }
        }
        let filterIcons = document.getElementsByClassName(
          "ant-table-filter-trigger"
        );
        for (let filterIcon of filterIcons) {
          filterIcon.classList.remove("active");
        }
      }
    } else {
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
        setRowCount(data.length);
      }
    }
  }, [data]);
  const tableFilterChanged = (pagination, filters, sorter, extra) => {
    if (extra.action == "filter") {
      setRowCount(extra.currentDataSource.length);
    }
  };
  return (
    <>
      <Table
        bordered
        loading={autoRefresh ? false : isLoading}
        //tableLayout="auto"
        rowSelection={rowSelection}
        components={components}
        columns={columns}
        dataSource={data}
        scroll={{ x: "100%", y: tableHeight }}
        pagination={false}
        //sticky
        size="middle"
        onRow={onRowAction}
        onChange={tableFilterChanged}
        rowClassName={(record, rowIndex) =>
          record.validFlag == 1 ? "valid-row" : ""
        }
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
                      : ""}{" "}
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
