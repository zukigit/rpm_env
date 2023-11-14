/**
 * create and return FormObject data
 * @param {*} id
 * @param {*} isPublic
 * @param {*} multiple
 * @param {*} updateDate
 * @param {*} name
 * @param {*} userName
 * @param {*} authority
 * @param {*} lastWorkingDay
 * @param {*} description
 * @param {*} timeout
 * @param {*} timeoutType
 * @returns  formObject.
 */

export const createFormObject = (
  id,
  isPublic,
  multiple,
  updateDate,
  name,
  userName,
  authority,
  lastWorkingDay,
  description,
  timeout,
  timeoutType,
  dates,
  editable,
  idEditable,
  isLocked = 0
) => {
  return {
    id: id,
    isPublic: isPublic,
    multiple: multiple,
    updateDate: updateDate,
    name: name,
    userName: userName,
    authority: authority,
    lastWorkingDay: lastWorkingDay,
    description: description,
    timeout: timeout,
    timeoutType: timeoutType,
    dates: dates,
    editable: editable,
    idEditable: idEditable,
    isLocked: isLocked,
  };
};
/**
 * Create and return calendar create/edit/new version/new object.
 * @param {String} calendarId
 * @param {String} calendarName
 * @param {String} userName
 * @param {String} publicFlag
 * @param {Date String} updateDate
 * @param {String} desc
 * @param {String} formType
 * @param {Array} dates Array of calendar selected dates
 * @param {Date String} createdDate
 * @param {Bool string} validFlag
 * @param {Date String} lastday
 * @param {Bool string} editable
 * @param {int String} authority
 * @returns calendar form array
 */
export const createCalendarFormObjectRequest = (
  urlDate,
  urlId,
  calendarId,
  calendarName,
  userName,
  publicFlag,
  updateDate,
  desc,
  formType,
  dates,
  createdDate,
  validFlag,
  lastday,
  editable,
  authority,
  notInitialize,
  isLocked = 0
) => {
  return {
    urlDate: urlDate,
    urlId: urlId,
    calendarId: calendarId,
    calendarName: calendarName,
    userName: userName,
    publicFlag: publicFlag,
    updateDate: updateDate,
    desc: desc,
    formType: formType,
    dates: dates,
    createdDate: createdDate,
    validFlag: validFlag,
    lastday: lastday,
    editable: editable,
    authority: authority,
    notInitialize: notInitialize,
    isLocked: isLocked,
  };
};

/**
 * Create and return calendar create/edit/new version/new request object.
 * @param {*} filterId
 * @param {*} filterName
 * @param {*} userName
 * @param {*} publicFlag
 * @param {*} updateDate
 * @param {*} desc
 * @param {*} formType
 * @param {*} dates
 * @returns
 */
export const createFilterFormObjectRequest = (
  urlDate,
  urlId,
  filterId,
  filterName,
  userName,
  publicFlag,
  updateDate,
  desc,
  formType,
  createdDate,
  validFlag,
  editable,
  authority,
  baseDateFlag,
  designatedDay,
  shiftDay,
  baseCalendarId,
  calendarList,
  isLocked
) => {
  return {
    urlDate: urlDate,
    urlId: urlId,
    filterId: filterId,
    filterName: filterName,
    userName: userName,
    publicFlag: publicFlag,
    updateDate: updateDate,
    desc: desc,
    formType: formType,
    createdDate: createdDate,
    validFlag: validFlag,
    editable: editable,
    authority: authority,
    baseDateFlag: baseDateFlag,
    designatedDay: designatedDay,
    shiftDay: shiftDay,
    baseCalendarId: baseCalendarId,
    calendarList: calendarList,
    isLocked: isLocked,
  };
};
