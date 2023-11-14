import moment from "moment";

/**
 * array date between given 2 dates , including given 2 dates.
 * @param {Date} startDate : Start date of calendar select.
 * @param {Date} endDate : End date of calendar select.
 * @returns : array of dates between start and end date, including start and end date.
 */
export function dateArrayFromStartEnd(startDate, endDate) {
  const date = new Date(startDate.getTime());

  const dates = [];

  while (date <= endDate) {
    dates.push(new Date(date));
    date.setDate(date.getDate() + 1);
  }
  return dates;
}
/**
 * set rs-year-calendar dataSource date from given date.
 * @param {Date array} dates : array of dates to be selected.
 * @returns : dataSource array to apply on rs-year-calendar.
 */
export function setDataSourcesByDate(dates) {
  let dataSources = [];
  if (dates.length != undefined && dates.length != 0) {
    dates.map((date) => {
      dataSources.push({
        color: "rgb(245, 187, 0)",
        startDate: new Date(moment(date, "YYYYMMDD")),
        endDate: new Date(moment(date, "YYYYMMDD")),
      });
    });
  }
  return dataSources;
}

/**
 * get an array of date from rc-year-calendar dataSource array.
 * @param {rc-year-calendar.dataSource} dataSources : array of rc-year-calendar dataSource
 * @returns Date array
 */
export function dataSourceToDateList(dataSources) {
  let dateLists = [];
  dataSources.map((dataSource) => {
    dateLists = [
      ...dateLists,
      ...dateArrayFromStartEnd(dataSource.startDate, dataSource.endDate),
    ];
  });
  return dateLists;
}

/**
 * Remove date from array of calendar selected date.
 * @param {Date array} dates : Original date array.
 * @param {Date array} newDates : Dates to be removed.
 * @returns array of updated date.
 */
export function removeDateFromList(dates, newDates) {
  //let tmpNewDates = newDates.filter(newDate=>dates.indexOf(newDate)!=0);
  let tmpDates = [];
  let eqFlag = 0;
  dates.map((date) => {
    newDates.map((newDate) => {
      if (
        new Date(moment(newDate, "YYYYMMDD")).getDate() ==
          new Date(moment(date, "YYYYMMDD")).getDate() &&
        new Date(moment(newDate, "YYYYMMDD")).getMonth() ==
          new Date(moment(date, "YYYYMMDD")).getMonth() &&
        new Date(moment(newDate, "YYYYMMDD")).getFullYear() ==
          new Date(moment(date, "YYYYMMDD")).getFullYear()
      ) {
        eqFlag = 1;
      }
    });
    if (eqFlag == 0) {
      tmpDates = [...tmpDates, date];
    }
    eqFlag = 0;
  });
  return tmpDates;
}

/**
 * Change color of  selected calendar cells and update the selected date list.
 * @param {event.target} e_t Calendar event target.
 * @param {int} selectedYear current year of calendar.
 * @param {Date} minDate min date of current selected date.
 * @param {Date} maxDate Max date of current selected date
 * @param {Date array} dates Array of already selected dates.
 * @returns updated selected-date list.
 */
export function colorDateOnCalendar(
  e_t,
  selectedYear,
  minDate,
  maxDate,
  dates
) {
  let resultDays = [];
  let toRemoves = [];
  e_t.querySelectorAll(".month-container").forEach(function (month) {
    var monthId = parseInt(month.dataset.monthId);
    if (minDate.getMonth() <= monthId && maxDate.getMonth() >= monthId) {
      month
        .querySelectorAll("td.day:not(.old):not(.new)")
        .forEach(function (day) {
          var day_int = parseInt(day.querySelector(".day-content").innerText);
          var changeFlg = 0;
          if (
            maxDate.getMonth() == minDate.getMonth() &&
            minDate.getDate() <= day_int &&
            day_int <= maxDate.getDate()
          ) {
            changeFlg = 1;
          } else if (
            maxDate.getMonth() > minDate.getMonth() &&
            minDate.getMonth() == monthId &&
            minDate.getDate() <= day_int
          ) {
            changeFlg = 1;
          } else if (
            minDate.getMonth() < monthId &&
            monthId < maxDate.getMonth()
          ) {
            changeFlg = 1;
          } else if (
            maxDate.getMonth() > minDate.getMonth() &&
            maxDate.getMonth() == monthId &&
            maxDate.getDate() >= day_int
          ) {
            changeFlg = 1;
          }
          if (changeFlg == 1) {
            if (day.style.backgroundColor != "rgb(245, 187, 0)") {
              day.style.backgroundColor = "rgb(245, 187, 0)";
              resultDays = [
                ...resultDays,
                moment(new Date(selectedYear, monthId, day_int)).format(
                  "YYYYMMDD"
                ),
              ];
            } else {
              day.removeAttribute("style");
              toRemoves = [
                ...toRemoves,
                moment(new Date(selectedYear, monthId, day_int)).format(
                  "YYYYMMDD"
                ),
              ];
            }
          }
          changeFlg = 0;
        });
    }
  });
  //remove duplicates.
  resultDays = [...resultDays, ...removeDateFromList(dates, toRemoves)];
  return resultDays;
}
