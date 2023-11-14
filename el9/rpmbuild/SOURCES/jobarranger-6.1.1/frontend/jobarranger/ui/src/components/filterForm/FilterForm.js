import React, { useEffect, useState } from "react";
import YearCalendar from "../calendar/YearCalendar";
import {
  initFilter,
  initFilterEdit,
  getCalendarDate,
  setFilterFormData,
} from "../../store/FilterSlice";
import { createFormObject } from "../../factory/FormObjectFactory";
import Layout from "antd/lib/layout/layout";
import "./FilterForm.scss";
import FormObject from "../form/formObject/FormObject";
import ScheduleFormObject from "../form/scheduleFormObject/ScheduleFormObject";
import objectLockService from "../../services/objectLockService";
import { t } from "i18next";
import { useDispatch, useSelector } from "react-redux";
import { FORM_TYPE, OBJECT_CATEGORY } from "../../constants";
import {
  Form,
  Col,
  Row,
  TreeSelect,
  Radio,
  InputNumber,
  Select,
  Spin,
} from "antd";
import {
  lockObject,
  setObjectFormEditable,
  setunLock,
} from "../../store/ObjectListSlice";

const { TreeNode } = TreeSelect;

const FilterFormComponent = ({
  objectId,
  date,
  objectType,
  formType = FORM_TYPE.EDIT,
  publicType,
}) => {
  const dispatch = useDispatch();
  const filterInfo = useSelector((state) => state["filter"].data);
  const [filteredDates, setFilteredDates] = useState(undefined);
  const [designatedDay, setDesignatedDay] = useState(undefined);
  const [baseCalendar, setBaseCalendar] = useState(undefined);
  const calendarDate = useSelector((state) => state["filter"].calendarDate);
  const selectedYear = useSelector((state) => state["filter"].selectedYear);
  const [shiftDays, setShiftDays] = useState(1);
  const [baseDate, setBaseDate] = useState(0);
  const [editable, setEditable] = useState(1);
  const [checkDDate, setCheckDDate] = useState(true);
  const subForm = Form.useFormInstance();
  const [publicCalendarList, setPublicCalendarList] = useState();
  const [privateCalendarList, setPrivateCalendarList] = useState();
  const { Option } = Select;
  const [isReloaded, reloaded] = useState(false);

  useEffect(() => {
    //unlock on reload
    if (formType != FORM_TYPE.SCHEDULE) {
      var localStoredValues = JSON.parse(
        sessionStorage.getItem("formIsRefreshed") || "[]"
      );
      if ("isLocked" in localStoredValues && localStoredValues.isLocked == 0) {
        let lockedObject = {
          objectId: localStoredValues.objectId,
          objectType: localStoredValues.objectType,
        };
        objectLockService.deleteLockAsync([lockedObject]).then((response) => {
          reloaded(true);
        });
        dispatch(setunLock());
      } else {
        reloaded(true);
      }
      sessionStorage.removeItem("formIsRefreshed");
    } else {
      reloaded(true);
    }
    //
  }, [objectId]);

  //Get initialize or get data for edit
  useEffect(() => {
    if (isReloaded === true) {
      if (formType === FORM_TYPE.CREATE) {
        let obj = {
          type: formType,
          isPublic: publicType,
        };
        dispatch(initFilter(obj));
      } else {
        let object = {
          id: objectId,
          date: date,
          type: formType,
        };
        dispatch(initFilterEdit(object));
      }
    }
  }, [objectId, isReloaded]);

  //
  useEffect(() => {
    const pubCalendarDatas = [];
    const priCalendarDatas = [];
    let formEditable = 1;
    if (filterInfo) {
      let formObject = {};
      let editable = 1;
      let idEditable = 1;
      let tmpId = "";
      if (Object.keys(filterInfo).length > 0) {
        if (filterInfo.editable == 0 || filterInfo.isLocked == 1) {
          formEditable = 0;
        } else {
          formEditable = 1;
        }
        switch (formType) {
          case FORM_TYPE.SCHEDULE:
            editable = 0;
            idEditable = 1;
            break;
          case FORM_TYPE.CREATE:
            editable = 1;
            idEditable = 1;
            break;
          case FORM_TYPE.EDIT:
            editable = formEditable;
            idEditable = 0;
            break;
          case FORM_TYPE.NEW_OBJECT:
            editable = formEditable;
            idEditable = 1;
            break;
          case FORM_TYPE.NEW_VERSION:
            editable = formEditable;
            idEditable = 0;
            break;
        }
        if (!objectId) {
          formObject = createFormObject(
            `${t(`obj-${OBJECT_CATEGORY.FILTER}`)}_${filterInfo.lastid}`,
            publicType,
            "",
            "",
            "",
            filterInfo.userName,
            true,
            "",
            "",
            "",
            "",
            "",
            editable,
            idEditable,
            filterInfo.isLocked
          );
          dispatch(setObjectFormEditable(true));
        } else {
          if (formType == FORM_TYPE.NEW_OBJECT) {
            tmpId = `${t(`obj-${OBJECT_CATEGORY.FILTER}`)}_${
              filterInfo.filterId
            }`;
            dispatch(setObjectFormEditable(true));
          } else {
            tmpId = filterInfo.filterId;
            //lock if editable.
            if (filterInfo.editable == 0 || filterInfo.isLocked == 1) {
              if (formType != FORM_TYPE.NEW_VERSION) {
                dispatch(setObjectFormEditable(false));
              }
            } else {
              if (objectType != "schedule") {
                let object = {
                  objectId: filterInfo.filterId,
                  date: filterInfo.updateDate,
                  category: "filter",
                };
                dispatch(lockObject(object));
              }
              dispatch(setObjectFormEditable(true));
            }
          }

          formObject = createFormObject(
            tmpId,
            parseInt(filterInfo.publicFlag) ? true : false,
            "",
            filterInfo.updateDate,
            filterInfo.filterName,
            filterInfo.userName,
            filterInfo.authority,
            "",
            filterInfo.desc,
            "",
            "",
            filterInfo.dates,
            editable,
            idEditable,
            filterInfo.isLocked
          );
        }
        subForm.setFieldsValue({
          getCalendar: filterInfo.baseCalendarId,
          baseDays: filterInfo.baseDateFlag
            ? parseInt(filterInfo.baseDateFlag)
            : 0,
          designatedDay:
            parseInt(filterInfo.designatedDay) > 0
              ? filterInfo.designatedDay
              : "",
          shiftDay: filterInfo.shiftDay ? parseInt(filterInfo.shiftDay) : 1,
        });
        let baseDate = filterInfo.baseDateFlag;
        if (baseDate == null) {
          setBaseDate(0);
        } else {
          setBaseDate(parseInt(filterInfo.baseDateFlag));
        }
        setDesignatedDay(parseInt(filterInfo.designatedDay));
        setShiftDays(
          filterInfo.shiftDay === undefined ? 1 : parseInt(filterInfo.shiftDay)
        );
        setCheckDDate(parseInt(filterInfo.baseDateFlag) === 2 ? false : true);
        setBaseCalendar(filterInfo.baseCalendarId);
        let pubCal = filterInfo.calendarList.pubCal;
        for (let i = 0; i < pubCal.length; i++) {
          pubCalendarDatas.push(
            <TreeNode
              key={pubCal[i].calendar_id}
              value={pubCal[i].calendar_id}
              title={pubCal[i].calendar_id}
            />
          );
        }
        let pivCal = filterInfo.calendarList.pivCal;
        for (let i = 0; i < pivCal.length; i++) {
          priCalendarDatas.push(
            <TreeNode
              key={pivCal[i].calendar_id}
              value={pivCal[i].calendar_id}
              title={pivCal[i].calendar_id}
            />
          );
        }
        setEditable(editable);
        dispatch(setFilterFormData(formObject));
      }
    }
    setPublicCalendarList(pubCalendarDatas);
    setPrivateCalendarList(priCalendarDatas);
  }, [filterInfo]);

  const baseDays = (e) => {
    setBaseDate(e.target.value);
    if (e.target.value == 2) {
      setCheckDDate(false);
    } else {
      setCheckDDate(true);
      setDesignatedDay(NaN);
      subForm.setFields([{ name: "designatedDay", value: "", errors: [""] }]);
    }
  };

  useEffect(() => {
    setFilteredDates(undefined);
    if (baseCalendar !== undefined) {
      let searchData = {
        calendarId: baseCalendar,
      };
      dispatch(getCalendarDate(searchData));
    }
  }, [baseCalendar]);

  //get dates
  const selectCalendar = (value) => {
    let searchData = {
      calendarId: value,
    };
    // setFilteredDates(undefined);
    dispatch(getCalendarDate(searchData));
  };

  const changeDesignated = (value) => {
    if (value === null) {
      setDesignatedDay(undefined);
    } else {
      setDesignatedDay(value);
    }
  };

  const shiftDayChange = (value) => {
    setShiftDays(value);
  };

  const selectorFunction = (state) => {
    return state["filter"].data;
  };

  //Calendar data to time convert
  function formatDateToString(fulldate, day, month, year) {
    let data = {
      dateobj: new Date(`${year},${month + 1},${day}`),
      fulldate: fulldate,
      day: day,
      month: month,
      year: year,
    };
    return data;
  }

  //calendar date select
  useEffect(() => {
    filterOperationDate();
  }, [calendarDate, baseDate, designatedDay, shiftDays, selectedYear]);

  var lastday = function (y, m) {
    return new Date(y, m + 1, 0).getDate();
  };

  function filterOperationDate() {
    var dates = new Array();
    if (calendarDate) {
      let baseDay = 1;
      let baseType = parseInt(baseDate);
      if (baseType === 0) {
        baseDay = 1;
      } else if (baseType === 2) {
        baseDay = designatedDay == 0 ? NaN : designatedDay;
      }
      let shiftDay = parseInt(shiftDays);
      var operationYears = [];

      var datesArray = [];
      for (var i = 0; i < calendarDate.length; i++) {
        var operating_date = calendarDate[i].operating_date.toString();
        datesArray.push(
          formatDateToString(
            operating_date,
            parseInt(operating_date.substring(6, 8)),
            parseInt(operating_date.substring(4, 6)) - 1,
            parseInt(operating_date.substring(0, 4))
          )
        );
        let year = parseInt(operating_date.substring(0, 4));
        if (!operationYears.includes(year)) {
          operationYears.push(year);
        }
        let preYear = year - 1;
        if (!operationYears.includes(preYear)) {
          operationYears.push(preYear);
        }
        let nexYear = year + 1;
        if (!operationYears.includes(nexYear)) {
          operationYears.push(nexYear);
        }
      }

      for (var y = 0; y < operationYears.length; y++) {
        let baseYear = operationYears[y];

        let operatingMonth = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];
        let datesArrayCount = calendarDate.length;

        for (var i = 0; i < datesArray.length; i++) {
          let operatingMonthCount = operatingMonth.length;
          if (!--datesArrayCount) {
            if (shiftDay == 0) {
              let filterArray = [];
              {
                operatingMonth.map((vo) => {
                  let operatingDate = null;
                  if (baseType == 1) {
                    operatingDate = {
                      day: lastday(baseYear, vo),
                      month: vo,
                      year: baseYear,
                    };
                  } else if (baseType == 0) {
                    operatingDate = {
                      day: 1,
                      month: vo,
                      year: baseYear,
                    };
                  } else {
                    operatingDate = {
                      day: baseDay,
                      month: vo,
                      year: baseYear,
                    };
                  }
                  for (var i = 0; i < datesArray.length; i++) {
                    if (
                      datesArray[i].day == operatingDate.day &&
                      datesArray[i].month == operatingDate.month &&
                      datesArray[i].year == operatingDate.year
                    ) {
                      if (!filterArray.includes(datesArray[i].fulldate)) {
                        filterArray.push(datesArray[i].fulldate);
                      }
                    }
                  }
                  if (!--operatingMonthCount) {
                    {
                      filterArray.map((vf) => {
                        dates.push(vf);
                      });
                    }
                  }
                });
              }
            } else {
              if (shiftDay > 0) {
                let filterArray = [];
                {
                  operatingMonth.map((vo) => {
                    let operatingDate = null;
                    if (baseType === 1) {
                      operatingDate = {
                        day: lastday(baseYear, vo),
                        month: vo,
                        year: baseYear,
                        dateobj: new Date(baseYear, vo, lastday(baseYear, vo)),
                      };
                    } else if (baseType === 0) {
                      operatingDate = {
                        day: 1,
                        month: vo,
                        year: baseYear,
                        dateobj: new Date(baseYear, vo, baseDay),
                      };
                    } else {
                      let lastDay = lastday(baseYear, vo);
                      if (baseDay > lastDay) {
                        operatingDate = {
                          day: 1,
                          month: vo + 1,
                          year: baseYear,
                          dateobj: new Date(baseYear, vo, baseDay),
                        };
                      } else {
                        operatingDate = {
                          day: baseDay,
                          month: vo,
                          year: baseYear,
                          dateobj: new Date(baseYear, vo, baseDay),
                        };
                      }
                    }

                    let baseShiftDay = null;
                    let baseDayExist = false;
                    for (var i = 0; i < datesArray.length; i++) {
                      if (
                        datesArray[i].day === operatingDate.day &&
                        datesArray[i].month === operatingDate.month &&
                        datesArray[i].year === operatingDate.year
                      ) {
                        baseShiftDay = {
                          index: i,
                          shift: 0,
                          fulldate: datesArray[i].fulldate,
                        };
                        baseDayExist = true;
                      }
                    }

                    if (baseShiftDay === null) {
                      for (var i = 0; i < datesArray.length; i++) {
                        if (datesArray[i].dateobj > operatingDate.dateobj) {
                          baseShiftDay = {
                            index: i,
                            shift: 1,
                            fulldate: datesArray[i].fulldate,
                          };
                          break;
                        }
                      }
                    }

                    if (baseShiftDay !== null) {
                      if (!baseDayExist) {
                        let shiftIndex = parseInt(
                          baseShiftDay.index -
                            baseShiftDay.shift +
                            parseInt(shiftDay)
                        );
                        if (shiftIndex < datesArray.length) {
                          if (
                            !filterArray.includes(
                              datesArray[shiftIndex].fulldate
                            )
                          ) {
                            filterArray.push(datesArray[shiftIndex].fulldate);
                          }
                        }
                      } else {
                        filterArray.push(
                          datesArray[baseShiftDay.index].fulldate
                        );
                      }
                    }

                    if (!--operatingMonthCount) {
                      {
                        filterArray.map((vf) => {
                          dates.push(vf);
                        });
                      }
                    }
                  });
                }
              } else {
                let filterArray = [];
                {
                  operatingMonth.map((vo) => {
                    let operatingDate = null;
                    if (baseType == 1) {
                      operatingDate = {
                        day: lastday(baseYear, vo),
                        month: vo,
                        year: baseYear,
                        dateobj: new Date(baseYear, vo, lastday(baseYear, vo)),
                      };
                    } else if (baseType == 0) {
                      operatingDate = {
                        day: 1,
                        month: vo,
                        year: baseYear,
                        dateobj: new Date(baseYear, vo, baseDay),
                      };
                    } else {
                      operatingDate = {
                        day: baseDay,
                        month: vo,
                        year: baseYear,
                        dateobj: new Date(baseYear, vo, baseDay),
                      };
                    }
                    let baseShiftDay = null;
                    let baseDayExist = false;
                    for (var i = 0; i < datesArray.length; i++) {
                      if (
                        datesArray[i].day == operatingDate.day &&
                        datesArray[i].month == operatingDate.month &&
                        datesArray[i].year == operatingDate.year
                      ) {
                        baseShiftDay = {
                          index: i,
                          shift: 0,
                          fulldate: datesArray[i].fulldate,
                        };
                        baseDayExist = true;
                        break;
                      }
                    }

                    if (baseShiftDay === null) {
                      for (var i = datesArray.length - 1; i >= 0; i--) {
                        if (datesArray[i].dateobj < operatingDate.dateobj) {
                          baseShiftDay = {
                            index: i,
                            shift: 1,
                            fulldate: datesArray[i].fulldate,
                          };
                          break;
                        }
                      }
                    }

                    if (baseShiftDay !== null) {
                      if (!baseDayExist) {
                        let shiftIndex = parseInt(
                          baseShiftDay.index +
                            baseShiftDay.shift +
                            parseInt(shiftDay)
                        );
                        if (shiftIndex >= 0) {
                          if (
                            !filterArray.includes(
                              datesArray[shiftIndex].fulldate
                            )
                          ) {
                            filterArray.push(datesArray[shiftIndex].fulldate);
                          }
                        }
                      } else {
                        filterArray.push(
                          datesArray[baseShiftDay.index].fulldate
                        );
                      }
                    }
                    if (!--operatingMonthCount) {
                      {
                        filterArray.map((vf) => {
                          dates.push(vf);
                        });
                      }
                    }
                  });
                }
              }
            }
          }
        }
      }
      setFilteredDates(dates);
    }
  }
  useEffect(() => {
    subForm.validateFields(["designatedDay"]);
  }, [designatedDay]);

  const valid = false;

  return (
    <Form
      id="filter-sub-form"
      name="filter-sub-form"
      form={subForm}
      initialValues={{ baseDays: 0, shiftDay: 1 }}
    >
      <Layout className="object-info-layout">
        {objectType == FORM_TYPE.SCHEDULE ? (
          <ScheduleFormObject
            formId={OBJECT_CATEGORY.FILTER}
            objectSlice="filter"
            isCalendar={true}
            objectType={objectType}
          />
        ) : (
          <FormObject
            formId={OBJECT_CATEGORY.FILTER}
            objectSlice="filter"
            objectType={objectType}
          />
        )}
        <Row type="flex" gutter={14} hidden={objectType == FORM_TYPE.SCHEDULE}>
          <Col>
            <Form.Item
              name="getCalendar"
              label={t("menu-cal")}
              rules={[
                {
                  required: true,
                  message: t("txt-choose-bas-cal"),
                },
              ]}
            >
              <TreeSelect
                treeLine={
                  true && {
                    valid,
                  }
                }
                showSearch
                listHeight={500}
                style={{
                  width: 300,
                }}
                onChange={selectCalendar}
                disabled={editable === 0}
                placeholder={t("lab-calendar-type")}
              >
                <TreeNode
                  key={t("sel-pub-cal")}
                  value={t("sel-pub-cal")}
                  title={t("sel-pub-cal")}
                  selectable={false}
                >
                  {publicCalendarList}
                </TreeNode>
                <TreeNode
                  key={t("sel-prv-cal")}
                  value={t("sel-prv-cal")}
                  title={t("sel-prv-cal")}
                  selectable={false}
                >
                  {privateCalendarList}
                </TreeNode>
              </TreeSelect>
            </Form.Item>
          </Col>
          <Col>
            <Form.Item name="baseDays" label={t("lab-base-date")}>
              <Radio.Group onChange={baseDays} disabled={editable === 0}>
                <Radio value={0}>{t("rdo-fst-day")}</Radio>
                <Radio value={1}>{t("rdo-lst-day")}</Radio>
                <Radio value={2}>{t("rdo-dsg-day")}</Radio>
              </Radio.Group>
            </Form.Item>
          </Col>
          <Col>
            <Form.Item
              name="designatedDay"
              rules={[
                {
                  required: !checkDDate,
                  message: t("txt-fild-des-rq"),
                },
              ]}
            >
              <InputNumber
                min={1}
                max={31}
                onChange={changeDesignated}
                disabled={checkDDate || editable === 0}
              />
            </Form.Item>
          </Col>
          <Col>
            <Form.Item name="shiftDay" label={t("rdo-sft-day")}>
              <Select
                id="shiftDay"
                disabled={editable === 0}
                onChange={shiftDayChange}
              >
                <Option value={-7}>-7</Option>
                <Option value={-6}>-6</Option>
                <Option value={-5}>-5</Option>
                <Option value={-4}>-4</Option>
                <Option value={-3}>-3</Option>
                <Option value={-2}>-2</Option>
                <Option value={-1}>-1</Option>
                <Option value={0}>0</Option>
                <Option value={1}>1</Option>
                <Option value={2}>2</Option>
                <Option value={3}>3</Option>
                <Option value={4}>4</Option>
                <Option value={5}>5</Option>
                <Option value={6}>6</Option>
                <Option value={7}>7</Option>
              </Select>
            </Form.Item>
          </Col>
        </Row>
      </Layout>
      <Layout className="year-calendar-layout">
        <YearCalendar
          selectorFunction={selectorFunction}
          filterDates={filteredDates}
          objectType={objectType}
        />
      </Layout>
    </Form>
  );
};

export default FilterFormComponent;
