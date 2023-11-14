import { Card, Checkbox, Col, Modal, Row, Select, Form } from "antd";
import { t } from "i18next";
import React, { useEffect, useRef, useState } from "react";
import Draggable from "react-draggable";
import { useDispatch, useSelector } from "react-redux";
import {
  setInitRegistShow,
  setCalendarChanged,
  setCalendarDates,
} from "../../../store/CalendarSlice";
import moment from "moment";
import store from "../../../store";
import "../../calendar/YearCalendarFunc";

const CalendarInitRegistDialog = () => {
  //
  const ADD = "add";
  const REMOVE = "remove";
  //
  const form = Form.useFormInstance();
  const children = [];

  const dateBeforeInitLoad = useRef([]);
  //previous checked items
  const isInitialized = useRef(true);
  const prevDaySelect = useRef([]);
  const prevEveryMonthSelect = useRef([]);
  const calendarAllSelected = useRef([]);
  const calendarDates = useSelector((state) => state["calendar"].data.dates);

  let i = 1;
  while (i <= 31) {
    children.push(<Select.Option key={i}>{i}</Select.Option>);
    i++;
  }
  const dispatch = useDispatch();
  const dayOptions = [
    {
      label: t("lab-sun"),
      value: "0",
    },
    {
      label: t("lab-mon"),
      value: "1",
    },
    {
      label: t("lab-tue"),
      value: "2",
    },
    {
      label: t("lab-wed"),
      value: "3",
    },
    {
      label: t("lab-thu"),
      value: "4",
    },
    {
      label: t("lab-fri"),
      value: "5",
    },
    {
      label: t("lab-sat"),
      value: "6",
    },
  ];
  const isInitRegistShowSelect = useSelector(
    (state) => state["calendar"].isInitRegistShow
  );
  const getSelectedYear = useSelector((state) => state["filter"].selectedYear);
  const [disabled, setDisabled] = useState(false);
  const [bounds, setBounds] = useState({
    left: 0,
    top: 0,
    bottom: 0,
    right: 0,
  });
  const draggleRef = useRef(null);

  function padNumber(number) {
    var ret = new String(number);
    if (ret.length == 1) ret = "0" + ret;
    return ret;
  }

  function daysInMonth(month, year) {
    return new Date(year, month, 0).getDate();
  }

  var monthsArray = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
  var dateString;
  var month;

  const setChanges = (currentSelected, change) => {
    let calendarUniqueSelected = [];
    if (change == ADD) {
      calendarAllSelected.current = [
        ...calendarAllSelected.current,
        ...currentSelected,
      ];
    } else {
      //remove [datess] from [calendarAllSelected]
      //calendarAllSelected.current = [...calendarAllSelected.current, ...datess];
      let tmp_allSelected = calendarAllSelected.current;
      let tmp_currentSelected = currentSelected;
      tmp_allSelected = tmp_allSelected.filter((element) => {
        if (tmp_currentSelected.length > 0) {
          let result = !tmp_currentSelected.includes(element);
          let ind = tmp_currentSelected.indexOf(element);
          if (ind > -1) {
            tmp_currentSelected.splice(ind, 1);
          }
          return result;
        } else {
          return true;
        }
      });
      calendarAllSelected.current = tmp_allSelected;
    }
    if (isInitialized.current) {
      calendarUniqueSelected = getUnique([
        ...calendarAllSelected.current,
        ...dateBeforeInitLoad.current,
      ]);
    } else {
      calendarUniqueSelected = getUnique(calendarAllSelected.current);
    }
    dispatch(setCalendarDates(calendarUniqueSelected));
    form.setFieldsValue({ dates: calendarUniqueSelected });
  };
  const daySelectChange = (checkedValue) => {
    let year = getSelectedYear;
    let selectedDay = [];
    if (year == undefined) {
      year = new Date().getFullYear();
    }
    for (var i = 0; i < checkedValue.length; i++) {
      let day = checkedValue[i];
      for (var j = 0; j < monthsArray.length; j++) {
        month = padNumber(monthsArray[j]);
        var getWeekDays = daysInMonth(month, year);

        for (var x = 1; x <= getWeekDays; x++) {
          var newDate = new Date(year, monthsArray[j] - 1, x);
          if (newDate.getDay() == day) {
            dateString =
              month + "/" + padNumber(newDate.getDate()) + "/" + year;
            if (!selectedDay.includes(dateString)) {
              selectedDay.push(moment(dateString).format("YYYYMMDD"));
            }
          }
        }
      }
    }
    if (prevDaySelect.current && prevDaySelect.current.length > 0) {
      setChanges(prevDaySelect.current, REMOVE);
    }
    prevDaySelect.current = selectedDay;
    setChanges(selectedDay, ADD);
  };

  const begOfMonthChange = (e) => {
    let begOfMonthSelect = [];
    let year = getSelectedYear;
    if (year == undefined) {
      year = new Date().getFullYear();
    }
    for (var i = 0; i < 12; i++) {
      begOfMonthSelect.push(moment(new Date(year, i, 1)).format("YYYYMMDD"));
    }
    if (e.target.checked) {
      setChanges(begOfMonthSelect, ADD);
    } else {
      setChanges(begOfMonthSelect, REMOVE);
    }
  };

  const endOfMonthChange = (e) => {
    let endOfMonthSelect = [];
    let year = getSelectedYear;
    if (year == undefined) {
      year = new Date().getFullYear();
    }
    for (var i = 0; i < 12; i++) {
      endOfMonthSelect.push(
        moment(new Date(year, i + 1, 0)).format("YYYYMMDD")
      );
    }
    if (e.target.checked) {
      setChanges(endOfMonthSelect, ADD);
    } else {
      setChanges(endOfMonthSelect, REMOVE);
    }
  };

  // const notInitializedCheck = (e) => {
  //   isInitialized.current = e.target.checked;
  //   setChanges([], ADD);
  // };
  const notInitializedCheck = (e) => {
    let year = getSelectedYear;
    let initializedDates = [];
    isInitialized.current = e.target.checked;
    for (var i = 0; i < calendarDates.length; i++) {
      if (calendarDates[i].substring(0, 4) != year) {
        initializedDates.push(calendarDates[i])
      }
    }
    console.log(calendarDates)

    setChanges(initializedDates, ADD);
  };

  const everyMonthSpecChange = (value) => {
    let everyMonthSelect = [];
    let day;
    let year = getSelectedYear;
    if (year == undefined) {
      year = new Date().getFullYear();
    }
    for (var i = 0; i < value.length; i++) {
      day = padNumber(value[i]);

      for (var j = 0; j < monthsArray.length; j++) {
        month = padNumber(monthsArray[j]);
        dateString = month + "/" + day + "/" + year;
        const date = moment(dateString, 'MM/DD/YYYY', true);
        if (date.isValid()) {
          if (!everyMonthSelect.includes(dateString)) {
            everyMonthSelect.push(moment(dateString).format("YYYYMMDD"));
          }
        }

      }
    }
    if (
      prevEveryMonthSelect.current &&
      prevEveryMonthSelect.current.length > 0
    ) {
      setChanges(prevEveryMonthSelect.current, REMOVE);
    }
    prevEveryMonthSelect.current = everyMonthSelect;
    setChanges(everyMonthSelect, ADD);
  };

  function getUnique(array) {
    var uniqueArray = [];

    if (Array.isArray(array)) {
      for (let i = 0; i < array.length; i++) {
        if (uniqueArray.indexOf(array[i]) === -1) {
          uniqueArray.push(array[i]);
        }
      }
    } else {
      if (uniqueArray.indexOf(array) === -1) {
        uniqueArray.push(array);
      }
    }
    return uniqueArray;
  }

  const handleOk = (e) => {
    prevEveryMonthSelect.current = [];
    prevDaySelect.current = [];
    calendarAllSelected.current = [];
    isInitialized.current = true;
    dispatch(setCalendarChanged(true));
    dispatch(setInitRegistShow(false));
  };

  const handleCancel = (e) => {
    form.setFieldsValue({ dates: dateBeforeInitLoad.current });
    dispatch(setCalendarDates(dateBeforeInitLoad.current));
    prevEveryMonthSelect.current = [];
    prevDaySelect.current = [];
    isInitialized.current = true;
    calendarAllSelected.current = [];
    dispatch(setInitRegistShow(false));
  };

  const onStart = (_event, uiData) => {
    const { clientWidth, clientHeight } = window.document.documentElement;
    const targetRect = draggleRef.current?.getBoundingClientRect();

    if (!targetRect) {
      return;
    }

    setBounds({
      left: -targetRect.left + uiData.x,
      right: clientWidth - (targetRect.right - uiData.x),
      top: -targetRect.top + uiData.y,
      bottom: clientHeight - (targetRect.bottom - uiData.y),
    });
  };
  useEffect(() => {
    prevDaySelect.current = [];
  }, []);
  useEffect(() => {
    if (isInitRegistShowSelect) {
      dateBeforeInitLoad.current = store.getState().calendar.dateBeforeInitLoad;
    }
  }, [isInitRegistShowSelect]);
  return (
    <>
      <Modal
        maskClosable={false}
        destroyOnClose={true}
        width={600}
        okText={t("btn-ok")}
        cancelText={t("btn-cancel")}
        title={
          <div
            style={{
              width: "100%",
              cursor: "move",
            }}
            onMouseOver={() => {
              if (disabled) {
                setDisabled(false);
              }
            }}
            onMouseOut={() => {
              setDisabled(true);
            }}
            onFocus={() => { }}
            onBlur={() => { }} // end
          >
            {t("title-initial")}
          </div>
        }
        visible={isInitRegistShowSelect}
        onOk={handleOk}
        onCancel={handleCancel}
        modalRender={(modal) => (
          <Draggable
            disabled={disabled}
            bounds={bounds}
            onStart={(event, uiData) => onStart(event, uiData)}
          >
            <div ref={draggleRef}>{modal}</div>
          </Draggable>
        )}
      >
        <div>{t("lab-one")}</div>
        <div>{t("lab-two")}</div>
        <div>{t("lab-three")}</div>
        <br />
        <Row>
          <Col span={12}>
            <Card
              title={<strong>{t("lab-day-week")}</strong>}
              style={{ width: 250, borderWidth: "medium" }}
            >
              <Checkbox.Group options={dayOptions} onChange={daySelectChange} />
            </Card>
          </Col>
          <Col span={12}>
            <Card style={{ width: 250, borderWidth: "medium" }}>
              <Checkbox style={{ margin: 0 }} onChange={begOfMonthChange}>
                {t("lab-begin-month")}
              </Checkbox>
              <Checkbox style={{ margin: 0 }} onChange={endOfMonthChange}>
                {t("lab-end-month")}
              </Checkbox>
            </Card>
            <br />
            <label>
              <strong>{t("lab-every-month")}</strong>
            </label>
            <br />
            <Select
              mode="multiple"
              allowClear
              style={{
                width: "100%",
              }}
              placeholder="Please select"
              onChange={everyMonthSpecChange}
            >
              {children}
            </Select>
          </Col>
        </Row>
        <Row>
          <Col>
            <Checkbox
              style={{ margin: 0 }}
              defaultChecked={true}
              onChange={notInitializedCheck}
            >
              {t("lab-not-initial")}
            </Checkbox>
          </Col>
        </Row>
      </Modal>
    </>
  );
};

export default CalendarInitRegistDialog;
