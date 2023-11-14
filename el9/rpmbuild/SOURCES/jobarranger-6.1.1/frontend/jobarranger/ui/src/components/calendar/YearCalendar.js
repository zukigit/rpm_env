import React from "react";
import Calendar from "rc-year-calendar";
import "rc-year-calendar/locales/rc-year-calendar.ja";
import { useState, useEffect, useRef } from "react";
import { useSelector, useDispatch } from "react-redux";
import { t } from "i18next";
import { setDataSourcesByDate, colorDateOnCalendar } from "./YearCalendarFunc";
import "./YearCalendar.scss";
import { Form } from "antd";
import { setSelectedYear } from "../../store/FilterSlice";
import {
  setCalendarChanged,
  setDateBeforeInitLoad,
  // setDatasource,
} from "../../store/CalendarSlice";
import store from "../../store";

const currentYear = new Date().getFullYear();

const YearCalendar = ({ selectorFunction, filterDates, objectType }) => {
  const [dataSource, setDataSource] = useState([]);
  const isEnabled = useRef(false);
  const form = Form.useFormInstance();
  const dateChangedResults = useRef([]);
  let selectedYear = currentYear;
  const calendarData = useSelector((state) => selectorFunction(state));
  const calendarLocae = store.getState().user.userInfo.language;
  const dispatch = useDispatch();
  useEffect(() => {
    if (calendarData) {
      if (Object.keys(calendarData).length > 0) {
        let initDatesArray = [];
        if (filterDates) {
          //initDatesArray = Object.values(filterDates);
          setDataSource(setDataSourcesByDate(filterDates));
          dateChangedResults.current = Object.values(filterDates);
          dispatch(setDateBeforeInitLoad(filterDates));
        } else if (calendarData.dates) {
          setDataSource(setDataSourcesByDate(calendarData.dates));
          dateChangedResults.current = Object.values(calendarData.dates);
          dispatch(setDateBeforeInitLoad(calendarData.dates));
        } else {
          setDataSource([]);
          dateChangedResults.current = [];
          dispatch(setDateBeforeInitLoad([]));
        }

        // dateChangedResults.current = [
        //   ...dateChangedResults.current,
        //   ...initDatesArray,
        // ];
        form.setFieldsValue({
          dates: dateChangedResults.current,
        });

        if (
          calendarData.editable == 0 ||
          objectType == "schedule" ||
          objectType == "filter" ||
          calendarData.isLocked == 1
        ) {
          isEnabled.current = false;
        } else {
          isEnabled.current = true;
        }
      }
    }
  }, [calendarData, filterDates]);


  const onYearChange = (e) => {
    selectedYear = e.currentYear;
    form.setFieldsValue({
      dates: dateChangedResults.current,
    });
    setDataSource(setDataSourcesByDate(dateChangedResults.current));
    dispatch(setSelectedYear({ selectedYear: selectedYear }));
  };

  const onRangeSelected = (e) => {
    if (isEnabled.current) {
      dateChangedResults.current = [
        ...colorDateOnCalendar(
          e.target,
          selectedYear,
          e.startDate,
          e.endDate,
          dateChangedResults.current
        ),
      ];
      dispatch(setDateBeforeInitLoad(dateChangedResults.current));
      var uniqueArray = [];
      dateChangedResults.current.forEach((element) => {
        var index = uniqueArray.indexOf(element);
        if (index == -1) {
          uniqueArray.push(element);
        }
      });
      form.setFieldsValue({
        dates: uniqueArray,
      });
      dispatch(setCalendarChanged(true));
    }
  };

  return (
    <Form id="year-calendar-form" name="year-calendar-form" form={form}>
      <Calendar
        language={calendarLocae == "JP" ? "ja" : ""}
        year={selectedYear}
        enableRangeSelection="true"
        dataSource={dataSource}
        style="background"
        onYearChanged={(e) => onYearChange(e)}
        onRangeSelected={(e) => onRangeSelected(e)}
      />
    </Form>
  );
};
export default YearCalendar;
